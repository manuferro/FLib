using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using FLib.Kace.Ticket;

namespace FLib.Kace
{
    public class KaceClient
    {
        public string Login { get; set; }
        public string Passwd { get; set; }
        public string Addr { get; set; }
        public string Organization { get; set; }
        public TicketList Tickets { get; set; }
        private RestClient _client = null;
        private RestRequest _request = null;
        private IRestResponse _response = null;
        private HttpStatusCode _statusCode;
        private string _status;
        private string _responseContent;
        private bool _loggedIn = false;


        public KaceClient()
        {
            Organization = "Default";
        }

        private bool kaceRequest(string path,  RestRequest request, bool isLogin = false)
        {
            if (_client == null) _client = new RestClient(Addr);
            request.RequestFormat = DataFormat.Json;
            //add standard header from previous response
            if (!isLogin)
            {
                request.AddHeader("x-dell-api-version", Const.KACE_VERSION);
                if ((_response != null) && (_response.Cookies != null))
                    foreach (RestResponseCookie c in _response.Cookies)
                        request.AddCookie(c.Name, c.Value.ToString());

                if ((_response != null) && (_response.Headers != null))
                    foreach (Parameter p in _response.Headers)
                        if (p.Name.StartsWith("x-dell"))
                            request.AddHeader(p.Name, p.Value.ToString());
            }

            //_request = request;

            if (request.Method == Method.POST)
                _response = _client.Post(request);
            else if (request.Method == Method.GET)
                _response = _client.Get(request);
            else if (request.Method == Method.PUT)
                _response = _client.Put(request);

            _statusCode = _response.StatusCode;
            _status = _response.StatusDescription;
            _responseContent = _response.Content;

            if (_statusCode == HttpStatusCode.OK) return true;
            return false;
        }

        public bool login()
        {

            var param = new KaceLogin { userName = Login, password = Passwd, organizationName = Organization };
            _request = new RestRequest(Addr, Method.POST, DataFormat.Json);
            _request.RequestFormat = DataFormat.Json;
            _request.AddJsonBody(param);

            if (kaceRequest(Const.URL_LOGIN, _request)) 
            {
                _loggedIn = true;
                return true;
            }
            return false;
        }

        private bool getGenericRequest(string url, int limit = 100, string filter = "", string shaping = "")
        {
            _request = new RestRequest(Addr, Method.GET, DataFormat.Json);
            _request.RequestFormat = DataFormat.Json;
            if (shaping != "") _request.AddParameter("shaping", shaping);
            if (limit > 0) _request.AddParameter("paging", "limit " + limit);
            if (filter != "") _request.AddParameter("filtering", filter);
            return (kaceRequest(url, _request));
        }


        public bool getTicketList(int limit = 100, string filter = "")
        {
            bool _result =  getGenericRequest(Const.URL_TICK, limit, filter, Const.TICK_SHAPING);
            if (_result)
            {
                Tickets = JsonConvert.DeserializeObject<TicketList>(_responseContent);
            }
            return _result;
        }

        private bool getTicketDetails(int id)
        {
            return getGenericRequest(Const.URL_TICK_DETAIL.Replace("$$ID$",id.ToString()), 0, "", "");
        }

        private bool getTicketChanges(int id)
        {
            return getGenericRequest(Const.URL_TICK_CHANGES.Replace("$$ID$", id.ToString()), 0, "", "");
        }

        public string Status { get { return _status; } }

    }

    public class KaceLogin
    {
        public string password { get; set; }
        public string userName { get; set; }
        public string organizationName { get; set; }
    }
}
