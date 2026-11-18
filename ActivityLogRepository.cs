using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CyberLeePart2
{
    public class ActivityLogRepository
    {
        public void AddActivity(string description)
        {
            using (MySqlConnection connection =
                   new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                string query = "INSERT INTO activity_logs " +
                               "(activity_description) VALUES (@description)";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@description", description);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<string> GetRecentActivities(int limit)
        {
            List<string> activities = new List<string>();

            using (MySqlConnection connection =
                   new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                string query = "SELECT activity_description, activity_date " +
                               "FROM activity_logs " +
                               "ORDER BY activity_date DESC, activity_id DESC " +
                               "LIMIT @limit";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@limit", limit);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string description =
                                reader.GetString("activity_description");

                            DateTime date =
                                reader.GetDateTime("activity_date");

                            activities.Add(
                                date.ToString("dd/MM/yyyy HH:mm") +
                                " - " + description);
                        }
                    }
                }
            }

            return activities;
        }
    }
}