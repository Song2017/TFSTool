using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TFSUtils;

namespace Helper
{
    public static class LocalFileFunction
    {
        private static readonly string outputFolder = "result";

        public static bool SaveWorkItemList(this List<VKWorkItem> VKWorkItems)
        {
            string filename = Path.Combine(Directory.GetCurrentDirectory(), outputFolder, DateTime.UtcNow.ToString("yyyyMMdd_hhmmss"));
            string filePathCSV = string.Format("{0}.csv", filename);
            string filePathJason = string.Format("{0}.json", filename);
            if (!CreateFileToLoacal(filePathCSV))
            {
                return false;
            }
            if (!CreateFileToLoacal(filePathJason))
            {
                return false;
            }
            StreamWriter sw = null;
            StreamWriter sw2 = null;
            try
            {
                sw = new StreamWriter(filePathCSV);
                foreach (VKWorkItem epic in VKWorkItems)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("\"" + epic.ID + "\"");
                    sb.Append(",");
                    sb.Append("\"" + epic.Title.ToStringEx().Replace(@"\",@"\\") + "\"");
                    sb.Append(",");
                    sb.Append("\"" + epic.WorkItemType + "\"");
                    sb.Append(",");
                    sb.Append("\"" + epic.IterationPath + "\"");
                    sb.Append(",");
                    sb.Append("\"" + epic.State + "\"");
                    sb.Append(",");
                    sb.Append("\"" + epic.ChangedDate.ToStringEx() + "\""); 
                    sb.Append(",");
                    sb.Append("\"" + epic.AssignedTo + "\"");
                    sb.Append(",");
                    sb.Append("\"" + epic.StartDate.ToLongDateString() + "\"");
                    sw.WriteLine(sb.ToString());
                }
                string jsonData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(VKWorkItems);
                sw2 = new StreamWriter(filePathJason);
                sw2.WriteLine(jsonData);
                sw2.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            finally
            {
                sw.Close();
                sw2.Close();
            }

        }

        private static bool CreateFileToLoacal(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                    File.Create(filename).Close();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
