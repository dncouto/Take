﻿namespace ServerChat.Model
{
    public class MessageDTO
    {
        public bool Private { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Message { get; set; }
    }
}