using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Moralis.Platform.Utilities
{
    /// <summary>
    /// A collection of utility methods and properties for writing to the app-specific persistent storage folder.
    /// </summary>
    internal static class FileUtilities
    {
        /// <summary>
        /// Asynchronously read all of the little-endian 16-bit character units (UTF-16) contained within the file wrapped by the provided <see cref="FileInfo"/> instance.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/> instance wrapping the target file that string content is to be read from</param>
        /// <returns>A task that should contain the little-endian 16-bit character string (UTF-16) extracted from the <paramref name="file"/> if the read completes successfully</returns>
        public static async Task<string> ReadAllTextAsync(this FileInfo file)
        {
            using StreamReader reader = new StreamReader(file.OpenRead(), Encoding.Unicode);
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Asynchronously writes the provided little-endian 16-bit character string <paramref name="content"/> to the file wrapped by the provided <see cref="FileInfo"/> instance.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/> instance wrapping the target file that is to be written to</param>
        /// <param name="content">The little-endian 16-bit Unicode character string (UTF-16) that is to be written to the <paramref name="file"/></param>
        /// <returns>A task that completes once the write operation to the <paramref name="file"/> completes</returns>
        public static async Task WriteContentAsync(this FileInfo file, string content)
        {
            using FileStream stream = new FileStream(Path.GetFullPath(file.FullName), FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.SequentialScan | FileOptions.Asynchronous);
            byte[] data = Encoding.Unicode.GetBytes(content);
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
