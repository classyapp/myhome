﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Responses
{
    public class VerifyEmailResponse
    {
        public bool Verified { get; set; }
        public string ErrorMessage { get; set; }
    }
}
