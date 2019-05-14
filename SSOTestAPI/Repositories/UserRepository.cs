using Microsoft.Extensions.Configuration;
using SSOTestAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SSOTestAPI.Repositories
{
    public class UserRepository
    {
        private string connectionString;
        public UserRepository(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public void Register(UserModel user)
        {
            string queryString =
                "INSERT INTO [User] ([Username], [Password]) VALUES (@username, @password)";
            using (SqlConnection connection =
               new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.Password);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public UserModel Authenticate(UserModel user)
        {
            string queryString =
                "SELECT * FROM [User] WHERE [Username] = @username AND [Password] = @password";

            using (SqlConnection connection =
               new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.Password);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user.Id = (int)reader["Id"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return user;
        }
    }
}
