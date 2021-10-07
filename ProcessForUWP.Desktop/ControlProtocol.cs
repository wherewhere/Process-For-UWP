using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessForUWP.Desktop
{
	public enum ControlType
	{
		Kill,
		Start,
		Close,
		Exited,
		Dispose,
		PropertyGet,
		PropertySet,
		BeginErrorReadLine,
		BeginOutputReadLine,
	}
}
