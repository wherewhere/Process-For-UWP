using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessForUWP.Desktop
{
	public enum ControlType
	{
		Start,
		BeginErrorReadLine,
		BeginOutputReadLine,
		Exited,
		PropertyGet,
		PropertySet,
		Close,
		Dispose,
		Kill,
	}
}
