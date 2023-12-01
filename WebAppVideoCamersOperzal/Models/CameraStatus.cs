using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebAppVideoCamersOperzal.Models.Entities;

namespace WebAppVideoCamersOperzal.Models
{
    public class CameraStatus
    {
        private string url;
        private string user;
        private string password;

        public CameraStatus(string url, string user = null, string password = null)
        {
            this.url = url;
            this.user = user;
            this.password = password;
        }

        public bool Check3()
        {
            try
            {
                HttpWebRequest Web = (HttpWebRequest) WebRequest.Create(url);
                if (user != null && password != null)
                {
                    NetworkCredential credentail = new NetworkCredential(user, password);
                    Web.Credentials = credentail;
                }
                Web.Timeout = 1000;
                Web.AllowAutoRedirect = false;
                Web.Method = "HEAD";

                using (var responce = Web.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Check()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                if (!string.IsNullOrEmpty(user))
                {
                    string authHeaderValue = $"{user}:{password}";
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authHeaderValue)));
                }
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
                return httpResponseMessage.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
