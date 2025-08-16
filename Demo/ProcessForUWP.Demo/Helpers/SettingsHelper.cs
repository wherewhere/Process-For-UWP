using MetroLog;

namespace ProcessForUWP.Demo.Helpers
{
    internal static partial class SettingsHelper
    {
        public static readonly ILogManager LogManager = LogManagerFactory.CreateLogManager();
    }
}
