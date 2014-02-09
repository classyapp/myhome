﻿using Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.Controllers;
using MyHome.Models;
using Classy.DotNet.Responses;
using Classy.DotNet.Mvc;

namespace MyHome.Controllers
{
    public class ProfileController : Classy.DotNet.Mvc.Controllers.ProfileController<ProfessionalMetadata, ProfileReviewCriteria>
    {
        private readonly string MANDRILL_API_KEY = "ndg42WcyRHVLtLbvGqBjUA";

        public ProfileController()
            : base("MyHome.Controllers") {
                base.OnContactProfessional += ProfileController_OnContactProfessional;
                base.OnParseProfilesCsvLine += ProfileController_OnParseProfilesCsvLine;
        }

        public void ProfileController_OnParseProfilesCsvLine(object sender, ParseProfilesCsvLineArgs<ProfessionalMetadata> e)
        {
            if (e.IsHeaderLine) return;
            else
            {
                e.ProfessionalInfo = new ProfessionalInfoView
                {
                    Category = e.LineValues[0].CleanCsvString(),
                    CompanyName = e.LineValues[1].CleanCsvString(),
                    CompanyContactInfo = new ExtendedContactInfoView
                    {
                        Location = new LocationView
                        {
                            Address = new PhysicalAddressView
                            {
                                Street1 = e.LineValues[3].CleanCsvString(),
                                City = e.LineValues[4].CleanCsvString(),
                                PostalCode = e.LineValues[6].CleanCsvString(),
                                Country = e.LineValues[7].CleanCsvString()
                            }
                        },
                        Phone = e.LineValues[8].CleanCsvString(),
                        WebsiteUrl = e.LineValues[10].CleanCsvString(),
                        Email = e.LineValues[11].CleanCsvString()
                    }
                };
            }
        }

        public void ProfileController_OnContactProfessional(object sender, ContactProfessionalArgs<ProfessionalMetadata> e) {
            // email professional
            var message = new EmailMessage
            {
                subject = string.Format("פניה מאתר מיי הום: {0}", e.Subject),
                to = new List<EmailAddress> {
                    new EmailAddress {
                        email = e.ProfessionalProfile.ContactInfo.Email
                    }
                }
            };
            message.AddHeader("Reply-To", e.ReplyToEmail);
            message.AddGlobalVariable("CONTENT", e.Content);
            var api = new MandrillApi(MANDRILL_API_KEY);
            var sendResponse = api.SendMessage(message, "notification_contact_pro", null);
        }
    }
}