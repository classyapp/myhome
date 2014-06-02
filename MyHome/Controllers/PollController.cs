using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Classy.DotNet.Models.LogActivity;
using Classy.DotNet.Mvc.Controllers;
using Classy.DotNet.Mvc.Extensions;
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
            var listingViews = listingService.GetListings(listings.ToArray());

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
                UserVote = userVote
            };
        }

        public override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);

            routes.MapRoute(
                name: "SelectListingsModal",
                url: "polls/create/select-listings-modal",
                defaults: new { controller = "Poll", action = "SelectListingsModal" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "SelectListingPhotosModal",
                url: "polls/create/select-photos-modal",
                defaults: new { controller = "Poll", action = "SelectPhotosModal" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "VoteOnPoll",
                url: "polls/vote",
                defaults: new { controller = "Poll", action = "VoteOnPoll" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "CreateNewPoll",
                url: "polls/create-new",
                defaults: new { controller = "Poll", action = "CreateNewPoll" },
                namespaces: new string[] { Namespace }
            );
        }

        public override string ListingTypeName
        {
	        get {  return "Poll"; }
        }

        public ActionResult SelectListingsModal()
        {
            var listingService = new ListingService();
            string collectionType = AuthenticatedUserProfile.IsProfessional ? CollectionType.Project : CollectionType.PhotoBook;
            var collections = listingService.GetCollectionsByProfileId(AuthenticatedUserProfile.Id, collectionType, false, false, false);

            return PartialView("SelectListingsModal", collections);
        }

        [AcceptVerbs(HttpVerbs.Post)]
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
        public ActionResult VoteOnPoll(string pollId, string listingId)
        {
            // TODO: this isn't thread safe!!
            // TODO: try to implement at least OptimisticLocking

            var logActivityService = new LogActivityService();
            var listingService = new ListingService();

            // check if the user voted on this poll already
            var userPollActivity = logActivityService.GetLogActivity(new LogActivity<VotedOnPollActivityMetadata> {
                UserId = AuthenticatedUserProfile.Id,
                Activity = ActivityPredicate.VOTED_ON_POLL,
                ObjectId = pollId
            });
            if (userPollActivity != null && userPollActivity.Metadata.Vote == listingId)
                return Json("OK");

            // user didn't vote on this listing yet
            var listing = listingService.GetListingById(pollId, false, false, false, false, false);

            var votedOn = listing.Metadata.Single(x => x.Key.StartsWith("Listing_") && x.Value == listingId);
            var listingNumber = votedOn.Key.Substring(votedOn.Key.IndexOf("_") + 1);
            var voteKey = "Vote_" + listingNumber;
            if (listing.Metadata.ContainsKey(voteKey))
                listing.Metadata[voteKey] = (Convert.ToInt32(listing.Metadata[voteKey]) + 1).ToString();
            else
                listing.Metadata.Add(voteKey, "1");

            var metadata = listing.Metadata;

            listingService.UpdateListing(pollId,
                null, null, null, metadata, null, null, ListingUpdateFields.Metadata);

            logActivityService.LogActivity(new LogActivity<VotedOnPollActivityMetadata> {
                UserId = AuthenticatedUserProfile.Id,
                Activity = ActivityPredicate.VOTED_ON_POLL,
                ObjectId = pollId,
                Metadata = new VotedOnPollActivityMetadata {Vote = listingId}
            });

            return Json("OK");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateNewPoll(CreateNewPollRequest newPollRequest)
        {
            var listingService = new ListingService();
            var newCollection = listingService.CreateCollection("Poll", "My Polls", null, new IncludedListingView[0]);

            var metadata = new PollMetadata {
                Listings = newPollRequest.ListingIds.ToList()
            }.ToDictionary();
            var newPoll = listingService.CreateListing(newPollRequest.Title,
                newPollRequest.Content, "Poll", null, metadata, Request.Files);
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