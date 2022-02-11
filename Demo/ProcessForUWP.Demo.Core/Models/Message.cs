using System.Collections.Generic;

namespace AdbApkInstallerUWP.Core.Models
{
    public class Message
    {
        private static int currentId;
        public int Id { get; set; }
        public OperationTypeEnum OperationType { get; set; }
        public IEnumerable<string> Arguments { get; set; }

        public Message(OperationTypeEnum typeEnum)
        {
            Id = ++currentId;
            OperationType = typeEnum;
            Arguments = new List<string>();
        }

        public static Message MakeMessage(OperationTypeEnum typeEnum)
        {
            return new(typeEnum);
        }

        public Message AddArgument(string value)
        {
            (Arguments as List<string>).Add(value);
            return this;
        }
    }
}
