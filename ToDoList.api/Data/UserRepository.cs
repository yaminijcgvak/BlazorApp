
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using ToDoList.api.Models;


namespace BlazorAppApi.Data
    {
        public class UserRepository
        {
            private const string ConnectionString = "Data Source=users.db";

            public UserRepository()
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL
                    );
                ";
                    command.ExecuteNonQuery();
                }
            }

            public void AddUser(UserLogin user)
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                    INSERT INTO Users (Username, Password) VALUES ($username, $password);
                ";
                    command.Parameters.AddWithValue("$username", user.Username);
                    command.Parameters.AddWithValue("$password", user.Password); 
                    command.ExecuteNonQuery();
                }
            }

            public UserLogin? GetUser(string username)
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                    SELECT Username, Password FROM Users WHERE Username = $username;
                ";
                    command.Parameters.AddWithValue("$username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserLogin
                            {
                                Username = reader.GetString(0),
                                Password = reader.GetString(1)
                            };
                        }
                    }
                }
                return null;
            }
        }
    }


