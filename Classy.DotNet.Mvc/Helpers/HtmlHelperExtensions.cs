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
                profile.UserName,
                !string.IsNullOrEmpty(cssClass) ? string.Format("class=\"{0}\"", cssClass) : string.Empty, 
                requireLogin ? "authorize" : string.Empty,
                linkText);
            return new MvcHtmlString(output);
        }

        #endregion

        public static MvcHtmlString ListingLink(this System.Web.Mvc.HtmlHelper html, string listingType, ListingView listing)
        {
            return html.RouteLinkForCurrentLocale(listing.Title, string.Concat(listingType, "Details"), new { listingId = listing.Id, slug = ToSlug(listing.Title) });
        }

        public static MvcHtmlString CollectionLink(this System.Web.Mvc.HtmlHelper html, CollectionView collection)
        {
            return html.RouteLinkForCurrentLocale(collection.Title, "CollectionDetails", new { collectionId = collection.Id, slug = ToSlug(collection.Title) });
        }

        public static MvcHtmlString ProfileLink(this System.Web.Mvc.HtmlHelper html, ProfileView profile)
        {
            var name = profile.GetProfileName();
            return html.RouteLinkForCurrentLocale(name, "PublicProfile", new { profileId = profile.Id, slug = ToSlug(name) });
        }

        public static string GetProfileName(this ProfileView profile)
        {
            if (profile.ContactInfo == null && !profile.IsProfessional) return "unknown";
            var name = string.Empty;
            if (profile.IsProxy) name = profile.ProfessionalInfo.CompanyName;
            else if (profile.IsProfessional) name = profile.ProfessionalInfo.CompanyName;
            else name = string.IsNullOrEmpty(profile.ContactInfo.Name) ? profile.UserName : profile.ContactInfo.Name;
            return name;
        }

        public static MvcHtmlString ToSlug(this System.Web.Mvc.HtmlHelper html, string content)
        {
            return new MvcHtmlString(ToSlug(content));
        }

        private static string ToSlug(string content)
        {
            return content != null ? content.ToLower()
                .Replace("?", string.Empty)
                .Replace("&", "-and-")
                .Replace(".", "")
                .Replace("  ", " ")
                .Replace(" ", "-") : null;
        }
    }
}