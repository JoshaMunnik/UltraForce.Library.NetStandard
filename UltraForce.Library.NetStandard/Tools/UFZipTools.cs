using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Support methods for <see cref="ZipFile"/>.
  /// </summary>
  public static class UFZipTools
  {
    /// <summary>
    /// Adds an entry to the archive using text data.
    /// </summary>
    /// <param name="anArchive"></param>
    /// <param name="aFilename"></param>
    /// <param name="aText"></param>
    public static async Task AddTextAsync(ZipArchive anArchive, string aFilename, string aText)
    {
      ZipArchiveEntry entry = anArchive.CreateEntry(aFilename);
      using StreamWriter writer = new StreamWriter(entry.Open());
      await writer.WriteAsync(aText);
    }

    /// <summary>
    /// Adds an entry to the archive using text data.
    /// </summary>
    /// <param name="anArchive"></param>
    /// <param name="aFilename"></param>
    /// <param name="aLines"></param>
    public static async Task AddTextAsync(
      ZipArchive anArchive, string aFilename, IEnumerable<string> aLines
    )
    {
      ZipArchiveEntry entry = anArchive.CreateEntry(aFilename);
      using StreamWriter writer = new StreamWriter(entry.Open());
      foreach (string line in aLines)
      {
        await writer.WriteLineAsync(line);
      }
    }
  }
}