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

        public string UserName { get; set; }

        public string Password { get; set; }

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
            return _tfs.ConnectToTFS(URL, UserName, Password);
        }

        public List<VKWorkItem> GetVKWorkItems()
        {
            List<VKWorkItem> vkWorkItems = new List<VKWorkItem>();


            if (_tfs.ConnectToTFS(URL, UserName, Password))
                _tfs.GetVKWorkItems(QuertStr, ref vkWorkItems);

            return vkWorkItems;
        }
         
    }
}
