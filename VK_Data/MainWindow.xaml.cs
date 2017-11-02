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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;

namespace VK_Data
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    class VK
    {
        public int appID { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public Settings setting { get; set; }

        public VK()
        {
            appID = 6243960;
            setting = Settings.All;
        }

        VkApi vkAPI = new VkApi();
        
        public void Auth()
        {
            try
            {
                vkAPI.Authorize(appID, email, password, setting);
                
            }
            catch(VkApiAuthorizationException)
            {
                MessageBox.Show("Неверные данные!");
            }
        }

        public void Status()
        {
            Console.WriteLine(vkAPI.Status);
        }

        




    }


    public partial class MainWindow : Window
    {
        VK vk = new VK();
        

        public MainWindow()
        {
            
            
            InitializeComponent();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            

        }

        private void Auth_Click(object sender, RoutedEventArgs e)
        {
            if (Login.IsEnabled)
            {
                vk.email = Login.Text;
                vk.password = Password.Password;
                vk.Auth();
                Login.IsEnabled = false;
                Password.IsEnabled = false;
                Auth.Content = "Выйти";
            }
            else
            {
                Login.Text = "";
                Password.Password = "";
                Login.IsEnabled = true;
                Password.IsEnabled = true;
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
    }
}
