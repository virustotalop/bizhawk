﻿using BizHawk.Client.Common;
using BizHawk.Bizware.BizwareGL;
#if WINDOWS
using SlimDX.DirectSound;
#endif

namespace BizHawk.Client.EmuHawk
{
	public static class GlobalWin
	{
		public static MainForm MainForm;
		public static ToolManager Tools;
#if WINDOWS
		public static DirectSound DSound;
#endif
		public static IGL GL;
		public static Sound Sound;
		public static PresentationPanel PresentationPanel;
		public static OSDManager OSD = new OSDManager();
		public static DisplayManager DisplayManager;

		//input state which has been destined for game controller inputs are coalesced here
		//public static ControllerInputCoalescer ControllerInputCoalescer = new ControllerInputCoalescer();
		//input state which has been destined for client hotkey consumption are colesced here
		public static InputCoalescer HotkeyCoalescer = new InputCoalescer();
	}
}
