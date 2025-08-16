using ProcessForUWP.Core;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Storage.Streams;
using WinRTWrapper.CodeAnalysis;

namespace ProcessForUWP.Desktop
{
    [ComVisible(true)]
    [WinRTWrapperMarshaller(typeof(StreamWriter), typeof(IStreamWriter))]
    [GenerateWinRTWrapper(typeof(StreamWriter), GenerateMember.Defined)]
    public partial class RemoteStreamWriter : RemoteTextWriter, IStreamWriter
    {
        public partial bool AutoFlush { get; set; }

        /// <inheritdoc cref="StreamWriter.BaseStream"/>
        public IInputStream BaseStream => target.BaseStream.AsInputStream();
    }
}
