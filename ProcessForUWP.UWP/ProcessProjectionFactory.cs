using ProcessForUWP.Core;
using System;
using System.Runtime.InteropServices;

namespace ProcessForUWP.UWP
{
    /// <summary>
    /// Factory class for creating instances of the <see cref="IServerManager"/> COM interface.
    /// </summary>
    public static partial class ProcessProjectionFactory
    {
        /// <summary>
        /// The CLSCTX enumeration specifies the context in which the code that manages a class object will run.
        /// </summary>
        private const uint CLSCTX_ALL = 1 | 2 | 4 | 16;

        /// <summary>
        /// The default RPC type used by the factory.
        /// </summary>
        public static RPCType DefaultType { get; set; } = RPCType.COM;

        /// <summary>
        /// The CLSID of the <see cref="IServerManager"/> COM interface.
        /// </summary>
        public static Guid? CLSID { get; set; }

        /// <summary>
        /// The Activatable Class ID of the <see cref="IServerManager"/> WinRT interface.
        /// </summary>
        public static string? ActivatableClassID { get; set; }

        /// <summary>
        /// The CLSID of the IUnknown interface.
        /// </summary>
        public static readonly Guid CLSID_IUnknown = new("00000000-0000-0000-C000-000000000046");

        /// <summary>
        /// Gets or sets the loop timeout for checking if the COM server host is alive.
        /// </summary>
        public static TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The singleton instance of the <see cref="IServerManager"/> COM interface.
        /// </summary>
        private static IServerManager? m_serverManager;

        /// <summary>
        /// Gets the singleton instance of the <see cref="IServerManager"/> COM interface.
        /// </summary>
        public static IServerManager ServerManager
        {
            get
            {
                try
                {
                    if (m_serverManager?.IsServerRunning == true)
                    {
                        return m_serverManager;
                    }
                }
                catch { }

                switch (DefaultType)
                {
                    case RPCType.COM:
                        if (CLSID == null)
                        {
                            throw new ArgumentNullException(nameof(CLSID), "The CLSID of IServerManager is not set.");
                        }
                        else
                        {
                            m_serverManager = CreateServerManager(CLSID.Value, Timeout);
                            return m_serverManager;
                        }
                    case RPCType.WinRT:
                        if (string.IsNullOrEmpty(ActivatableClassID))
                        {
                            throw new ArgumentNullException(nameof(ActivatableClassID), "The ActivatableClassID of IServerManager is not set.");
                        }
                        else
                        {
                            m_serverManager = CreateServerManager(ActivatableClassID!, Timeout);
                            return m_serverManager;
                        }
                    default:
                        throw new NotImplementedException($"The specified RPC type {DefaultType} is not implemented.");
                }
            }
        }

        /// <summary>
        /// Checks if the Host process is still alive.
        /// </summary>
        /// <returns>Always returns <see langword="true"/>.</returns>
        private static bool IsAlive() => true;

        /// <summary>
        /// Creates an instance of the <see cref="IServerManager"/> COM interface using the specified CLSID.
        /// </summary>
        /// <param name="rclsid">The CLSID associated with the <see cref="IServerManager"/> COM interface.</param>
        /// <param name="timeout">The timeout in seconds to wait for the COM server check host to be alive.</param>
        /// <returns>The created instance of <see cref="IServerManager"/>.</returns>
        public static IServerManager CreateServerManager(in Guid rclsid, in TimeSpan timeout)
        {
#if NET
            int hresult = CoCreateInstance(rclsid, 0, CLSCTX_ALL, CLSID_IUnknown, out nint ppv);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
            IServerManager result = WinRT.Marshaler<IServerManager>.FromAbi(ppv);
#else
            int hresult = CoCreateInstance(rclsid, 0, CLSCTX_ALL, CLSID_IUnknown, out IServerManager result);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
#endif
            result.SetMonitor(IsAlive, timeout);
            return result;
        }

