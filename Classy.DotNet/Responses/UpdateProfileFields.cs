using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    [Flags]
    public enum UpdateProfileFields
    {
        None = 0,
        SetPassword = 1,
        ContactInfo = 2,
        ProfessionalInfo = 4,
        Metadata = 8,
        Password = 16,
        CoverPhotos = 32
    }
}
