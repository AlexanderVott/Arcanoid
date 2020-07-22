using System;

namespace RedDev.Kernel.Logs
{
	public class BaseLogListener : ILogListener
	{
		public bool isListenDelimiter { get; private set; }

		/// <summary>
		/// Конструктор базового класса. Регистрирует слушателя в системе логирования, выполняет инициализацию.
		/// </summary>
		/// <param name="listenedDelimiter">Указывает принимает ли слушатель разделительную линию в качестве сообщения.</param>
		protected BaseLogListener(bool listenedDelimiter)
		{
			isListenDelimiter = listenedDelimiter;
			LogSystem.instance.RegisterListener(this);
		}

		public virtual void Initialize()
		{
			throw new NotImplementedException("LogListener - Initialize");
		}

		public virtual void OnListen(LogMessage message)
		{
			throw new NotImplementedException("LogListener - OnListen");
		}

		public virtual void OnClose()
		{
			throw new NotImplementedException("LogListener - OnClose");
		}
	}
}