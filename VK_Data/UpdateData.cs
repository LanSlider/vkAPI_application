using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Drawing;

namespace VK_App
{
    [DataContract]
    class UpdateData
    {
        [DataMember]
        public string[] infoUser { get; set; }

        [DataMember]
        public Bitmap bitPhoto { get; set; }

        [DataMember]
        public List<string> wallList { get; set; }

        [DataMember]
        public List<string> friendList { get; set; }

        [DataMember]
        public string updateTime { get; set; }

        [DataMember]
        public List<string> errorList { get; set; }        
    }
}
