using System.Diagnostics;

namespace ImageService.Model
{
    public interface IImageServiceModel
    {
        /// <summary>
        ///     Adds the file to the output folder.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        string AddFile(string path, out EventLogEntryType result);
    }
}