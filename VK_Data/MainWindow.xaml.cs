using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Enums;

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

        public VK()
        {
            appID = 6243960;
            setting = Settings.All;
        }

        VkApi vkAPI = new VkApi();
        
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
            catch (VkApiException)
            {
                error = "Ошибка получения данных пользователя";
                return error;
            }
        }

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
        
        public MainWindow()
        {  
            InitializeComponent();

        }

        private void Auth_Click(object sender, RoutedEventArgs e)
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

        private void Button_ID_Click(object sender, RoutedEventArgs e)
        {
            long temp;
            if ((ID_User.Text != "") && (long.TryParse(ID_User.Text, out temp)))
            {
                vk.userID = temp;
                Error.Content =  vk.checkGetFriend();
                if (vk.error != "Пользователя с таким ID не существует")
                {
                    ID_User.Background = Brushes.White;           
                    var tmp = vk.GetFriend();
                    List_Friends.Items.Clear();
                    foreach (var friend in tmp)
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
                    var UserInfo = vk.GetInfoUser();
                    Name.Content = UserInfo.FirstName + " " + UserInfo.LastName;
                    Status.Text = UserInfo.Status;

                    BitmapImage bitPhoto = new BitmapImage();
                    bitPhoto.BeginInit();
                    bitPhoto.UriSource = new Uri(UserInfo.PhotoPreviews.Photo200);
                    bitPhoto.EndInit();
                    Image_Photo.Stretch = Stretch.Fill;
                    Image_Photo.Source = bitPhoto;

                    Error.Content = vk.checkGetWallUser();
                    List_Post.Items.Clear();
                    var wall = vk.GetWallUser();
                    foreach(var post in wall)
                    {
                        List_Post.Items.Add(post.Text + Environment.NewLine + "Дата: " + post.Date + "     Лайки: " + post.Likes.Count + "  Репосты: " + post.Reposts.Count + Environment.NewLine);
                    }


                }
            }
            else
            {
                ID_User.Text = "ID Пользователя";
                ID_User.Background = Brushes.SkyBlue;
            }
        }

        private void ID_User_GotFocus(object sender, RoutedEventArgs e)
        {
            ID_User.Text = "";
        }

    }
}
