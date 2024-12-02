using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPTBotApp.Models
{
    public class ChatResponse
    {
        public List<Choice> Choices { get; set; } = new();
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
