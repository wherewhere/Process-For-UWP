namespace ProcessForUWP.Core.Models
{
    /// <summary>
    /// The type of message
    /// </summary>
    public enum MessageType
    {
        Method,
        Message,
        PropertyGet,
        PropertySet,

        NewProcess,
        ProcessStart,
        ProcessExited,
        ProcessErrorData,
        ProcessOutputData,

        CopyFile,
    }
}
