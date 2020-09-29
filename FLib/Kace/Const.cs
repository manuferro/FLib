using System;
using System.Collections.Generic;
using System.Text;

namespace FLib.Kace
{
    class Const
    {
        public static string KACE_VERSION = "9";
        public static string URL_LOGIN = @"ams/shared/api/security/login";
        public static string URL_TICK = @"/api/service_desk/tickets/";
        public static string URL_TICK_DETAIL = @"/api/service_desk/tickets/$$ID$";
        public static string URL_TICK_CHANGES = @"/api/service_desk/tickets/$$ID$/changes";
        public static string URL_TICK_LINK = @"/adminui/ticket.php?ID=$$ID$";



        public static string TICK_SHAPING = @"hd_ticket all,category limited,owner limited,submitter limited,status limited,priority limited,impact limited";
        public static string TICK_CHANGES_SHAPING = @"hd_ticket_change regulat, user limited, attachments regular";
        public static string TICK__DETAILS_SHAPING = @"hd_ticket all, category limited,owner limited, submitter limited,status limited, priority limited,impact limited,changes all";
    }
}
