namespace ProcessForUWP.Core.Helpers
{
    public static class Utils
    {
        public static object GetProperty(this object item, string name) => item.GetType().GetProperty(name).GetValue(item, null);

        public static void SetProperty(this object item, string name, object value) => item.GetType().GetProperty(name).SetValue(item, value);

        public static void InvokeMethod(this object type, string name) => type?.GetType()?.GetMethod(name)?.Invoke(type, null);
    }
}
