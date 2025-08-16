using ProcessForUWP.Core;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Storage.Streams;
using WinRTWrapper.CodeAnalysis;

namespace ProcessForUWP.Desktop
{
    [ComVisible(true)]
    [WinRTWrapperMarshaller(typeof(StreamReader), typeof(IStreamReader))]
    [GenerateWinRTWrapper(typeof(StreamReader), GenerateMember.Defined)]
    public partial class RemoteStreamReader : RemoteTextReader, IStreamReader
    {
        /// <inheritdoc cref="StreamReader.BaseStream"/>
        public IOutputStream BaseStream => target.BaseStream.AsOutputStream();
        public partial bool EndOfStream { get; }

        public partial void DiscardBufferedData();
    }
}
