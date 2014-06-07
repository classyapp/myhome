using System;

namespace Classy.DotNet.Mvc.Config.PerfMetrics
{
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