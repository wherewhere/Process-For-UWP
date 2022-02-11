// See https://aka.ms/new-console-template for more information
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;

internal class Program
{
    private AppServiceConnection connection;

    static void Main(string[] args)
    {
        Program program = new();
        program.InitializeAppServiceConnection();
        while (true)
        {
            Thread.Sleep(100);
        }
    }

    private async void InitializeAppServiceConnection()
    {
        try
        {
            connection = new AppServiceConnection
            {
                AppServiceName = "ConnectToAdbApkInstallerUWP.Delegate",
                PackageFamilyName = Package.Current.Id.FamilyName
            };
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;

            AppServiceConnectionStatus status = await connection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                // something went wrong ...
                Console.WriteLine(status.ToString());
            }
        }
        catch (Exception ex)
        {
            SendMessage(new Response(-1, OperationStatuesEnum.Error, ex.Message));
        }
    }
}
