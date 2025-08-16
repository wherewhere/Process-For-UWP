using ProcessForUWP.Core;
using System;
using System.Runtime.InteropServices;
using WinRTWrapper.CodeAnalysis;

namespace ProcessForUWP.Desktop
{
    [ComVisible(true)]
    [WinRTWrapperMarshaller(typeof(EventArgs), typeof(IEventArgs))]
    [GenerateWinRTWrapper(typeof(EventArgs), GenerateMember.Defined)]
    public sealed partial class RemoteEventArgs : EventArgs, IEventArgs;
}
