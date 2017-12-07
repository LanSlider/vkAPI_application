using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using VK_App;
using System.Collections.Generic;

namespace VK_Data
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {  
            InitializeComponent();
        }

        private async Task<UpdateData> UpdateData(int id)
        {
            return await Task.Run(() =>
            {
                ServiceReference.ServiceClient client = new ServiceReference.ServiceClient();
                UpdateData data = new UpdateData();
                //try
                //{
                    string result = client.GetData(id);
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(result);
                    writer.Flush();
                    stream.Position = 0;
                    var dcs = new DataContractSerializer(typeof(UpdateData));
                    data = (UpdateData)dcs.ReadObject(stream);

                    return data;
                //}
                //catch (Exception ex)
                /*{
                    List<string> errorList = new List<string>();
                    errorList.Add("Сервис занят");
                    data.errorList = errorList;
                    return data;
                }*/
            });
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
            List_Error.Items.Clear();

            var result = await UpdateData(Convert.ToInt32(ID_User.Text));

            foreach(var friend in result.friendList)
            {
                List_Friends.Items.Add(friend);
            }

            foreach(var post in result.wallList)
            {
                List_Post.Items.Add(post);
            }

            foreach(var error in result.errorList)
            {
                List_Error.Items.Add(error);
            }


            if (result.infoUser[2] == "True")     
                Online.Foreground = System.Windows.Media.Brushes.GreenYellow;
            else
                Online.Foreground = System.Windows.Media.Brushes.White;

            updateTime.Content = result.updateTime;
        }

        private async void Button_ID_Click(object sender, RoutedEventArgs e)
        {
            long temp;

            if ((ID_User.Text != "") && (long.TryParse(ID_User.Text, out temp)) && (ID_User.Text.Length < 10))
            {
                timer_tick(sender, e);
                var data = await UpdateData(Convert.ToInt32(ID_User.Text));

                Online.Visibility = Visibility;
                Name.Content = data.infoUser[0];
                Status.Text = data.infoUser[1];

                Bitmap bitPhoto = new Bitmap(data.bitPhoto);

                MemoryStream ms = new MemoryStream();
                bitPhoto.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                Image_Photo.Source = bi;

                timer.Tick += new EventHandler(timer_tick);
                timer.Interval = new TimeSpan(0, 0, 20);
                timer.Start();
            }
            else
            {
                ID_User.Text = "ID Пользователя";
                ID_User.Background = System.Windows.Media.Brushes.SkyBlue;
            }
        }

        private void ID_User_GotFocus(object sender, RoutedEventArgs e)
        {
            ID_User.Text = "";
        }

    }
}
