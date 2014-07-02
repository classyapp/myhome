using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Models.LogActivity;
using Classy.DotNet.Models.Search;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.Extensions;
using Classy.DotNet.Mvc.ViewModels.Listing;
using Classy.DotNet.Responses;
using Classy.DotNet.Services;
using MyHome.Models;
using MyHome.Models.Polls;

namespace MyHome.Controllers
{
    public class  PollController : ListingController<PollMetadata, PhotoGridViewModel>
    {
        public PollController() : base("MyHome.Controllers")
        {
            base.OnListingLoaded += PollController_OnListingLoaded;
        }

        private void PollController_OnListingLoaded(object sender, ListingLoadedEventArgs<PollMetadata> listingLoadedEventArgs)
        {
            var listingService = new ListingService();
            var listings = listingLoadedEventArgs.ListingDetailsViewModel.Metadata.Listings;
            var listingViews = listingService.GetListings(listings.ToArray(), true);

            var logActivityService = new LogActivityService();
            var pollId = listingLoadedEventArgs.ListingDetailsViewModel.Listing.Id;

            string userVote = string.Empty;
            if (AuthenticatedUserProfile != null)
            {
                var votedOnPollActivity = logActivityService.GetLogActivity(new LogActivity<VotedOnPollActivityMetadata> {
                    UserId = AuthenticatedUserProfile.Id,
                    Activity = ActivityPredicate.VOTED_ON_POLL,
                    ObjectId = pollId
                });
                if (votedOnPollActivity != null && !votedOnPollActivity.Metadata.Vote.IsNullOrEmpty())
                    userVote = votedOnPollActivity.Metadata.Vote;
            }

            listingLoadedEventArgs.ListingDetailsViewModel.ExtraData = new PollViewExtraData {
                Listings = listingViews,
                UserVote = userVote,
                EndDate = listingLoadedEventArgs.ListingDetailsViewModel.Metadata.EndDate
            };
        }

