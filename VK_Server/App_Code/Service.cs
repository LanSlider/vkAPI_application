using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using VK_App;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;

// ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Реструктуризация" можно использовать для одновременного изменения имени класса "Service" в коде, SVC-файле и файле конфигурации.
public class Service : IService
{
    VK vk = new VK();

    public class VK
    {
        public int appID { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public long userID { get; set; }
        public Settings setting { get; set; }

        public List<string> errorList = new List<string>();

        private VkApi vkAPI = new VkApi();

        public VK()
        {
            appID = 6288455;
            email = "+375293227590";
            password = "123789654a";
            setting = Settings.All;
        }

        public void Auth()
        {
            try
            {
                vkAPI.Authorize(appID, email, password, setting);
            }
            catch (VkApiAuthorizationException)
            {
                errorList.Add("Неверные данные авторизации");
            }
        }

        public List<string> GetFriend()
        {
            List<string> friends = new List<string>();
            try
            {
                var temp = vkAPI.Friends.Get(userID, ProfileFields.FirstName | ProfileFields.LastName);

                foreach (var friend in temp)
                {
                    if (friend.Online == true)
                    {
                        friends.Add(friend.FirstName + " " + friend.LastName + "  ●");
                    }
                    else
                    {
                        friends.Add(friend.FirstName + " " + friend.LastName);
                    }
                }

                return friends;
            }
            catch (VkApiException)
            {
                errorList.Add("Пользователя с таким ID не существует");
                return friends;
            }
        }

        public Bitmap GetPhoto()
        {
            Bitmap loadedBitmap = null;
            try
            {
                var user = vkAPI.Users.Get(userID, ProfileFields.All);

                var request = System.Net.WebRequest.Create(user.PhotoPreviews.Photo200);
                var response = request.GetResponse();

                using (var responseStream = response.GetResponseStream())
                {
                    loadedBitmap = new Bitmap(responseStream);
                }

                return loadedBitmap;
            }
            catch
            {
                errorList.Add("Ошибка получения фото пользователя");
                return loadedBitmap;
            }
        }

        public string[] GetInfoUser()
        {
            string[] infoUser = { "Name", "Status", "Online" };
            try
            {
                var user = vkAPI.Users.Get(userID, ProfileFields.All);

                infoUser[0] = user.FirstName + " " + user.LastName;
                infoUser[1] = user.Status;
                infoUser[2] = user.Online.ToString();

                return infoUser;
            }
            catch (VkApiException)
            {
                errorList.Add("Ошибка получения данных пользователя");
                return infoUser;
            }
        }

        public List<string> GetWallUser()
        {
            List<string> wallList = new List<string>();
            try
            {
                int tmp = 3;
                var wall = vkAPI.Wall.Get(userID, out tmp);

                foreach (var post in wall)
                {
                    wallList.Add(post.Text + Environment.NewLine + "Дата: " + post.Date + "     Лайки: " + post.Likes.Count + "  Репосты: " + post.Reposts.Count + Environment.NewLine);
                }
                return wallList;
            }
            catch (VkApiException)
            {
                errorList.Add("Ошибка получения стены пользователя");
                return wallList;
            }
        }

    }



    public string GetData(int value)
    {
        UpdateData data = new UpdateData();
        List<Thread> threadList = new List<Thread>();

        vk.Auth();
        vk.userID = value;

        threadList.Add(new Thread(() => { data.friendList = vk.GetFriend(); }));
        threadList.Add(new Thread(() => { data.infoUser = vk.GetInfoUser(); }));
        threadList.Add(new Thread(() => { data.bitPhoto = vk.GetPhoto(); }));
        threadList.Add(new Thread(() => { data.wallList = vk.GetWallUser(); }));
        threadList.Add(new Thread(() => { data.errorList = vk.errorList; }));

        foreach (Thread thread in threadList)
        {
            thread.Start();
        }

        foreach (Thread thread in threadList)
        {
            thread.Join(new TimeSpan(0, 0, 6));
        }

        data.updateTime = DateTime.Now.ToString();

        var serializer = new DataContractSerializer(typeof(UpdateData));

        using (MemoryStream stream = new MemoryStream())
        {

            serializer.WriteObject(stream, data);

            stream.Position = 0;
            TextReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
