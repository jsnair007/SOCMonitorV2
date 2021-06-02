using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SOCMonitorV2
{
    public class clsEvent
    {
        public static  bool LogEvent(string sHead, string sLog) //For general Log
        {
            bool bResponse = false;
            try
            {
                //FileStream fs = new FileStream(@"D:\Dotnet Projects\SOC Shift Monitoring\Web\SOCMonitorV2\Logs\Message_" + DateTime.Now.ToString("ddMMMyyyy") + ".log", //LIVE Directory
                FileStream fs = new FileStream(@"D:\SOCMonitorV2\Logs\Message_" + DateTime.Now.ToString("ddMMMyyyy") + ".log", //LIVE Directory
                FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter FilestreamWriter = new StreamWriter(fs);
                FilestreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                FilestreamWriter.WriteLine();
                FilestreamWriter.WriteLine("#################### " + sHead + "at " + DateTime.Now.ToString() + " Starts ##################");
                FilestreamWriter.WriteLine();
                FilestreamWriter.WriteLine(sLog);
                FilestreamWriter.WriteLine();
                FilestreamWriter.WriteLine("____________________ " + sHead + " Ends ____________________");
                FilestreamWriter.WriteLine();
                FilestreamWriter.Flush();
                FilestreamWriter.Close();
                bResponse = true;
            }
            catch (Exception ee)
            {
                bResponse = false;
            }
            return bResponse;
        }
    }
}
