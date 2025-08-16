using System;
using System.Runtime.InteropServices;
using System.Threading;
using ProcessForUWP.Core;

namespace ProcessForUWP.Desktop
{
    /// <summary>
    /// Factory class for creating instances of the <see cref="IServerManager"/> COM interface.
    /// </summary>
    /// <param name="clsid">The CLSID of the <see cref="IServerManager"/> COM interface.</param>
    [ComVisible(true)]
#if NET8_0_OR_GREATER
    [System.Runtime.InteropServices.Marshalling.GeneratedComClass]
#endif
    public partial class ServerManagerFactory(Guid clsid) : IClassFactory
    {
        /// <summary>
        /// The EXE code that creates and manages objects of this class runs on same machine but is loaded in a separate process space.
        /// </summary>
        private const uint CLSCTX_LOCAL_SERVER = 0x4;

        /// <summary>
        /// Multiple applications can connect to the class object through calls to CoGetClassObject. If both the REGCLS_MULTIPLEUSE and CLSCTX_LOCAL_SERVER are set in a call to CoRegisterClassObject, the class object is also automatically registered as an in-process server, whether CLSCTX_INPROC_SERVER is explicitly set.
        /// </summary>
        private const int REGCLS_MULTIPLEUSE = 1;

        private const int E_NOINTERFACE = unchecked((int)0x80004002);
        private const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);

        private static readonly Guid _iid = typeof(IServerManager).GUID;

        /// <summary>
        /// The CLSID of the IUnknown interface.
        /// </summary>
        private static readonly Guid CLSID_IUnknown = new("00000000-0000-0000-C000-000000000046");

        private uint cookie;

        /// <inheritdoc/>
#if NET
        public void CreateInstance(nint pUnkOuter, in Guid riid, out nint ppvObject)
#else
        public void CreateInstance(nint pUnkOuter, in Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject)
#endif
        {
            ppvObject = 0;

            if (pUnkOuter != 0)
            {
                Marshal.ThrowExceptionForHR(CLASS_E_NOAGGREGATION);
            }

            if (riid == _iid || riid == CLSID_IUnknown)
            {
                // Create the instance of the .NET object
#if NET
                ppvObject = WinRT.MarshalInspectable<IServerManager>.FromManaged(new ServerManager());
#else
                ppvObject = new ServerManager();
#endif
            }
            else
            {
                // The object that ppvObject points to does not support the
                // interface identified by riid.
                Marshal.ThrowExceptionForHR(E_NOINTERFACE);
            }
        }

        /// <inheritdoc/>
        public void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock)
        {
        }

        /// <summary>
        /// Registers the class object with OLE so other applications can connect to it.
        /// </summary>
        public void RegisterClassObject()
        {
            int hresult = CoRegisterClassObject(
                clsid,
                this,
                CLSCTX_LOCAL_SERVER,
                REGCLS_MULTIPLEUSE,
                out cookie);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
        }

        /// <summary>
        /// Revokes the class object previously registered with OLE.
        /// </summary>
        public void RevokeClassObject()
        {
            int hresult = CoRevokeClassObject(cookie);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
        }

        /// <summary>
        /// Registers an EXE class object with OLE so other applications can connect to it.
        /// </summary>
        /// <param name="rclsid">The CLSID to be registered.</param>
        /// <param name="pUnk">A pointer to the IUnknown interface on the class object whose availability is being published.</param>
        /// <param name="dwClsContext">The context in which the executable code is to be run. For information on these context values, see the CLSCTX enumeration.</param>
        /// <param name="flags">Indicates how connections are made to the class object. For information on these flags, see the REGCLS enumeration.</param>
        /// <param name="lpdwRegister">A pointer to a value that identifies the class object registered; later used by the <see cref="CoRevokeClassObject"/> function to revoke the registration.</param>
        /// <returns>This function can return the standard return values E_INVALIDARG, E_OUTOFMEMORY, and E_UNEXPECTED, as well as the following values.</returns>
#if NET7_0_OR_GREATER
        [LibraryImport("api-ms-win-core-com-l1-1-0.dll")]
        private static partial int CoRegisterClassObject(in Guid rclsid, IClassFactory pUnk, uint dwClsContext, int flags, out uint lpdwRegister);
#else
        [DllImport("api-ms-win-core-com-l1-1-0.dll", ExactSpelling = true)]
        private static extern int CoRegisterClassObject([In] in Guid rclsid, [In] ServerManagerFactory pUnk, [In] uint dwClsContext, [In] int flags, [Out] out uint lpdwRegister);
