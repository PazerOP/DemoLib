using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BitSet;
using TF2Net.Data;

namespace TF2Net.NetMessages
{
    class DebugMessage : INetMessage
    {
        private static int _messageCount;
        private static int _maxMessageCount = 1;
        private readonly IList<bool> _bs = new List<bool>();
        private readonly IList<Candidate> _candidates = new List<Candidate>();
        private Candidate _bestCandidate;


        public string Description => _bestCandidate?.FieldLength.ToString() ?? "null";

        public NetMessageType Type { get; set; }


        public void ReadMsg(BitStream stream)
        {
            if (_messageCount + 1 > _maxMessageCount)
            {
                Debug.WriteLine("hit MaxMessageCOunt abourting");

                throw new ApplicationException();
            }

            _messageCount++;
            while (stream.Length - stream.Cursor > 2)
            {
                _bs.Add(stream.ReadBool());

                //removing DebugListeners to ignore Debug.Assert's while trying to parse with current length
                TraceListener[] listeners = new TraceListener[Debug.Listeners.Count];
                Debug.Listeners.CopyTo(listeners, 0);
                Debug.Listeners.Clear();

                try
                {
                    List<INetMessage> netMessages = NetMessageCoder.Decode(stream.Clone());
                    AddCandidates(netMessages);
                }
                catch (StackOverflowException)
                {
                    Debug.WriteLine("hit StackOverflowException aborting");
                    break;
                }
                catch (OverflowException exception)
                {
                    Debug.WriteLine("hit OverflowException aborting");
                    break;
                }
                catch (Exception exception)
                {
                }
                finally
                {
                    Debug.Listeners.AddRange(listeners);
                }
            }

            _bestCandidate = _candidates
                .Where(c => c.Messages.Count > 0)
                .Where(c => c.ChildDebugMessageLength != int.MaxValue)
                .OrderByDescending(c => c.Messages.Count)
                .ThenBy(c => c.ChildDebugMessageLength)
                .FirstOrDefault();

            if (_bestCandidate == null)
                Debug.WriteLine("Unknown message: no candidate");
            else
                Debug.WriteLine("Unknown message: {0} bits long with best candidate having {1} unknown bits ", _bestCandidate.FieldLength, _bestCandidate.ChildDebugMessageLength);

            _messageCount--;
        }

        private void AddCandidates(List<INetMessage> netMessages)
        {
            int childDebugMessageLength = netMessages.Count == 0
                ? 0
                : netMessages.Max(m =>
                {
                    DebugMessage debugMessage = m as DebugMessage;
                    if (debugMessage == null)
                        return 0;
                    if (debugMessage._bestCandidate == null)
                        return int.MaxValue;
                    return debugMessage._bestCandidate.FieldLength;
                });
            _candidates.Add(
                new Candidate
                {
                    Messages = netMessages,
                    FieldLength = _bs.Count,
                    ChildDebugMessageLength = childDebugMessageLength,
                });
        }

        public void ApplyWorldState(WorldState ws)
        {
            throw new NotImplementedException();
        }


        private class Candidate
        {
            public int FieldLength { get; set; }
            public List<INetMessage> Messages { get; set; }
            public int ChildDebugMessageLength { get; set; }
        }
    }
}
