using Crestron.SimplSharp;
//using Crestron.SimplSharp.CrestronIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHelpers
{
    public static class ReadWriteFile
    {
        #region Fields

        #endregion

        #region Properties


        public static bool Debug { get; set; }
        #endregion

        #region Delegates

        #endregion

        #region Events

        #endregion

        #region Constructors

        #endregion

        #region Internal Methods

        internal static void SendDebug(string data)
        {
            if (Debug)
            {
                CrestronConsole.PrintLine("\nError ReadnWrite File is: " + data);
                ErrorLog.Error("\nError ReadnWrite File is: " + data);
            }
        }

        #endregion

        #region Public Methods

        public static bool WriteToFile(string fileData,string fileLocation, bool overwriteFile)
        {
            try
            {
                if (File.Exists(fileLocation) && !overwriteFile)
                {
                    using (FileStream fs = new FileStream(fileLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(fileData);
                        }
                    }
                    return true;
                }
                else if(overwriteFile && File.Exists(fileLocation) || !File.Exists(fileLocation))
                {
                    using (FileStream fs = new FileStream(fileLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        StreamReader sr = new StreamReader(fs);
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            fs.SetLength(0);
                            sw.Write(fileData);
                        }
                    }
                    return true;
                }
                else
                {
                    SendDebug("WriteToFile: File did not exist, and was not set to create");
                    return false;
                }

            }
            catch (Exception e)
            {
                SendDebug("WriteToFile error is: " + e.Message);
                return false;
            }
        }

        public static bool ReadFromFile(out List<string> fileData , string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                fileData = lines.ToList();
                return true;
            }
            catch (Exception e)
            {
                SendDebug("Error Read From File is: " + e.Message);
                fileData = new List<string>();
                return false;
            }
        }

        #endregion
    }
}
