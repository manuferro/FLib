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
using System.Windows;

namespace FLib.Kace
{
    public class KaceClient
    {
        public string Login { get; set; }
        public string Passwd { get; set; }
        public string Addr { get; set; }
        public string Organization { get; set; }
        public TicketList Tickets { get; set; }
        public Ticket.Ticket CurrentTicket { get; set; }
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
        }

        private bool prepareRequest(string url, Method method, bool isLogin = false)
        {
            _request = new RestRequest(url, method, RestSharp.DataFormat.Json);
            _request.RequestFormat = RestSharp.DataFormat.Json;
            _request.Method = method;
            if ((!isLogin) && (_responseLogin != null))
            {

                if ((_response.Cookies != null))
                {
                    foreach (RestResponseCookie c in _responseLogin.Cookies)
                        _request.AddCookie(c.Name, c.Value.ToString());
                }

                if ((_responseLogin.Headers != null))
                {

                    foreach (Parameter p in _responseLogin.Headers)
                        if (p.Name.StartsWith("x-dell"))
                            _request.AddHeader(p.Name, p.Value.ToString());
                }

                _request.AddHeader("x-dell-api-version", Const.KACE_VERSION);
            }
           
            return false;
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


        private bool getGenericRequest(string url,  int limit = 100, string filter = "", string shaping = "")
        {
            prepareRequest(url, Method.GET, false);
            if (shaping != "") _request.AddParameter("shaping", shaping);
            if (limit > 0) _request.AddParameter("paging", "limit " + limit);
            if (filter != "") _request.AddParameter("filtering", filter);
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
                return true;
            }
            return false;
        }

        public bool getTicketList(string search="", int limit = 50)
        {
            if (limit > 500) limit = 500;
            bool _result = getGenericRequest(Const.URL_TICK, limit, search, Const.TICK_SHAPING);
            if (_result)
            {
                try
                {
                    Tickets = JsonConvert.DeserializeObject<TicketList>(_responseContent);
                }
                catch (Exception ex) { return false; }
                return true;
            }

            return false;
        }

        public bool getTicketListFromFile(string fileName = "")
        {
            if (File.Exists(fileName))
            {
                try
                {
                    Tickets = JsonConvert.DeserializeObject<TicketList>(File.ReadAllText(fileName));
                }
                catch (Exception ex) { return false; }
                return true;
            }

            return false;

        }


        public bool getTicketDetails(int id)
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

        private bool getTicketChanges(int id)
        {
            return getGenericRequest(Const.URL_TICK_CHANGES.Replace("$$ID$", id.ToString()), 0, "", "");
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
