using System;
using System.Collections.Generic;
using System.Text;

namespace FLib.Kace
{
    public class KUser
    {
        public int id { get; set; }
        public string user_name { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public string location { get; set; }

        public string getShortName()
        {
            if ((full_name != null) && (full_name.Contains(", ")) && (full_name.Contains(" (")))
            {
                string name = full_name;
                string shortName = "";
                name = name.Substring(0, name.IndexOf(" ("));
                shortName = name.Substring(0, name.IndexOf(", ")) + name.Substring(name.IndexOf(", ") + 2, 1).ToUpper();
                return shortName;
            }
            return "";
        }

        public string getLocation()
        {
            if ((full_name != null) && (full_name.Contains(")")) && (full_name.Contains(" (")))
            {
                string name = full_name;
                string shortName = "";
                shortName = name.Substring(name.IndexOf(" (") + 1);
                
                return shortName;
            }
            return "";
        }

    }
}
