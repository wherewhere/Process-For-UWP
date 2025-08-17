namespace ProcessForUWP.Core
{
    /// <inheritdoc cref="ITextWriter"/>
    public static class ITextWriterExtensions
    {
        /// <inheritdoc cref="ITextWriter.Write(string, object[])"/>
        public static void Write(this ITextWriter writer, string format, params object[] args) => writer.Write(format, args);

        /// <inheritdoc cref="ITextWriter.WriteLine(string, object[])"/>
        public static void WriteLine(this ITextWriter writer, string format, params object[] args) => writer.WriteLine(format, args);
    }
}
