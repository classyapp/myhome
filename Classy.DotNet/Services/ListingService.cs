using System;
using System.Collections.Generic;
using Classy.DotNet.Models.Search;
using CsQuery.ExtensionMethods;
using ServiceStack.Text;
using System.Web;
using System.Net;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;

namespace Classy.DotNet.Services
{
    public class ListingService : ServiceBase
    {
        private class FileToUpload
        {
            public byte[] Data { get; set; }
            public string ContentType { get; set; }
        }

        public enum ObjectType
        {
            Listing,
            Collection
        }

        // free search
        private readonly string FREE_SEARCH_URL = ENDPOINT_BASE_URL + "/free_search";
        // create listing
        private readonly string GET_LISTINGS_FOR_PROFILE_URL = ENDPOINT_BASE_URL + "/profile/{0}/listing/list?IncludeDrafts={1}";
        private readonly string CREATE_LISTING_URL = ENDPOINT_BASE_URL + "/listing/new";
        private readonly string UPDATE_LISTING_URL = ENDPOINT_BASE_URL + "/listing/{0}";
        private readonly string DELETE_LISTING_URL = ENDPOINT_BASE_URL + "/listing/{0}";
        private readonly string ADD_EXTERNAL_MEDIA_URL = ENDPOINT_BASE_URL + "/listing/{0}/media";
        private readonly string PUBLISH_LISTING_URL = ENDPOINT_BASE_URL + "/listing/{0}/publish";
        // get listings 
        private readonly string GET_LISTING_BY_ID_URL = ENDPOINT_BASE_URL + "/listing/{0}?";
        private readonly string GET_LISTINGS_BY_ID_URL = ENDPOINT_BASE_URL + "/listing/get-multiple";
        private readonly string GET_LISTING_MORE_INFO_URL = ENDPOINT_BASE_URL + "/listing/{0}/more";
        private readonly string SEARCH_LISTINGS_URL = ENDPOINT_BASE_URL + "/listing/search";
        // translations
        private readonly string LISTING_TRANSLATION_URL = ENDPOINT_BASE_URL + "/listing/{0}/translation/{1}";
        private readonly string COLLECTION_TRANSLATION_URL = ENDPOINT_BASE_URL + "/collection/{0}/translation/{1}";
        // post comment
        private readonly string POST_COMMENT_URL = ENDPOINT_BASE_URL + "/{0}/{1}/comment/new";
        // favorite listing
        private readonly string FAVORITE_LISTING_URL = ENDPOINT_BASE_URL + "/listing/{0}/favorite";
        // collections
        private readonly string CREATE_COLLECTION_URL = ENDPOINT_BASE_URL + "/collection/new";
        private readonly string UPDATE_COLLECTION_URL = ENDPOINT_BASE_URL + "/collection/{0}";
        private readonly string UPDATE_COLLECTION_COVER_URL = ENDPOINT_BASE_URL + "/collection/{0}/cover";
        private readonly string DELETE_COLLECTION_URL = ENDPOINT_BASE_URL + "/collection/{0}";
        private readonly string ADD_LISTINGS_TO_CLECTION_URL = ENDPOINT_BASE_URL + "/collection/{0}/listing/new";
        private readonly string REMOVE_LISTING_FROM_COLLECTION_URL = ENDPOINT_BASE_URL + "/collection/{0}/remove";
        private readonly string GET_COLLECTIONS_FOR_PROFILE_URL = ENDPOINT_BASE_URL + "/profile/{0}/collection/list/{1}";
        private readonly string GET_COLLECTION_BY_ID_URL = ENDPOINT_BASE_URL + "/collection/{0}?IncludeProfile=true&IncludeListings={1}&IncreaseViewCounter={2}&IncludeViewCounterOnListings={3}&IncludeComments={4}&IncludeCommenterProfiles={5}";
        private readonly string GET_APPROVED_COLLECTIONS = ENDPOINT_BASE_URL + "/collection/list/approved?maxCollections={0}&categories={1}&culture={2}";

