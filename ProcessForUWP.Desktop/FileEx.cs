using Newtonsoft.Json;
using ProcessForUWP.Core.Models;
using ProcessForUWP.Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;

namespace ProcessForUWP.Desktop
{
    /// <summary>
    /// Provides static methods for the creation, copying, deletion, moving, and opening of a single file, and aids in the creation of System.IO.FileStream objects.
    /// </summary>
    internal static class FileEx
    {
        internal static void Initialize() => Communication.RequestReceived += Connection_RequestReceived;

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                if (args.Request.Message.ContainsKey(nameof(FileEx)))
                {
                    Message message = JsonConvert.DeserializeObject<Message>(args.Request.Message[nameof(FileEx)] as string);
                    switch (message.MessageType)
                    {
                        case MessageType.CopyFile:
                            CopyFile(message);
                            break;
                        case MessageType.FileExists:
                            Exists(message);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is allowed.
        /// </summary>
        private static void CopyFile(Message message)
        {
            try
            {
                (string sourceFileName, string destFileName, bool overwrite) = message.GetPackage<(string, string, bool)>();
                File.Copy(sourceFileName, destFileName, overwrite);
                Communication.SendMessages(nameof(FileEx), MessageType.CopyFile, message.ID, StatuesType.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        private static void Exists(Message message)
        {
            try
            {
                string path = message.GetPackage<string>();
                bool result = File.Exists(path);
                Communication.SendMessages(nameof(FileEx), MessageType.FileExists, message.ID, result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
