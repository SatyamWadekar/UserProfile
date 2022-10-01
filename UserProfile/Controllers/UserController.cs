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
        public async Task<ActionResult> GetFaceBookUser(string appId, string appSecret)
        {
            var client = new FBClient(appId, appSecret);

            var facebookUser = client.GetUserData();

            return Ok(facebookUser);
        }

        [HttpGet("google")]
        public async Task<ActionResult> GetGoogleUser(string clientId, string clientSecret, string? code = "")
        {
            var client = new GoogleClient(clientId, clientSecret, code);

            var googleUser = client.GetUserData();

            return Ok(googleUser);
        }
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

        public FBClient(string appId, string appSecret)
        {
            this.appId = appId;
            this.appSecret = appSecret;

            UriBuilder uriBuilder = new UriBuilder("https://graph.facebook.com/oauth/access_token?client_id=" + appId + "&access_token=" + appSecret + "&scope=email");

            using WebClient webClient = new WebClient();
            string text = webClient.DownloadString(uriBuilder.Uri);

            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(text);
            accessToken = nameValueCollection["access_token"];
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

    public class GoogleClient
    {

        //
        // Summary:
        //     The client id.
        private readonly string tokenUrl = "https://accounts.google.com/o/oauth2/token";
        
        //
        // Summary:
        //     The client id.
        private readonly string clientId;

        //
        // Summary:
        //     The client secret.
        private readonly string clientSecret;

        //
        // Summary:
        //     The client code.
        private readonly string code;

        //
        // Summary:
        //     The _app secret.
        private readonly string accessToken;

        public GoogleClient(string clientId, string clientSecret, string code)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.code = code;

            accessToken = GetToken();
        }

        private string GetToken()
        {
            string poststring = tokenUrl + "?grant_type=authorization_code&code=" + code + "&client_id=" + clientId + "&client_secret=" + clientSecret + "";
            var request = (HttpWebRequest)WebRequest.Create(poststring);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "GET";
            var response = (HttpWebResponse)request.GetResponse();
            var streamReader = new StreamReader(response.GetResponseStream());
            string responseFromServer = streamReader.ReadToEnd();
            Tokenclass obj = JsonConvert.DeserializeObject<Tokenclass>(responseFromServer);
            return obj.access_token;
        }

        public GoogleUserModel GetUserData()
        {
            string url = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=" + accessToken + "";
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            GoogleUserModel userInfo = JsonConvert.DeserializeObject<GoogleUserModel>(responseFromServer);
            return userInfo;
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
    public class GoogleUserModel
    {
        public string id
        {
            get;
            set;
        }
        public string name
        {
            get;
            set;
        }
        public string given_name
        {
            get;
            set;
        }
        public string family_name
        {
            get;
            set;
        }
        public string link
        {
            get;
            set;
        }
        public string picture
        {
            get;
            set;
        }
        public string gender
        {
            get;
            set;
        }
        public string locale
        {
            get;
            set;
        }
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
    public class Tokenclass
    {
        public string access_token
        {
            get;
            set;
        }
        public string token_type
        {
            get;
            set;
        }
        public int expires_in
        {
            get;
            set;
        }
        public string refresh_token
        {
            get;
            set;
        }
    }
}
