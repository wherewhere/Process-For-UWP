using System;
using System.Text;

namespace ProcessForUWP.Demo.Helpers
{
    internal static partial class UIHelper
    {
        public static string ExceptionToMessage(this Exception ex)
        {
            StringBuilder builder = new StringBuilder().AppendLine();
            if (!string.IsNullOrWhiteSpace(ex.Message)) { builder.AppendLine($"Message: {ex.Message}"); }
            builder.AppendLine($"HResult: {ex.HResult} (0x{ex.HResult:X})");
            if (!string.IsNullOrWhiteSpace(ex.StackTrace)) { builder.AppendLine(ex.StackTrace); }
            if (!string.IsNullOrWhiteSpace(ex.HelpLink)) { builder.Append($"HelperLink: {ex.HelpLink}"); }
            return builder.ToString();
        }
    }
}
