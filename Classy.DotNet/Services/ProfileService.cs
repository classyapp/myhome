using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using Classy.DotNet.Security;
using System.Net;
using Classy.DotNet.Responses;
using System.IO;

namespace Classy.DotNet.Services
{
    public class ProfileService : ServiceBase
    {
        private readonly string GET_PROFILE_BY_ID_URL = ENDPOINT_BASE_URL + "/profile/{0}?LogImpression={1}&IncludeFollowedByProfiles={2}&IncludeFollowingProfiles={2}&IncludeReviews={3}&IncludeListings={4}&IncludeCollections={5}&IncludeFavorites={6}";
        private readonly string UPDATE_PROFILE_URL = ENDPOINT_BASE_URL + "/profile/{0}";
        private readonly string SEARCH_PROFILES_URL = ENDPOINT_BASE_URL + "/profile/search?";
        private readonly string CLAIM_PROXY_URL = ENDPOINT_BASE_URL + "/profile/{0}/claim";
        private readonly string CREATE_PROXY_URL = ENDPOINT_BASE_URL + "/profile/new";
        private readonly string APPROVE_PROXY_CLAIM_URL = ENDPOINT_BASE_URL + "/profile/{0}/approve";
        private readonly string FOLLOW_PROFILE_URL = ENDPOINT_BASE_URL + "/profile/{0}/follow";
        private readonly string GET_AUTHENTICATED_PROFILE = ENDPOINT_BASE_URL + "/profile";
        private readonly string GET_GOOGLE_CONTACTS_URL = ENDPOINT_BASE_URL + "/profile/social/google/contacts";
        private readonly string CHANGE_PASSWORD_URL = ENDPOINT_BASE_URL + "/profile/{0}";
        private readonly string CHANGE_IMAGE_URL = ENDPOINT_BASE_URL + "/profile/{0}";
        private readonly string PROFILE_TRANSLATION_URL = ENDPOINT_BASE_URL + "/profile/{0}/translation/{1}";

        private readonly string CLAIM_PROXY_DATA = @"{{""ProfessionalInfo"":{0},""Metadata"":{1}}}";
        private readonly string UPDATE_PROFILE_DATA = @"{{""ProfessionalInfo"":{0},""Metadata"":{1},""UpdateType"":{2}}}";

        private readonly string GET_FACEBOOK_ALBUMS = ENDPOINT_BASE_URL + "/profile/social/facebook/albums";

        public ProfileView GetProfileById(string profileId)
        {
            return GetProfileById(profileId, false, false, false, false, false, false);
        }

