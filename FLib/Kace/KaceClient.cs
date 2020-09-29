using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using FLib.Kace.Ticket;
using FLib.FType;
using System.Windows;
using System.Collections.Specialized;

namespace FLib.Kace
{
    public class KaceClient
    {
        private string _login = "";
        public string Login {
            get { return _login; }
            set { _login = value; setMeUsername(value); }
        }
        public string Passwd { get; set; }
        public string Addr { get; set; }
        public string Organization { get; set; }
        public TicketList Tickets { get; set; }
        public Ticket.Ticket CurrentTicket { get; set; }
        public FKeyValueStringList Token_cookies { get; set; }
        public FKeyValueStringList Token_headers { get; set; }
        public KUser me { get; set; }

        private RestClient _client = null;
        private RestRequest _request = null;
        private IRestResponse _response = null;
        private IRestResponse _responseLogin = null;
        private HttpStatusCode _statusCode;
        
        private string _status;
        private string _responseContent;
        private bool _loggedIn = false;

        


        public KaceClient()
        {
            Organization = "Default";
            Token_cookies = new FKeyValueStringList();
            Token_headers = new FKeyValueStringList();
        }

        private bool prepareRequest(string url, Method method, bool isLogin = false)
        {
            _request = new RestRequest(url, method, RestSharp.DataFormat.Json);
            _request.RequestFormat = RestSharp.DataFormat.Json;
            _request.Method = method;

            if ((!isLogin) )
            {

                if ((Token_cookies != null) && (Token_cookies.Count >= 0))
                {
                    foreach (FKeyValueString entry in Token_cookies)
                    {
                        _request.AddCookie(entry.Key, entry.Value);
                    }
                }

                if ((Token_headers != null) && (Token_headers.Count>0))
                {
                    foreach (FKeyValueString entry in Token_headers)
                    {
                        if (entry.Key.StartsWith("x-dell"))
                            _request.AddHeader(entry.Key, entry.Value);
                    }
                }
                _request.AddHeader("x-dell-api-version", Const.KACE_VERSION);
            }
            
           
            return false;
        }


        private void saveResponseCookies()
        {
            
            StringCollection s = new StringCollection();

            if ((_response.Cookies != null))
            {
                if (Token_cookies == null) Token_cookies = new FType.FKeyValueStringList();
                else Token_cookies.Clear();
                foreach (RestResponseCookie c in _responseLogin.Cookies)
                    Token_cookies.Add(c.Name, c.Value);
            }
        }

        private void saveResponseHeaders()
        {

                if ((_responseLogin.Headers != null))
            {
                if (Token_headers == null) Token_headers = new FType.FKeyValueStringList();
                else Token_headers.Clear();
                foreach (Parameter p in _responseLogin.Headers)
                    Token_headers.Add(p.Name, p.Value.ToString());
                
            }


        }

        private bool kaceRequest(  )
        {
            if (_client == null) _client = new RestClient(Addr);

            if (_request.Method == Method.POST)
                _response = _client.Post(_request);
            else if (_request.Method == Method.GET)
            {
                _response = _client.Get(_request);
            }
            else if (_request.Method == Method.PUT)
                _response = _client.Put(_request);

            _statusCode = _response.StatusCode;
            _status = _response.StatusDescription;
            _responseContent = _response.Content;

            if (_statusCode == HttpStatusCode.OK) return true;
            return false;
        }

        private bool putGenericUpdate(string url, string jsonData)
        {
            prepareRequest(url, Method.PUT, false);
            _request.AddParameter("application/json", jsonData, ParameterType.RequestBody);// AddJsonBody(tickets);
            return kaceRequest();

        }


        private bool getGenericRequest(string url,  int limit = 100, string filter = "", string shaping = "", string sorting = "")
        {
            prepareRequest(url, Method.GET, false);
            if (shaping != "") _request.AddParameter("shaping", shaping);
            if (limit > 0) _request.AddParameter("paging", "limit " + limit);
            if (filter != "") _request.AddParameter("filtering", filter);
            if (sorting != "") _request.AddParameter("sorting", sorting);

            return kaceRequest();

        }


