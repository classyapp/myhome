using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc
{
    public static class CsvStringExtensions
    {
        public static string CleanCsvString(this string csvString)
        {
            csvString = csvString.TrimStart('\"').TrimEnd('\"');
            return string.IsNullOrEmpty(csvString) ? null : csvString;
        }
    }
}
