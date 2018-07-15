using System;
using System.Collections.Generic;
using System.Text;

namespace vmstats_shared
{
    public class Metric
    {
        // constants that define the names of the metrics
        public static readonly string BASE = "Base";                // BASE metrics from which all the other metrics are derived


        public SortedDictionary<long, float> Values { get; set; }
        public string Name { get; set; }

        public Metric(string name, SortedDictionary<long, float> values)
        {
            Name = name;
            Values = values;
        }
    }
}
