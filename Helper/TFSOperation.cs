using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSUtils;

namespace Helper
{
    public class TFSOperation
    {
        private static TFSOperation instance;

        public string URL { get; set; }
        public string ProjectName { get; set; }
        public string QuertStr { get; set; }

        private TFSOperation()
        {
        }

        public static TFSOperation Instance
        {
            get
            {
                return instance ??
                    (instance = new TFSOperation());
            }
        }

        private TFSFunction _tfs = new TFSFunction();

        public bool ConnectTFS()
        {
            return _tfs.ConnectToTFS(URL);
        }

        public List<VKWorkItem> GetVKWorkItems()
        {
            List<VKWorkItem> vkWorkItems = new List<VKWorkItem>();



            if (_tfs.ConnectToTFS(URL))
            {
                _tfs.GetVKWorkItems(QuertStr, ref vkWorkItems);
            }
                

            return vkWorkItems;
        }

        public List<ChangeSetItem> GetChangeItems(string serverPath) {

            List<ChangeSetItem> changeSetItems = new List<ChangeSetItem>();

            if (_tfs.ConnectToTFS(URL))
                _tfs.GetChangesets(serverPath, changeSetItems);

            return changeSetItems;
        }
    }
}