        private readonly string EDIT_MULTIPLE_LISTINGS_URL = ENDPOINT_BASE_URL + "/listings/edit-multiple";

        #region // listings

        public void EditMultipleListings(string[] listingIds, int? editorsRank, string room, string style)
        {
            try
            {
                var metadata = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(room))
                    metadata.Add("Room", room);
                if (!string.IsNullOrEmpty(style))
                    metadata.Add("Style", style);

                var data = new {
                    ListingIds = listingIds,
                    EditorsRank = editorsRank,
                    Metadata = metadata
                }.ToJson();

                var client = ClassyAuth.GetAuthenticatedWebClient();
                client.UploadString(EDIT_MULTIPLE_LISTINGS_URL, "POST", data);
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public ListingView CreateListing(
            string title, 
            string content,
            string listingType,
            string[] categories,
            //TODO: Investigate combining Request & Response models?
            PricingInfoView pricingInfo,
            IDictionary<string, string> metadata,
            HttpFileCollectionBase files)
        {
            var filesToUpload = new List<FileToUpload>();
            foreach(var f in files.AllKeys)
            {
                var fileToUpload = new FileToUpload();
                fileToUpload.Data = new byte[files[f].ContentLength];
                files[f].InputStream.Read(fileToUpload.Data, 0, files[f].ContentLength);
                fileToUpload.ContentType = files[f].ContentType;
                filesToUpload.Add(fileToUpload);
            }

            return CreateListing(
                title,
                content,
                listingType,
                categories,
                pricingInfo,
                metadata,
                filesToUpload);
        }

        public ListingView CreateListing(
            string title,
            string content,
            string listingType,
            string[] categories,
            //TODO: Investigate combining Request & Response models?
            PricingInfoView pricingInfo,
            IDictionary<string, string> metadata,
            string externalMediaUrl)
        {
            var wc = new WebClient();
            var data = wc.DownloadData(externalMediaUrl);
            var contentType = wc.ResponseHeaders[HttpResponseHeader.ContentType];
            var filesToUpload = new List<FileToUpload>() {
                new FileToUpload {
                    Data = data,
                    ContentType = contentType
                }
            };

            return CreateListing(
                title,
                content,
                listingType,
                categories,
                pricingInfo,
                metadata,
                filesToUpload);
        }

        private ListingView CreateListing(
            string title, 
            string content,
            string listingType,
            string[] categories,
            //TODO: Investigate combining Request & Response models?
            PricingInfoView pricingInfo,
            IDictionary<string, string> metadata,
            IList<FileToUpload> files)
        {
            var client = ClassyAuth.GetAuthenticatedWebClient();
            var data = new
            {
                Title = title,
                Content = content,
                Categories = categories,
                ListingType = listingType,
                Pricing = pricingInfo,
                Metadata = metadata
            }.ToJson();

            // create the listing
            ListingView listing = null;
            try
            {
                var listingJson = client.UploadString(CREATE_LISTING_URL, data);
                listing = listingJson.FromJson<ListingView>();
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }

            // add media files
            string url = null;
            try
            {
                url = string.Format(ADD_EXTERNAL_MEDIA_URL, listing.Id);
                foreach (var f in files)
                {
                    var req = ClassyAuth.GetAuthenticatedWebRequest(url);
                    HttpUploadFile(req, f.Data, f.ContentType);
                }
            }
            catch (WebException fex)
            {
                // delete the listing on error
                this.DeleteListing(listing.Id);
                throw fex.ToClassyException();
            }

            // publish
            try
            {
                url = string.Format(PUBLISH_LISTING_URL, listing.Id);
                var updatedJson = client.UploadString(url, "".ToJson());
                listing = updatedJson.FromJson<ListingView>();
            }
            catch (Exception pex)
            {
                // Do nothing for now...
            }

            return listing;
        }

        public ListingView UpdateListing(
            string listingId,
            string title,
            string content,
            string[] categories,
            PricingInfoView pricingInfo,
            IDictionary<string, string> metadata,
            IList<string> hashtags,
            IDictionary<string, IList<string>> editorKeywords,
            ListingUpdateFields fields)
        {
            var client = ClassyAuth.GetAuthenticatedWebClient();
            var data = new
            {
                Title = title,
                Content = content,
                Pricing = pricingInfo,
                Categories = categories,
                Metadata = metadata,
                Hashtags = hashtags,
                EditorKeywords = editorKeywords,
                Fields = fields
            }.ToJson();

            // create the listing
            ListingView listing = null;
            try
            {
                var listingJson = client.UploadString(string.Format(UPDATE_LISTING_URL, listingId), "PUT", data);
                listing = listingJson.FromJson<ListingView>();
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }

            return listing;
        }

        public object DeleteListing(string listingId)
        {
            var client = ClassyAuth.GetAuthenticatedWebClient();
            var data = new
            {
                ListingId = listingId

            }.ToJson();

            // create the listing
            try
            {
                var listingJson = client.UploadString(string.Format(DELETE_LISTING_URL, listingId), "DELETE", data);
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }

            return listingId;
        }

        public ListingView GetListingById(
            string listingId,
            bool IncludeComments,
            bool IncludeProfile,
            bool IncludeCommenterProfiles,
            bool IncludeFavoritedByProfiles,
            bool LogImpression)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(GET_LISTING_BY_ID_URL, listingId);
                if (IncludeComments) url = string.Concat(url, "&includecomments=true");
                if (IncludeProfile) url = string.Concat(url, "&includeprofile=true");
                if (IncludeCommenterProfiles) url = string.Concat(url, "&includecommenterprofiles=true");
                if (IncludeFavoritedByProfiles) url = string.Concat(url, "&includefavoritedbyprofiles=true");
                if (LogImpression) url = string.Concat(url, "&logimpression=true");
                var listingJson = client.DownloadString(url);
                var listing = listingJson.FromJson<ListingView>();
                return listing;
            }
            catch(WebException wex)
            { 
                throw wex.ToClassyException(); 
            }
        }

