using ProcessForUWP.Desktop.Helpers;

Communication.InitializeAppServiceConnection();
EventWaitHandle WaitHandle = new AutoResetEvent(false);
_ = WaitHandle.WaitOne();
