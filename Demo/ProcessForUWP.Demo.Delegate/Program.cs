using ProcessForUWP.Desktop;
using System;
switch (args)
{
    case ["-RegisterProcessAsComServer", ..]:
        await Factory.StartComServerAsync(new Guid("89575E89-722D-4D0A-9BE7-0AB808C9C4BB")).ConfigureAwait(false);
        break;
    case ["-RegisterProcessAsWinRTServer", ..]:
        await Factory.StartWinRTServerAsync("ProcessForUWP.Delegate").ConfigureAwait(false);
        break;
}