        /// <summary>
        /// Creates an instance of the <see cref="IServerManager"/> WinRT interface using the specified Activatable Class ID.
        /// </summary>
        /// <param name="activatableClassId">The Activatable Class ID associated with the <see cref="IServerManager"/> WinRT interface.</param>
        /// <param name="timeout">The timeout in seconds to wait for the COM server check host to be alive.</param>
        /// <returns>The created instance of <see cref="IServerManager"/>.</returns>
        public static IServerManager CreateServerManager(string activatableClassId, in TimeSpan timeout)
        {
#if NET
            nint classId = WinRT.MarshalString.FromManaged(activatableClassId);
            int hresult = RoActivateInstance(classId, out nint instance);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
            IServerManager result = WinRT.Marshaler<IServerManager>.FromAbi(instance);
#else
            int hresult = WindowsCreateString(activatableClassId, (uint)activatableClassId.Length, out var classId);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
            hresult = RoActivateInstance(classId, out IServerManager result);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
#endif
            result.SetMonitor(IsAlive, timeout);
            return result;
        }

        /// <summary>
        /// Creates and default-initializes a single object of the class associated with a specified CLSID.
        /// </summary>
        /// <param name="rclsid">The CLSID associated with the data and code that will be used to create the object.</param>
        /// <param name="pUnkOuter">If NULL, indicates that the object is not being created as part of an aggregate.
        /// If non-<see langword="null"/>, pointer to the aggregate object's IUnknown interface (the controlling IUnknown).</param>
        /// <param name="dwClsContext">Context in which the code that manages the newly created object will run. The values are taken from the enumeration CLSCTX.</param>
        /// <param name="riid">A reference to the identifier of the interface to be used to communicate with the object.</param>
        /// <param name="ppv">Address of pointer variable that receives the interface pointer requested in <paramref name="riid"/>.
        /// Upon successful return, *<paramref name="ppv"/> contains the requested interface pointer. Upon failure, *<paramref name="ppv"/> contains <see langword="null"/>.</param>
        /// <returns>This function can return the following values.</returns>
#if NET7_0_OR_GREATER
        [LibraryImport("api-ms-win-core-com-l1-1-0.dll")]
        private static partial int CoCreateInstance(in Guid rclsid, nint pUnkOuter, uint dwClsContext, in Guid riid, out nint ppv);
#elif NET
        [DllImport("api-ms-win-core-com-l1-1-0.dll", ExactSpelling = true)]
        private static extern int CoCreateInstance([In] in Guid rclsid, [In, Optional] nint pUnkOuter, [In] uint dwClsContext, [In] in Guid riid, [Out] out nint ppv);
#else
        [DllImport("api-ms-win-core-com-l1-1-0.dll", ExactSpelling = true)]
        private static extern int CoCreateInstance([In] in Guid rclsid, [In, Optional] nint pUnkOuter, [In] uint dwClsContext, [In] in Guid riid, [Out] out IServerManager ppv);
#endif

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

        /// <summary>
        /// Activates the specified Windows Runtime class.
        /// </summary>
        /// <param name="activatableClassId">The class identifier that is associated with the activatable runtime class.</param>
        /// <param name="instance">A pointer to the activated instance of the runtime class.</param>
        /// <returns>This function can return one of these values.</returns>
#if NET7_0_OR_GREATER
        [LibraryImport("api-ms-win-core-winrt-l1-1-0.dll")]
        private unsafe static partial int RoActivateInstance(nint activatableClassId, out nint instance);
#elif NET
        [DllImport("api-ms-win-core-winrt-l1-1-0.dll", ExactSpelling = true)]
        private unsafe static extern int RoActivateInstance([In] nint activatableClassId, [Out] out nint instance);
#else
        [DllImport("api-ms-win-core-winrt-l1-1-0.dll", ExactSpelling = true)]
        private unsafe static extern int RoActivateInstance([In] nint activatableClassId, [Out] out IServerManager instance);
#endif
    }
}
