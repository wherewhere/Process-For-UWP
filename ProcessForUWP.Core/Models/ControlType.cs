namespace ProcessForUWP.Core.Models
{
    public enum ControlType
    {
        Kill,
        Start,
        Close,
        Exited,
        Refresh,
        Dispose,
        Message,
        NewProcess,
        PropertyGet,
        PropertySet,
        BeginErrorReadLine,
        BeginOutputReadLine,
    }
}
