using ProcessForUWP.Core.Models;
using ProcessForUWP.UWP.Helpers;
using System;

namespace ProcessForUWP.UWP
{
    /// <summary>
    /// Provides static methods for the creation, copying, deletion, moving, and opening of a single file, and aids in the creation of System.IO.FileStream objects.
    /// </summary>
    public static class FileEx
    {
        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is allowed.
        /// </summary>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="destFileName">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false.</param>
        /// <exception cref="FieldAccessException">Cannot copy this file.</exception>
        public static void CopyFile(string sourceFileName, string destFileName, bool overwrite)
        {
            try
            {
                Communication.SendMessages(nameof(FileEx), MessageType.CopyFile, (sourceFileName, destFileName, overwrite));
                (bool iscopyed, Message message) = Communication.Receive(nameof(FileEx), 0, MessageType.CopyFile);
                if (!(iscopyed && message.GetPackage<StatuesType>() == StatuesType.Success))
                {
                    throw new Exception("Cannot copy this file.");
                }
            }
            catch (Exception)
            {
                throw new FieldAccessException("Cannot copy this file.");
            }
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>
        /// <see langword="true"/> if the caller has the required permissions and path contains the name of
        /// an existing file; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if path is
        /// <see langword="null"/>, an invalid path, or a zero-length string. If the caller does not have sufficient
        /// permissions to read the specified file, no exception is thrown and the method
        /// returns <see langword="false"/> regardless of the existence of path.
        /// </returns>
        public static bool Exists(string path)
        {
            try
            {
                Communication.SendMessages(nameof(FileEx), MessageType.FileExists, path);
                (bool ischecked, Message message) = Communication.Receive(nameof(FileEx), 0, MessageType.FileExists);
                return ischecked && message.GetPackage<bool>();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
