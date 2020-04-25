namespace ClientChat.Model
{
    public class MessageDTO
    {
        public static MessageDTO Build(bool priv, string from, string to, string message)
        {
            return new MessageDTO(priv, from, to, message);
        }

        private MessageDTO(bool priv, string from, string to, string message)
        {
            Private = priv;
            From = from;
            To = to;
            Message = message;
        }

        public bool Private { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Message { get; set; }
    }
}
