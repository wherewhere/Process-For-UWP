#if DEBUG
global using ProcessProjectionFactory = ProcessForUWP.UWP.ProcessProjectionFactory;
global using RPCType = ProcessForUWP.UWP.RPCType;
#else
global using ProcessProjectionFactory = ProcessForUWP.Demo.Projection.ProcessProjectionFactory;
global using RPCType = ProcessForUWP.Demo.Projection.RPCType;
#endif