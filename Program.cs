using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Threading;
using System.Linq;

//Namespace and class definition for the Cybersecurity Chatbot program.
namespace CybersecurityChatbot
{
    class Program
    {
        //Stores the last topic the user showed interest in.
        static string rememberedTopic = null;

        //Default username; gets updated based on user input
        static string userName = "User";

        //Filename for storing user preferences such as name and interests
        static string prefsFile = "userprefs.txt";

        //Filename for logging all chatbot conversations
        static string logFile = "chatlog.txt";

        //Random number generator used for selecting random responses or tips
        static Random rand = new Random();
        
        
        
        // Entry point of the Cybersecurity chatbot application
        static void Main(string[] args)
        {
            ShowLoadingEffect("Initializing Cybersecurity Chatbot");
            PlayVoiceGreeting("greeting..wav");
            DisplayAsciiArt();
            LoadUserPreferences();

            if (string.IsNullOrEmpty(userName))
            {
                //Ask for User's Name
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("What is your name? ");
                Console.ResetColor();
                string userName = Console.ReadLine()?.Trim();
               
            }
           


                GreetUser(userName); //Display welcome message
            StartChatbot(); //Begin interactive chat session
        }
        // Displays a loading effect in the console with the given message followed by the three dots
        static void ShowLoadingEffect(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{message}");
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                Console.Write(".");
            }
            Console.WriteLine("\n");
            Console.ResetColor();
        }
        //Task 1: Play voice greeting
        static void PlayVoiceGreeting(string filePath)
        {
            try
            {
                if (!File.Exists("greeting..wav"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Audio file not found at {filePath}");
                    Console.ResetColor();
                    return;
                }

                using (SoundPlayer player = new SoundPlayer("greeting..wav"))
                {
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error playing sound: " + ex.Message);
                Console.ResetColor();
            }
        }
        //Task 2: Display ASCII art
        static void DisplayAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            string asciiArt = @"
╔═══════════════════════════════════════════════╗
║  ██╗     ██╗███████╗ █████╗ ██╗  ██╗          ║
║  ██║     ██║██╔════╝██╔══██╗██║  ██║          ║
║  ██║     ██║███████╗███████║███████║          ║
║  ██║     ██║╚════██║██╔══██║██╔══██║          ║
║  ███████╗██║███████║██║  ██║██║  ██║          ║
║  ╚══════╝╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝          ║
║      WELCOME TO LIZAH CYBERSECURITY BOT       ║
╚═══════════════════════════════════════════════╝
";
            Console.WriteLine(asciiArt);
            Console.ResetColor();
        }
           //Loads user preferences from a file if it exists
        static void LoadUserPreferences()
        {
            if (File.Exists(prefsFile))
            {
                var lines = File.ReadAllLines(prefsFile);
                if (lines.Length >= 1) userName = lines[0];
                if (lines.Length >= 2) rememberedTopic = lines[1];
            }
        }
        // Saves user preferences to a file
        static void SaveUserPreferences()
        {
            File.WriteAllLines(prefsFile, new[] { userName, rememberedTopic ?? "" });
        }
        // Logs a conversation entry appending the spaker and message to the log file,
        // including a timestamp for each entry.
        static void LogConversation(string speaker, string message)
        {
            File.AppendAllText(logFile, $"{DateTime.Now:HH:mm} {speaker}: {message}\n");
        }
        // Task 3; Greet the User
        static void GreetUser(string userName)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Hello, {userName}! Welcome to the Cybersecurity Chatbot.");
            Console.ResetColor();
            
        }
        //Task 4: Start chatbot conversation
        static void StartChatbot()
        {
            Dictionary<string, string> exactResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "how are you?", "I'm just a bot, but I'm here to help you stay safe online!" },
                { "what's your purpose?", "I educate users about Cybersecurity threats and how to stay safe." },
                { "what can i ask you about?", "You can ask me about phishing, password safety, scams, and safe browsing." },
                { "safe browsing", "Avoid clicking on suspicious links and use trusted websites only." }
            };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nType 'exit' to leave the chatbot.");
            Console.ResetColor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n{userName}: ");
                Console.ResetColor();

                //Reads user input, trims whitespace and converts it to lowercase for uniform processing.
                string input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input))
                {
                    PrintBotResponse("Please enter a valid question.");
                    continue;
                }

                LogConversation(userName, input);

                if (input == "exit")
                {
                    PrintBotResponse("Goodbye! Stay safe online!");
                    break;
                }

                // Analyzes the user's input for sentiment and responds empathetically if detected.
                string sentiment = DetectSentiment(input);
                if (sentiment != null)
                {
                    PrintBotResponse(sentiment);
                    continue;
                }

                if (input.Contains("phishing"))
                {
                    PrintBotResponse(GetRandomPhishingTip());
                    continue;
                }

                if (input.Contains("interested in"))
                {
                    int idx = input.IndexOf("interested in") + "interested in".Length;
                    rememberedTopic = input.Substring(idx).Trim();
                    SaveUserPreferences();
                    PrintBotResponse($"Great! I'll remember that you're interested in {rememberedTopic}.");
                    continue;
                }

                //Checks the user's input for known cybersecurity-related keywords and responds if a match is found.
                string keywordResponse = CheckForKeywords(input);
                if (keywordResponse != null)
                {
                    PrintBotResponse(keywordResponse);
                    continue;
                }

                if (!string.IsNullOrEmpty(rememberedTopic))
                {
                    PrintBotResponse($"As someone interested in {rememberedTopic}, make sure you regularly review your account settings.");
                    continue;
                }

                if (exactResponses.TryGetValue(input, out string response))
                {
                    PrintBotResponse(response);
                }
                else
                {
                    PrintBotResponse(GetFallbackResponse());
                }
            }
        }
        //This helps normalize user input for consistent keyword or sentiment detection.
        static string NormalizeInput(string input)
        {
            return new string(input.Where(c => !char.IsPunctuation(c)).ToArray()).ToLower();
        }

        //Analyzes the input string for emotional keywords to detect user sentiment.
        static string DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared"))
                return "It's okay to feel worried. Cybersecurity threats can be scary, but you're not alone!";
            if (input.Contains("curious"))
                return "Curiosity is the key to awareness! I'm glad you're asking questions.";
            if (input.Contains("frustrated"))
                return "I understand—it can be overwhelming. Let's tackle it step-by-step.";
            return null;
        }

        //Checks the input string for specific cybersecurity -related keywords
        //and returns an appropriate educational response if a keyword is found.
        static string CheckForKeywords(string input)
        {
            if (input.Contains("password"))
                return "Use strong, unique passwords for each account. Consider a password manager.";
            if (input.Contains("scam"))
                return "Scams use fear or urgency to trick you. Be cautious and double-check messages.";
            if (input.Contains("privacy"))
                return "Limit personal information online and adjust privacy settings on your apps.";
            return null;
        }
           
        //Returns a random phishing prevention tip from a predefined list
        //to educate users on identifying and avoiding phishing attempts.
        static string GetRandomPhishingTip()
        {
            List<string> tips = new List<string>
            {
                "Don't click on links in suspicious emails.",
                "Verify the sender's address before replying to any message.",
                "Look for spelling errors or strange formatting in emails.",
                "Phishers often impersonate banks or delivery services.",
                "Enable 2FA to add an extra layer of security."
            };
            return tips[rand.Next(tips.Count)];
        }


        //Returns a random fallback response when the chatbot
        //cannot match the user input to known keywords or responses
        static string GetFallbackResponse()
        {
            string[] responses =
            {
                "I'm not sure I understand. Can you try rephrasing?",
                "Sorry, I didn't catch that. Try asking about phishing, scams, or privacy.",
                "Let’s focus on cybersecurity topics like password safety or online scams!",
                "Interesting... Can you ask it a different way?"
            };
            return responses[rand.Next(responses.Length)];
        }


        //Prints the chatbot's response in green to the console,
        //logs the message to a conversation log, and resets the console color afterward.
        static void PrintBotResponse(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Chatbot: {message}");
            Console.ResetColor();
            LogConversation("Chatbot", message);
        }
    }
}
