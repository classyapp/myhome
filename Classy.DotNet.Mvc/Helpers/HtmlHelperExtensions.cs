﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.WebPages.Html;
using System.Web.Mvc.Html;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Localization;
using Classy.DotNet.Responses;
using System.Configuration;
using System.Text;
using System.Globalization;

namespace Classy.DotNet.Mvc
{
    public static class HtmlHelperExtensions
    {
        //public static RenderComment(this HtmlHelper html, string comment)
        //{
        //    var hashRegex = new Regex(@"\B#\w\w+");
        //    var hashtags = hashRegex.Matches(comment)
        //        .Cast<Match>()
        //        .Select(m => m.Value)
        //        .ToArray();

        //    var userRegex = new Regex(@"\B@\w\w+");
        //    var usernames = userRegex.Matches(comment)
        //        .Cast<Match>()
        //        .Select(m => m.Value)
        //        .ToArray();

        //    return 
        //}

        #region // triger listing actions

        public static MvcHtmlString TriggerListingActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ListingView listing)
        {
            return TriggerListingActionLink(html, linkText, actionToTrigger, listing, null, true);
        }

        public static MvcHtmlString TriggerListingActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ListingView listing, string cssClass)
        {
            return TriggerListingActionLink(html, linkText, actionToTrigger, listing, cssClass, true);
        }

        public static MvcHtmlString TriggerListingActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ListingView listing, string cssClass, bool requireLogin)
        {
            var needsAuth = requireLogin && !html.ViewContext.HttpContext.User.Identity.IsAuthenticated;
            string link = "<a href=\"#\" trigger-listing-action=\"{0}\" listing-type=\"{1}\" listing-id=\"{2}\" {3} {4}>{5}</a>";
            string output = string.Format(link,
                actionToTrigger,
                listing.ListingType.ToLower(),
                listing.Id,
                !string.IsNullOrEmpty(cssClass) ? string.Format("class=\"{0}\"", cssClass) : string.Empty,
                requireLogin ? "authorize" : string.Empty,
                linkText);
            return new MvcHtmlString(output);
        }

        #endregion

        #region // triger profile actions

        public static MvcHtmlString TriggerProfileActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ProfileView profile)
        {
            return TriggerProfileActionLink(html, linkText, actionToTrigger, profile, null, true);
        }

        public static MvcHtmlString TriggerProfileActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ProfileView profile, string cssClass)
        {
            return TriggerProfileActionLink(html, linkText, actionToTrigger, profile, cssClass, true);
        }

        public static MvcHtmlString TriggerProfileActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ProfileView profile, string cssClass, bool requireLogin)
        {
            string link = "<a href=\"#\" trigger-profile-action=\"{0}\" profile-id=\"{1}\" {2} {3}>{4}</a>";
            string output = string.Format(link,
                actionToTrigger,
                profile.Id,
                !string.IsNullOrEmpty(cssClass) ? string.Format("class=\"{0}\"", cssClass) : string.Empty,
                requireLogin ? "authorize" : string.Empty,
                linkText);
            return new MvcHtmlString(output);
        }

        #endregion

        // photo thumb
        public static MvcHtmlString Thumbnail(this System.Web.Mvc.HtmlHelper html, ListingView listing, int size)
        {
            if (listing.ExternalMedia != null && listing.ExternalMedia.Count() > 0)
            {
                string url = string.Format("//{0}/thumbnail/{1}?Width={2}&format=json",
                    ConfigurationManager.AppSettings["Classy:CloudFrontDistributionUrl"], listing.ExternalMedia[0].Key, size);
                return new MvcHtmlString(string.Format("<img src=\"{0}\" title=\"{1}\" alt=\"{2}\" class=\"img-responsive\" />",
                                    url, listing.Title, listing.Title));
            }

            return new MvcHtmlString(string.Format("<img src=\"{0}\" title=\"{1}\" alt=\"{2}\" class=\"img-responsive\" />",
                                    "/img/missing-thumb.png", listing.Title, listing.Title));
        }

        public static MvcHtmlString ListingLink(this System.Web.Mvc.HtmlHelper html, string listingType, ListingView listing)
        {
            return html.RouteLinkForCurrentLocale(listing.Title, string.Concat(listingType, "Details"), new { listingId = listing.Id, slug = ToSlug(listing.Title) });
        }

        public static MvcHtmlString CollectionLink(this System.Web.Mvc.HtmlHelper html, CollectionView collection)
        {
            return html.RouteLinkForCurrentLocale(collection.Title, "CollectionDetails", new { collectionId = collection.Id, view = "grid", slug = ToSlug(collection.Title) });
        }

        public static MvcHtmlString ProfileLink(this System.Web.Mvc.HtmlHelper html, ProfileView profile)
        {
            var name = profile.GetProfileName();
            return html.RouteLinkForCurrentLocale(name, "PublicProfile", new { profileId = profile.Id, slug = ToSlug(name) });
        }

        public static string ProfileUrl(this System.Web.Mvc.UrlHelper url, ProfileView profile)
        {
            return url.RouteUrlForCurrentLocale("PublicProfile", new { profileId = profile.Id });
        }

        public static MvcHtmlString ToSlug(this System.Web.Mvc.HtmlHelper html, string content)
        {
            return new MvcHtmlString(content.ToSlug());
        }

        public static string ToSlug(this string content)
        {
            return _ToSlug(content);
        }

        private static string _ToSlug(string content)
        {
            return content != null ? content.ToLower()
                .Replace("?", string.Empty)
                .Replace("-", string.Empty)
                .Replace("/", string.Empty)
                .Replace("&", "-and-")
                .Replace("+", "-and-")
                .Replace(".", string.Empty)
                .Replace("  ", " ")
                .Replace(" ", "-") : null;
        }

        public static string ToValidUrl(this string content)
        {
            var url = content.StartsWith("http://") ? content : string.Concat("http://", content);
            return url;
        }
    }
}