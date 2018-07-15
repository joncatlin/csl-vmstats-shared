using Akka.Routing;
using System;
using System.Collections.Generic;

namespace vmstats_shared
{
    public class Messages
    {
        #region funcational messages
        /// <summary>
        /// Message that contains some metrics to be processed
        /// </summary>
        public class MetricsToBeProcessed {
            public string vmName { get; set; }
            public string date { get; set; }
            public string elementName { get; set; }
            public long time { get; set; }
            public float element { get; set; }

            public MetricsToBeProcessed (string vmName, string date, long time, float element, string elementName)
            {
                this.vmName = vmName;
                this.date = date;
                this.time = time;
                this.element = element;
                this.elementName = elementName;
            }
        }

        /// <summary>
        /// Message that signals an actor is finished processing and is stopping
        /// </summary>
        public class Stopping { }

        /// <summary>
        /// Message that signals the current processing of the directory has found no files
        /// </summary>
        public class NoMoreMetrics { }

        /// <summary>
        /// Message that signals the processing for a specific file is complete
        /// </summary>
        public class FileComplete
        {
            public string name { get; set; }
            public string[] headings { get; set; }

            public FileComplete(string name, string[] headings)
            {
                this.name = name;
                this.headings = headings;
            }
        }

        /// <summary>
        /// Message sent to inform of a potentially new MetricStoreActor in the system
        /// </summary>
        public class PotentialNewActor
        {
            public string VmName { get; private set; }
            public string Date { get; private set; }
            public long SeqNr { get; private set; }

            public PotentialNewActor(string vmName, string date, long seqNr)
            {
                VmName = vmName;
                Date = date;
                SeqNr = seqNr;
            }
        }

        /// <summary>
        /// Message sent from the client requesting a transform pipeline be built and executed.
        /// </summary>
        public class ProcessCommand
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string VmPattern { get; set; }
            public string Dsl { get; set; }
            public string ConnectionId { get; set; }
        }


        /// <summary>
        /// Message that signals to find all the metricstore names and log them
        /// </summary>
        public class FindMetricStoreActorNames { };

        #endregion

        /// <summary>
        /// This class holds a single transform, comprisiong a name and optionally some parameters
        /// The transformation can be supplied with or without parameters to override the default 
        /// settings within the transformation actor.
        /// </summary>
        public class Transform
        {
            public Transform(string name, Dictionary<string, string> paramaters)
            {
                Name = name;
                Parameters = paramaters;
            }

            public Transform(string name)
            {
                Name = name;
                Parameters = new Dictionary<string, string>();
            }

            public string Name { get; set; }
            public Dictionary<string, string> Parameters { get; set; }
        }


        /// <summary>
        /// This class holds the result that is passed from the vmstats server to the GUI server.
        /// </summary>
        public class Result
        {
            public Result (string connectionId, long[] xdata, float[] ydata)
            {
                ConnectionId = connectionId;
                Xdata = xdata;
                Ydata = ydata;
            }

            public string ConnectionId { get; private set; }
            public long[] Xdata { get; set; }
            public float[] Ydata { get; set; }
        }


        /// <summary>
        /// This class holds a single Metric and a series of Transforms to be performed upon it. It also
        /// contains a unique ID so that if a transform actor is part of a consistent hashing routing group
        /// then all the transforms for this particular transform series will be routed to the same actor.
        /// 
        /// This is required in order for the Combine transform to work properly
        /// </summary>
        public class TransformSeries : IConsistentHashable
        {
            public TransformSeries(Metric metric, Queue<Transform> transforms, Guid groupID, string connectionId)
            {
                Measurements = metric;
                Transforms = transforms;
                GroupID = groupID;
                ConnectionId = connectionId;
            }

            public Guid GroupID { get; private set; }
            public object ConsistentHashKey { get { return GroupID; } }

            public Metric Measurements { get; private set; }
            public Queue<Transform> Transforms { get; private set; }
            public string ConnectionId { get; private set; }
        }

        /// <summary>
        /// This class requests a Metric be obtained from the population of MetricStoreActors and then formed into
        /// a TransformSeries and sent to the TransformActor population for processing.
        /// </summary>
        public class BuildTransformSeries
        {
            public BuildTransformSeries(string metricName, Queue<Transform> transforms, Guid groupID)
            {
                MetricName = metricName;
                Transforms = transforms;
                GroupID = groupID;
            }

            public Guid GroupID { get; private set; }
            public string MetricName { get; private set; }
            public Queue<Transform> Transforms { get; private set; }
            public string ConnectionId { get; set; }
        }


        /// <summary>
        /// This class requests that a transform pipeline start to be processed
        /// </summary>
        public class StartProcessingTransformPipeline
        {
            public StartProcessingTransformPipeline(ProcessCommand cmd, Queue<BuildTransformSeries>queue)
            {
                this.cmd = cmd;
                this.queue = queue;
            }

            public ProcessCommand cmd { get; private set; }
            public Queue<BuildTransformSeries> queue { get; private set; }

        }


    }
}
