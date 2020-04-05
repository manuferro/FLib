﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FLib.Kace
{
    public class KUserList
    {
        public List<KUser> Users { get; set; }

        public KUserList(string Json)
        {
            if (Json != null)
                getFromJson(Json);
        }

        public void getFromJson(string Json)
        {
            Object test = JsonConvert.DeserializeObject(Json);

            var userList = JsonConvert.DeserializeObject<KUserList>(Json);
            Users = userList.Users;
        }
    }
}
