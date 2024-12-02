using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPTBotApp.Models
{
    public class ChatRequest
    {
        public string Model { get; set; }
        public List<ChatMessage> Messages { get; set; } = new();
    }

    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
