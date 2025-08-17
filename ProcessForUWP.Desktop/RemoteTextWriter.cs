using ProcessForUWP.Core;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Foundation;
using WinRTWrapper.CodeAnalysis;

namespace ProcessForUWP.Desktop
{
    [ComVisible(true)]
    [WinRTWrapperMarshaller(typeof(TextWriter), typeof(ITextWriter))]
    [GenerateWinRTWrapper(typeof(TextWriter), GenerateMember.Defined)]
    public partial class RemoteTextWriter : ITextWriter
    {
        public partial string NewLine { get; set; }
        public partial void Flush();
        public partial IAsyncAction FlushAsync();
        public partial void Write(ulong value);
        public partial void Write(uint value);
        public partial void Write(string format, params object[] args);
        public partial void Write(string format, object arg0, object arg1, object arg2);
        public partial void Write(string format, object arg0, object arg1);
        public partial void Write(string format, object arg0);
        public partial void Write(string? value);
        public partial void Write(float value);
        public partial void Write(long value);
        public partial void Write(int value);
        public partial void Write(double value);
        public partial void Write(char[] buffer, int index, int count);
        public partial void Write(char[]? buffer);
        public partial void Write(char value);
        public partial void Write(bool value);
        public partial void Write(object? value);
        public partial IAsyncAction WriteAsync(string? value);
        public partial IAsyncAction WriteAsync(char[]? buffer);
        public partial IAsyncAction WriteAsync(char value);
        public partial IAsyncAction WriteAsync(char[] buffer, int index, int count);
        public partial void WriteLine();
        public partial void WriteLine(ulong value);
        public partial void WriteLine(uint value);
        public partial void WriteLine(string format, params object[] args);
        public partial void WriteLine(string format, object arg0, object arg1, object arg2);
        public partial void WriteLine(string format, object arg0, object arg1);
        public partial void WriteLine(string format, object arg0);
        public partial void WriteLine(string? value);
        public partial void WriteLine(float value);
        public partial void WriteLine(long value);
        public partial void WriteLine(int value);
        public partial void WriteLine(double value);
        public partial void WriteLine(char[] buffer, int index, int count);
        public partial void WriteLine(char[]? buffer);
        public partial void WriteLine(char value);
        public partial void WriteLine(bool value);
        public partial void WriteLine(object? value);
        public partial IAsyncAction WriteLineAsync();
        public partial IAsyncAction WriteLineAsync(string? value);
        public partial IAsyncAction WriteLineAsync(char[]? buffer);
        public partial IAsyncAction WriteLineAsync(char value);
        public partial IAsyncAction WriteLineAsync(char[] buffer, int index, int count);
        public partial void Dispose();
#if COMP_NETSTANDARD2_1
        public partial IAsyncAction DisposeAsync();
#else
        /// <inheritdoc cref="TextWriterEx.DisposeAsync(TextWriter)"/>
        public IAsyncAction DisposeAsync() => System.WindowsRuntimeSystemExtensions.AsAsyncAction(target.DisposeAsync());
#endif
    }
}
