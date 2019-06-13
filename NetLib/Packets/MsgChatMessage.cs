using System;


namespace NetLib.Packets
{
    [Serializable]
    public class MsgChatMessage
    {
        public string Text { get; set; }
    }
}
