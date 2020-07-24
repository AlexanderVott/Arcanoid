using System;
using System.Collections.Generic;
using System.Diagnostics;
using RedDev.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedDev.Kernel.Logs
{
	public static class LogGroup
	{
		public const string Debug   = "Debug";
		public const string REST    = "REST";
		public const string System  = "System";
		public const string States  = "FMS";
		public const string Unity   = "Unity";
		public const string Network = "Network";
	}

	/// <summary>
	/// Система логирования отладочной и прочей информации.
	/// </summary>
	public class LogSystem: Singleton<LogSystem>
	{
		private List<ILogListener> _listeners = new List<ILogListener>();

		/// <summary>
		/// Доступ к слушателям по индексу.
		/// </summary>
		/// <param name="index">Индекс слушателя в системе.</param>
		/// <returns>Возвращает слушателя, если индекс является действительным. В противном случае null.</returns>
		public ILogListener this[int index] => (index >= 0) && (index < _listeners.Count) ? _listeners[index] : null;

		private bool _skipSelfCallback = false;

		/// <summary>
		/// Количество слушателей в системе.
		/// </summary>
		public int Count => _listeners.Count;

		private Dictionary<string, LogGroupSettings> _groups = new Dictionary<string, LogGroupSettings>();

		void Awake()
		{
			new FileLogListener(true);

			SetupGroup(LogGroup.System, AlertLevel.Notify, CheckLevel.Any);
			SetupGroup(LogGroup.Unity, AlertLevel.Notify, CheckLevel.Any);
			SetupGroup(LogGroup.Unity, AlertLevel.Notify, CheckLevel.Any);
			SetupGroup(LogGroup.Network, AlertLevel.Notify, CheckLevel.Any);

			SetupGroup(LogGroup.Debug, 
				Application.isEditor ? AlertLevel.Warning : AlertLevel.Error, 
				CheckLevel.Fail);

			//TODO: Чтение из конфигурационного файла

			Application.logMessageReceived -= LogCallback;
			Application.logMessageReceived += LogCallback;
		}

		void OnApplicationQuit()
		{
			Info("Logging stop: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
			Clear();
			Application.logMessageReceived -= LogCallback;
		}

		#region Регистрация слушателей
		/// <summary>
		/// Регистрирует слушателей системы логирования и выполняет их инициализацию.
		/// </summary>
		/// <returns>Возвращает индекс в списке слушателей. Если слушатель пустой, то возвращает -1.</returns>
		public int RegisterListener(BaseLogListener listener)
		{
			if (listener == null)
				return -1;
			listener.Initialize();
			_listeners.Add(listener);
			return _listeners.Count - 1;
		}

		/// <summary>
		/// Удаление слушателя из системы по индексу.
		/// </summary>
		/// <param name="index">Индекс слушателя в списке.</param>
		public void UnregisterListener(int index)
		{
			if ((index < 0) || (index > _listeners.Count))
				return;

			_listeners[index].OnClose();
			_listeners.RemoveAt(index);
		}

		/// <summary>
		/// Удаление слушателя из системы по экземпляру слушателя.
		/// </summary>
		/// <param name="listener">Слушатель, которого следует убрать из системы.</param>
		public void UnregisterListener(BaseLogListener listener)
		{
			listener.OnClose();
			_listeners.Remove(listener);
		}

		public static void Clear()
		{
			if (instance == null)
				return;
			foreach (var listener in instance._listeners)
				listener.OnClose();
			instance._listeners.Clear();
		}
		#endregion

		#region Callback, отправка слушателям и движку
		/// <summary>
		/// Обработчик сообщений Unity3D.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="stacktrace"></param>
		/// <param name="type"></param>
		private static void LogCallback(string condition, string stacktrace, LogType type)
		{
			if (instance._skipSelfCallback)
			{
				instance._skipSelfCallback = false;
				return;
			}
#if UNITY_EDITOR
			// убираем двойной вывод сообщений в редакторе
			instance._skipSelfCallback = true;
#endif
			instance.Output(type.ToAlertlevel(), LogGroup.Unity, condition, stacktrace);
			instance._skipSelfCallback = false;
		}

		/// <summary>
		/// Отправляет сообщение всем слушателям.
		/// </summary>
		/// <param name="message"></param>
		private void SendToListeners(LogMessage message)
		{
			foreach (var listener in _listeners)
				listener.OnListen(message);
		}

		/// <summary>
		/// Отправляет сообщение через движок.
		/// </summary>
		/// <param name="alertLevel"></param>
		/// <param name="message"></param>
		/// <param name="obj"></param>
		[Conditional("UNITY_EDITOR")]
		public void SendToUnity(AlertLevel alertLevel, string message, Object obj = null)
		{
			if (_skipSelfCallback)
				return;

			_skipSelfCallback = true;
			switch (alertLevel)
			{
				case AlertLevel.Warning:
					UnityEngine.Debug.Log(message, obj);
					break;
				case AlertLevel.Error:
					UnityEngine.Debug.LogError(message, obj);
					break;
				case AlertLevel.FatalError:
					UnityEngine.Debug.LogError(message, obj);
					break;
				default:
					UnityEngine.Debug.Log(message, obj);
					break;
			}
		}
		#endregion

		#region Непосредственно работа с собщениями

		/// <summary>
		/// Отправка сообщения в движок и слушателям.
		/// </summary>
		/// <param name="alertLevel"></param>
		/// <param name="groupName"></param>
		/// <param name="message"></param>
		/// <param name="stacktrace"></param>
		/// <param name="obj"></param>
		private void Output(AlertLevel alertLevel, string groupName, string message, string stacktrace = null, Object obj = null)
		{
			var group = GetGroupOrDefault(groupName);

			if (group.alertLevel == AlertLevel.Silence)
				return;

			if (group.alertLevel <= alertLevel)
			{
				var msgContent = groupName + "> " + message;
				SendToUnity(alertLevel, msgContent, obj);
				LogMessage msg = new LogMessage();
				msg.level = alertLevel;
				msg.groupName = groupName;
				msg.content = message;
				msg.stackTrace = stacktrace;
				SendToListeners(msg);
			}
		}

		public static void Print(AlertLevel alertLevel, string groupName, string message, string stacktrace = null, Object obj = null)
		{
			instance.Log(alertLevel, groupName, message, stacktrace, obj);
		}

		public void Log(AlertLevel alertLevel, string groupName, string message, string stacktrace = null, Object obj = null)
		{
			Output(alertLevel, groupName, message, stacktrace, obj);
		}

		[Conditional("DEBUG")]
		public static void Info(string message, Object obj = null)
		{
			UnityEngine.Debug.Log(message, obj);
		}

		public static void Warning(string message, Object obj = null)
		{
			UnityEngine.Debug.LogWarning(message, obj);
		}

		public static void Error(string message, Object obj = null)
		{
			UnityEngine.Debug.LogError(message, obj);
		}

		public static void Exception(Exception except)
		{
			UnityEngine.Debug.LogException(except);
		}

		/// <summary>
		/// Отправляет сообщение с разделительной линией всем слушателям, которые принимают данный вид сообщений.
		/// </summary>
		public static void PrintDelimiter()
		{
			var msg = new LogMessage {content = "==============", level = AlertLevel.Notify};
			foreach (var listener in instance._listeners)
				if (listener.isListenDelimiter)
					listener.OnListen(msg);
		}
		#endregion

		#region Работа с группами логирования
		private LogGroupSettings RegisterGroup(string nameGroup, bool silentRegistration = false)
		{
			var group = new LogGroupSettings
			{
				alertLevel = AlertLevel.Error,
				checkLevel = CheckLevel.Fail
			};
			_groups.Add(nameGroup.ToLower(), group);
			if (!silentRegistration)
				Output(AlertLevel.Warning, LogGroup.System, "New group '" + nameGroup + "' registered.");
			return group;
		}

		public LogGroupSettings GetGroupOrDefault(string groupName)
		{
			groupName = groupName.ToLower();
			if (!_groups.TryGetValue(groupName, out var result))
				result = RegisterGroup(groupName, true);
			return result;
		}

		public void SetupGroup(string groupName, AlertLevel alertlevel, CheckLevel checkLevel)
		{
			groupName = groupName.ToLower();
			if (!_groups.TryGetValue(groupName, out var group))
				group = RegisterGroup(groupName, true);

			group.alertLevel = alertlevel;
			group.checkLevel = checkLevel;
		}

		/// <summary>
		/// Переключение уровня записи у перечисленных групп.
		/// </summary>
		/// <param name="args">Массив с названиями групп, которых необходимо начать выводить.</param>
		public void DebugToggle(string[] args)
		{
			if ((args == null) || (args.Length == 0) || (args[0].ToLower().Equals("all")))
			{
				Log(AlertLevel.Notify, LogGroup.System, "Trace for all groups");
				foreach (var g in _groups.Values)
				{
					g.alertLevel = AlertLevel.Notify;
					g.checkLevel = CheckLevel.Any;
				}

				return;
			}

			foreach (var g in _groups.Values)
			{
				if (g.alertLevel < AlertLevel.Error)
					g.alertLevel = AlertLevel.Error;
				if (g.checkLevel < CheckLevel.Fail)
					g.checkLevel = CheckLevel.Fail;
			}

			SetupGroup(LogGroup.System, AlertLevel.Notify, CheckLevel.Any);
			SetupGroup(LogGroup.Unity, AlertLevel.Notify, CheckLevel.Any);
			
			foreach (var groupName in args)
			{
				SetupGroup(groupName, AlertLevel.Notify, CheckLevel.Any);
				Info("Trace for " + groupName);
			}
		}
		#endregion
	}
}