﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using System.Web;
using System.Net;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;

namespace Classy.DotNet.Services
{
    public class ListingService : ServiceBase
    {
        // create listing
        private readonly string GET_LISTINGS_FOR_PROFILE_URL = ENDPOINT_BASE_URL + "/profile/{0}/listing/list?IncludeDrafts={1}";
        private readonly string CREATE_LISTING_URL = ENDPOINT_BASE_URL + "/listing/new";
        private readonly string UPDATE_LISTING_URL = ENDPOINT_BASE_URL + "/listing/{0}";
        private readonly string DELETE_LISTING_URL = ENDPOINT_BASE_URL + "/listing/{0}";
        private readonly string ADD_EXTERNAL_MEDIA_URL = ENDPOINT_BASE_URL + "/listing/{0}/media";
        private readonly string PUBLISH_LISTING_URL = ENDPOINT_BASE_URL + "/listing/{0}/publish";
        // get listings
        private readonly string GET_LISTING_BY_ID_URL = ENDPOINT_BASE_URL + "/listing/{0}?";
        private readonly string SEARCH_LISTINGS_URL = ENDPOINT_BASE_URL + "/listing/search";
        // post comment
        private readonly string POST_COMMENT_URL = ENDPOINT_BASE_URL + "/listing/{0}/comment/new";
        // favorite listing
        private readonly string FAVORITE_LISTING_URL = ENDPOINT_BASE_URL + "/listing/{0}/favorite";
        // collections
        private readonly string CREATE_COLLECTION_URL = ENDPOINT_BASE_URL + "/collection/new";
        private readonly string UPDATE_COLLECTION_URL = ENDPOINT_BASE_URL + "/collection/{0}/edit";
        private readonly string ADD_LISTINGS_TO_CLECTION_URL = ENDPOINT_BASE_URL + "/collection/{0}/listing/new";
        private readonly string GET_COLLECTIONS_FOR_PROFILE_URL = ENDPOINT_BASE_URL + "/profile/{0}/collection/list";
        private readonly string GET_COLLECTION_BY_ID_URL = ENDPOINT_BASE_URL + "/collection/{0}?IncludeProfile=true&IncludeListings={1}&IncreaseViewCounter={2}&IncludeViewCounterOnListings={3}";

        #region // listings

        public ListingView CreateListing(
            string title, 
            string content,
            string listingType,
            //TODO: Investigate combining Request & Response models?
            PricingInfoView pricingInfo,
            IDictionary<string, string> metadata,
            HttpFileCollectionBase files)
        {
            var client = ClassyAuth.GetAuthenticatedWebClient();
            var data = new
            {
                Title = title, 
                Content = content,
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
            catch(WebException wex)
            {
                throw wex.ToClassyException();
            }

            // add media files
            var url = string.Format(ADD_EXTERNAL_MEDIA_URL, listing.Id);
            foreach (var f in files.AllKeys)
            {
                var fileContent = new byte[files[f].ContentLength];
                files[f].InputStream.Read(fileContent, 0, files[f].ContentLength);
                var req = ClassyAuth.GetAuthenticatedWebRequest(url);
                HttpUploadFile(req, fileContent, files[f].ContentType);
            }

            // publish
            url = string.Format(PUBLISH_LISTING_URL, listing.Id);
            var updatedJson = client.UploadString(url, "".ToJson());
            listing = updatedJson.FromJson<ListingView>();

            return listing;
        }

        public ListingView UpdateListing(
            string listingId,
            string title,
            string content,
            string listingType,
            //TODO: Investigate combining Request & Response models?
            PricingInfoView pricingInfo,
            IDictionary<string, string> metadata,
            HttpFileCollectionBase files)
        {
            var client = ClassyAuth.GetAuthenticatedWebClient();
            var data = new
            {
                Title = title,
                Content = content,
                ListingType = listingType,
                Pricing = pricingInfo,
                Metadata = metadata

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
        
        public SearchResultsView<ListingView> SearchListings(
            string tag,
            string listingType,
            IDictionary<string, string> metadata,
            double? priceMin,
            double? priceMax,
            LocationView location,
            int page)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = SEARCH_LISTINGS_URL;
                var data = new
                {
                    Tag = tag,
                    ListingType = listingType,
                    Metadata = metadata,
                    PriceMin = priceMin,
                    PriceMax = priceMax,
                    Location = location,
                    Page = page
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
                var data = new
                {
                    ProfileId = profileId,
                    IncludeDrafts = includeDrafts
                }.ToJson();

                var listingsJson = client.DownloadString(url);
                var listings = listingsJson.FromJson<IList<ListingView>>();
                return listings;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public CommentView PostComment(string listingId, string content)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(POST_COMMENT_URL, listingId);
                var commentJson = client.UploadString(url, string.Concat("{\"Content\":\"", content, "\"}"));
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

        #endregion

        #region // collections

        public CollectionView GetCollectionById(string collectionId, bool includeListings, bool increaseViewCounter, bool increaseViewCounterOnListings)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var collectionJson = client.DownloadString(string.Format(GET_COLLECTION_BY_ID_URL, collectionId, includeListings, increaseViewCounter, increaseViewCounterOnListings));
                var collection = collectionJson.FromJson<CollectionView>();
                return collection;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public IList<CollectionView> GetCollectionsByProfileId(string profileId, bool includeListings, bool increaseViewCounter, bool increaseViewCounterOnListings)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var collectionsJson = client.DownloadString(string.Format(GET_COLLECTIONS_FOR_PROFILE_URL, profileId));
                var collections = collectionsJson.FromJson<IList<CollectionView>>();
                return collections;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public CollectionView CreateCollection(
            string title,
            string content,
            string[] listingIds)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var data = new
                {
                    Title = title,
                    Content = content,
                    IncludedListings = listingIds.Select(l => new IncludedListing { ListingId = l, Comments = string.Empty })
                }.ToJson();
                var collectionJson = client.UploadString(CREATE_COLLECTION_URL, data);
                var collection = collectionJson.FromJson<CollectionView>();
                return collection;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public CollectionView AddListingToCollection(
            string collectionId,
            string[] listingIds)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(ADD_LISTINGS_TO_CLECTION_URL, collectionId);
                var data = new
                {
                    CollectionId = collectionId,
                    IncludedListings = listingIds
                }.ToJson();
                var collectionJson = client.UploadString(url, data);
                var collection = collectionJson.FromJson<CollectionView>();
                return collection;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public CollectionView UpdateCollection(string collectionId,
            string title,
            string content,
            IList<IncludedListing> listings)
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
                var collectionJson = client.UploadString(url, data);
                var collection = collectionJson.FromJson<CollectionView>();
                return collection;
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
    }
}
