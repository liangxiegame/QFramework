namespace ICSharpCode.SharpZipLib.Core
{
	/// <summary>
	/// WindowsPathUtils provides simple utilities for handling windows paths.
	/// </summary>
	public abstract class WindowsPathUtils
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WindowsPathUtils"/> class.
		/// </summary>
		internal WindowsPathUtils()
		{
		}

		/// <summary>
		/// Remove any path root present in the path
		/// </summary>
		/// <param name="path">A <see cref="string"/> containing path information.</param>
		/// <returns>The path with the root removed if it was present; path otherwise.</returns>
		/// <remarks>Unlike the <see cref="System.IO.Path"/> class the path isnt otherwise checked for validity.</remarks>
		public static string DropPathRoot(string path)
		{
			string result = path;

			if (!string.IsNullOrEmpty(path)) {
				if ((path[0] == '\\') || (path[0] == '/')) {
					// UNC name ?
					if ((path.Length > 1) && ((path[1] == '\\') || (path[1] == '/'))) {
						int index = 2;
						int elements = 2;

						// Scan for two separate elements \\machine\share\restofpath
						while ((index <= path.Length) &&
							(((path[index] != '\\') && (path[index] != '/')) || (--elements > 0))) {
							index++;
						}

						index++;

						if (index < path.Length) {
							result = path.Substring(index);
						} else {
							result = "";
						}
					}
				} else if ((path.Length > 1) && (path[1] == ':')) {
					int dropCount = 2;
					if ((path.Length > 2) && ((path[2] == '\\') || (path[2] == '/'))) {
						dropCount = 3;
					}
					result = result.Remove(0, dropCount);
				}
			}
			return result;
		}
	}
}