        public List<ListingView> GetListings(string[] listingIds, bool includeProfiles)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var data = new {
                    ListingIds = listingIds,
                    IncludeProfiles = includeProfiles
                }.ToJson();
                var listingsJson = client.UploadString(GET_LISTINGS_BY_ID_URL, "POST", data);
                var listings = listingsJson.FromJson<List<ListingView>>();
                return listings;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public FreeSearchResultsView FreeSearch(string q, int amount, int page)
        {
            using (var client = ClassyAuth.GetWebClient())
            {
                var url = FREE_SEARCH_URL;
                var data = new {
                    Q = q, Page = page, Amount = amount
                }.ToJson();
                var listingsJson = client.UploadString(url, data);
                var results = listingsJson.FromJson<FreeSearchResultsView>();

                return results;
            }
        }

        public SearchResultsView<ListingView> SearchUntaggedListings(int page, string[] listingTypes, string date, int pageSize = 12)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(SEARCH_LISTINGS_URL);
                var data = new
                {
                    Page = page,
                    ListingTypes = listingTypes,
                    Date = date,
                    PageSize = pageSize,
                    SortMethod = SortMethod.Date,
                    Metadata = new Dictionary<string, string> {
                        { "Room", "home-spaces" }
                    }
                }.ToJson();
                var listingsJson = client.UploadString(url, data);
                var results = listingsJson.FromJson<SearchResultsView<ListingView>>();
                return results;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public SearchResultsView<ListingView> SearchListings(
            string[] tags,
            string[] listingTypes,
            IDictionary<string, string[]> metadata,
            double? priceMin,
            double? priceMax,
            LocationView location,
            int page,
            int pageSize = 12)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = SEARCH_LISTINGS_URL;
                var data = new
                {
                    Tags = tags,
                    ListingTypes = listingTypes,
                    Metadata = metadata,
                    PriceMin = priceMin,
                    PriceMax = priceMax,
                    Location = location,
                    Page = page,
                    PageSize = pageSize
                }.ToJson();
                var listingsJson = client.UploadString(url, data);
                var results = listingsJson.FromJson<SearchResultsView<ListingView>>();
                return results;
            }
            catch(WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public IList<ListingView> GetListingsByProfileId(string profileId, bool includeDrafts)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(GET_LISTINGS_FOR_PROFILE_URL, profileId, includeDrafts);
                
                var listingsJson = client.DownloadString(url);
                var listings = listingsJson.FromJson<IList<ListingView>>();
                return listings;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public CommentView PostComment(string objectId, string content, ObjectType type)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(POST_COMMENT_URL, type.ToString().ToLower(), objectId);
                var commentJson = client.UploadString(url, 
                    new { Content = content, Type = type }.ToJson());
                var comment = commentJson.FromJson<CommentView>();
                return comment;
            }
            catch(WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void FavoriteListing(string listingId)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(FAVORITE_LISTING_URL, listingId);
                client.UploadString(url, "{}");
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void UnfavoriteListing(string listingId)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(FAVORITE_LISTING_URL, listingId);
                client.UploadString(url, "DELETE", "{}");
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        #endregion

        #region // collections

        public CollectionView GetCollectionById(string collectionId, bool includeListings, bool increaseViewCounter, bool increaseViewCounterOnListings, bool includeComments)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var collectionJson = client.DownloadString(string.Format(GET_COLLECTION_BY_ID_URL, collectionId, includeListings, increaseViewCounter, increaseViewCounterOnListings, includeComments, includeComments));
                var collection = collectionJson.FromJson<CollectionView>();
                return collection;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public IList<CollectionView> GetCollectionsByProfileId(string profileId, string collectionType, bool includeListings, bool increaseViewCounter, bool increaseViewCounterOnListings)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var collectionsJson = client.DownloadString(string.Format(GET_COLLECTIONS_FOR_PROFILE_URL, profileId, collectionType));
                var collections = collectionsJson.FromJson<IList<CollectionView>>();
                return collections;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

         public CollectionView CreateCollection(
            string type,
            string title,
            string content,
            IList<IncludedListingView> includedListings)
        {
            return CreateCollection(null, type, title, content, includedListings);
        }

        public CollectionView CreateCollection(
            string profileId,
            string type,
            string title,
            string content,
            IList<IncludedListingView> includedListings)
        {
            try
            {
                using (var client = ClassyAuth.GetAuthenticatedWebClient())
                {
                    var data = new {
                        ProfileId = profileId,
                        Type = type,
                        Title = title,
                        Content = content,
                        IncludedListings = includedListings
                    }.ToJson();
                    var collectionJson = client.UploadString(CREATE_COLLECTION_URL, data);
                    var collection = collectionJson.FromJson<CollectionView>();
                    return collection;
                }
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public CollectionView AddListingToCollection(
            string collectionId,
            IList<IncludedListingView> includedListings)
        {
            try
            {
                using (var client = ClassyAuth.GetAuthenticatedWebClient())
                {
                    var url = string.Format(ADD_LISTINGS_TO_CLECTION_URL, collectionId);
                    var data = new {
                        CollectionId = collectionId,
                        IncludedListings = includedListings
                    }.ToJson();
                    var collectionJson = client.UploadString(url, data);
                    var collection = collectionJson.FromJson<CollectionView>();
                    return collection;
                }
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public CollectionView UpdateCollection(string collectionId,
            string title,
            string content,
            IList<IncludedListingView> listings)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(UPDATE_COLLECTION_URL, collectionId);
                var data = new
                {
                    Title = title,
                    Content = content,
                    IncludedListings = listings
                }.ToJson();
                var collectionJson = client.UploadString(url, "PUT", data);
                var collection = collectionJson.FromJson<CollectionView>();
                return collection;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void UpdateCollectionCoverPhotos(string collectionId, string[] keys)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(UPDATE_COLLECTION_COVER_URL, collectionId);
                var data = new
                {
                    Keys = keys
                }.ToJson();
                client.UploadString(url, "POST", data);
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public IList<CollectionView> GetApprovedCollections(string[] categories, int maxCollections, string culture)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var collectionsJson = client.DownloadString(string.Format(GET_APPROVED_COLLECTIONS, maxCollections, categories.ToJsv(), culture));
                var collections = collectionsJson.FromJson<IList<CollectionView>>();
                return collections;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void RemoveListingFromCollection(string collectionId, string[] listingIds)
        {
            try
            {
                var data = new
                {
                    CollectionId = collectionId,
                    ListingIds = listingIds
                }.ToJson();
                var client = ClassyAuth.GetAuthenticatedWebClient();
                client.UploadString(string.Format(REMOVE_LISTING_FROM_COLLECTION_URL, collectionId), data);
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            } 
        }

        public void DeleteCollection(string collectionId)
        {
            try
            {
                var data = new
                {
                    CollectionId = collectionId
                }.ToJson();
                var client = ClassyAuth.GetAuthenticatedWebClient();
                client.UploadString(string.Format(DELETE_COLLECTION_URL, collectionId), "DELETE", data);
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }  
        }

        #endregion

        #region // upload file

        private static void HttpUploadFile(HttpWebRequest wr, byte[] fileContent, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;

            var rs = wr.GetRequestStream();
            //header
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string headerTemplate = "Content-Disposition: form-data; name=\"file\"; filename=\"file\"\r\nContent-Type: {0}\r\n\r\n";
            string header = string.Format(headerTemplate, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            //file
            rs.Write(fileContent, 0, fileContent.Length);
            //footer
            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            try
            {
                var res = wr.GetResponse();
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Translation
        public ListingTranslationView GetTranslation(string listingId, string cultureCode)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(LISTING_TRANSLATION_URL, listingId, cultureCode);
                string json = client.DownloadString(url);
                return json.FromJson<ListingTranslationView>();
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void SaveTranslation(string listingId, ListingTranslationView listingTranslation)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(LISTING_TRANSLATION_URL, listingId, listingTranslation.Culture);
                var data = new
                {
                    ListingId = listingId,
                    CultureCode = listingTranslation.Culture,
                    Title = listingTranslation.Title,
                    Content = listingTranslation.Content
                };
                string json = client.UploadString(url, data.ToJson());
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void DeleteTranslation(string listingId, string cultureCode)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(LISTING_TRANSLATION_URL, listingId, cultureCode);
                var data = new
                {
                    ProfileId = listingId,
                    CultureCode = cultureCode
                };
                string json = client.UploadString(url, "DELETE", data.ToJson());
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public CollectionTranslationView GetCollectionTranslation(string collectionId, string cultureCode)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(COLLECTION_TRANSLATION_URL, collectionId, cultureCode);
                string json = client.DownloadString(url);
                return json.FromJson<CollectionTranslationView>();
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void SaveCollectionTranslation(string collectionId, CollectionTranslationView translation)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(COLLECTION_TRANSLATION_URL, collectionId, translation.Culture);
                var data = new
                {
                    CollectionId = collectionId,
                    CultureCode = translation.Culture,
                    Title = translation.Title,
                    Content = translation.Content
                };
                string json = client.UploadString(url, data.ToJson());
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public void DeleteCollectionTranslation(string collectionId, string cultureCode)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(COLLECTION_TRANSLATION_URL, collectionId, cultureCode);
                var data = new
                {
                    CollectionId = collectionId,
                    CultureCode = cultureCode
                };
                string json = client.UploadString(url, "DELETE", data.ToJson());
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }
        #endregion

        public ListingMoreInfoView GetLisingMoreInfo(string listingId, Dictionary<string, string[]> metadata, Dictionary<string, string[]> query)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(GET_LISTING_MORE_INFO_URL, listingId);
                var data = new { ListingId = listingId, Metadata = metadata, Query = query };
                var listingJson = client.UploadString(url, data.ToJson());
                var listing = listingJson.FromJson<ListingMoreInfoView>();
                return listing;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }            
        }
    }
}
