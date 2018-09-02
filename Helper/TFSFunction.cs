using Microsoft.TeamFoundation.Client;
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

        public TFSFunction()
        {
        }

        public bool ConnectToTFS(string tfsUrl, string username, string password)
        {
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
                        Description = wi.Description
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


    }

}
