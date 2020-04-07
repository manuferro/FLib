using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace FLib.Kace.Ticket
{
    public class Ticket
    {
        public int id { get; set; }
        public int hd_queue_id { get; set; }
        public string title { get; set; }
        public int custom_1 { get; set; }   //time in minutes
        public string custom_16 { get; set; }
        public string custom_4 { get; set; }    //Phone call to tech,
        public string resolution { get; set; }
        public KCategory category { get; set; }
        public KUser owner { get; set; }
        public KUser submitter { get; set; }
        public KStatus status { get; set; }
        public KPriority priority { get; set; }
        public KImpact impact { get; set; }
        public Visibility closed_visibility {
            get { 
                
                if ((status != null) && (status.state == "closed")) return Visibility.Hidden; 
                return Visibility.Visible; 
            }
        } 
    }

    public class KPriority
    {
        public int id { get; set; }
        public string name { get; set; }
        public int ordinal { get; set; }
        public string color { get; set; }
    }

    public class KImpact
    {
        public int id { get; set; }
        public string name { get; set; }
        public int ordinal { get; set; }

    }

    public class KStatus
    {
        public int id { get; set; }
        public string name { get; set; }
        public int ordinal { get; set; }
        public string state { get; set; }
    }


    public class KCategory
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
