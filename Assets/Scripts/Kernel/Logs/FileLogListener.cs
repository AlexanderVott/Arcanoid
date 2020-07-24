using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using Object = System.Object;

namespace RedDev.Kernel.Logs
{
	public class FileLogListener : BaseLogListener
	{
		private const int THREAD_SLEEP = 200;

		private bool _initialized = false;
		private string _fileName;

		/// <summary>
		/// Количество хранящихся логов.
		/// </summary>
		private int keepLastLogs = 3;

		/// <summary>
		/// Объект блокировки безопасности работы с потоком.
		/// </summary> 
		private Object _locker = new Object();

		/// <summary>
		/// Буфер сообщений системы, предназначенных для сброса в хранилище.
		/// </summary>
		private LogMessagesBuffer _messagesBuffer = new LogMessagesBuffer();

		private Thread _worker = null;
		private volatile bool _stopWorker = false;

		private FileStream _logSteam = null;

		private bool _flushIteration = false;

		public FileLogListener(bool flushIteration) : base(true)
		{
			_flushIteration = flushIteration;
		}

		~FileLogListener()
		{
			OnClose();
		}

		public override void Initialize()
		{
			if (_initialized)
			{
				LogSystem.Print(AlertLevel.Error, LogGroup.System, "FileLogListener double initializing...");
				return;
			}
#if UNITY_EDITOR
			InitializeDirectory(Application.dataPath + "/../logs");
#else
			InitializeDirectory(Application.persistentDataPath + "/logs");
#endif
			StartLogging();
		}

		public override void OnListen(LogMessage message)
		{
			_messagesBuffer.container.Add(message);
		}

		public override void OnClose()
		{
			_stopWorker = true;
		}

		private void InitializeDirectory(string path)
		{
			try
			{
				DirectoryInfo info = new DirectoryInfo(path);
				if (!info.Exists)
					info.Create();
				CleanupLogs(info);

				var fileName = Application.productName + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".log";
				_fileName = info.FullName + "/" + fileName;
				_logSteam = File.OpenWrite(_fileName);
			}
			catch (Exception e)
			{
				Debug.LogError("Could not initialize logger: " + e);
				_initialized = false;
			}
			finally
			{
				_initialized = true;
			}
		}

		private void CleanupLogs(DirectoryInfo dir)
		{
			var files = dir.GetFiles();
			if (files.Length < keepLastLogs)
				return;

			var list = new List<FileInfo>(files);
			list.Sort((f1, f2) => String.Compare(f1.Name, f2.Name, StringComparison.Ordinal));
			
			var countForRemove = list.Count - keepLastLogs;
			for (int i = 0; i < countForRemove; i++)
			{
				try
				{
					list[i].Delete();
				}
				catch (Exception e)
				{
					Debug.LogError("Could not delete log file " + list[i].FullName + ": " + e);
				}
			}
		}

		private void Worker()
		{
			while (!_stopWorker)
			{
				if ((!_initialized) || (_logSteam == null))
					return;

				try
				{
					if (_messagesBuffer.Count == 0)
					{
						Thread.Sleep(THREAD_SLEEP);
						continue;
					}

					LogMessagesBuffer flushBuffer;
					var newBuffer = new LogMessagesBuffer();
					lock (_locker)
					{
						flushBuffer = _messagesBuffer;
						_messagesBuffer = newBuffer;
					}

					var builder = new StringBuilder();
					foreach (var msg in flushBuffer.container)
						builder.Append(msg.groupName + "> " + msg);

					var content = Encoding.UTF8.GetBytes(builder.ToString());
					_logSteam.Write(content, 0, content.Length);
					if (_flushIteration)
						_logSteam.Flush();
				}
				catch
				{
				}
			}

			if (_logSteam != null)
			{
				var endContent = Encoding.UTF8.GetBytes(new[] {(char) 9});
				_logSteam.Write(endContent, 0, endContent.Length);
				_logSteam.Close();
			}
			_logSteam = null;
		}

		private void StartLogging()
		{
			if ((!_initialized) || (_logSteam == null))
				return;

			_stopWorker = false;
			_worker = new Thread(Worker);
			_worker.Start();
		}
	}
}