using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Enums;
using System.Collections.ObjectModel;

namespace VK_Data
{
    class UpdateData
    {
        public ReadOnlyCollection<VkNet.Model.User> friend { get; set; }
        public VkNet.Model.User infoUser { get; set; }
        public BitmapImage bitPhoto { get; set; }
        public ReadOnlyCollection<VkNet.Model.Post> wall { get; set; }

    }
}
