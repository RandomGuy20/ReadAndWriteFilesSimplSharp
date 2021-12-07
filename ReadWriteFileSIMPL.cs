using Crestron.SimplSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHelpers
{
    public class ReadWriteFileSIMPL
    {
        #region Fields

        public bool debug;
        private string fileLocation;

        FileChangeMonitor monitor;
        #endregion

        #region properties

        #endregion

        #region Delegates

        public delegate void SerialOutputChange(ushort val, SimplSharpString data);

        #endregion

        #region Events

        public SerialOutputChange onSerialChange { get; set; }

        #endregion

        #region Constructors

        public void Initialize(string FileLocation)
        {
            try
            {
                fileLocation = FileLocation;
                monitor = new FileChangeMonitor(FileLocation, 10);
                monitor.onFileChanged += Monitor_onFileChanged;
                monitor.Start();
                ReadFromFile(fileLocation);
            }
            catch (Exception e)
            {
                SendDebug("\n ReadWriteSIMPl error in constructor is: " + e.Message);
            }

        }


        #endregion

        #region Internal Methods

        internal void SetSerialOutputs(string[] lines)
        {
            for (ushort i = 0; i < lines.Length; i++)
            {
                onSerialChange(i, (SimplSharpString)lines[i]);
            }

        }

        internal void SendDebug(string data)
        {
            if (debug)
            {
                CrestronConsole.PrintLine("\nError ReadnWriteSIMPL File is: " + data);
                ErrorLog.Error("\nError ReadnWriteSIMPL File is: " + data);
            }
        }

        private void Monitor_onFileChanged()
        {
            ReadFromFile(fileLocation);
        }


        #endregion

        #region Public Methods

        public void WriteToFile(SimplSharpString fileData,SimplSharpString filePath)
        {

            try
            {
                string data = fileData.ToString();
                string path = filePath.ToString();
                if (ReadWriteFile.WriteToFile(data,path,true))
                    SendDebug("\nSIMPl Write to file - success!");
                else
                    SendDebug("\nSIMPL Write To File - Could not write");
            }
            catch (Exception e)
            {
                SendDebug("\nSIMPL Error Write To File: " + e.Message);
            }

        }

        public void ReadFromFile(SimplSharpString filePath)
        {
            try
            {

                string path = filePath.ToString();
                List<string> data = new List<string>();
                if (ReadWriteFile.ReadFromFile(out data,path))
                {
                    SetSerialOutputs(data.ToArray());
                    SendDebug("\nSIMPl Read From file - success!");
                }
                else
                {
                    SendDebug("\nSIMPL Read From File - Could not write");
                }
            }
            catch (Exception e)
            {
                SendDebug("\nSIMPL Error Read From File: " + e.Message);
            }
        }

        public void SetDebugSIMPL(ushort val)
        {
            debug = Convert.ToBoolean(val);
        }

        public void SetDebugReadWrite(ushort val)
        {
            ReadWriteFile.Debug = Convert.ToBoolean(val);
        }

        #endregion
    }
}
