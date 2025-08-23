#include "pch.h"
#include "ProcessProjectionFactory.h"
#include "ProcessProjectionFactory.g.cpp"

namespace winrt::ProcessForUWP::Demo::Projection::implementation
{
    IServerManager ProcessProjectionFactory::ServerManager()
    {
        try
        {
            if (m_serverManager != nullptr && m_serverManager.IsServerRunning())
            {
                return m_serverManager;
            }
        }
        catch (...) {}

        switch (m_defaultType)
        {
        case RPCType::COM:
            if (m_clsid == nullptr)
            {
                throw hresult_invalid_argument::hresult_invalid_argument(L"The CLSID of IServerManager is not set.");
            }
            else
            {
                m_serverManager = create_instance<::IServerManager>(m_clsid.Value(), CLSCTX_ALL);
                m_serverManager.SetMonitor(IsAlive, m_timeout);
                return m_serverManager;
            }
            break;
        case RPCType::WinRT:
            if (m_activatableClassID.empty())
            {
                throw hresult_invalid_argument::hresult_invalid_argument(L"The ActivatableClassID of IServerManager is not set.");
            }
            else
            {
                IActivationFactory factory = winrt::get_activation_factory(m_activatableClassID);
                m_serverManager = factory.ActivateInstance<::IServerManager>();
                m_serverManager.SetMonitor(IsAlive, m_timeout);
                return m_serverManager;
            }
            break;
        default:
            throw hresult_not_implemented::hresult_not_implemented(L"The specified RPC type is not implemented.");
        }
    }
}
