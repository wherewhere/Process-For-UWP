using ProcessForUWP.Core;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Foundation;
using WinRTWrapper.CodeAnalysis;

namespace ProcessForUWP.Desktop
{
    [ComVisible(true)]
    [WinRTWrapperMarshaller(typeof(TextReader), typeof(ITextReader))]
    [GenerateWinRTWrapper(typeof(TextReader), GenerateMember.Defined)]
    public partial class RemoteTextReader : ITextReader
    {
        public partial void Close();
        public partial void Dispose();
        public partial int Peek();
        public partial int Read();
        public partial int Read(char[] buffer, int index, int count);
        public partial IAsyncOperation<int> ReadAsync(char[] buffer, int index, int count);
        public partial int ReadBlock(char[] buffer, int index, int count);
        public partial IAsyncOperation<int> ReadBlockAsync(char[] buffer, int index, int count);
        public partial string? ReadLine();
        public partial IAsyncOperation<string?> ReadLineAsync();
        public partial string? ReadToEnd();
        public partial IAsyncOperation<string?> ReadToEndAsync();
    }
}
