namespace ProcessForUWP.Core.Models
{
    public enum MessageType
    {
        Kill,
        Start,
        Close,
        Exited,
        Refresh,
        Dispose,
        Message,
        ErrorData,
        NewProcess,
        OutputData,
        PropertyGet,
        PropertySet,
        BeginErrorReadLine,
        BeginOutputReadLine,
    }
}
