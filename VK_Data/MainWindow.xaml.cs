using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using System.Threading;

namespace VK_Data
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public class VK
    {
        public int appID { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public long userID { get; set; }
        public string error { get; set; }
        public Settings setting { get; set; }

        private VkApi vkAPI = new VkApi();

        public VK()
        {
            appID = 6243960;
            setting = Settings.All;
        }
        
        public string Auth()
        {
            try
            {
                vkAPI.Authorize(appID, email, password, setting);
                error = "";
                return error;
            }
            catch (VkApiAuthorizationException)
            {
                error = "Неверные данные авторизации";
                return error;
            }      
        }

        public string checkGetFriend()
        {
            try
            {
                var tmp = vkAPI.Friends.Get(userID, ProfileFields.FirstName | ProfileFields.LastName);
                error = "";
                return error;

            }
            catch (VkApiException)
            {
                error = "Пользователя с таким ID не существует";
                return error;
            }
        }

        public ReadOnlyCollection<VkNet.Model.User> GetFriend()
        {
                var temp = vkAPI.Friends.Get(userID, ProfileFields.FirstName | ProfileFields.LastName);
                return temp;            
        }

        public string checkGetInfoUser()
        {
            try
            {
                var user = vkAPI.Users.Get(userID, ProfileFields.All);
                error = "";
                return error;

            }
            /*catch (VkApiException)
            {
                error = "Ошибка получения данных пользователя";
                return error;
            }*/
            catch (InvalidParameterException)
            {
                error = "Ошибка получения данных пользователя";
                return error;
            }
        }

        /*public string checkCountRequest()
        {
            try
            {
                var tmp = vkAPI.Users.Get("14534534", ProfileFields.All);
                error = "";
                return error;
            }
            catch(TooManyRequestsException)
            {
                error = "Слишком много запросов";
                return error;
            }
        }*/

        public VkNet.Model.User GetInfoUser()
        {
            var user = vkAPI.Users.Get(userID, ProfileFields.All);
            return user;
        }

        public string checkGetWallUser()
        {
            try
            {
                int i = 3;
                var user = vkAPI.Wall.Get(userID, out i);
                error = "";
                return error;

            }
            catch (VkApiException)
            {
                error = "Ошибка получения стены пользователя";
                return error;
            }
        }

        public ReadOnlyCollection<VkNet.Model.Post> GetWallUser()
        {
            int tmp = 3;
            var wall = vkAPI.Wall.Get(userID, out tmp);
            return wall;
        }

    }


    public partial class MainWindow : Window
    {
        VK vk = new VK();

        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {  
            InitializeComponent();
        }

        private async Task<UpdateData> UpdateData()
        {
            return await Task.Run(() =>
            {
                List<Thread> threadList = new List<Thread>();
                var data = new UpdateData();

                threadList.Add(new Thread(() => 
                {
                    data.friend = vk.GetFriend();
                }));

                threadList.Add(new Thread(() => 
                {
                    data.infoUser = vk.GetInfoUser();
                    
                }));

                threadList.Add(new Thread(() =>
                {
                    data.wall = vk.GetWallUser();
                }));         

                foreach (Thread thread in threadList)
                {
                    thread.Start();
                }

                foreach (Thread thread in threadList)
                {
                    thread.Join();
                }            
                               
                return data;
            });
        }

        private async void Auth_Click(object sender, RoutedEventArgs e)
        {
            if (Login.IsEnabled)
            {   
                if((Login.Text != "") && (Login.Text != "Логин"))
                {
                    Login.Background = Brushes.White;
                    if ((Password.Password != "") && (Password.Password != "Пароль"))
                    {
                        vk.email = Login.Text;
                        vk.password = Password.Password;

                        Error.Content = vk.Auth();                        
                        if (vk.error != "Неверные данные авторизации")
                        {      
                            Password.Background = Brushes.White;
                            Login.IsEnabled = false;
                            Password.IsEnabled = false;
                            Button_ID.IsEnabled = true;
                            Auth.Content = "Выйти";
                        }
                        else
                        {
                            Login.Text = "";
                            Password.Password = "";
                            
                        }
                    }
                    else
                    {
                        Password.Password = "Пароль";
                        Password.Background = Brushes.SkyBlue;
                    }
                } 
                else
                {
                    Login.Text = "Логин";
                    Login.Background = Brushes.SkyBlue;
                }
                
            }
            else
            {
                Login.Text = "";
                Password.Password = "";
                Login.IsEnabled = true;
                Password.IsEnabled = true;
                Button_ID.IsEnabled = false;
                Auth.Content = "Войти";
            }
        }

        private void Login_GotFocus(object sender, RoutedEventArgs e)
        {
            Login.Text = "";
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            Password.Password = "";
        }

        private async void timer_tick(object sender, EventArgs e)
        {       
            List_Friends.Items.Clear();
            List_Post.Items.Clear();

            var result = await UpdateData();

            foreach (var friend in result.friend)
            {
                if (friend.Online == true)
                {
                    List_Friends.Items.Add(friend.FirstName + " " + friend.LastName + "  ●");
                }
                else
                {
                    List_Friends.Items.Add(friend.FirstName + " " + friend.LastName);
                }
            }

            foreach (var post in result.wall)
            {
                List_Post.Items.Add(post.Text + Environment.NewLine + "Дата: " + post.Date + "     Лайки: " + post.Likes.Count + "  Репосты: " + post.Reposts.Count + Environment.NewLine);
            }

            updateTime.Content = System.DateTime.Now.ToLongTimeString();

        }

        private async void Button_ID_Click(object sender, RoutedEventArgs e)
        {
            long temp;
            //Error.Content = vk.checkCountRequest();
            //if (vk.error != "Слишком много запросов")
            //{
                if ((ID_User.Text != "") && (long.TryParse(ID_User.Text, out temp)))
                {
                    vk.userID = temp;
                    Error.Content = vk.checkGetFriend();
                    if (vk.error != "Пользователя с таким ID не существует")
                    {
                        ID_User.Background = Brushes.White;

                        var data = await UpdateData();

                        timer_tick(sender, e);

                        Name.Content = data.infoUser.FirstName + " " + data.infoUser.LastName;
                        Status.Text = data.infoUser.Status;

                        BitmapImage bitPhoto = new BitmapImage();
                        bitPhoto.BeginInit();
                        bitPhoto.UriSource = new Uri(data.infoUser.PhotoPreviews.Photo200);
                        bitPhoto.EndInit();

                        Image_Photo.Stretch = Stretch.Fill;
                        Image_Photo.Source = bitPhoto;
                      
                        Error.Content = vk.checkGetWallUser();

                        timer.Tick += new EventHandler(timer_tick);
                        timer.Interval = new TimeSpan(0, 0, 20);
                        timer.Start();

                    }
                }
                else
                {
                    ID_User.Text = "ID Пользователя";
                    ID_User.Background = Brushes.SkyBlue;
                }
            //}
        }

        private void ID_User_GotFocus(object sender, RoutedEventArgs e)
        {
            ID_User.Text = "";
        }

    }
}
