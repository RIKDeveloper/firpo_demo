using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Security.Cryptography;

namespace pamagiti
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Defect
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Query
    {
        public int Id { get; set; }
        public Device device { get; set; }
        public Defect defect { get; set; }
        public string Desc { get; set; }
        public User Client { get; set; }
        public User Executor { get; set; }
        public Status Status { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateFinish { get; set; }
        public string Comment { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { set; get; }
        public string Surname { set; get; }
        public string Patronomic { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public Role Role { get; set; }
        public string Login { set; get; }
        public string Password { set; get; }
    }

    public static class BD
    {
        private static string connectionString = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;";

        private static SqlConnection connection = null;
        public static async void Create_Connection(MenuItem sender)
        {
            connection = new SqlConnection(connectionString);
            try
            {
                // Открываем подключение
                await connection.OpenAsync();
                Console.WriteLine("Подключение открыто");
                sender.Header = "Подключение открыто";
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // если подключение открыто
                if (connection.State == ConnectionState.Open)
                {
                    // закрываем подключение
                    await connection.CloseAsync();
                    Console.WriteLine("Подключение закрыто...");
                }
            }
            Console.WriteLine("Программа завершила работу.");
            Console.Read();
        }

        public static List<Query> Get_Queries()
        {
            List<Query> queries = new List<Query>();
            string sqlExpression = "select * from dbo.query";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object id = reader["id"];
                        object name = reader["name"];
                        object age = reader["age"];
                        Console.WriteLine($"{id} \t{name} \t{age}");
                    }
                }
            }

            return queries;
        }

        public static List<Dictionary<string, object>> Get(string sqlExpression)
        {
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> dict = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                dict[reader.GetName(i)] = reader.GetValue(i);
                            }
                            res.Add(dict);
                        }
                    }
                    
                }
            }

            return res;
        }

        public static void Set(string sqlExpression)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
        }

        public static User Login_User(string username, string password)
        {
            User user = new User();
            Dictionary<string, object> userDict = Get($"select * from dbo.user where login='{username}' and password='{SHA256.Create(password)}'")[0];
            user.Login = username;
            user.Password = password;
            user.Phone = (string)userDict["phone"];
            user.Surname = (string)userDict["surname"];
            user.Email = (string)userDict["email"];
            user.Patronomic = (string)userDict["patronomic"];
            user.Name = (string)userDict["name"];
            user.Role = new Role();
            Dictionary<string, object> role = Get($"select * from dbo.role where id={(string)userDict["role_id"]}")[0];
            user.Role.Id = (int)userDict["role_id"];
            user.Role.Name = (string)role["name"];
            return user;
        }

        public static void Set_User(User user)
        {
            Set($"INSERT INTO dbo.user value ({user.Name})");
        }
        
    }
}
