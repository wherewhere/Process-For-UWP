using ProcessForUWP.Core;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using WinRTWrapper.CodeAnalysis;

namespace ProcessForUWP.Desktop
{
    [ComVisible(true)]
    [WinRTWrapperMarshaller(typeof(Component), typeof(Core.IComponent))]
    [GenerateWinRTWrapper(typeof(Component), GenerateMember.Defined)]
    public partial class RemoteComponent : Core.IComponent
    {
#if NET
        /// <summary>
        /// The event weak table for the <see cref="Component.Disposed"/> event.
        /// </summary>
        private readonly System.Runtime.CompilerServices.ConditionalWeakTable<EventHandler<IEventArgs>, EventHandler> _Disposed_EventWeakTable = [];

        /// <inheritdoc cref="Component.Disposed"/>
        public event EventHandler<IEventArgs> Disposed
        {
            add
            {
                void wrapper(object? sender, EventArgs e) => value(this, RemoteEventArgs.ConvertToWrapper(e));
                EventHandler handler = wrapper;
                target.Disposed += handler;
                _Disposed_EventWeakTable.Add(value, handler);
            }
            remove
            {
                if (_Disposed_EventWeakTable.TryGetValue(value, out EventHandler? handler))
                {
                    target.Disposed -= handler;
                    _Disposed_EventWeakTable.Remove(value);
                }
            }
        }
#else
        /// <summary>
        /// The singleton flag for the <see cref="Component.Disposed"/> event registration.
        /// </summary>
        private bool _is_Disposed_EventRegistered = false;

        /// <summary>
        /// The event registration token table for the <see cref="Component.Disposed"/> event.
        /// </summary>
        private readonly System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable<EventHandler> _Disposed_EventTable = new();

        /// <inheritdoc cref="Component.Disposed"/>
        public event EventHandler<IEventArgs> Disposed
        {
            add
            {
                if (!_is_Disposed_EventRegistered)
                {
                    target.Disposed += (sender, e) => _Disposed_EventTable.InvocationList?.Invoke(sender, e);
                    _is_Disposed_EventRegistered = true;
                }
                void wrapper(object? sender, EventArgs e) => value(this, RemoteEventArgs.ConvertToWrapper(e));
                return _Disposed_EventTable.AddEventHandler(wrapper);
            }
            remove
            {
                _Disposed_EventTable.RemoveEventHandler(value);
            }
        }
#endif

        public partial void Dispose();
        public override partial string ToString();
    }
}
