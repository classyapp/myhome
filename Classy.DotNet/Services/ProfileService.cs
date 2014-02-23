using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using Classy.DotNet.Security;
using System.Net;
using Classy.DotNet.Responses;

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
            catch(WebException wex)
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
                var data = new {
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
                    ProfessionalInfo = fields.HasFlag(UpdateProfileFields.ProfessionalInfo) ? proInfo : null,
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
    }
}
