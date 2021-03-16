namespace CryoDI
{
	/// <summary>
	/// Поведение при возникновении нежелательной ситуации
	/// </summary>
	public enum Reaction
	{
		LogWarning,
		LogError,
		ThrowException,
		Ignore
	}
}