using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHelpers
{
    public class FileChangeMonitor
    {
        #region Fields

        private CTimer changeTimer;
        private string currentHash;
        private string currentFilePath;

        private int fileCheckTime;

        private bool isRunning;

        #endregion Fields

        #region Properties

        public int FileCheckTime
        {
            get
            {
                return fileCheckTime;
            }
            set
            {
                if (changeTimer != null)
                {
                    changeTimer.Stop();
                    changeTimer.Dispose();
                }
                fileCheckTime = value;
                Start();
            }
        }

        public bool IsRunning { get { return isRunning; } }

        #endregion Properties

        #region Delegates

        public delegate void FileMonitorCallback();

        #endregion Delegates

        #region Events

        public event FileMonitorCallback onFileChanged;

        #endregion Events

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">Location of the file</param>
        /// <param name="seconds">Frequency to check file contents in seconds</param>
        public FileChangeMonitor(string filePath, int seconds)
        {
            try
            {
                currentFilePath = filePath;
                isRunning = false;

                if (VerifyFile(currentFilePath))
                {
                    CrestronConsole.PrintLine("FileChange Constructor is File Path exist");
                    ErrorLog.Error("FileChange Constructor is File Path exist");
                    currentHash = HashBuilder(filePath);
                    fileCheckTime = (seconds * 1000);
                }
                else
                {
                    CrestronConsole.PrintLine("FileChange Constructor is File Path does not exist");
                    ErrorLog.Error("FileChange Constructor is File Path does not exist");
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error in FileChange Monitor Constructor is: " + e);
                ErrorLog.Error("Error in FileChange Monitor Constructor is: " + e);
                throw;
            }
        }

        #endregion Constructor

        #region Internal Methods

        private void CheckFileCallbackTimer(object obj)
        {
            try
            {
                var newHash = HashBuilder(currentFilePath);

                if (!String.Equals(currentHash, newHash, StringComparison.OrdinalIgnoreCase))
                {
                    currentHash = newHash;
                    onFileChanged();
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("FileChange CheckFileCallback error is: " + e);
                ErrorLog.Error("FileChange CheckFileCallback error is: " + e);
            }
        }

        private bool VerifyFile(string filePath)
        {
            return File.Exists(filePath);
        }

        private string HashBuilder(string filePath)
        {
            try
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(File.ReadToEnd(filePath, Encoding.ASCII));
                    var computedHash = md5.ComputeHash(bytes);
                    var builder = new StringBuilder();
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        builder.Append(computedHash[i].ToString("X2"));
                    }
                    return builder.ToString();
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error in FileChangeMonitor HashBuilder is: " + e);
                ErrorLog.Error("Error in FileChangeMonitor HashBuilder is: " + e);
                return "";
            }

        }

        #endregion

        #region Public Methods

        public void Start()
        {
            if (changeTimer != null)
            {
                changeTimer.Stop();
                changeTimer.Dispose();
            }
            changeTimer = new CTimer(CheckFileCallbackTimer, null, 0, fileCheckTime);
            isRunning = true;
        }

        public void Stop()
        {
            changeTimer.Stop();
            changeTimer.Dispose();
            isRunning = false;
        }

        #endregion Public Methods
    }
}
