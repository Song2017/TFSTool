using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    [DataContract]
    public class ChangeSetItem
    {
        [DataMember]
        public int ChangesetId { get; set; }


        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string Committer { get; set; }

        [DataMember]
        public string CommitterDisplayName { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public string Owner { get; set; }

        [DataMember]
        public string OwnerDisplayName { get; set; }
    }
}
