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
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public KCategory category { get; set; }
        public KUser owner { get; set; }
        public KUser submitter { get; set; }
        public KStatus status { get; set; }
        public KPriority priority { get; set; }
        public KImpact impact { get; set; }
        public List<Change> changes { get; set; }
        public List<SlaDate> sla_dates { get; set; }
        public string resolution_plaintext { get; set; }


        public Visibility closed_visibility {
            get { 
                
                if ((status != null) && (status.state == "closed")) return Visibility.Hidden; 
                return Visibility.Visible; 
            }
        } 

        public string plainResolution
        {
            get
            {
                if (resolution == null) return "";
                try
                {
                    string _plainResolution = System.Web.HttpUtility.HtmlDecode(resolution);
                    return _plainResolution.Replace("<br>", "\r\n").Replace("<BR>", "\r\n").Replace("<br />", "\r\n").Replace("<BR />", "\r\n");
                }
                catch(Exception ex) { return ""; }
                
            }
            set
            {
                resolution = value;
            }
        }

        public KUser me { get; set; }
        public string manuele
        {
            get { return "pippo"; }
            set { }
        }

        public int isOwnerMe
        {
            get { if ((me != null) && (owner != null) && (me.user_name == owner.user_name)) return 1; return 0; }
            set { }
        }


    }

    public class StandardTicketValues
    {
        public string title { get; set; }
        public int custom_1 { get; set; }   //time in minutes
        public string custom_16 { get; set; }
        public string custom_4 { get; set; }    //Phone call to tech,
        public string resolution { get; set; }
        public KCategory category { get; set; }
        public KUser owner { get; set; }
        public KStatus status { get; set; }
        public KPriority priority { get; set; }
        public KImpact impact { get; set; }
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

    public class SlaDate
    {
        public int priority_id { get; set; }
        public string resolution_date { get; set; }
    }
}
