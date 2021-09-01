internal class Singleton<T> where T : class, new()
{
	public static readonly T instance = new T();
}