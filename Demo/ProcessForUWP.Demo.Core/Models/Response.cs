namespace AdbApkInstallerUWP.Core.Models
{

    public class Response
    {
        public Response(int messageId, OperationStatuesEnum statues, object message)
        {
            MessageId = messageId;
            Statues = statues;
            Message = message;
        }

        public int MessageId { get; set; }
        public OperationStatuesEnum Statues { get; set; }
        public object Message { get; set; }
    }
}
