using System;
using System.Text;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Stuff
{
    public class HTTPManager
    {
        private string serverUrl;
        private string pathToMethods;
        public dynamic token;

        public HTTPManager()
        {
            serverUrl = ConfigurationManager.AppSettings["WebServerRoot"];
            pathToMethods = ConfigurationManager.AppSettings["PathToMethods"];
        }

        public httpAnswer Authenticate(string login, string mdp)
        {
            LoginPW parameters = new LoginPW()
            {
                username = login,
                password = mdp
            };
            string jsonParameters = JsonConvert.SerializeObject(parameters);
            httpAnswer result = sendHttpRequest("authenticate",jsonParameters,"");
            return result;
        }

        public httpAnswer sendHttpRequest(string methodName,string jsonParameters = "", string token = "",string mode = "POST")
        {
            httpAnswer result = null;
            string url = serverUrl + pathToMethods + methodName;
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            string postData = jsonParameters;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json; charset=utf-8";
            if(token != "")
            {
                request.Headers.Add("X-Api-Token", token);

            }
            request.Headers.Add(HttpRequestHeader.Pragma, "no-cache");
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            try {
                WebResponse response = request.GetResponse();

                using (dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);

                    string responseFromServer = reader.ReadToEnd();
                    dynamic resultRaw = JsonConvert.DeserializeObject<dynamic>(responseFromServer);
                    string innerAnswer = JsonConvert.SerializeObject(resultRaw.d);

                    result = new httpAnswer()
                    {
                        d = new httpResult
                        {
                            error = false,
                            errorNumber = 200,
                            message = innerAnswer
                        }
                    };
                }

            }
            catch (Exception e)
            {
                int errorNumber;
                result = new httpAnswer() {
                    d = new httpResult
                    {
                        error = true,
                        errorNumber = int.TryParse(Regex.Match(e.Message, @"\d+\.*\d+").Value, out errorNumber) ? errorNumber:-1,
                        message = e.Message
                     }
                };
            }
            return result;
        }

        public class LoginPW
        {
            public string username { get; set; }

            public string password { get; set; }
        }

        public class httpAnswer
        {
            public httpResult d { get; set; }
        }

        public class httpResult
        { 
            public bool error { get; set; }
            public int errorNumber { get; set; }
            public string message { get; set; }
        }
    }
}
