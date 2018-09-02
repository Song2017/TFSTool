using Microsoft.Office.Interop.Outlook;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace TFSUtils
{
    public class OutlookApplication : IDisposable
    {
        public bool IsNew { get; private set; }
        private Application _application;

        public Application GetApplicationObject()
        {
            if (Process.GetProcessesByName("OUTLOOK").Count() > 0)
            {
                _application = Marshal.GetActiveObject("Outlook.Application") as Application;
                IsNew = false;
            }
            else
            {
                _application = new Application();
                IsNew = true;
            }
            return _application;
        }

      
        public void Dispose()
        {
            _application.Quit();
        }

        public bool SendEmail(OlItemType olMailItem, string to, string cc, string sub, string documentText, OlImportance olImportanceNormal)
        {
            if (_application == null)
                GetApplicationObject();

            try
            {
                MailItem mailItem = _application.CreateItem(olMailItem);
                mailItem.To = to;
                mailItem.CC = cc;
                mailItem.Subject = sub;
                mailItem.HTMLBody = documentText;
                mailItem.Importance = olImportanceNormal;
                mailItem.Send();
            }
            catch (System.Exception e)
            {
                throw e;
            }

            return true;
        }
    }
}
