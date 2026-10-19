using System;
using System.IO;
using System.Media;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Input;

namespace CyberLeePart2
{
    public partial class MainWindow : Window
    {
        private readonly SpeechSynthesizer speaker;
        private SoundPlayer? greetingPlayer;
        private readonly Chatbot chatbot;

        public MainWindow()
        {
            InitializeComponent();

            speaker = new SpeechSynthesizer();
            chatbot = new Chatbot();

            PlayGreeting();
        }

        private void PlayGreeting()
        {
            string greetingPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Greeting.wav");

            if (File.Exists(greetingPath))
            {
                try
                {
                    greetingPlayer = new SoundPlayer(greetingPath);
                    greetingPlayer.Load();
                    greetingPlayer.Play();
                }
                catch (Exception)
                {
                    speaker.SpeakAsync(
                        "Hello! Welcome to CyberLee, your Cybersecurity Awareness Assistant.");
                }
            }
            else
            {
                speaker.SpeakAsync(
                    "Hello! Welcome to CyberLee, your Cybersecurity Awareness Assistant.");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            ChatPanel.AppendText("You: " + message + Environment.NewLine);

            string response = chatbot.GetResponse(message);

            ChatPanel.AppendText(
                "CyberLee: " + response + Environment.NewLine + Environment.NewLine);

            ChatPanel.ScrollToEnd();

            speaker.SpeakAsync(response);

            MessageInput.Clear();
            MessageInput.Focus();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Clear();

            speaker.SpeakAsyncCancelAll();

            ChatPanel.AppendText(
                "CyberLee: Chat cleared. How can I help you with cybersecurity?");
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
                e.Handled = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            speaker.SpeakAsyncCancelAll();
            speaker.Dispose();

            if (greetingPlayer != null)
            {
                greetingPlayer.Stop();
                greetingPlayer.Dispose();
            }

            base.OnClosed(e);
        }
    }
}