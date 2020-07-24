using System;
using System.Text;

namespace RedDev.Kernel.Logs
{
	/// <summary>
	/// Класс описывает структуру отдельного сообщения лог-системы.
	/// </summary>
	public class LogMessage
	{
		private const string _logTimestampFormat = "[dd-MM-yyyy HH:mm:ss]\t";
		private const string _logLevelFormat = "[{0}]\t";
		private const string _logGroupFormat = "{0}> ";
		private const string _logStackTrackeFormat = "-----Stacktrace: \n{0}\n";
		
		/// <summary>
		/// Тип сообщения.
		/// </summary>
		public AlertLevel level;

		/// <summary>
		/// Время создания сообщения.
		/// </summary>
		public DateTime time;

		/// <summary>
		/// Название группы от которой происходит сообщение.
		/// </summary>
		public string groupName;

		/// <summary>
		/// Содержимое сообщения.
		/// </summary>
		public string content;

		/// <summary>
		/// Stacktrace, является необязательным полем, если это сообщение об исключении.
		/// </summary>
		public string stackTrace;

		public LogMessage()
		{
			time = DateTime.Now;
		}

		public override string ToString()
		{
			var result = new StringBuilder(time.ToString(_logTimestampFormat));
			result.Append(String.Format(_logLevelFormat, level.ToString()));
			result.Append(String.Format(_logGroupFormat, groupName));
			result.Append(content);
			if ((level >= AlertLevel.FatalError) && (String.IsNullOrEmpty(stackTrace)))
				result.Append(String.Format(_logStackTrackeFormat, stackTrace));
			result.Append("\n");
			return result.ToString();
		}
	}
}