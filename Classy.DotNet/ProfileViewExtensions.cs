using Classy.DotNet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Classy.DotNet
{
    public static class ProfileViewExtensions
    {
        public static bool CanEdit(this ProfileView profile)
        {
            var isAdmin = false;
            var isProfileOwner = false;
            var context = HttpContext.Current;
            if (context != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                isAdmin = (HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.Permissions.Contains("admin");
                isProfileOwner = (HttpContext.Current.User.Identity as Classy.DotNet.Security.ClassyIdentity).Profile.Id == profile.Id;
            }
            return isAdmin || isProfileOwner;
        }

        public static string GetProfileName(this ProfileView profile)
        {
            if (profile.ContactInfo == null && !profile.IsProfessional) return "unknown";
            string name;
            if (profile.IsProxy) name = profile.ProfessionalInfo.CompanyName;
            else if (profile.IsProfessional) name = profile.ProfessionalInfo.CompanyName;
            else name = string.IsNullOrEmpty(profile.ContactInfo.Name) ? profile.UserName : profile.ContactInfo.Name;
            return name ?? "unknown";
        }

        public static string GetProfessionalAddressOneLine(this ProfileView profile)
        {
            if (profile.ProfessionalInfo.CompanyContactInfo != null &&
                profile.ProfessionalInfo.CompanyContactInfo.Location != null &&
                profile.ProfessionalInfo.CompanyContactInfo.Location.Address != null)
            {
                var address = string.Concat(
                    profile.ProfessionalInfo.CompanyContactInfo.Location.Address.City, ", ", 
                    profile.ProfessionalInfo.CompanyContactInfo.Location.Address.Country, ", ",
                    profile.ProfessionalInfo.CompanyContactInfo.Location.Address.PostalCode);
                return address;
            }
            else return null;
        }
    }
}
