using DotNetOpenAuth.AspNet.Clients;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

namespace UserProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RequireHttps]
    public class UserController : ControllerBase
    {
       
        [HttpGet("facebook")]
        public async Task<ActionResult> GetFaceBookUser(string appId, string appSecreat)
        {
            var client = new FBClient(appId, appSecreat);

            var facebookUser = client.GetUserData();

            return Ok(facebookUser);
        }

        //[HttpGet("google")]
        //public Task<ActionResult> GetGoogleUser()
        //{
        //
        //}
    }

    public class FBClient
    {
        //
        // Summary:
        //     The _app id.
        private readonly string appId;

        //
        // Summary:
        //     The _app secret.
        private readonly string appSecret;
        
        //
        // Summary:
        //     The _app secret.
        private readonly string accessToken;

        public FBClient(string appId, string appSecreat)
        {
            this.appId = appId;
            this.appSecret = appSecret;

            UriBuilder uriBuilder = new UriBuilder("https://graph.facebook.com/oauth/access_token?client_id=" + appId + "&access_token=" + appSecreat + "&scope=email");

            using WebClient webClient = new WebClient();
            string text = webClient.DownloadString(uriBuilder.Uri);
            
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(text);
            accessToken =  nameValueCollection["access_token"];
        }

        public FacebookGraphData GetUserData()
        {
            WebRequest webRequest = WebRequest.Create("https://graph.facebook.com/me?access_token=" + accessToken);
            FacebookGraphData facebookGraphData;
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using Stream stream = webResponse.GetResponseStream();
                facebookGraphData = JsonConvert.DeserializeObject<FacebookGraphData>(stream.ToString());
            }

            return facebookGraphData;
        }
    }

    public static class BaseMethods
    {
        public static void AddItemIfNotEmpty(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (!string.IsNullOrEmpty(value))
            {
                dictionary[key] = value;
            }
        }
    }
    public class FacebookUserModel
    {
        public string id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; }
        public string locale { get; set; }
        public string link { get; set; }
        public string username { get; set; }
        public int timezone { get; set; }
        public FacebookLocation location { get; set; }
        public Picture picture { get; set; }
    }
    public class Picture
    {
        public PicureData data { get; set; }
    }

    public class PicureData
    {
        public string url { get; set; }
        public bool is_silhouette { get; set; }
    }
    public class FacebookLocation
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