        public bool login()
        {
            prepareRequest(Const.URL_LOGIN, Method.POST,true);
            var param = new KaceLogin { userName = Login, password = Passwd, organizationName = Organization };
            _request.AddJsonBody(param);
            if (kaceRequest())
            {
                _responseLogin = _response;
                //Token = (RestResponse)_response;
                saveResponseCookies();
                saveResponseHeaders();
                //getToken(); //save token for next run
                return true;
            }
            return false;
        }



        /// <summary>
        /// Retrive the ticket list from Kace server (after login)
        /// </summary>
        /// <param name="search"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public bool getTicketList(string search="", int limit = 50, string sorting = "created desc")
        {
            if (limit > 500) limit = 500;
            
            bool _result = getGenericRequest(Const.URL_TICK, limit, search, Const.TICK_SHAPING, sorting);
            if (_result)
            {
                try
                {
                    Tickets = JsonConvert.DeserializeObject<TicketList>(_responseContent);
                    Tickets.me = me;
                }
                catch (Exception ex) { return false; }
                return true;
            }

            return false;
        }




        /// <summary>
        /// Retrive the ticket list from a json file (mainly for test purpouse)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool getTicketListFromFile(string fileName = "")
        {
            if (File.Exists(fileName))
            {
                try
                {
                    Tickets = JsonConvert.DeserializeObject<TicketList>(File.ReadAllText(fileName));
                    Tickets.me = me;

                }
                catch (Exception ex) { return false; }
                return true;
            }

            return false;

        }

        public void setMeUsername(string value)
        {
            if (me == null) me = new KUser();
            me.user_name = value;
        }

        public  bool getTicketDetails(int id)
        {
            bool _result =  getGenericRequest(Const.URL_TICK_DETAIL.Replace("$$ID$",id.ToString()), 0, "", Const.TICK__DETAILS_SHAPING);

            if (_result)
            {
                try
                {
                    TicketList _tickets = JsonConvert.DeserializeObject<TicketList>(_responseContent);
                    if (_tickets.Count == 1)
                    {
                        CurrentTicket = _tickets.Tickets[0];
                        Tickets.updateTicket(CurrentTicket);
                    }
                }
                catch (Exception ex) { return false; }
                return true;
            }

            return false;

        }

        public bool getTicketChanges(int id)
        {
            bool _result = getGenericRequest(Const.URL_TICK_CHANGES.Replace("$$ID$", id.ToString()),100,"",Const.TICK_CHANGES_SHAPING);

            if (_result)
            {
                try
                {
                    ChangeList _changes = JsonConvert.DeserializeObject<ChangeList>(_responseContent);
                    CurrentTicket.changes = _changes.Changes;
                }
                catch (Exception ex) { return false; }
                return true;
            }

            return false;
        }


        /// <summary>
        /// update ticket via put
        /// strating from json parameter
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public bool updateTicket(int id, string jsonData)
        {
            string url = Const.URL_TICK_DETAIL.Replace("$$ID$", id.ToString());
            
            bool _result = putGenericUpdate(url, jsonData);

            return _result;
        }

        public bool updateTicketSingleParam(int id, string param, string value)
        {
            return updateTicket(id, @"{ ""Tickets"": [ { """+ param + @""": """ + value + @"""   }  ]}");
        }


        /// <summary>
        /// update just the title of the ticket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newTitle"></param>
        /// <returns></returns>
        public bool updateTicketTitle(int id, string newTitle)
        {
            return updateTicketSingleParam(id, "title", newTitle);
        }


        public string getUrlticket(int id)
        {
            string url = Addr + Const.URL_TICK_LINK.Replace("$$ID$", id.ToString());
            return url;
        }

        public string Status { get { return _status; } }
        public string Content { get { return _responseContent; } }

    }




    public class KaceLogin
    {
        public string password { get; set; }
        public string userName { get; set; }
        public string organizationName { get; set; }
    }
}
