﻿using Classy.DotNet.Services;
using ServiceStack.Text;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Classy.DotNet.Security
{
    public class ClassyAuth
    {
        // private
        private static string _sApiKey;
        private static string _sEndpointUrl;

        // public
        public static string ApiKey { get { return _sApiKey; } }
        public static string EndpointBaseUrl { get { return _sEndpointUrl;  } }

        // private static
        private static string[] AuthCookieNames = { "ss-id", "ss-pid", "ss-opt", "X-UAId" };
        private const string COOKIE_USER_ID = "X-UAId";
        private const string COOKIE_SESSION_ID = "ss-id";
        private const string COOKIE_PERMANENT_SESSION_ID = "ss-pid";
        private const string CFG_API_KEY = "Classy:AppId";
        private const string CFG_ENDPOINT_BASE_URL = "Classy:EndpointBaseUrl";

        // ctor
        static ClassyAuth()
        {
            _sApiKey = System.Configuration.ConfigurationManager.AppSettings[CFG_API_KEY];
            _sEndpointUrl = System.Configuration.ConfigurationManager.AppSettings[CFG_ENDPOINT_BASE_URL];
        }

        #region // static auth methods

        public static bool AuthenticateUser(string username, string password, bool rememberMe)
        {
            try
            {
                var context = HttpContext.Current;
                var creds = new
                {
                    UserName = username,
                    Password = password,
                    RememberMe = rememberMe
                }.ToJson();
                var credsBytes = Encoding.UTF8.GetBytes(creds);
                var client = HttpWebRequest.Create(string.Concat(EndpointBaseUrl, "/auth/credentials")) as HttpWebRequest;
                client.Method = "POST";
                client.ContentType = "application/json";
                client.Accept = "application/json";
                client.Headers.Add("X-Classy-Env", GetEnvHeader());
                var stream = client.GetRequestStream();
                stream.Write(credsBytes, 0, credsBytes.Length);
                stream.Close();
                var response = client.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FillInCookiesCollection(context.Response.Cookies, response.Headers[HttpResponseHeader.SetCookie], context.Request.Url);
                    var authJson = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                    var auth = authJson.FromJson<ClassyAuthResponse>();
                    return SetPrincipalInternal(auth.SessionId);
                }
                return false;
            }
            catch (WebException)
            {
                return false;
            }
        }

        public static bool AuthenticateOrConnectFacebookUser(string token)
        {
            try
            {
                var context = HttpContext.Current;
                var client = HttpWebRequest.Create(string.Concat(EndpointBaseUrl, "/auth/facebook?format=json&oauth_token=", token)) as HttpWebRequest;
                client.Method = "GET";
                client.ContentType = "application/json";
                client.Accept = "application/json";
                client.Headers.Add("X-Classy-Env", GetEnvHeader());
                var cookies = new CookieContainer();
                foreach (var n in AuthCookieNames)
                {
                    if (context.Request.Cookies.Get(n) != null)
                        cookies.Add(ToCookie(context.Request.Cookies[n], new Uri(EndpointBaseUrl).Host));
                }
                client.Headers.Add(HttpRequestHeader.Cookie, cookies.GetCookieHeader(new Uri(EndpointBaseUrl)));
                var response = client.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FillInCookiesCollection(context.Response.Cookies, response.Headers[HttpResponseHeader.SetCookie], context.Request.Url);
                    var authJson = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                    var auth = authJson.FromJson<ClassyAuthResponse>();
                    return SetPrincipalInternal(auth.SessionId);
                }
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        public static bool AuthenticateOrConnectGoogleUser(string token)
        {
            try
            {
                var context = HttpContext.Current;
                var client = HttpWebRequest.Create(string.Concat(EndpointBaseUrl, "/auth/GoogleOAuth?format=json&oauth_token=", token)) as HttpWebRequest;
                client.Method = "GET";
                client.ContentType = "application/json";
                client.Accept = "application/json";
                client.Headers.Add("X-Classy-Env", GetEnvHeader());
                var cookies = new CookieContainer();
                foreach (var n in AuthCookieNames)
                {
                    if (context.Request.Cookies.Get(n) != null)
                        cookies.Add(ToCookie(context.Request.Cookies[n], new Uri(EndpointBaseUrl).Host));
                }
                client.Headers.Add(HttpRequestHeader.Cookie, cookies.GetCookieHeader(new Uri(EndpointBaseUrl)));
                var response = client.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FillInCookiesCollection(context.Response.Cookies, response.Headers[HttpResponseHeader.SetCookie], context.Request.Url);
                    var authJson = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                    var auth = authJson.FromJson<ClassyAuthResponse>();
                    return SetPrincipalInternal(auth.SessionId);
                }
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        public static void Logout()
        {
            var client = GetWebClient();
            client.DownloadString(string.Concat(EndpointBaseUrl, "/auth/logout"));
            ClearAuthCookies();
        }

        #endregion

        #region // static registration methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="rememberMe"></param>
        /// <returns>The new profile or throws a ClassyValidationException</returns>
        public static bool Register(string username, string email, string password)
        {
            try
            {
                // register user
                var registration = new
                {
                    UserName = username,
                    Email = email,
                    Password = password,
                    AutoLogin = true
                }.ToJson();
                var context = System.Web.HttpContext.Current;
                var regBytes = Encoding.UTF8.GetBytes(registration);
                var client = HttpWebRequest.Create(string.Concat(EndpointBaseUrl, "/register")) as HttpWebRequest;
                client.Method = "POST";
                client.ContentType = "application/json";
                client.Accept = "application/json";
                client.Headers.Add("X-Classy-Env", GetEnvHeader());
                var stream = client.GetRequestStream();
                stream.Write(regBytes, 0, regBytes.Length);
                stream.Close();
                var response = client.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FillInCookiesCollection(context.Response.Cookies, response.Headers[HttpResponseHeader.SetCookie], context.Request.Url);
                    var respJson = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                    var reg = respJson.FromJson<ClassyRegistrationResponse>();
                    return SetPrincipalInternal(reg.UserId);
                }
                return false;
            }
            catch (WebException wex)
            {
                if (wex.IsBadRequest())
                {
                    throw wex.ToClassyException();
                }
                return false;
            }
        }

        #endregion

        #region // helpers webclients

        public static WebClient GetWebClient(bool suppressAuthentication = false)
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            client.Headers.Add("X-Classy-Env", GetEnvHeader());
            // pass auth info if any (otherwise impressions are logged as guest even when logged in)
            var context = System.Web.HttpContext.Current;
            if (!suppressAuthentication &&  IsLoggedIn())
            {
                var cookies = new CookieContainer();
                foreach (var n in AuthCookieNames)
                {
                    if (context.Request.Cookies.Get(n) != null)
                        cookies.Add(ToCookie(context.Request.Cookies[n], new Uri(EndpointBaseUrl).Host));
                }
                client.Headers.Add(HttpRequestHeader.Cookie, cookies.GetCookieHeader(new Uri(EndpointBaseUrl)));
            }
           
            return client;
        }

        private static string GetEnvHeader()
        {
            HttpCookie gpsCookie = null;
            HttpCookie countryCookie = null;
            HttpCookie currencyCookie = null;
            HttpRequest request = null;
            GPSLocation location = null;

            try
            {
                // on register routes the request is not applicable
                request = HttpContext.Current.Request;
            }
            catch { }

            if (request != null)
            {
                gpsCookie = HttpContext.Current.Request.Cookies[Responses.AppView.GPSLocationCookieName];
                if (gpsCookie != null)
                {
                    location = Newtonsoft.Json.JsonConvert.DeserializeObject<GPSLocation>(gpsCookie.Value);
                }
                countryCookie = HttpContext.Current.Request.Cookies[Responses.AppView.CountryCookieName];
                currencyCookie = HttpContext.Current.Request.Cookies[Responses.AppView.CurrencyCookieName];
            }
            return new
            {
                CultureCode = System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                CountryCode = countryCookie == null ? Responses.AppView.DefaultCountry : countryCookie.Value,
                GPSCoordinates = location,
                CurrencyCode = currencyCookie == null ? Responses.AppView.DefaultCurrency : currencyCookie.Value,
                AppId = ApiKey
            }.ToJson();
        }

        public static WebClient GetAuthenticatedWebClient(bool throwIfNull = true)
        {
            var context = HttpContext.Current;
            if (!IsLoggedIn())
            {
                ClearAuthCookies();
                if (throwIfNull) throw new UnauthorizedAccessException("user not logged in. must be logged in to complete operation.");
                return null;
            }

            var client = new WebClient { Encoding = Encoding.UTF8 };
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            client.Headers.Add("X-Classy-Env", GetEnvHeader());
            var cookies = new CookieContainer();
            foreach (var n in AuthCookieNames)
            {
                if (context.Request.Cookies.Get(n) != null)
                    cookies.Add(ToCookie(context.Request.Cookies[n], new Uri(EndpointBaseUrl).Host));
            }
            client.Headers.Add(HttpRequestHeader.Cookie, cookies.GetCookieHeader(new Uri(EndpointBaseUrl)));
            return client;
        }

        public static HttpWebRequest GetAuthenticatedWebRequest(string url, bool throwIfNull = true)
        {
            var context = HttpContext.Current;
            if (!IsLoggedIn())
            {
                ClearAuthCookies();
                if (throwIfNull) throw new UnauthorizedAccessException("user not logged in. must be logged in to complete operation.");
                return null;
            }

            var client = WebRequest.Create(url) as HttpWebRequest;
            client.Headers.Add("X-Classy-Env", GetEnvHeader());
            var cookies = new CookieContainer();
            foreach (var n in AuthCookieNames)
            {
                if (context.Request.Cookies.Get(n) != null)
                    cookies.Add(ToCookie(context.Request.Cookies[n], new Uri(EndpointBaseUrl).Host));
            }
            client.Headers.Add(HttpRequestHeader.Cookie, cookies.GetCookieHeader(new Uri(EndpointBaseUrl)));
            return client;
        }

        private static bool IsLoggedIn()
        {
            var context = HttpContext.Current;

            return context.Request.Cookies.AllKeys.Contains(COOKIE_USER_ID) &&
                (context.Request.Cookies.AllKeys.Contains(COOKIE_SESSION_ID) ||
                context.Request.Cookies.AllKeys.Contains(COOKIE_PERMANENT_SESSION_ID));

        }

        #endregion

        #region // identity handling

        private static void ClearAuthCookies()
        {
            var context = HttpContext.Current;
            foreach (var cookieName in AuthCookieNames)
            {
                if (context.Request.Cookies.AllKeys.Contains(cookieName))
                {
                    var cookie = context.Request.Cookies[cookieName];
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    cookie.Domain = context.Request.Url.Host;
                    context.Response.Cookies.Add(cookie);
                }
            }
        }

        public static void SetPrincipal()
        {
            var context = HttpContext.Current;
            if (context.Request.Cookies.AllKeys.Contains(COOKIE_USER_ID))
            {
                var authCookie = context.Request.Cookies[COOKIE_USER_ID];
                SetPrincipalInternal(authCookie.Value);
            }
        }

        private static bool SetPrincipalInternal(string profileId)
        {
            var context = HttpContext.Current;
            try
            {
                var service = new ProfileService();
                var profile = service.GetAuthenticatedProfile();
                var identity = new ClassyIdentity
                {
                    Name = profile.UserName,
                    IsAuthenticated = true,
                    Profile = profile
                };
                var principal = new ClassyPrincipal(identity);
                context.User = principal;
                return true;
            }
            catch (ClassyException)
            {
                ClearAuthCookies();
                return false;
            }
        }

        #region // cookie collection manipulation methods 

        private static void FillInCookiesCollection(HttpCookieCollection cookieCollection, string header, Uri uri)
        {
            // "ss-id=Xy4xA8CopnmhgvrDuDuW; path=/; HttpOnly,ss-pid=qouowGC6cXVu0Gx714NL; expires=Fri, 02-Sep-2033 19:24:05 GMT; path=/; HttpOnly,X-UAId=2; expires=Fri, 02-Sep-2033 19:24:05 GMT; path=/; HttpOnly"
            var part = new Regex("(.*?)=(.*?);");
            var cookies = header.Split(new string[] { "HttpOnly," }, StringSplitOptions.None);
            foreach (var c in cookies)
            {
                var cookie = new Cookie();
                foreach (Match m in part.Matches(c))
                {
                    var key = m.Groups[1].Value;
                    var value = m.Groups[2].Value;
                    switch (key.ToLower().Trim())
                    {
                        case "expires":
                            cookie.Expires = DateTime.Parse(value);
                            break;
                        case "path":
                            cookie.Path = value;
                            break;
                        default:
                            cookie.Name = key;
                            cookie.Value = value;
                            break;
                    }
                }
                cookie.Domain = uri.Host;
                //cookie.HttpOnly = true;
                //cookie.Secure = true;
                cookieCollection.Add(ToHttpCookie(cookie));
            }
        }

        private static HttpCookie ToHttpCookie(Cookie cookie)
        {
            return new HttpCookie(cookie.Name, cookie.Value) 
            {
                Domain = cookie.Domain,
                Expires = cookie.Expires,
                Path = cookie.Path//,
                //Secure = cookie.Secure,
                //HttpOnly = cookie.HttpOnly
            };
        }

        private static Cookie ToCookie(HttpCookie cookie, string newDomain)
        {
            return new Cookie
            {
                Name = cookie.Name,
                Value = cookie.Value,
                Domain = newDomain,
                Expires = cookie.Expires,
                Path = cookie.Path//,
                //Secure = cookie.Secure,
                //HttpOnly = cookie.HttpOnly
            };
        }
        
        #endregion

        #endregion

        public static bool RequestPasswordReset(string email)
        {
            try
            {
                var wc = GetWebClient();
                string json = wc.UploadString(string.Concat(EndpointBaseUrl, "/auth/forgot"),
                    new
                    {
                        Host = HttpContext.Current.Request.Url.Authority,
                        Email = email
                    }.ToJson());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ResetPassword(string hash, string password)
        {
            try
            {
                var wc = GetWebClient();
                string json = wc.UploadString(string.Concat(EndpointBaseUrl, "/auth/reset"),
                    new
                    {
                        Hash = hash,
                        Password = password 
                    }.ToJson());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool VerifyResetRequest(string hash)
        {
            try
            {
                var wc = GetWebClient();
                wc.DownloadString(string.Format("{0}/auth/reset?Hash={1}", EndpointBaseUrl, hash));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}