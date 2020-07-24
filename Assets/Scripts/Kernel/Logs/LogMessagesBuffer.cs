using System.Collections.Generic;

namespace RedDev.Kernel.Logs
{
	/// <summary>
	/// Буфер сообщений лог-системы.
	/// Используется для двойной буферизации сброса данных в хранилище.
	/// </summary>
	public class LogMessagesBuffer
	{
		/// <summary>
		/// Контейнер сообщений.
		/// </summary>
		public List<LogMessage> container { get; private set; }

		public int Count { get { return container.Count; } }

		public LogMessage this[int index]
		{
			get { return index >= 0 && index < Count ? container[index] : null; }
		}

		public LogMessagesBuffer()
		{
			container = new List<LogMessage>();
		}
	}
}