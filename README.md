CyberLee – Cybersecurity Awareness Assistant

Project Description

CyberLee is a WPF desktop application developed using C# in Visual Studio. It is an interactive Cybersecurity Awareness Assistant designed to educate users about cybersecurity through chatbot conversations, voice responses, task management and quizzes.

The application helps users understand common cybersecurity threats and develop safer online habits.

Features

1. Chatbot

* Interactive chatbot interface built using WPF.
* Voice greeting when the application starts.
* Text-to-speech responses.
* Personalised conversations using the user’s name.
* Cybersecurity guidance on passwords, phishing, privacy, viruses, scams, Wi-Fi and hacking.
* Random responses and conversation flow.
* Basic sentiment detection.
* Chat clearing functionality.

2. Task Assistant

* Add cybersecurity tasks with a title and description.
* Set optional task reminders.
* View saved tasks.
* Mark tasks as completed.
* Delete tasks.
* Store tasks in a MySQL database.
* Tasks remain saved when the application is closed and reopened.

3. Cybersecurity Quiz

* Interactive cybersecurity quiz containing 12 questions.
* Multiple-choice answers.
* Immediate feedback after submitting an answer.
* Score tracking.
* Final quiz score displayed when completed.

4. Natural Language Processing (NLP)

* Basic keyword-based language processing.
* Recognises questions about phishing and password safety.
* Identifies requests about tasks, reminders and quizzes.
* Provides relevant responses based on user input.

5. Activity Log

* Records important chatbot activities.
* Records task and quiz activities.
* Displays recent activities.
* Includes a Show More and Show Less option.
* Stores activity records in MySQL.
* Activities remain available after restarting the application.

Technologies Used

* C#
* .NET
* WPF (Windows Presentation Foundation)
* Visual Studio 2022
* MySQL Server 8.0
* MySQL Workbench
* MySql.Data NuGet package
* System.Speech
* WAV audio

Database

CyberLee uses a MySQL database called CyberSecurityBotDB.

The database contains two main tables:

* cyber_tasks – stores cybersecurity tasks, reminders and completion status.
* activity_logs – stores application activity records and timestamps.

The database connection settings are managed through DatabaseConfig.cs.

Note: The database password must be configured locally and must not be uploaded to a public GitHub repository.

How to Run

1. Install Visual Studio 2022.
2. Install MySQL Server and MySQL Workbench.
3. Create the CyberSecurityBotDB database and required tables.
4. Open the CyberLee solution in Visual Studio.
5. Restore the required NuGet packages.
6. Create a windows environment variable named CYBERLEE_DB_CONNECTION containing your local MySQL connection string. Do not upload your database password to GitHub
7. Build the solution.
8. Run the application using the green Start button in Visual Studio.

Main Project Files

* MainWindow.xaml – defines the graphical interface and application tabs.
* MainWindow.xaml.cs – handles application events and functionality.
* Chatbot.cs – manages chatbot responses and conversation logic.
* CyberTask.cs – represents cybersecurity tasks.
* DatabaseConfig.cs – contains database connection settings.
* TaskRepository.cs – manages task database operations.
* ActivityLogEntry.cs – represents activity log records.
* ActivityLogRepository.cs – manages activity log database operations.
* Greeting.wav – contains the audio greeting.

Conclusion

CyberLee demonstrates object-oriented programming, WPF graphical interface development, chatbot functionality, speech technology, basic natural language processing, database integration and cybersecurity awareness.

The application provides users with an interactive way to learn about cybersecurity while managing security-related tasks and testing their knowledge.
