using System;
using System.IO;

/// <summary>
/// Contains information about a file returned by the 
/// <see cref="FastDirectoryEnumerator"/> class.
/// </summary>
[Serializable]
public class FileData
{
    /// <summary>
    /// Attributes of the file.
    /// </summary>
    public readonly FileAttributes Attributes;

    public DateTime CreationTime
    {
        get { return this.CreationTimeUtc.ToLocalTime(); }
    }

    /// <summary>
    /// File creation time in UTC
    /// </summary>
    public readonly DateTime CreationTimeUtc;

    /// <summary>
    /// Gets the last access time in local time.
    /// </summary>
    public DateTime LastAccesTime
    {
        get { return this.LastAccessTimeUtc.ToLocalTime(); }
    }
        
    /// <summary>
    /// File last access time in UTC
    /// </summary>
    public readonly DateTime LastAccessTimeUtc;

    /// <summary>
    /// Gets the last access time in local time.
    /// </summary>
    public DateTime LastWriteTime
    {
        get { return this.LastWriteTimeUtc.ToLocalTime(); }
    }
        
    /// <summary>
    /// File last write time in UTC
    /// </summary>
    public readonly DateTime LastWriteTimeUtc;
        
    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public readonly long Size;

    /// <summary>
    /// Name of the file
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Full path to the file.
    /// </summary>
    public readonly string Path;

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
        return this.Name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileData"/> class.
    /// </summary>
    /// <param name="dir">The directory that the file is stored at</param>
    /// <param name="findData">WIN32_FIND_DATA structure that this
    /// object wraps.</param>
    internal FileData(string dir, WIN32_FIND_DATA findData) 
    {
        this.Attributes = findData.dwFileAttributes;


        this.CreationTimeUtc = ConvertDateTime(findData.ftCreationTime_dwHighDateTime, 
            findData.ftCreationTime_dwLowDateTime);

        this.LastAccessTimeUtc = ConvertDateTime(findData.ftLastAccessTime_dwHighDateTime,
            findData.ftLastAccessTime_dwLowDateTime);

        this.LastWriteTimeUtc = ConvertDateTime(findData.ftLastWriteTime_dwHighDateTime,
            findData.ftLastWriteTime_dwLowDateTime);

        this.Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow);

        this.Name = findData.cFileName;
        this.Path = System.IO.Path.Combine(dir, findData.cFileName);
    }

    private static long CombineHighLowInts(uint high, uint low)
    {
        return (((long)high) << 0x20) | low;
    }

    private static DateTime ConvertDateTime(uint high, uint low)
    {
        long fileTime = CombineHighLowInts(high, low);
        return DateTime.FromFileTimeUtc(fileTime);
    }
}