#endif

        /// <summary>
        /// Informs OLE that a class object, previously registered with the <see cref="CoRegisterClassObject"/> function, is no longer available for use.
        /// </summary>
        /// <param name="dwRegister">A token previously returned from the <see cref="CoRegisterClassObject"/> function.</param>
        /// <returns>This function can return the standard return values E_INVALIDARG, E_OUTOFMEMORY, and E_UNEXPECTED, as well as the following values.</returns>
#if NET7_0_OR_GREATER
        [LibraryImport("api-ms-win-core-com-l1-1-0.dll")]
        private static partial int CoRevokeClassObject(uint dwRegister);
#else
        [DllImport("api-ms-win-core-com-l1-1-0.dll", ExactSpelling = true)]
        private static extern int CoRevokeClassObject([In] uint dwRegister);
#endif
    }

    /// <summary>
    /// Provides methods and constants for COM class object registration.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// The event that signals when the COM server should exit.
        /// </summary>
        private static ManualResetEventSlim? comServerExitEvent;

        /// <summary>
        /// Starts the COM server with the specified CLSID and waits for it to exit.
        /// </summary>
        /// <param name="clsid">The CLSID to be registered.</param>
        public static void StartComServer(in Guid clsid)
        {
            comServerExitEvent = new ManualResetEventSlim(false);
            comServerExitEvent.Reset();
            ServerManager.ServerManagerDestructed += comServerExitEvent.Set;
            ServerManagerFactory factory = new(clsid);
            factory.RegisterClassObject();
            _ = ServerManager.CheckComRefAsync();
            comServerExitEvent.Wait();
            factory.RevokeClassObject();
            comServerExitEvent = null;
        }
    }

    /// <summary>
    /// Represents a monitor that checks if a remote object is alive.
    /// </summary>
    public sealed partial class RemoteMonitor : IDisposable
    {
        private bool disposed;
        private readonly Timer _timer;
        private readonly Action _dispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMonitor"/> class.
        /// </summary>
        /// <param name="handler">The handler to check if the remote object is alive.</param>
        /// <param name="dispose">The action to dispose the remote object.</param>
        /// <param name="period">The period to check if the remote object is alive.</param>
        public RemoteMonitor(IsAliveHandler handler, Action dispose, in TimeSpan period)
        {
            _dispose = dispose;
            _timer = new(_ =>
            {
                bool isAlive = false;
                try
                {
                    isAlive = handler.Invoke();
                }
                catch
                {
                    isAlive = false;
                }
                finally
                {
                    if (!isAlive)
                    {
                        Dispose();
                    }
                }
            }, null, TimeSpan.Zero, period);
        }

        /// <summary>
        /// Finalizes the instance of the <see cref="RemoteMonitor"/> class.
        /// </summary>
        ~RemoteMonitor() => Dispose();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                _timer.Dispose();
                _dispose?.Invoke();
                GC.SuppressFinalize(this);
            }
        }
    }

    /// <summary>
    /// The <see cref="IClassFactory"/> interface inherits from the IUnknown interface.
    /// </summary>
    /// <remarks><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nn-unknwn-iclassfactory"/></remarks>
#if NET8_0_OR_GREATER
    [System.Runtime.InteropServices.Marshalling.GeneratedComInterface]
#else
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
#endif
    [Guid("00000001-0000-0000-C000-000000000046")]
    internal partial interface IClassFactory
    {
        /// <summary>
        /// Creates an uninitialized object.
        /// </summary>
        /// <param name="pUnkOuter">If the object is being created as part of an aggregate, specify a pointer to the
        /// controlling IUnknown interface of the aggregate. Otherwise, this parameter must be <see langword="null"/>.</param>
        /// <param name="riid">A reference to the identifier of the interface to be used to communicate with the newly created object.
        /// If <paramref name="pUnkOuter"/> is <see langword="null"/>, this parameter is generally the IID of the initializing interface;
        /// if <paramref name="pUnkOuter"/> is non-<see langword="null"/>, <paramref name="riid"/> must be IID_IUnknown.</param>
        /// <param name="ppvObject">The address of pointer variable that receives the interface pointer requested in <paramref name="riid"/>.
        /// Upon successful return, *ppvObject contains the requested interface pointer. If the object does not support the interface specified
        /// in <paramref name="riid"/>, the implementation must set *<paramref name="ppvObject"/> to <see langword="null"/>.</param>
#if NET
        void CreateInstance(nint pUnkOuter, in Guid riid, out nint ppvObject);
#else
        void CreateInstance([In] nint pUnkOuter, [In] in Guid riid, [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);
#endif

        /// <summary>
        /// Locks an object application open in memory. This enables instances to be created more quickly.
        /// </summary>
        /// <param name="fLock">If <see langword="true"/>, increments the lock count; if <see langword="false"/>, decrements the lock count.</param>
        void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
    }
}
