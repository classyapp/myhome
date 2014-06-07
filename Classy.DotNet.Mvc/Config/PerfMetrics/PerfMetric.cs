using System;
using System.Collections.Generic;

namespace Classy.DotNet.Mvc.Config.PerfMetrics
{
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
}