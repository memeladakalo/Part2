using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace CyberSecurityChatbot._1
{
    class ResponseSystem
    {

        private static Dictionary<string, string> responses = new Dictionary<string, string>()
    {
        {"how are you", "I'm just a bot, but I'm here to help you stay safe online!"},
        {"what's your purpose", "I educate users about cybersecurity threats and how to stay safe."},
        {"what can I ask you about", "You can ask me about phishing, password safety, and safe browsing."},
        {"phishing", "Phishing is a scam where attackers trick you into revealing personal information."},
        {"password safety", "Use strong passwords with a mix of letters, numbers, and symbols."},
        {"safe browsing", "Avoid clicking on suspicious links and use trusted websites only."}
    };

        public static void RespondToUser()
        {
            while (true)
            {
                Console.Write("\nAsk me a question (or type 'exit' to quit): ");
                string userInput = Console.ReadLine().ToLower();

                if (userInput == "exit")
                {
                    Console.WriteLine("Goodbye! Stay safe online.");
                    break;
                }

                if (responses.ContainsKey(userInput))
                {
                    Console.WriteLine(responses[userInput]);
                }
                else
                {
                    Console.WriteLine("Sorry, I don't understand that. Try asking about phishing, password safety, or safe browsing.");
                }
            }
        }
    
}
}

