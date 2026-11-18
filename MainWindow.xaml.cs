using System;
using System.IO;
using System.Media;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace CyberLeePart2
{
    public partial class MainWindow : Window
    {
        private readonly SpeechSynthesizer speaker;
        private SoundPlayer? greetingPlayer;
        private readonly Chatbot chatbot;
        private readonly TaskRepository taskRepository;
        private readonly ActivityLogRepository activityLogRepository;
        //ACTIVITY LOG
        private readonly List<string> activityLog = new List<string>();
        private bool showAllActivities = false;
        // CYBERSECURITY QUIZ VARIABLES

        private int currentQuestionIndex = -1;
        private int quizScore = 0;
        private bool quizStarted = false;
        private bool answerSubmitted = false;

        private class QuizQuestion
        {
            public string Question { get; set; }
            public string OptionA { get; set; }
            public string OptionB { get; set; }
            public string OptionC { get; set; }
            public string OptionD { get; set; }
            public string CorrectAnswer { get; set; }

            public QuizQuestion(string question, string a, string b,
                                string c, string d, string answer)
            {
                Question = question;
                OptionA = a;
                OptionB = b;
                OptionC = c;
                OptionD = d;
                CorrectAnswer = answer;
            }
        }

        private readonly QuizQuestion[] quizQuestions =
        {
    new QuizQuestion("What is phishing?",
        "A type of computer",
        "A scam used to steal information",
        "A safe website",
        "An antivirus program", "B"),

    new QuizQuestion("Which password is strongest?",
        "123456",
        "password",
        "MyName",
        "T9#mK82!pL", "D"),

    new QuizQuestion("What should you do with a suspicious email?",
        "Click every link",
        "Reply with your password",
        "Delete or report it",
        "Forward it to everyone", "C"),

    new QuizQuestion("What does antivirus software do?",
        "Protects against malicious software",
        "Creates passwords",
        "Sends emails",
        "Increases screen brightness", "A"),

    new QuizQuestion("What is two-factor authentication?",
        "Using two computers",
        "An extra layer of account security",
        "Sharing your password",
        "Changing your username", "B"),

    new QuizQuestion("Why should software be updated?",
        "To remove security improvements",
        "To make passwords public",
        "To fix security weaknesses",
        "To slow down the computer", "C"),

    new QuizQuestion("What is malware?",
        "A security update",
        "A web browser",
        "A strong password",
        "Malicious software", "D"),

    new QuizQuestion("What should you do on public Wi-Fi?",
        "Avoid sensitive transactions on unsecured networks",
        "Share your banking password",
        "Disable all security",
        "Open every link", "A"),

    new QuizQuestion("Which information should you keep private?",
        "The weather",
        "Your banking password",
        "Public news",
        "A movie title", "B"),

    new QuizQuestion("What is social engineering?",
        "Repairing a computer",
        "Installing software",
        "Manipulating people to reveal information",
        "Creating a website", "C"),

    new QuizQuestion("What should you do if your account is hacked?",
        "Ignore the problem",
        "Share your password",
        "Delete all your files immediately",
        "Change your password and secure your account", "D"),

    new QuizQuestion("Why should you back up important data?",
        "To recover information if it is lost",
        "To share passwords",
        "To invite hackers",
        "To remove antivirus software", "A")
};

        public MainWindow()
        {
            InitializeComponent();

            speaker = new SpeechSynthesizer();
            chatbot = new Chatbot();
            taskRepository = new TaskRepository();
            activityLogRepository = new ActivityLogRepository();

            PlayGreeting();

            LoadTasks();
            RefreshActivityList();
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

        // CHATBOT FUNCTIONS

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            ChatPanel.AppendText("You: " + message + Environment.NewLine);
            string nlpResponse = ProcessNaturalLanguage(message);

            string response = nlpResponse;

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

        // TASK ASSISTANT FUNCTIONS

        private void LoadTasks()
        {
            try
            {
                dgvTasks.ItemsSource = null;
                dgvTasks.ItemsSource = taskRepository.GetAllTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load tasks. Please check your database connection.\n\n"
                    + ex.Message,
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            string description = txtTaskDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Please enter both a task title and description.");
                return;
            }

            DateTime? reminderDate = null;

            if (chkUseReminder.IsChecked == true)
            {
                if (dtpReminder.SelectedDate == null)
                {
                    MessageBox.Show("Please select a reminder date.");
                    return;
                }

                reminderDate = dtpReminder.SelectedDate;
            }

            try
            {
                CyberTask newTask = new CyberTask
                {
                    Title = title,
                    Description = description,
                    ReminderDate = reminderDate,
                    IsCompleted = false
                };

                taskRepository.AddTask(newTask);
                RecordActivity("Added task: " + title);

                MessageBox.Show("Task added successfully!");

                txtTaskTitle.Clear();
                txtTaskDescription.Clear();
                chkUseReminder.IsChecked = false;
                dtpReminder.SelectedDate = null;

                LoadTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not add task:\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask selectedTask = dgvTasks.SelectedItem as CyberTask;

            if (selectedTask == null)
            {
                MessageBox.Show("Please select a task first.");
                return;
            }

            try
            {
                taskRepository.MarkTaskCompleted(selectedTask.TaskId);
                RecordActivity("Completed task: " + selectedTask.Title);

                MessageBox.Show("Task marked as completed!");

                LoadTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not complete task:\n" + ex.Message);
            }
        }

        private void btnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask selectedTask = dgvTasks.SelectedItem as CyberTask;

            if (selectedTask == null)
            {
                MessageBox.Show("Please select a task to delete.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete this task?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    taskRepository.DeleteTask(selectedTask.TaskId);
                    RecordActivity("Deleted task: " + selectedTask.Title);

                    MessageBox.Show("Task deleted successfully!");

                    LoadTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not delete task:\n" + ex.Message);
                }
            }
        }

        private void btnRefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
        }

        // CLOSE APPLICATION
        // START QUIZ

        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex = 0;
            quizScore = 0;
            quizStarted = true;
            RecordActivity("Started Cybersecurity Quiz");
            answerSubmitted = false;

            lblQuizScore.Text = "Score: 0";
            lblQuizFeedback.Text = "Good luck! Choose your answer.";

            DisplayQuestion();
        }

        // DISPLAY QUESTION

        private void DisplayQuestion()
        {
            if (currentQuestionIndex >= quizQuestions.Length)
            {
                lblQuestionNumber.Text = "Quiz completed!";
                lblQuestion.Text = "You have finished the Cybersecurity Quiz.";

                rdoA.Visibility = Visibility.Collapsed;
                rdoB.Visibility = Visibility.Collapsed;
                rdoC.Visibility = Visibility.Collapsed;
                rdoD.Visibility = Visibility.Collapsed;

                btnSubmitAnswer.IsEnabled = false;
                btnNextQuestion.IsEnabled = false;

                lblQuizScore.Text = "Final Score: " + quizScore +
                                    " / " + quizQuestions.Length;

                lblQuizFeedback.Text = "Well done! You have completed the quiz.";
                RecordActivity("Completed Cybersecurity Quiz. Final score: " + quizScore + " / " + quizQuestions.Length);

                quizStarted = false;
                return;
            }

            QuizQuestion question = quizQuestions[currentQuestionIndex];

            lblQuestionNumber.Text =
                "Question " + (currentQuestionIndex + 1) +
                " of " + quizQuestions.Length;

            lblQuestion.Text = question.Question;

            rdoA.Content = question.OptionA;
            rdoB.Content = question.OptionB;
            rdoC.Content = question.OptionC;
            rdoD.Content = question.OptionD;

            rdoA.IsChecked = false;
            rdoB.IsChecked = false;
            rdoC.IsChecked = false;
            rdoD.IsChecked = false;

            rdoA.Visibility = Visibility.Visible;
            rdoB.Visibility = Visibility.Visible;
            rdoC.Visibility = Visibility.Visible;
            rdoD.Visibility = Visibility.Visible;

            btnSubmitAnswer.IsEnabled = true;
            btnNextQuestion.IsEnabled = false;

            answerSubmitted = false;

            lblQuizFeedback.Text = "Select an answer and click Submit Answer.";
            lblQuizScore.Text = "Score: " + quizScore;
        }

        // SUBMIT ANSWER

        private void btnSubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (!quizStarted)
            {
                MessageBox.Show("Please start the quiz first.");
                return;
            }

            if (answerSubmitted)
            {
                MessageBox.Show("You have already answered this question.");
                return;
            }

            string selectedAnswer = "";

            if (rdoA.IsChecked == true) selectedAnswer = "A";
            else if (rdoB.IsChecked == true) selectedAnswer = "B";
            else if (rdoC.IsChecked == true) selectedAnswer = "C";
            else if (rdoD.IsChecked == true) selectedAnswer = "D";

            if (string.IsNullOrEmpty(selectedAnswer))
            {
                MessageBox.Show("Please select an answer.");
                return;
            }

            QuizQuestion question = quizQuestions[currentQuestionIndex];

            if (selectedAnswer == question.CorrectAnswer)
            {
                quizScore++;

                lblQuizFeedback.Text = "Correct! Well done!";
            }
            else
            {
                lblQuizFeedback.Text =
                    "Incorrect. The correct answer is " +
                    question.CorrectAnswer + ".";
            }

            answerSubmitted = true;

            lblQuizScore.Text = "Score: " + quizScore;

            btnSubmitAnswer.IsEnabled = false;
            btnNextQuestion.IsEnabled = true;
        }

        // NEXT QUESTION

        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (!answerSubmitted)
            {
                MessageBox.Show("Please submit your answer first.");
                return;
            }

            currentQuestionIndex++;

            DisplayQuestion();
        }
        // RECORD ACTIVITY
        private void RecordActivity(string activity)
        {
            activityLogRepository.AddActivity(activity);
            RefreshActivityList();
        }

        // DISPLAY ACTIVITY
        private void RefreshActivityList()
        {
            int limit = showAllActivities ? 100 : 5;

            lstActivity.ItemsSource =
                activityLogRepository.GetRecentActivities(limit);
        }

        // REFRESH BUTTON
        private void btnRefreshActivity_Click(object sender, RoutedEventArgs e)
        {
            RefreshActivityList();
        }

        // SHOW MORE BUTTON
        private void btnShowMore_Click(object sender, RoutedEventArgs e)
        {
            showAllActivities = !showAllActivities;

            btnShowMore.Content = showAllActivities
                ? "Show Less"
                : "Show More";

            RefreshActivityList();
        }
        // NATURAL LANGUAGE PROCESSING
        private string ProcessNaturalLanguage(string message)
        {
            string input = message.ToLower().Trim();

            if (input.Contains("show") && input.Contains("task"))
            {
                RecordActivity("Asked to view tasks");
                return "You can view all your cybersecurity tasks in the Task Assistant tab.";
            }
            else if (input.Contains("add") && input.Contains("task"))
            {
                RecordActivity("Asked to add a task");
                return "To add a task, open the Task Assistant tab, enter a title and description, then click Add Task.";
            }
            else if (input.Contains("quiz") || input.Contains("test my knowledge"))
            {
                RecordActivity("Asked about the cybersecurity quiz");
                return "You can test your cybersecurity knowledge in the Cyber Quiz tab. Click Start Quiz to begin!";
            }
            else if (input.Contains("phishing"))
            {
                RecordActivity("Asked about phishing");
                return "Phishing is a cyberattack where criminals use fake emails or messages to trick people into sharing private information.";
            }
            else if (input.Contains("password"))
            {
                RecordActivity("Asked about password safety");
                return "Use strong, unique passwords, avoid sharing them, and enable two-factor authentication whenever possible.";
            }
            else if (input.Contains("reminder"))
            {
                RecordActivity("Asked about task reminders");
                return "You can set a reminder when adding a task in the Task Assistant tab.";
            }
            else
            {
                return chatbot.GetResponse(message);
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