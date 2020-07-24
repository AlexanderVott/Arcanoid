namespace RedDev.Kernel.Logs
{
	public enum CheckLevel : byte
	{
		Any = 0,
		Success,
		Fail,
		Silence
	}

	public static class CheckLevelExtensions
	{
		public static string GetName(this CheckLevel level)
		{
			switch (level)
			{
				case CheckLevel.Any:
					return "<Any>";
				case CheckLevel.Success:
					return "<Success>";
				case CheckLevel.Fail:
					return "<Fail>";
				default:
					return "<silence>";
			}
		}
	}
}