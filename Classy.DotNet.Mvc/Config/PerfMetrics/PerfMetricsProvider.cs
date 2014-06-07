using System;
using System.Collections.Generic;
using System.Web;

namespace Classy.DotNet.Mvc.Config.PerfMetrics
{
    public class PerfMetricsProvider
    {
        public const string PerfMetricsKey = "__PerfMetrics__";

        [ThreadStatic] public static Guid CurrentMetric;

        public static List<PerfMetric> Get()
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
                return new List<PerfMetric>();

            if (!httpContext.Items.Contains(PerfMetricsKey))
                httpContext.Items.Add(PerfMetricsKey, new List<PerfMetric>());

            var perfMetrics = httpContext.Items[PerfMetricsKey] as List<PerfMetric>;
            if (perfMetrics == null)
                return new List<PerfMetric>();

            return perfMetrics;
        }

        public static void Set(PerfMetric perfMetric)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
                return;

            if (!httpContext.Items.Contains(PerfMetricsKey))
                httpContext.Items.Add(PerfMetricsKey, new List<PerfMetric>());

            var perfMetrics = httpContext.Items[PerfMetricsKey] as List<PerfMetric>;
            if (perfMetrics == null)
                return;

            perfMetrics.Add(perfMetric);
        }
    }
}