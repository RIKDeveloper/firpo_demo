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
        public Device Device { get; set; }
        public Defect Defect { get; set; }
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
        private static string connectionString = "Data Source=KIKRDEV-COMP\\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;";

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
            Dictionary<string, object> userDict = Get($"select * from dbo.[user] where login='{username}' and password='{SHA256.Create(password)}'")[0];
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

        public static List<User> Get_Users()
        {
            List<User> users = new List<User>();   
            List <Dictionary<string, object>> userDictList = Get($"select *, r.id r_id, r.name r_name from dbo.[user] u join dbo.role r on u.role_id=r.id");
            for (int i = 0; i < userDictList.Count; i++)
            {
                Dictionary<string, object> userDict = userDictList[i];
                User user = new User();
                user.Login = (string)userDict["login"];
                user.Password = (string)userDict["password"];
                user.Phone = (string)userDict["phone"];
                user.Surname = (string)userDict["surname"];
                user.Email = (string)userDict["email"];
                user.Patronomic = (string)userDict["patronomic"];
                user.Name = (string)userDict["name"];
                user.Role = new Role();
                user.Role.Id = (int)userDict["r_id"];
                user.Role.Name = (string)userDict["r_name"];
                users.Add(user);    
            }
            return users;
        }

        public static void Set_User(User user)
        {
            Set($"INSERT INTO dbo.[user] value ({user.Name}, {user.Surname}, {user.Patronomic}, {user.Role.Id}, {user.Phone}, {user.Email}, {user.Login}, {SHA256.Create(user.Password)})");
        }

        public static List<Query> Get_Queries(User user)
        {
            List<Query> queries = new List<Query>();
            List<Dictionary<string, object>> queriesDictList = Get($"select q.*, def.id def_id, def.name def_name, dev.id dev_id, dev.name dev_name, e.id e_id, e.name e_name, e.surname e_surname, c.id c_id, c.name c_name, c.surname c_surname, s.id s_id, s.name s_name from dbo.query q join dbo.defect def on q.defect_id=def.id join dbo.device dev on q.device_id=dev.id join dbo.[user] e on q.executor_id=e.id join dbo.[user] c on q.client_id=c.id join dbo.[status] s on q.status_id=s.id");
            for (int i = 0; i < queriesDictList.Count; i++)
            {
                Query q = new Query();
                q.Executor = new User();
                q.Executor.Id = (int)queriesDictList[i]["e_id"];
                q.Executor.Name = (string)queriesDictList[i]["e_name"];
                q.Executor.Surname = (string)queriesDictList[i]["e_surname"];
                q.Client = new User();
                q.Client.Id = (int)queriesDictList[i]["c_id"];
                q.Client.Name = (string)queriesDictList[i]["c_name"];
                q.Client.Surname = (string)queriesDictList[i]["c_surname"];
                q.Defect = new Defect();
                q.Defect.Id = (int)queriesDictList[i]["def_id"];
                q.Defect.Name = (string)queriesDictList[i]["def_name"];
                q.Device = new Device();
                q.Device.Id = (int)queriesDictList[i]["dev_id"];
                q.Device.Name = (string)queriesDictList[i]["dev_name"];
                q.Comment = (string)queriesDictList[i]["comment"];
                q.Desc = (string)queriesDictList[i]["description"];
                q.DateFinish = (DateTime)queriesDictList[i]["date_finish"];
                q.DateStart = (DateTime)queriesDictList[i]["date_start"];
                q.Status = new Status();
                q.Status.Id = (int)queriesDictList[i]["s_id"];
                q.Status.Name = (string)queriesDictList[i]["s_name"];

                queries.Add(q);
            }

            return queries;
        }
    }
}
