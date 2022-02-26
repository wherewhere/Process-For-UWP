namespace ProcessForUWP.Core.Models
{
    /// <summary>
    /// MessageType
    /// </summary>
    public enum MessageType
    {
        Kill,
        Start,
        Close,
        Exited,
        Refresh,
        Dispose,
        Message,
        CopyFile,
        ErrorData,
        NewProcess,
        OutputData,
        PropertyGet,
        PropertySet,
        BeginErrorReadLine,
        BeginOutputReadLine,
    }
}
