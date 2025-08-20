using ProcessForUWP.Core;
using System;
using System.Runtime.InteropServices;
using System.Threading;

#if NET
using IActivationFactory = WinRT.Interop.IActivationFactory;
#else
using IActivationFactory = System.Runtime.InteropServices.WindowsRuntime.IActivationFactory;
#endif

#if !NET7_0_OR_GREATER
using unsafe DllGetActivationFactoryPointer = delegate* unmanaged[Stdcall]<nint, out nint, nint>;
#endif

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
    public partial class ServerManagerClassFactory(Guid clsid) : IClassFactory
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
        private static extern int CoRegisterClassObject([In] in Guid rclsid, [In] ServerManagerClassFactory pUnk, [In] uint dwClsContext, [In] int flags, [Out] out uint lpdwRegister);
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
    /// Factory class for creating instances of the <see cref="IServerManager"/> WinRT interface.
    /// </summary>
    /// <param name="classId">The activatable class ID of the <see cref="IServerManager"/> WinRT interface.</param>
    [ComVisible(true)]
    public partial class ServerManagerActivationFactory(string classId) : IActivationFactory
    {
        private const nint S_OK = 0;
        private const nint E_INVALIDARG = unchecked((int)0x80070057);

        private nint cookie;

        /// <inheritdoc/>
#if NET
        public nint ActivateInstance() => WinRT.MarshalInspectable<IServerManager>.FromManaged(new ServerManager());
#else
        public object ActivateInstance() => new ServerManager();
#endif

        /// <inheritdoc cref="DllGetActivationFactory"/>
#if NET
        private nint GetActivationFactory(nint activatableClassId, out nint factory)
#else
        private unsafe nint GetActivationFactory(nint activatableClassId, out IActivationFactory? factory)
#endif
        {
            try
            {
#if NET
                string _classId = WinRT.MarshalString.FromAbi(activatableClassId);
                if (_classId == classId)
                {
                    factory = WinRT.MarshalInterface<IActivationFactory>.FromManaged(this);
                    return S_OK;
                }
#else
                char* ptr = WindowsGetStringRawBuffer(activatableClassId, out uint length);
                string _classId = new(ptr, 0, (int)length);
                if (_classId == classId)
                {
                    factory = this;
                    return S_OK;
                }
#endif
                factory = default;
                return E_INVALIDARG;
            }
            catch (Exception ex)
            {
                factory = default;
#if NET
                return WinRT.ExceptionHelpers.GetHRForException(ex);
#else
                return Marshal.GetHRForException(ex);
#endif
            }
        }

        /// <summary>
        /// Registers the activation factory with the Windows Runtime so other applications can connect to it.
        /// </summary>
        public void RegisterActivationFactory()
        {
#if NET
            nint activatableClassId = WinRT.MarshalString.FromManaged(classId);
            int hresult;
#else
            int hresult = WindowsCreateString(classId, (uint)classId.Length, out nint activatableClassId);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
#endif
            hresult = RoRegisterActivationFactories([activatableClassId], [GetActivationFactory], 1, out cookie);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
        }

        /// <summary>
        /// Revokes the activation factory previously registered with the Windows Runtime.
        /// </summary>
        public void RevokeActivationFactory() => RoRevokeActivationFactories(cookie);

#if !NET7_0_OR_GREATER
        /// <summary>
        /// Creates a new HSTRING based on the specified source string.
        /// </summary>
        /// <param name="sourceString">A null-terminated string to use as the source for the new HSTRING. To create a new, empty, or <see langword="null"/> string, pass <see langword="null"/> for sourceString and 0 for <paramref name="length"/>.</param>
        /// <param name="length">The length of <paramref name="sourceString"/>, in Unicode characters. Must be 0 if <paramref name="sourceString"/> is <see langword="null"/>.</param>
        /// <param name="string">A pointer to the newly created HSTRING, or NULL if an error occurs. Any existing content in string is overwritten. The HSTRING is a standard handle type.</param>
        /// <returns>This function can return one of these values.</returns>
        [DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", ExactSpelling = true)]
        private static extern int WindowsCreateString([In, Optional][MarshalAs(UnmanagedType.LPWStr)] string sourceString, [In] uint length, [Out] out nint @string);
#endif

#if !NET7_0_OR_GREATER
        [DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe char* WindowsGetStringRawBuffer([In, Optional] nint hstring, [Out, Optional] out uint length);
#endif

        /// <summary>
        /// Retrieves the activation factory from a DLL that contains activatable Windows Runtime classes.
        /// </summary>
        /// <param name="activatableClassId">The class identifier that is associated with an activatable runtime class.</param>
        /// <param name="factory">A pointer to the activation factory that corresponds with the class specified by <paramref name="activatableClassId"/>.</param>
        /// <returns>This entry point can return one of these values.</returns>
#if NET
        private delegate nint DllGetActivationFactory([In] nint activatableClassId, [Out] out nint factory);
#else
        private delegate nint DllGetActivationFactory([In] nint activatableClassId, [Out] out IActivationFactory? factory);
#endif

        /// <summary>
        /// Registers an array out-of-process activation factories for a Windows Runtime exe server.
        /// </summary>
        /// <param name="activatableClassIds">An array of class identifiers that are associated with activatable runtime classes.</param>
        /// <param name="activationFactoryCallbacks">An array of callback functions that you can use to retrieve the activation factories that correspond with <paramref name="activatableClassIds"/>.</param>
        /// <param name="count">The number of items in the <paramref name="activatableClassIds"/> and <paramref name="activationFactoryCallbacks"/> arrays.</param>
        /// <param name="cookie">A cookie that identifies the registered factories.</param>
        /// <returns>This function can return one of these values.</returns>
#if NET7_0_OR_GREATER
        [LibraryImport("api-ms-win-core-winrt-l1-1-0.dll")]
        private unsafe static partial int RoRegisterActivationFactories([In] nint[] activatableClassIds, [In] DllGetActivationFactory[] activationFactoryCallbacks, uint count, out nint cookie);
#else
        private static unsafe int RoRegisterActivationFactories(nint[] activatableClassIds, DllGetActivationFactory[] activationFactoryCallbacks, uint count, out nint cookie)
        {
            fixed (nint* __cookie_native = &cookie)
            {
                fixed (nint* __activatableClassIds_native = activatableClassIds)
                {
                    DllGetActivationFactoryPointer* __activationFactoryCallbacks_native = (DllGetActivationFactoryPointer*)Marshal.AllocHGlobal(sizeof(DllGetActivationFactoryPointer*) * activationFactoryCallbacks.Length);
                    for (int i = 0; i < activationFactoryCallbacks.Length; i++)
                    {
                        __activationFactoryCallbacks_native[i] = (DllGetActivationFactoryPointer)Marshal.GetFunctionPointerForDelegate(activationFactoryCallbacks[i]);
                    }
                    return RoRegisterActivationFactories(__activatableClassIds_native, __activationFactoryCallbacks_native, (uint)activationFactoryCallbacks.Length, __cookie_native);
                }
            }
            // Local P/Invoke
            [DllImport("api-ms-win-core-winrt-l1-1-0.dll", ExactSpelling = true)]
            static extern unsafe int RoRegisterActivationFactories([In] nint* __activatableClassIds_native, [In] DllGetActivationFactoryPointer* __activationFactoryCallbacks_native, [In] uint __count_native, [Out] nint* __cookie_native);
        }
#endif

        /// <summary>
        /// Removes an array of registered activation factories from the Windows Runtime.
        /// </summary>
#if NET7_0_OR_GREATER
        [LibraryImport("api-ms-win-core-winrt-l1-1-0.dll")]
        private static partial void RoRevokeActivationFactories(nint cookie);
#else
        [DllImport("api-ms-win-core-winrt-l1-1-0.dll", ExactSpelling = true)]
        private static extern void RoRevokeActivationFactories([In] nint cookie);
#endif
    }

    /// <summary>
    /// Provides methods and constants for COM class object registration.
    /// </summary>
    public static partial class Factory
    {
        /// <summary>
        /// Initializes the thread for multi-threaded concurrency. The current thread is initialized in the MTA.
        /// </summary>
        private const int RO_INIT_MULTITHREADED = 1;

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
            ServerManagerClassFactory factory = new(clsid);
            factory.RegisterClassObject();
            _ = ServerManager.CheckReferenceAsync();
            comServerExitEvent.Wait();
            factory.RevokeClassObject();
            comServerExitEvent = null;
        }

        /// <summary>
        /// Starts the WinRT server with the specified activatable class ID and waits for it to exit.
        /// </summary>
        /// <param name="activatableClassId">The activatable class ID to be registered.</param>
        public static unsafe void StartWinRTServer(string activatableClassId)
        {
            _ = RoInitialize(RO_INIT_MULTITHREADED);
            comServerExitEvent = new ManualResetEventSlim(false);
            comServerExitEvent.Reset();
            ServerManager.ServerManagerDestructed += comServerExitEvent.Set;
            ServerManagerActivationFactory factory = new(activatableClassId);
            factory.RegisterActivationFactory();
            _ = ServerManager.CheckReferenceAsync();
            comServerExitEvent.Wait();
            factory.RevokeActivationFactory();
            comServerExitEvent = null;
        }

        /// <summary>
        /// Initializes the Windows Runtime on the current thread with the specified concurrency model.
        /// </summary>
        /// <param name="initType">The concurrency model for the thread. The default is <see cref="RO_INIT_MULTITHREADED"/>.</param>
        /// <returns>This function can return the standard return values E_INVALIDARG, E_OUTOFMEMORY, and E_UNEXPECTED, as well as the following values.</returns>
#if NET7_0_OR_GREATER
        [LibraryImport("api-ms-win-core-winrt-l1-1-0.dll")]
        private static partial int RoInitialize(int initType);
#else
        [DllImport("api-ms-win-core-winrt-l1-1-0.dll", ExactSpelling = true)]
        private static extern int RoInitialize([In] int initType);
#endif
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
