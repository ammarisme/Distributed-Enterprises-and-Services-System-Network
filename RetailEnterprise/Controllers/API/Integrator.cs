using Newtonsoft.Json.Linq;
using RetailEnterprise.DAL;
using RetailEnterprise.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace RetaiEnterprise.Controllers.API
{
    public class Integrator
    {
        public string IntegrationSystemUrl = "http://localhost:51980";

        private ApplicationDbContext db { get; set;}

        public string SystemIdNumber { get; set; }

        public Integrator(){
            db = new ApplicationDbContext();

            Setting setting = db.Settings.Find(1);
            SystemIdNumber = setting.SystemIdNumber;
        }

        // send a json object to the route in apiRoute
        // apiRoute must be like /api/{controller}/{action}

        public HttpWebResponse sendJsonObject(JObject jsonBody, string apiRoute)
        {
            // creating the http request
            var http = (HttpWebRequest) WebRequest.Create(new Uri(IntegrationSystemUrl+apiRoute));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            if (jsonBody["EnterpriseId"] == null)
            {
                jsonBody.Add("EnterpriseId", SystemIdNumber);
            }

            // parsing the json object
            string parsedContent = jsonBody.ToString();
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            // opeing a new stream and gettin data
            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();
            HttpWebResponse response = null;
            try
            {
            response = (HttpWebResponse)http.GetResponse();
            }
            catch (WebException ex)
            {
                Console.Write(ex);
            }
            return response;
        }


        public HttpWebResponse getResponse(string subRoute)
        {
            // google address is there just to initiate the var
            var http = (HttpWebRequest)WebRequest.Create("http://google.com");
            try
            {
                http = (HttpWebRequest)WebRequest.Create(new Uri(IntegrationSystemUrl+ subRoute));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "GET";
            }
            catch (UriFormatException ex)
            {
                // use a null check at the caller to handle the error
                System.Diagnostics.Trace.Write(ex);
                return null;
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                return response;
            }
            catch (WebException ex)
            {
                // use null checker for error handling
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                return null;
            }
        }   

        
    }
}