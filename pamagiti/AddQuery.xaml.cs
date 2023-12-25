using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace pamagiti
{
    /// <summary>
    /// Логика взаимодействия для AddQuery.xaml
    /// </summary>
    public partial class AddQuery : Window
    {
        private User user { get; set; }
        private string type { get; set; }
        private List<Defect> defects { get; set; }
        private List<string> defectsString { get; set; }
        private List<Device> devices { get; set; }
        private List<string> devicesString { get; set; }
        private List<User> executors { get; set; }
        private List<string> executorsString { get; set; }
        private Query query { get; set; }
        public AddQuery(string type, User user, Query query)
        {
            InitializeComponent();
            this.user = user;
            this.type = type;
            this.query = query;
            this.defects = BD.GetDefects();
            this.devices = BD.GetDevices();
            this.defectsString = new List<string>();
            this.devicesString = new List<string>();
            for (int i = 0; i < defects.Count; i++)
            {
                defectsString.Add(defects[i].Name);
            }
            for (int i = 0;i < devices.Count; i++) 
            {
                devicesString.Add(devices[i].Name);
            }

            switch (this.type)
            {
                case "add":
                    date_create.SelectedDate = DateTime.Now;
                    clients.ItemsSource = new string[] { $"{this.user.Surname} {this.user.Name} {this.user.Patronomic}" };
                    clients.SelectedIndex = 0;
                    clients.IsEnabled = false;
                    type_defect.ItemsSource = this.defectsString;
                    type_device.ItemsSource = this.devicesString;
                    break;
                case "change":
                    if (query != null)
                    {
                        this.executors = BD.GetExecutor();
                        this.executorsString = new List<string>();
                        for (int i = 0; i < executors.Count; i++)
                        {
                            executorsString.Add($"{this.executors[i].Surname} {this.executors[i].Name} {this.executors[i].Patronomic}");
                        }
                        if (query.Executor.Name != null)
                        {
                            if (executors.Find(x => x.Id == query.Executor.Id) == null)
                            {
                                User u = BD.GetUser(query.Executor.Id);
                                executors.Add(u);
                                executorsString.Add($"{u.Surname} {u.Name} {u.Patronomic}");
                            }
                        }
                        executor.ItemsSource = executorsString;
                        if (query.Executor.Name != null)
                        {
                            executor.SelectedIndex = executors.FindIndex(x => x.Id == query.Executor.Id);
                        }
                        decription.IsEnabled = false;
                        clients.IsEnabled = false;
                        clients.ItemsSource = new string[] { $"{this.user.Surname} {this.user.Name} {this.user.Patronomic}" };
                        clients.SelectedIndex = 0;
                        type_defect.ItemsSource = new string[] { query.Defect.Name };
                        type_defect.SelectedIndex = 0;
                        type_device.ItemsSource = new string[] { query.Device.Name };
                        type_device.SelectedIndex = 0;
                        type_device.IsEnabled = false;
                        type_defect.IsEnabled = false;
                        decription.Text = query.Desc;
                        comment.IsReadOnly = false;
                        date_create.SelectedDate = query.DateStart;
                        switch (user.Role.Id)
                        {
                            case 0:
                                save.Content = "Применить";
                                this.Title = "Просмотр заявки";
                                type_defect.IsEnabled = false;
                                type_device.IsEnabled = false;
                                save.Visibility = Visibility.Collapsed;
                                break;
                            case 3:
                            case 4:
                            case 1:
                                save.Content = "Применить";
                                status_block.Visibility = Visibility.Visible;
                                executor_block.Visibility = Visibility.Visible;
                                date_finish_block.Visibility = Visibility.Visible;
                                comment_block.Visibility = Visibility.Visible;
                                this.Title = "Изменение заявки";
                                this.Height = 450;
                                status.ItemsSource = new string[] { BD.Statuses[1].Name, BD.Statuses[4].Name };
                                status.SelectedIndex = 0;
                                date_finish.IsEnabled = true;
                                break;
                            case 2:
                                save.Content = "Применить";
                                status_block.Visibility = Visibility.Visible;
                                executor_block.Visibility = Visibility.Visible;
                                date_finish_block.Visibility = Visibility.Visible;
                                comment_block.Visibility = Visibility.Visible;
                                this.Title = "Изменение заявки";
                                this.Height = 450;
                                status.ItemsSource = new string[] { BD.Statuses[2].Name, BD.Statuses[3].Name, BD.Statuses[4].Name };
                                status.SelectedIndex = 0;
                                date_finish.IsEnabled = true;
                                break;
                        }
                    }  
                    break;
            }
        }

        private void MenuItem_Click_Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Extensions.Exit_User();
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            Extensions.Close_App();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case "add":
                    if (type_defect.SelectedIndex == -1)
                    {
                        MessageBox.Show("Необходимо выбрать тип дефекта");
                        return;
                    }
                    if (type_device.SelectedIndex == -1)
                    {
                        MessageBox.Show("Необходимо выбрать тип устройства");
                        return;
                    }
                    Query q = new Query();
                    q.Defect = defects[type_defect.SelectedIndex];
                    q.Device = devices[type_device.SelectedIndex];
                    q.Client = user;
                    q.Desc = decription.Text;
                    q.DateStart = DateTime.Now;
                    try
                    {
                        BD.AddQuery(q);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case "change":
                    if (date_finish.SelectedDate != null)
                        query.DateFinish = (DateTime)date_finish.SelectedDate;
                    if (user.Role.Id == 2)
                    {
                        query.Status.Id = status.SelectedIndex + 3;
                    } 
                    else
                    {
                        if (status.SelectedIndex == 0)
                            query.Status.Id = 2;
                        else
                            query.Status.Id = 5;
                    }
                    if (executor.SelectedIndex != null)
                        query.Executor = executors[executor.SelectedIndex];
                    query.Comment = comment.Text;
                    try
                    {
                        BD.ChangeQuery(query, user);
                        this.Close();
                    } catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
            }
            
        }
    }
}
