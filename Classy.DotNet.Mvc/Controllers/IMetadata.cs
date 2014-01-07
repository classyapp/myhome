﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.Controllers
{
    // marker interface for constraining types passe into generic controllers
    public interface IMetadata<TMetadata> where TMetadata : new()
    {
        IDictionary<string, string> ToDictionary();
        TMetadata FromDictionary(IDictionary<string, string> metadata);
        TMetadata FromStringArray(string[] strings);
        string ToSlug();
    }
}
