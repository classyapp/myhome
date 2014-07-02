﻿using System.Collections.Generic;

namespace Classy.DotNet.Responses
{
    public class CurrencyListItemView : ListItemView
    {
        public string Sign { get; set; }
    }

    public class ListItemView
    {
        public string Value { get; set; }
        public string ParentValue { get; set; }
        public IDictionary<string, string> Text { get; set; }
    }

    public class LocalizationListResourceView
    {
        /// <summary>
        /// the resoruce key.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// dictionary of values. the key for the dictionary is the culture name.
        /// </summary>
        public IList<ListItemView> ListItems { get; set; }
    }
}
