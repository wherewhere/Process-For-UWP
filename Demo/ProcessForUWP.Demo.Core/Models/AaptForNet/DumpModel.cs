using System.Collections.Generic;

namespace AAPTForNet.Models
{
    public class DumpModel
    {
        public string FilePath { get; }
        public bool IsSuccess { get; }
        public List<string> Messages { get; }

        public DumpModel(string path, bool success, List<string> msg)
        {
            this.FilePath = path;
            this.IsSuccess = success;
            this.Messages = msg;
        }
    }
}
