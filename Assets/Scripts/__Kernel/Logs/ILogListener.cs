namespace RedDev.Kernel.Logs
{
	/// <summary>
	/// Интерфейс описывающий слушателей системы логирования.
	/// </summary>
	public interface ILogListener
	{
		/// <summary>
		/// Указывает принимает ли слушатель сообщения разделительной линии.
		/// </summary>
		bool isListenDelimiter { get; }

		/// <summary>
		/// Инициализация слушателя.
		/// Вызывается при регистрации слушателя в системе логирования.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Непосредственно метод прослушки сообщений.
		/// </summary>
		void OnListen(LogMessage message);

		/// <summary>
		/// Закрытие слушателя, завершение прослушивания логирования.
		/// </summary>
		void OnClose();
	}
}