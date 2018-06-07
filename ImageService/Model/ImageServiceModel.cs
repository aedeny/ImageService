using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ImageService.Model
{
    public class ImageServiceModel : IImageServiceModel
    {
        private static readonly Regex Regex = new Regex(":");


        private readonly string _outputFolder;
        private readonly int _thumbnailSize;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageServiceModel" /> class.
        /// </summary>
        /// <param name="outputFolder">The output folder.</param>
        /// <param name="thumbnailSize">Size of the thumbnail.</param>
        public ImageServiceModel(string outputFolder, int thumbnailSize)
        {
            _outputFolder = outputFolder;
            _thumbnailSize = thumbnailSize;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adds the file to the output folder.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public string AddFile(string path, out EventLogEntryType result)
        {
            result = EventLogEntryType.Error;

            try
            {
                if (!File.Exists(path))
                {
                    return "File does not exist.";
                }

                // Tries to get date taken from image. If property doesn't exist, gets date created.
                DateTime dateTime;
                try
                {
                    dateTime = GetDateTakenFromImage(path);
                }
                catch (Exception)
                {
                    dateTime = File.GetCreationTime(path);
                }

                CreateDirectoriesStructure(dateTime);

                string pathSuffix = dateTime.Year + "\\" + dateTime.Month + "\\" +
                                    Path.GetFileNameWithoutExtension(path);

                string outputFilePath = _outputFolder + "\\" + pathSuffix;
                string extension = Path.GetExtension(path);

                int i = 0;
                string copyNumber = "";

                // If a file named 'name.image' already exists, a file named 'name(1).image' will be created.
                while (File.Exists(outputFilePath + copyNumber + extension))
                {
                    i++;
                    copyNumber = "(" + i + ")";
                }

                outputFilePath += copyNumber + extension;

                string thumbnailPath = _outputFolder + "\\thumbnails\\" + pathSuffix + copyNumber + extension;

                // Creates a thumbnail
                using (Image image = Image.FromFile(path))
                using (Image thumb = image.GetThumbnailImage(_thumbnailSize, _thumbnailSize, () => false, IntPtr.Zero))
                {
                    thumb.Save(Path.ChangeExtension(thumbnailPath, "jpg"));
                }

                // Copies the file to the output folder
                File.Copy(path, outputFilePath, true);

                result = EventLogEntryType.Information;
                return outputFilePath;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        ///     Gets the date taken from image.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private static DateTime GetDateTakenFromImage(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(0x9004);
                string dateTaken = Regex.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        /// <summary>
        ///     Creates the directories structure.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        private void CreateDirectoriesStructure(DateTime dateTime)
        {
            // Creates image folder
            Directory.CreateDirectory(_outputFolder + "\\" + dateTime.Year + "\\" + dateTime.Month);

            // Creates thumbnail folder
            Directory.CreateDirectory(_outputFolder + "\\thumbnails\\" + dateTime.Year + "\\" + dateTime.Month);
        }
    }
}