using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat
{
    [Serializable]
    public class UserLogin
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string AvataPath { get; set; }
        public string Name { get; set; }
    }
}