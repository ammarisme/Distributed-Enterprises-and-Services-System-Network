using IntegrationSystem.DAL;
using IntegrationSystem.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace IntegrationSystem.Controllers.API
{
    public class SystemRouter
    {
        /*
         Methods that come within this class does few tasks.
         * 1. Finding routes for particular service / enterprise system
         * 2. Sending POST data to service/enterprise
         * 3. Recieve GET data from services
         * 4. Recieve GET data from enterprises
         */

        // to access routing information
        ApplicationDbContext db = new ApplicationDbContext();


        public SystemRouter(){

        }

        // Routing methods

        // this method will give the route of the requested enterprise..
        // but this won't check if the route is up or down
        public string getEnterpriseRoute(int SystemIdNumber){
            if(db.Enterprises.Where(e => e.EnterpriseId == SystemIdNumber).Count() > 0 ){
                Enterprise enterprise = db.Enterprises.Find(SystemIdNumber);
                return enterprise.Uri;
            }
            else
            {
                return null;
            }  
        }

        // this method will give the route of the requested service..
        // but this won't check if the route is up or down
        public string getServiceRoute(int SystemIdNumber)
        {
            if (db.Services.Where(e => e.ServiceId == SystemIdNumber).Count() > 0)
            {
                Service service = db.Services.Find(SystemIdNumber);
                return service.Uri;
            }
            else
            {
                return null;
            }
        }

        // Communication methods
        /* 
         * send a json object to the route in api route.
         * using POST method.
         * api route up or down status is not checked
         * apiRoute must be like /api/{controller}/{action} 
         */
        public HttpWebResponse postJsonData(JObject jsonBody, string apiRoute)
        {
            // creating the http object temporily
            var http = (HttpWebRequest)WebRequest.Create(new Uri("http://google.com"));

            try
            {
                http = (HttpWebRequest)WebRequest.Create(new Uri(apiRoute));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";
            }catch(UriFormatException ex){
                // if the uri requested is wrong.. means the service router is programmed incorrectly
                // handle this error at the calling end by null checking of return value
                return null;
            }

            // parsing the json object
            string parsedContent = jsonBody.ToString();
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            // opeing a new stream and gettin data
            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();
            
            // ready to send the POST request
            HttpWebResponse response = null; 
            try
            {
                response = (HttpWebResponse)http.GetResponse();
                return response;// consist of response code (Ok, Conflict, Created ..etc)
            }
            catch (WebException ex)
            {
                // there was an error before sending the request.
                // handle this by a null checker
                return null;
            }
        }
        /*
         * Getting data using GET request
         * works pretty much like the sendPostData method
         * Error can happen at two places.
         * 1. the route is in incorrect format.
         * 2. route is in correct format but didn't respond
         * @parameters 
         * subRoute - route relative to the system root
         * 
         */
        public HttpWebResponse getResponse(string subRoute)
        {
            // google address is there just to initiate the var
            var http= (HttpWebRequest) WebRequest.Create("http://google.com");
            try
            {
                http = (HttpWebRequest)WebRequest.Create(new Uri(subRoute));
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