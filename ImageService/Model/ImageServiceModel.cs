using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Communication;
using Infrastructure.Event;

namespace ImageService.Model
{
    public class ImageServiceModel : IImageServiceModel
    {
        private static readonly Regex Regex = new Regex(":");


        private readonly string _outputFolder;
        private readonly string _tempOutputFolder;
        private readonly int _thumbnailSize;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageServiceModel" /> class.
        /// </summary>
        /// <param name="outputFolder">The output folder.</param>
        /// <param name="thumbnailSize">Size of the thumbnail.</param>
        public ImageServiceModel(string outputFolder, int thumbnailSize)
        {
            _outputFolder = outputFolder;
            _tempOutputFolder = _outputFolder + @"\temp\";
            _thumbnailSize = thumbnailSize;
            AndroidTcpClientHandler.Instance.ImageRecieved += OnImageRecieved;
        }

        /// <summary>
        /// Adds an image file from bytes array to output folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImageRecieved(object sender, ImageReceivedEventArgs e)
        {
            string newImagePath = _tempOutputFolder + e.ImageName;
            // Saves a temp image copy to a temp directory, adds it to the output folder and deletes temp copy.

            Directory.CreateDirectory(_tempOutputFolder);
            File.WriteAllBytes(newImagePath, e.Bytes);
            AddFile(newImagePath, out EventLogEntryType _);
            File.Delete(newImagePath);

            // Removes temp directory
            if (Directory.GetFiles(_tempOutputFolder).Length == 0)
            {
                Directory.Delete(_tempOutputFolder);
            }
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
                string newOutputPath = outputFilePath + copyNumber + extension;
                while (File.Exists(newOutputPath))
                {
                    if (FilesEqual(newOutputPath, path))
                    {
                        return newOutputPath;
                    }

                    i++;
                    copyNumber = "(" + i + ")";
                    newOutputPath = outputFilePath + copyNumber + extension;
                }

                string thumbnailPath = _outputFolder + "\\thumbnails\\" + pathSuffix + copyNumber + extension;

                // Creates a thumbnail
                using (Image image = Image.FromFile(path))
                using (Image thumb = image.GetThumbnailImage(_thumbnailSize, _thumbnailSize, () => false, IntPtr.Zero))
                {
                    thumb.Save(Path.ChangeExtension(thumbnailPath, "jpg"));
                }

                // Copies the file to the output folder
                File.Copy(path, newOutputPath, true);

                result = EventLogEntryType.Information;
                return newOutputPath;
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

        private static bool FilesEqual(string imagePath1, string imagePath2)
        {
            byte[] file1 = File.ReadAllBytes(imagePath1);
            byte[] file2 = File.ReadAllBytes(imagePath2);
            if (file1.Length != file2.Length)
            {
                return false;
            }

            return !file1.Where((t, i) => t != file2[i]).Any();
        }
    }
}