﻿using System;
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
using System.IO.Packaging;

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
        public static List<Status> Statuses = new List<Status> { new Status { Id = 1, Name = "Новая" }, new Status { Id = 2, Name = "Одобрена" }, new Status { Id = 3, Name= "В работе" }, new Status { Id = 4, Name= "Выполнена" }, new Status { Id = 5, Name = "Отказано"} };
        public static List<Role> Roles = new List<Role> { new Role { Id = 0, Name = "Клиент" }, new Role { Id = 1, Name = "Менеджер" }, new Role { Id = 2, Name = "Исполнитель" }, new Role { Id = 3, Name = "Администратор" }, new Role { Id = 4, Name = "Директор" } };
        private static string connectionString = "Server=KIKRDEV-NOTE\\SQLEXPRESS;Initial Catalog=data;Integrated Security=True;Connect Timeout=1;Encrypt=False;Trust Server Certificate=False;";
        public async static void Check_Connection(Login login)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand("SELECT 1", connection);
                    await command.ExecuteNonQueryAsync();
                }
                catch (SqlException e)
                {
                    ErrorView ev = new ErrorView(e.Message);
                    ev.Show();
                    login.Close();
                }
            }
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
                int a = command.ExecuteNonQuery();
                Console.WriteLine(a);
            }
        }

        public static User Login_User(string username, string password)
        {
            User user = new User();
            List<Dictionary<string, object>> userDictList = Get($"select * from dbo.[user] where login='{username}' and password='{password}'");
            if (userDictList.Count > 0)
            {
                Dictionary<string, object> userDict = userDictList[0];
                user.Id = (int)userDict["id"];
                user.Login = username;
                user.Password = password;
                user.Phone = (string)userDict["phone"];
                user.Surname = (string)userDict["surname"];
                user.Email = (string)userDict["email"];
                user.Patronomic = (string)userDict["patronomic"];
                user.Name = (string)userDict["name"];
                user.Role = new Role();
                Dictionary<string, object> role = Get($"select * from dbo.[role] where id='{userDict["role_id"]}'")[0];
                user.Role.Id = (int)userDict["role_id"];
                user.Role.Name = (string)role["name"];
            }
            
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
            Set($"INSERT INTO dbo.[user] values ('{user.Name}', '{user.Surname}', '{user.Patronomic}', '{user.Role.Id}', '{user.Phone}', '{user.Email}', '{user.Login}', '{user.Password}')");
        }

        public static List<Query> Get_Queries(User user)
        {
            List<Query> queries = new List<Query>();
            List<Dictionary<string, object>> queriesDictList = null;
            if (user.Role.Id == 0)
            {
                queriesDictList = Get($"select q.*, def.id def_id, def.name def_name, dev.id dev_id, dev.name dev_name, e.id e_id, e.name e_name, e.surname e_surname, c.id c_id, c.name c_name, c.surname c_surname, s.id s_id, s.name s_name from dbo.query q join dbo.defect def on q.defect_id=def.id join dbo.device dev on q.device_id=dev.id left join dbo.[user] e on q.executor_id=e.id join dbo.[user] c on q.client_id=c.id join dbo.[status] s on q.status_id=s.id where q.client_id={user.Id}");
            }
            else if (user.Role.Id == 1 || user.Role.Id == 3 || user.Role.Id == 4)
            {
                queriesDictList = Get($"select q.*, def.id def_id, def.name def_name, dev.id dev_id, dev.name dev_name, e.id e_id, e.name e_name, e.surname e_surname, c.id c_id, c.name c_name, c.surname c_surname, s.id s_id, s.name s_name from dbo.query q join dbo.defect def on q.defect_id=def.id join dbo.device dev on q.device_id=dev.id left join dbo.[user] e on q.executor_id=e.id join dbo.[user] c on q.client_id=c.id join dbo.[status] s on q.status_id=s.id");
            }
            else
            {
                queriesDictList = Get($"select q.*, def.id def_id, def.name def_name, dev.id dev_id, dev.name dev_name, e.id e_id, e.name e_name, e.surname e_surname, c.id c_id, c.name c_name, c.surname c_surname, s.id s_id, s.name s_name from dbo.query q join dbo.defect def on q.defect_id=def.id join dbo.device dev on q.device_id=dev.id left join dbo.[user] e on q.executor_id=e.id join dbo.[user] c on q.client_id=c.id join dbo.[status] s on q.status_id=s.id where q.executor_id={user.Id}");
            }
            
            for (int i = 0; i < queriesDictList.Count; i++)
            {
                Query q = new Query();
                q.Id = (int)queriesDictList[i]["id"];
                q.Executor = new User();
                if (queriesDictList[i]["e_id"] is not DBNull)
                {
                    q.Executor.Id = (int)queriesDictList[i]["e_id"];
                    q.Executor.Name = (string)queriesDictList[i]["e_name"];
                    q.Executor.Surname = (string)queriesDictList[i]["e_surname"];
                }
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
                if (queriesDictList[i]["comment"] is not DBNull)
                    q.Comment = (string)queriesDictList[i]["comment"];
                q.Desc = (string)queriesDictList[i]["description"];
                if (queriesDictList[i]["date_finish"] is not DBNull)
                    q.DateFinish = (DateTime)queriesDictList[i]["date_finish"];
                q.DateStart = (DateTime)queriesDictList[i]["date_start"];
                q.Status = new Status();
                q.Status.Id = (int)queriesDictList[i]["s_id"];
                q.Status.Name = (string)queriesDictList[i]["s_name"];

                queries.Add(q);
            }

            return queries;
        }
        public static List<Defect> GetDefects()
        {
            List<Defect> defects = new List<Defect>();
            List<Dictionary<string, object>> queriesDictList = Get($"SELECT [id],[name] FROM [dbo].[defect]");
            for (int i = 0; i < queriesDictList.Count; i++)
            {
                Defect defect = new Defect();
                defect.Name = (string)queriesDictList[i]["name"];
                defect.Id = (int)queriesDictList[i]["id"];
                defects.Add(defect);
            }
            return defects;
        }
        public static List<Device> GetDevices()
        {
            List<Device> devices = new List<Device>();
            List<Dictionary<string, object>> queriesDictList = Get($"SELECT [id],[name] FROM [dbo].[device]");
            for (int i = 0; i < queriesDictList.Count; i++)
            {
                Device device = new Device();
                device.Name = (string)queriesDictList[i]["name"];
                device.Id = (int)queriesDictList[i]["id"];
                devices.Add(device);
            }
            return devices;
        }
        public static void AddQuery(Query query)
        {
            try
            {
                Set($"INSERT INTO dbo.[query](device_id, client_id, defect_id, description, date_start, status_id) VALUES ({query.Device.Id.ToString()}, {query.Client.Id.ToString()}, {query.Defect.Id.ToString()}, '{query.Desc}', '{query.DateStart.ToString()}', 1)");
            } 
            catch (SqlException e)
            {
                throw new Exception("Ошибка добавления заявки!");
            }
        }
        public static void ChangeQuery(Query query, User user) 
        {
            try
            {
                if (user.Role.Id == 1 || user.Role.Id == 3 || user.Role.Id == 4)
                {
                    string q = $"UPDATE dbo.[query] SET executor_id={query.Executor.Id}, status_id={query.Status.Id}, comment='{query.Comment}' where id={query.Id}";
                    Set($"UPDATE dbo.[query] SET executor_id={query.Executor.Id}, status_id={query.Status.Id}, comment='{query.Comment}' where id={query.Id}");
                } else
                {
                    Set($"UPDATE dbo.[query] SET status_id={query.Status.Id}, comment='{query.Comment}' where id={query.Id}");
                }
                
            }
            catch (SqlException e)
            {
                throw new Exception("Ошибка изменения заявки!");
            }
        }
        public static List<User> GetExecutor() 
        {
            List<User> users = new List<User>();
            List<Dictionary<string, object>> userDictList = Get($"Select * from (select u.*, r.id r_id, r.name r_name from dbo.[user] u left join dbo.role r on u.role_id=r.id) q where q.role_id = 2");
            for (int i = 0; i < userDictList.Count; i++)
            {
                Dictionary<string, object> userDict = userDictList[i];
                User user = new User();
                user.Id = (int)userDict["id"];
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
        public static User GetUser(int id)
        {
            User user = new User();
            Dictionary<string, object> userDict = Get($"select u.*, r.id r_id, r.name r_name from dbo.[user] u join dbo.role r on u.role_id=r.id where u.id = {id}")[0];
            user.Id = (int)userDict["id"];
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
            return user;
        }
        public static void AddUser(User user)
        {
            try
            {
                Set($"INSERT INTO [dbo].[user] ([name], [surname], [patronomic], [role_id], [phone], [email], [login], [password]) VALUES ('{user.Name}', '{user.Surname}', '{user.Patronomic}', '{user.Role.Id}', '{user.Phone}', '{user.Email}', '{user.Login}', '{user.Password}')");
            } catch (SqlException e)
            {
                throw new Exception("Данные о пользователе заполнены не корректно!");
            }
        }
    }
}
