﻿using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Nintendo.N64.NativeApi;

namespace BizHawk.Emulation.Cores.Nintendo.N64
{
	class N64Input
	{
		private mupen64plusInputApi api;
		public CoreComm CoreComm { get; private set; }
		public IController Controller { get; set; }

		public bool LastFrameInputPolled { get; set; }
		public bool ThisFrameInputPolled { get; set; }
		public ControllerDefinition ControllerDefinition { get { return N64ControllerDefinition; } }

		public static readonly ControllerDefinition N64ControllerDefinition = new ControllerDefinition
		{
			Name = "Nintento 64 Controller",
			BoolButtons =
			{
				"P1 A Up", "P1 A Down", "P1 A Left", "P1 A Right", "P1 DPad U", "P1 DPad D", "P1 DPad L", "P1 DPad R", "P1 Start", "P1 Z", "P1 B", "P1 A", "P1 C Up", "P1 C Down", "P1 C Right", "P1 C Left", "P1 L", "P1 R", 
				"P2 A Up", "P2 A Down", "P2 A Left", "P2 A Right", "P2 DPad U", "P2 DPad D", "P2 DPad L", "P2 DPad R", "P2 Start", "P2 Z", "P2 B", "P2 A", "P2 C Up", "P2 C Down", "P2 C Right", "P2 C Left", "P2 L", "P2 R", 
				"P3 A Up", "P3 A Down", "P3 A Left", "P3 A Right", "P3 DPad U", "P3 DPad D", "P3 DPad L", "P3 DPad R", "P3 Start", "P3 Z", "P3 B", "P3 A", "P3 C Up", "P3 C Down", "P3 C Right", "P3 C Left", "P3 L", "P3 R", 
				"P4 A Up", "P4 A Down", "P4 A Left", "P4 A Right", "P4 DPad U", "P4 DPad D", "P4 DPad L", "P4 DPad R", "P4 Start", "P4 Z", "P4 B", "P4 A", "P4 C Up", "P4 C Down", "P4 C Right", "P4 C Left", "P4 L", "P4 R", 
				"Reset", "Power"
			},
			FloatControls =
			{
				"P1 X Axis", "P1 Y Axis",
				"P2 X Axis", "P2 Y Axis",
				"P3 X Axis", "P3 Y Axis",
				"P4 X Axis", "P4 Y Axis"
			},
			FloatRanges =
			{
				new[] {-128.0f, 0.0f, 127.0f},
				new[] {-128.0f, 0.0f, 127.0f},
				new[] {-128.0f, 0.0f, 127.0f},
				new[] {-128.0f, 0.0f, 127.0f},
				new[] {-128.0f, 0.0f, 127.0f},
				new[] {-128.0f, 0.0f, 127.0f},
				new[] {-128.0f, 0.0f, 127.0f},
				new[] {-128.0f, 0.0f, 127.0f}
			}
		};

		public N64Input(mupen64plusApi core, CoreComm comm)
		{
			api = new mupen64plusInputApi(core);
			CoreComm = comm;

			api.SetM64PInputCallback(new mupen64plusInputApi.InputCallback(GetControllerInput));

			core.VInterrupt += ShiftInputPolledBools;
		}

		public void ShiftInputPolledBools()
		{
			LastFrameInputPolled = ThisFrameInputPolled;
			ThisFrameInputPolled = false;
		}

		/// <summary>
		/// Translates controller input from EmuHawk into
		/// N64 controller data
		/// </summary>
		/// <param name="i">Id of controller to update and shove</param>
		public int GetControllerInput(int i)
		{
			CoreComm.InputCallback.Call();
			ThisFrameInputPolled = true;

			// Analog stick right = +X
			// Analog stick up = +Y
			string p = "P" + (i + 1);
			sbyte x;
			if (Controller.IsPressed(p + " A Left")) { x = -127; }
			else if (Controller.IsPressed(p + " A Right")) { x = 127; }
			else { x = (sbyte)Controller.GetFloat(p + " X Axis"); }

			sbyte y;
			if (Controller.IsPressed(p + " A Up")) { y = 127; }
			else if (Controller.IsPressed(p + " A Down")) { y = -127; }
			else { y = (sbyte)Controller.GetFloat(p + " Y Axis"); }

			int value = ReadController(i + 1);
			value |= (x & 0xFF) << 16;
			value |= (y & 0xFF) << 24;
			return value;
		}

		/// <summary>
		/// Read all buttons from a controller and translate them
		/// into a form the N64 understands
		/// </summary>
		/// <param name="num">Number of controller to translate</param>
		/// <returns>Bitlist of pressed buttons</returns>
		public int ReadController(int num)
		{
			int buttons = 0;

			if (Controller["P" + num + " DPad R"]) buttons |= (1 << 0);
			if (Controller["P" + num + " DPad L"]) buttons |= (1 << 1);
			if (Controller["P" + num + " DPad D"]) buttons |= (1 << 2);
			if (Controller["P" + num + " DPad U"]) buttons |= (1 << 3);
			if (Controller["P" + num + " Start"]) buttons |= (1 << 4);
			if (Controller["P" + num + " Z"]) buttons |= (1 << 5);
			if (Controller["P" + num + " B"]) buttons |= (1 << 6);
			if (Controller["P" + num + " A"]) buttons |= (1 << 7);
			if (Controller["P" + num + " C Right"]) buttons |= (1 << 8);
			if (Controller["P" + num + " C Left"]) buttons |= (1 << 9);
			if (Controller["P" + num + " C Down"]) buttons |= (1 << 10);
			if (Controller["P" + num + " C Up"]) buttons |= (1 << 11);
			if (Controller["P" + num + " R"]) buttons |= (1 << 12);
			if (Controller["P" + num + " L"]) buttons |= (1 << 13);

			return buttons;
		}
	}
}
