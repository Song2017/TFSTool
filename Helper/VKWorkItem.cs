using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    [DataContract]
    public class VKWorkItem
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string IterationPath { get; set; }

        [DataMember]
        public string State { get; set; }
        [DataMember]
        public bool IsOpen { get; set; }

        [DataMember]
        public string WorkItemType { get; set; }

        [DataMember]
        public int Effort { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string AssignedTo { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime FinishDate { get; set; }

        [DataMember]
        public DateTime ChangedDate { get; internal set; }
        [DataMember]
        public string Description { get; internal set; }
    }
}
