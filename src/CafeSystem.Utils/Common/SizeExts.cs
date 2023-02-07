using System;

namespace CafeSystem.Utils;

/// <summary>
/// Provides extension methods to represents file sizes.
/// </summary>
public static class SizeExts
{
    
    /// <summary>
    /// Calculate bytes from a kilobytes value.
    /// </summary>
    /// <param name="value">Kilobytes value</param>
    /// <returns>Equivalent bytes</returns>
    public static long FromKilobytes(this double value)
    {
        return Convert.ToInt64(
            Math.Floor(value / 1024));
    }

    /// <summary>
    /// Converts bytes to kilobytes.
    /// </summary>
    /// <param name="value">Bytes value</param>
    /// <returns>Converted bytes in kilobytes</returns>
    public static double ToKilobytes(this long value)
    {
        return value / 1024;
    }

    /// <summary>
    /// Calculates bytes from a megabytes value.
    /// </summary>
    /// <param name="value">The value in megabytes</param>
    /// <returns>The bytes value equivalent to the megabytes value</returns>
    public static long FromMegabytes(this double value)
    {
        return Convert.ToInt64(
            Math.Floor(value / Math.Pow(1024, 2)));
    }

    /// <summary>
    /// Converts bytes value to megabytes
    /// </summary>
    /// <param name="value">The bytes value</param>
    /// <returns>Converted Bytes value in megabytes</returns>
    public static double ToMegabytes(this long value)
    {
        return value / (1024 * 1024);
    }

    /// <summary>
    /// Displays the value in bytes to a FileSize like representation.
    /// for example : 1KB, 20MB, 1.5GB, ...
    /// </summary>
    /// <param name="value">The value in bytes</param>
    /// <param name="sizeUnit">The SizeUnit you want to display the value in it.</param>
    /// <returns></returns>
    public static string DisplayInFileSize(this long value, FileSizeUnit sizeUnit)
    {
        return (value / Math.Pow(1024, (Int64)sizeUnit)).ToString("0.00");
    }
}