        public override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);
            RegisterRoutesByAttributes(routes, ListingTypeName);
        }

        public override string ListingTypeName
        {
	        get {  return "Poll"; }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [MapRoute("GetRelatedPhotos", "polls/related-photos")]
        public ActionResult GetRelatedPhotos(string pollId)
        {
            var listingService = new ListingService();
            var poll = listingService.GetListingById(pollId, false, false, false, false, false);

            var pollListings = new List<string>(4);
            var j = 0;
            while (poll.Metadata.ContainsKey("Listing_" + j))
            {
                pollListings.Add(poll.Metadata["Listing_" + j]);
                j++;
            }
            var pollsListings = listingService.GetListings(pollListings.ToArray(), true);

            var pollProfiles = pollsListings.Select(x => x.ProfileId).Distinct();

            var moreListings = new List<ListingView>();
            pollProfiles.ForEach(profileId =>
            {
                var profileListings = listingService.GetListingsByProfileId(profileId, false, true);
                moreListings.AddRange(profileListings.Take(6));
            });

            return PartialView("MorePhotosFromPoll", moreListings);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [MapRoute("GetMorePolls", "polls/more-polls")]
        public ActionResult GetMorePolls()
        {
            var listingService = new ListingService();
            var searchResults = listingService.SearchListings(null, new[] {"Poll"}, null, null, null, null, 0, 20, SortMethod.Date);

            if (searchResults == null || searchResults.Results.IsNullOrEmpty())
                return Content("");

            // randomize the results
            var randomResults = searchResults.Results.OrderBy(_ => Guid.NewGuid()).Take(2).ToArray();

            var model = new List<MorePollsModel>();
            for (var i = 0; i < randomResults.Count(); i++)
            {
                var pollListings = new List<string>(4);
                var j = 0;
                while (randomResults[i].Metadata.ContainsKey("Listing_" + j))
                {
                    pollListings.Add(randomResults[i].Metadata["Listing_" + j]);
                    j++;
                }
                var pollsListings = listingService.GetListings(pollListings.ToArray(), true);

                var profileService = new ProfileService();
                var pollProfile = profileService.GetProfileById(randomResults[i].ProfileId);
                 
                model.Add(new MorePollsModel {
                    Id = randomResults[i].Id,
                    Title = randomResults[i].Title,
                    PollCreator = pollProfile.UserName,
                    PollCreatorId = randomResults[i].ProfileId,
                    ImageKeys = string.Join(",", pollsListings.Select(x => x.ExternalMedia[0].Key).ToList())
                });
            }

            return PartialView("MorePolls", model);
        }

        [MapRoute("SelectListingsModal", "polls/create/select-listings-modal")]
        public ActionResult SelectListingsModal()
        {
            var listingService = new ListingService();
            string collectionType = AuthenticatedUserProfile.IsProfessional ? CollectionType.Project : CollectionType.PhotoBook;
            var collections = listingService.GetCollectionsByProfileId(AuthenticatedUserProfile.Id, collectionType, false, false, false);

            return PartialView("SelectListingsModal", collections);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [MapRoute("SelectListingPhotosModal", "polls/create/select-photos-modal")]
        public ActionResult SelectPhotosModal(string collectionId)
        {
            var listingService = new ListingService();
            var collectionView = listingService.GetCollectionById(collectionId, true, false, false, false);

            var listings = collectionView.Listings.Select(x => new {
                Id = x.Id,
                Title = x.Title,
                Image = x.ExternalMedia.IsNullOrEmpty() ? string.Empty : x.ExternalMedia[0].Key
            });

            return Json(listings);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [MapRoute("VoteOnPoll", "polls/vote")]
        public ActionResult VoteOnPoll(string pollId, string listingId)
        {
            // TODO: this isn't thread safe!!
            // TODO: try to implement at least OptimisticLocking

            var logActivityService = new LogActivityService();
            var listingService = new ListingService();

            var listing = listingService.GetListingById(pollId, false, false, false, false, false);
            
            // check if this poll ended already
            if (listing.Metadata.ContainsKey("EndDate") && Convert.ToDateTime(listing.Metadata["EndDate"]) < DateTime.Now)
                return Content("Voted Ended Already");

            // check if the user voted on this poll already
            var userPollActivity = logActivityService.GetLogActivity(new LogActivity<VotedOnPollActivityMetadata> {
                UserId = AuthenticatedUserProfile.Id,
                Activity = ActivityPredicate.VOTED_ON_POLL,
                ObjectId = pollId
            });
            // check if user voted on the same listing already
            if (userPollActivity != null && userPollActivity.Metadata.Vote == listingId)
                return Json("OK");

            // user voted on this poll but a different listing
            var votedOn = listing.Metadata.Single(x => x.Key.StartsWith("Listing_") && x.Value == listingId);
            var listingNumber = votedOn.Key.Substring(votedOn.Key.IndexOf("_") + 1);
            var voteKey = "Vote_" + listingNumber;
            if (listing.Metadata.ContainsKey(voteKey))
                listing.Metadata[voteKey] = (Convert.ToInt32(listing.Metadata[voteKey]) + 1).ToString();
            else
                listing.Metadata.Add(voteKey, "1");
            
            // find previous vote and decrement value
            if (userPollActivity != null && !userPollActivity.Metadata.Vote.IsNullOrEmpty())
            {
                var previousVote = userPollActivity.Metadata.Vote;
                var previousVotedOn = listing.Metadata.Single(x => x.Key.StartsWith("Listing_") && x.Value == previousVote);
                var previousVoteNumber = previousVotedOn.Key.Substring(previousVotedOn.Key.IndexOf("_") + 1);
                listing.Metadata["Vote_" + previousVoteNumber] = (Convert.ToInt32(listing.Metadata["Vote_" + previousVoteNumber]) - 1).ToString();
            }

            var metadata = listing.Metadata;

            listingService.UpdateListing(pollId,
                null, null, null, null, metadata, listing.Hashtags, null, ListingUpdateFields.Metadata);

            logActivityService.LogActivity(new LogActivity<VotedOnPollActivityMetadata> {
                UserId = AuthenticatedUserProfile.Id,
                Activity = ActivityPredicate.VOTED_ON_POLL,
                ObjectId = pollId,
                Metadata = new VotedOnPollActivityMetadata {Vote = listingId}
            });

            return Json("OK");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [MapRoute("CreateNewPoll", "polls/create-new")]
        public ActionResult CreateNewPoll(CreateNewPollRequest newPollRequest)
        {
            var listingService = new ListingService();
            var newCollection = listingService.CreateCollection("Poll", "My Polls", null, new IncludedListingView[0]);

            var metadata = new PollMetadata {
                Listings = newPollRequest.ListingIds.ToList()
            }.ToDictionary();
            var newPoll = listingService.CreateListing(newPollRequest.Title,
                newPollRequest.Content, "Poll", null, null, metadata, Request.Files);
            listingService.AddListingToCollection(newCollection.Id, new[] {
                new IncludedListingView {
                    Id = newPoll.Id,
                    Comments = null,
                    ProfileId = AuthenticatedUserProfile.Id,
                    ListingType = "Poll"
                }
            });
            
            return Redirect(Url.RouteUrl("PollDetails", new { controller = "Poll", action = "GetListingById", listingId = newPoll.Id }));
        }
	}
}