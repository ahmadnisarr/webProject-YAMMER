using Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Infrastructure
{
    public class userRepository:Iuser
    {
        private readonly string _connectionString;
        public userRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool isExist(string email, String password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Email FROM AspNetUsers WHERE Email = @email";
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@email", email);
                Console.WriteLine("check it");
                using (SqlDataReader sqlDataReader = selectCommand.ExecuteReader())
                {
                    if (sqlDataReader.Read())
                    {
                        Console.WriteLine("Verfified in Repo");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

        }
    }
}
