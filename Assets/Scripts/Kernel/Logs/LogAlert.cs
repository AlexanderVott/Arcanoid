using UnityEngine;

namespace RedDev.Kernel.Logs
{
	public enum AlertLevel : byte
	{
		Notify = 0,
		Warning,
		Error,
		FatalError,
		Silence
	}

	public static class AlertLevelExtensions
	{

		public static string GetName(this AlertLevel level)
		{
			switch (level)
			{
				case AlertLevel.Notify:
					return "[notify]    : ";
				case AlertLevel.Warning:
					return "[warning]   : ";
				case AlertLevel.Error:
					return "[error]     : ";
				case AlertLevel.FatalError:
					return "[fatalerror]: ";
				default:
					return "[silence]   : ";
			}
		}

		public static AlertLevel ToAlertlevel(this LogType logtype)
		{
			switch (logtype)
			{
				case LogType.Log:
					return AlertLevel.Notify;
				case LogType.Warning:
					return AlertLevel.Warning;
				case LogType.Assert:
					return AlertLevel.Error;
				case LogType.Error:
					return AlertLevel.Error;
				case LogType.Exception:
					return AlertLevel.FatalError;
				default:
					return AlertLevel.Silence;
			}
		}
	}
}
