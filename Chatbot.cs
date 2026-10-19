using System;
using System.Collections.Generic;

namespace CyberLeePart2
{
    public class Chatbot
    {
        private readonly Random random = new Random();
        private string userName = "";
        private string favouriteTopic = "";

        private readonly Dictionary<string, List<string>> responses =
            new Dictionary<string, List<string>>()
            {
                { "password", new List<string>
                    {
                        "Use a strong, unique password for every important account.",
                        "Never share your password or security codes.",
                        "Consider using a trusted password manager."
                    }
                },
                { "phishing", new List<string>
                    {
                        "Check the sender's email address before trusting a message.",
                        "Avoid suspicious links and unexpected attachments.",
                        "Verify urgent requests through official contact details."
                    }
                },
                { "privacy", new List<string>
                    {
                        "Limit the personal information you share online.",
                        "Review the privacy settings on your social media accounts.",
                        "Use multi-factor authentication to protect important accounts."
                    }
                },
                { "virus", new List<string>
                    {
                        "Keep your operating system and security software updated.",
                        "Download software only from trusted sources.",
                        "Avoid opening unexpected attachments."
                    }
                },
                { "scam", new List<string>
                    {
                        "Be careful with offers promising prizes or easy money.",
                        "Never share your banking PIN or password.",
                        "Verify offers through official websites."
                    }
                },
                { "wifi", new List<string>
                    {
                        "Use a strong password for your home Wi-Fi.",
                        "Avoid sensitive transactions on untrusted public Wi-Fi.",
                        "Keep your router updated."
                    }
                },
                { "hacking", new List<string>
                    {
                        "Use unique passwords and multi-factor authentication.",
                        "Keep your devices updated.",
                        "Report suspected account compromise and secure your account."
                    }
                }
            };

        public string GetResponse(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "Please type a message so I can help you.";
            }

            string input = message.Trim().ToLowerInvariant();

            // Remember the user's name.
            if (input.StartsWith("my name is "))
            {
                userName = message.Trim().Substring(11).Trim();

                if (string.IsNullOrWhiteSpace(userName))
                {
                    return "Please type your name after 'My name is'.";
                }

                return "Nice to meet you, " + userName
                    + "! I am CyberLee, your cybersecurity assistant.";
            }

            // Recognise the user's name when asked.
            if (input == "what is my name?" ||
                input == "what is my name" ||
                input == "what's my name")
            {
                if (string.IsNullOrWhiteSpace(userName))
                {
                    return "I don't know your name yet. Type 'My name is' followed by your name.";
                }

                return "Your name is " + userName + ".";
            }

            // Recognise a name entered by itself.
            // This assumes the user types their name after the greeting.
            if (IsLikelyName(message))
            {
                userName = message.Trim();

                return "Nice to meet you, " + userName
                    + "! You can ask me about passwords, phishing, privacy, viruses, scams, Wi-Fi or hacking.";
            }

            // Greetings.
            if (input == "hi" || input == "hello" ||
                input == "hey" || input.StartsWith("hello ") ||
                input.StartsWith("hi ") || input.StartsWith("hey "))
            {
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    return "Hello, " + userName
                        + "! What cybersecurity topic would you like to learn about?";
                }

                return "Hello! I am CyberLee, your cybersecurity awareness assistant. Ask me about passwords, phishing, privacy, viruses, scams, Wi-Fi or hacking.";
            }

            // Sentiment detection.
            if (input.Contains("worried") || input.Contains("scared") ||
                input.Contains("frustrated") || input.Contains("afraid") ||
                input.Contains("upset"))
            {
                return "I understand your concern. We can work through the online safety issue step by step.";
            }

            if (input.Contains("happy") || input.Contains("excited"))
            {
                return "I'm glad to hear that! Keep learning how to stay safe online.";
            }

            if (input.Contains("thank"))
            {
                return "You're welcome! Stay safe online.";
            }

            if (input.Contains("help"))
            {
                return "I can help with passwords, phishing, privacy, viruses, scams, Wi-Fi and hacking prevention.";
            }

            if (input == "bye" || input == "goodbye")
            {
                return "Goodbye! Remember to stay safe online.";
            }

            // Remember the user's favourite topic.
            if (input.StartsWith("my favourite topic is ") ||
                input.StartsWith("my favorite topic is "))
            {
                int separator = input.IndexOf(" is ");
                favouriteTopic = message.Substring(separator + 4).Trim();

                return "I'll remember your favourite topic during this session: "
                    + favouriteTopic + ".";
            }

            if (input.Contains("favourite topic") ||
                input.Contains("favorite topic"))
            {
                if (string.IsNullOrWhiteSpace(favouriteTopic))
                {
                    return "Tell me your favourite topic by typing 'My favourite topic is privacy'.";
                }

                return "Your favourite topic is " + favouriteTopic + ".";
            }

            // Find a matching cybersecurity topic.
            foreach (KeyValuePair<string, List<string>> topic in responses)
            {
                if (input.Contains(topic.Key))
                {
                    List<string> answers = topic.Value;
                    return answers[random.Next(answers.Count)];
                }
            }

            return "I'm not sure I understood that. Try asking about passwords, phishing, privacy, viruses, scams, Wi-Fi or hacking.";
        }

        private bool IsLikelyName(string message)
        {
            string value = message.Trim();

            if (value.Length < 2 || value.Length > 40)
                return false;

            // A name entered by itself should contain letters,
            // spaces, apostrophes or hyphens only.
            foreach (char character in value)
            {
                if (!char.IsLetter(character) &&
                    character != ' ' &&
                    character != '\'' &&
                    character != '-')
                {
                    return false;
                }
            }

            // Avoid treating common commands as names.
            string lower = value.ToLowerInvariant();

            string[] commonWords =
            {
                "hello", "hi", "hey", "help", "thanks",
                "thank you", "bye", "goodbye", "password",
                "phishing", "privacy", "virus", "scam",
                "wifi", "hacking"
            };

            foreach (string word in commonWords)
            {
                if (lower == word)
                    return false;
            }

            return true;
        }
    }
}