        public ProfileView GetProfileById(string profileId, bool logImpression, bool includeSocialConnections, bool includeReviews, bool includeListings, bool includeCollections, bool includeFavorites)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(GET_PROFILE_BY_ID_URL, profileId, logImpression, includeSocialConnections, includeReviews, includeListings, includeCollections, includeFavorites);
                var profileJson = client.DownloadString(url);
                var profile = profileJson.FromJson<ProfileView>();
                return profile;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public ProfileView GetAuthenticatedProfile()
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = GET_AUTHENTICATED_PROFILE;
                var profileJson = client.DownloadString(url);
                var profile = profileJson.FromJson<ProfileView>();
                return profile;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public ProfileView CreateProxyProfile(string batchId, ProfessionalInfoView proInfo, IDictionary<string, string> metadata)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = CREATE_PROXY_URL;
                var data = new
                {
                    BatchId = batchId,
                    ProfessionalInfo = proInfo,
                    Metadata = metadata
                };
                var profileJson = client.UploadString(url, data.ToJson());
                var profile = profileJson.FromJson<ProfileView>();
                return profile;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public ProfileView UpdateProfile(
            string profileId,
            ExtendedContactInfoView contactInfo,
            ProfessionalInfoView proInfo,
            IDictionary<string, string> metadata,
            UpdateProfileFields fields)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(UPDATE_PROFILE_URL, profileId);
                var data = new
                {
                    ContactInfo = fields.HasFlag(UpdateProfileFields.ContactInfo) ? contactInfo : null,
                    ProfessionalInfo = fields.HasFlag(UpdateProfileFields.ProfessionalInfo) || fields.HasFlag(UpdateProfileFields.CoverPhotos) ? proInfo : null,
                    Metadata = fields.HasFlag(UpdateProfileFields.Metadata) ? metadata : null,
                    Fields = fields
                }.ToJson();
                var profileJson = client.UploadString(url, "PUT", data);
                var profile = profileJson.FromJson<ProfileView>();
                return profile;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public SearchResultsView<ProfileView> SearchProfiles(
            string displayName,
            string category,
            LocationView location,
            IDictionary<string, string> metadata,
            bool professionalsOnly,
            bool ignoreLocation,
            int page)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(SEARCH_PROFILES_URL, displayName);
                var data = new
                {
                    DisplayName = displayName,
                    Category = category,
                    Location = location,
                    Metadata = metadata,
                    ProfessionalsOnly = professionalsOnly,
                    IgnoreLocation = ignoreLocation,
                    Page = page
                }.ToJson();
                var profilesJson = client.UploadString(url, data);
                var profiles = profilesJson.FromJson<SearchResultsView<ProfileView>>();
                return profiles;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public ProxyClaimView ClaimProfileProxy(
            string proxyId,
            ProfessionalInfoView proInfo,
            IDictionary<string, string> metadata)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(CLAIM_PROXY_URL, proxyId);
                var data = string.Format(CLAIM_PROXY_DATA, proInfo.ToJson(), metadata.ToJson());
                var claimJson = client.UploadString(url, data);
                var claim = claimJson.FromJson<ProxyClaimView>();
                return claim;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public ProxyClaimView ApproveProxyClaim(string claimId)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(APPROVE_PROXY_CLAIM_URL, claimId);
                var claimJson = client.UploadString(url, "{}");
                var claim = claimJson.FromJson<ProxyClaimView>();
                return claim;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void FollowProfile(string username)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(FOLLOW_PROFILE_URL, username);
                client.UploadString(url, "{}");
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public IList<SocialPhotoAlbumView> GetFacebookAlbums()
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var json = client.DownloadString(GET_FACEBOOK_ALBUMS);
                return json.FromJson<IList<SocialPhotoAlbumView>>();
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public IList<GoogleContactView> GetGoogleContacts()
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = GET_GOOGLE_CONTACTS_URL;
                var contactsJson = client.DownloadString(url);
                return contactsJson.FromJson<IList<GoogleContactView>>();
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void ChangePassword(string password, string profileId)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(CHANGE_PASSWORD_URL, profileId);
                var data = new
                {
                    Password = password,
                    Fields = UpdateProfileFields.SetPassword // Set Password
                }.ToJson();
                client.UploadString(url, "PUT", data);
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public string UpdateProfile(string profileId, System.Web.HttpPostedFileBase image)
        {
            try
            {
                var url = string.Format(CHANGE_IMAGE_URL, profileId);
                var fileContent = new byte[image.ContentLength];
                image.InputStream.Read(fileContent, 0, image.ContentLength);
                var req = ClassyAuth.GetAuthenticatedWebRequest(url);
                WebResponse response = HttpUploadFile(req, fileContent, image.ContentType, new Dictionary<string, object> { { "ProfileId", profileId }, { "Fields", 16 } });

                byte[] bytes = response.GetResponseStream().ReadFully();
                string json = Encoding.UTF8.GetString(bytes);
                ProfileView profile = json.FromJson<ProfileView>();

                return profile.Avatar.Url;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        #region // upload file

        private static WebResponse HttpUploadFile(HttpWebRequest request, byte[] fileContent, string contentType, Dictionary<string, object> fields)
        {
            request.PreAuthenticate = true;
            request.AllowWriteStreamBuffering = true;

            string boundary = System.Guid.NewGuid().ToString();

            request.ContentType = string.Format("multipart/form-data;boundary={0}", boundary);
            request.Method = "PUT";

            // Build Contents for Post
            string header = string.Format("--{0}", boundary);
            string footer = header + "--";
            // string builders are for the text above and below the file - file is kept in its original format.
            StringBuilder contents = new StringBuilder();
            StringBuilder contents2 = new StringBuilder();

            // Zip File
            contents.AppendLine(header);
            contents.AppendLine("Content-Disposition:form-data; name=\"myFile\"; filename=\"file\"");
            contents.AppendLine("Content-Type: " + contentType);
            contents.AppendLine();

            contents2.AppendLine();
            foreach (var field in fields.Keys)
            {
                contents2.AppendLine(header);
                contents2.AppendLine(string.Format("Content-Disposition:form-data; name=\"{0}\"", field));
                contents2.AppendLine();
                contents2.AppendLine(fields[field].ToString());
            }
            // Form Field 1

            // Footer
            contents2.AppendLine(footer);

            // This is sent to the Post
            byte[] bytes = Encoding.UTF8.GetBytes(contents.ToString());
            byte[] bytes2 = Encoding.UTF8.GetBytes(contents2.ToString());

            // now we have all of the bytes we are going to send, so we can calculate the size of the stream
            request.ContentLength = bytes.Length + fileContent.Length + bytes2.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Write(fileContent, 0, fileContent.Length);
            requestStream.Write(bytes2, 0, bytes2.Length);
            requestStream.Flush();
            requestStream.Close();

            try
            {
                return request.GetResponse();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        public ProfileTranslationView GetTranslation(string profileId, string cultureCode)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(PROFILE_TRANSLATION_URL, profileId, cultureCode);
                string json = client.DownloadString(url);
                return json.FromJson<ProfileTranslationView>();
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void SaveTranslation(string profileId, ProfileTranslationView profileTranslation)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(PROFILE_TRANSLATION_URL, profileId, profileTranslation.CultureCode);
                var data = new { 
                    ProfileId = profileId,
                    CultureCode = profileTranslation.CultureCode,
                    CompanyName = profileTranslation.CompanyName,
                    Metadata = profileTranslation.Metadata
                };
                string json = client.UploadString(url, data.ToJson());
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void DeleteTranslation(string profileId, string cultureCode)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(PROFILE_TRANSLATION_URL, profileId, cultureCode);
                var data = new
                {
                    ProfileId = profileId,
                    CultureCode = cultureCode
                };
                string json = client.UploadString(url, "DELETE", data.ToJson());
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }
    }
}
