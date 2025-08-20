using ProcessForUWP.Desktop;
using System;
switch (args)
{
    case ["-RegisterProcessAsComServer", ..]:
        Factory.StartComServer(new Guid("89575E89-722D-4D0A-9BE7-0AB808C9C4BB"));
        break;
    case ["-RegisterProcessAsWinRTServer", ..]:
        Factory.StartWinRTServer("ProcessForUWP.Delegate");
        break;
}