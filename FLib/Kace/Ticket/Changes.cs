using System;
using System.Collections.Generic;
using System.Text;

namespace FLib.Kace.Ticket
{
    public class Change
    {
        public int id { get; set; }
        public int hd_ticket_id { get; set; }
        public string timestamp { get; set; }
        public int user_id { get; set; }
        public string comment { get; set; }
        public string description { get; set; }
        public string owners_only_description { get; set; }
        public int owners_only { get; set; }
        public KUser user { get; set; }
    }

    public class ChangeList
    {
        public int Count { get; set; }
        public List<object> Warnings { get; set; }
        public List<Change> Changes { get; set; }
    }
}
