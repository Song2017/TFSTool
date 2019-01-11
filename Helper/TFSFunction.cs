using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Net;
using TFSUtils;

namespace Helper
{
    public class TFSFunction
    {

        public bool IsConnected { get; private set; }
        private WorkItemStore _workItemStore;
        private VersionControlServer _versionControlServer;

        public TFSFunction()
        {
        }

        public bool ConnectToTFS(string tfsUrl)
        {
            string username = Utils.GetConfig(AppConstants.TFS_USERNAME);
            string password = Utils.GetConfig(AppConstants.TFS_PASSWORD);
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                throw new Exception("Please set tfs credential in app.config.");
            }
            if (IsConnected) return true;
            try
            {
                int tryTimes = 10;
                while (--tryTimes > 0)
                {
                    WindowsCredential wCre = new WindowsCredential(new NetworkCredential(username, password));
                    TfsClientCredentials tfsCred = new TfsClientCredentials(wCre)
                    {
                        AllowInteractive = false
                    };
                    TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(tfsUrl), tfsCred);
                    tpc.Authenticate();
                    _workItemStore = tpc.GetService<WorkItemStore>();
                    _versionControlServer = tpc.GetService<VersionControlServer>();
                    IsConnected = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            IsConnected = false;
            return false;
        }

        public bool GetVKWorkItems(string quertStr, ref List<VKWorkItem> vkWorkItems)
        {
            if (!IsConnected || _workItemStore == null)
                return false;

            try
            {
                WorkItemCollection items = _workItemStore.Query(quertStr);
                VKWorkItem vkWorkItem;

                foreach (WorkItem wi in items)
                {
                    vkWorkItem = new VKWorkItem
                    {
                        ID = wi.Id,
                        IterationPath = wi.IterationPath,
                        Title = wi.Title,
                        State = wi.State,
                        WorkItemType = wi.Type.Name.ToStringEx(),
                        StartDate = wi.CreatedDate,
                        ChangedDate = wi.ChangedDate,
                        Description = wi.Description,
                        AssignedTo = wi.Fields["System.AssignedTo"].Value.ToStringEx()
                    };

                    vkWorkItems.Add(vkWorkItem);
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool GetChangesets(string serverPath , List<ChangeSetItem> csItems)
        {

            if (!IsConnected || _workItemStore == null)
                return false;

            //Get the source code control service. 

            var history = _versionControlServer.QueryHistory(
                serverPath,  //Source control path to the item. Like $/Project/Path ...
                LatestVersionSpec.Instance, //Search latest version
                0, //No unique deletion id. 
                RecursionType.Full, //Full recursion on the path
                null, //All users 
                new DateVersionSpec(DateTime.Now - TimeSpan.FromDays(7)), //From the 7 days ago ... 
                LatestVersionSpec.Instance, //To the current version ... 
                Int32.MaxValue, //Include all changes. Can limit the number you get back.
                false, //Don't include details of items, only metadata. 
                false //Slot mode is false. 
                );
            //Enumerate of the changesets. 
            foreach (Changeset changeset in history)
            {
                ChangeSetItem csi = new ChangeSetItem();
                csi.ChangesetId = changeset.ChangesetId;
                csi.Comment = changeset.Comment;
                csi.Committer = changeset.Committer;
                csi.CommitterDisplayName = changeset.CommitterDisplayName;
                csi.CreationDate = changeset.CreationDate;
                csi.Owner = changeset.Owner;
                csi.OwnerDisplayName = changeset.OwnerDisplayName;
                csItems.Add(csi);
            }
            return true;
        }
    }

}
