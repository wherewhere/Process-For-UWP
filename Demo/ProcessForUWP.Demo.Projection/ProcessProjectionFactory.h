#pragma once

#include "ProcessProjectionFactory.g.h"
#include "winrt/ProcessForUWP.Core.h"

using namespace winrt;
using namespace winrt::ProcessForUWP::Core;
using namespace winrt::Windows::Foundation;

namespace winrt::ProcessForUWP::Demo::Projection::implementation
{
    static RPCType m_defaultType = RPCType::COM;
    static IReference<guid> m_clsid = nullptr;
    static hstring m_activatableClassID = {};
    static IServerManager m_serverManager = nullptr;
    static TimeSpan m_timeout = std::chrono::seconds(30);

    struct ProcessProjectionFactory : ProcessProjectionFactoryT<ProcessProjectionFactory>
    {
        static RPCType DefaultType() { return m_defaultType; }
        static void DefaultType(RPCType const& value) { m_defaultType = value; }
        static IReference<guid> CLSID() { return m_clsid; }
        static void CLSID(IReference<guid> const& value) { m_clsid = value; }
        static hstring ActivatableClassID() { return m_activatableClassID; }
        static void ActivatableClassID(hstring const& value) { m_activatableClassID = value; }
        static TimeSpan Timeout() { return m_timeout; }
        static void Timeout(TimeSpan const& value) { m_timeout = value; }
        static IServerManager ServerManager();
        static bool IsAlive() { return true; }
    };
}

namespace winrt::ProcessForUWP::Demo::Projection::factory_implementation
{
    struct ProcessProjectionFactory : ProcessProjectionFactoryT<ProcessProjectionFactory, implementation::ProcessProjectionFactory>
    {
    };
}
