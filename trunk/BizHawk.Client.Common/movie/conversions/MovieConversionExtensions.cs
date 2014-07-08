﻿using System;
using BizHawk.Common.ReflectionExtensions;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common.MovieConversionExtensions
{
	public static class MovieConversionExtensions
	{
		public static TasMovie ToTasMovie(this IMovie old)
		{
			var newFilename = old.Filename + "." +  TasMovie.Extension;
			var tas = new TasMovie(newFilename);
			tas.HeaderEntries.Clear();
			foreach (var kvp in old.HeaderEntries)
			{
				tas.HeaderEntries[kvp.Key] = kvp.Value;
			}

			tas.SyncSettingsJson = old.SyncSettingsJson;

			tas.Comments.Clear();
			foreach (var comment in old.Comments)
			{
				tas.Comments.Add(comment);
			}

			tas.Subtitles.Clear();
			foreach (var sub in old.Subtitles)
			{
				tas.Subtitles.Add(sub);
			}

			tas.TextSavestate = old.TextSavestate;
			tas.BinarySavestate = old.BinarySavestate;

			for (var i = 0; i < old.InputLogLength; i++)
			{
				var input = old.GetInputState(i);
				tas.AppendFrame(input);
			}

			return tas;
		}

		public static Bk2Movie ToBk2(this IMovie old)
		{
			var newFilename = old.Filename + "." + Bk2Movie.Extension;
			var bk2 = new Bk2Movie(newFilename);
			bk2.HeaderEntries.Clear();
			foreach(var kvp in old.HeaderEntries)
			{
				bk2.HeaderEntries[kvp.Key] = kvp.Value;
			}

			bk2.SyncSettingsJson = old.SyncSettingsJson;

			bk2.Comments.Clear();
			foreach(var comment in old.Comments)
			{
				bk2.Comments.Add(comment);
			}

			bk2.Subtitles.Clear();
			foreach(var sub in old.Subtitles)
			{
				bk2.Subtitles.Add(sub);
			}

			bk2.TextSavestate = old.TextSavestate;
			bk2.BinarySavestate = old.BinarySavestate;

			for (var i = 0; i < old.InputLogLength; i++)
			{
				var input = old.GetInputState(i);
				bk2.AppendFrame(input);
			}

			return bk2;
		}

		// TODO: This doesn't really belong here, but not sure where to put it
		public static void PopulateWithDefaultHeaderValues(this IMovie movie, string author = null)
		{
			movie.Author = author ?? Global.Config.DefaultAuthor;
			movie.EmulatorVersion = VersionInfo.GetEmuVersion();
			movie.Platform = Global.Game.System;

			movie.SyncSettingsJson = ConfigService.SaveWithType(Global.Emulator.GetSyncSettings());

			if (Global.Game != null)
			{
				movie.GameName = PathManager.FilesystemSafeName(Global.Game);
				movie.Hash = Global.Game.Hash;
				if (Global.Game.FirmwareHash != null)
				{
					movie.FirmwareHash = Global.Game.FirmwareHash;
				}
			}
			else
			{
				movie.GameName = "NULL";
			}

			if (Global.Emulator.BoardName != null)
			{
				movie.BoardName = Global.Emulator.BoardName;
			}

			if (Global.Emulator.HasPublicProperty("DisplayType"))
			{
				var region = Global.Emulator.GetPropertyValue("DisplayType");
				if ((DisplayType)region == DisplayType.PAL)
				{
					movie.HeaderEntries.Add(HeaderKeys.PAL, "1");
				}
			}

			movie.Core = ((CoreAttributes)Attribute
				.GetCustomAttribute(Global.Emulator.GetType(), typeof(CoreAttributes)))
				.CoreName;
		}
	}
}
