using System;
using System.Collections.Generic;
using System.Web;

namespace Classy.DotNet.Mvc.Config
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

    public class PerfMetric
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int Time { get; set; }
        public int Count { get; set; }

        public Guid Parent { get; set; }
        public List<PerfMetric> Children { get; set; }

        public PerfMetric()
        {
            Id = Guid.NewGuid();
            Children = new List<PerfMetric>();
        }
    }

    public class Perf : IDisposable
    {
        private readonly DateTime _startTime;
        private readonly string _name;

        public static Perf Measure(string perfMetricName)
        {
            return new Perf(perfMetricName);
        }

        public Perf(string name)
        {
            _startTime = DateTime.Now;
            _name = name;
        }

        public void Dispose()
        {
            var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
            PerfMetricsProvider.Set(new PerfMetric {
                Name = _name,
                Time = (int)elapsed
            });
        }
    }
}