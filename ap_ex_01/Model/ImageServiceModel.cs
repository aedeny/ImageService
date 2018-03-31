using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using ImageService.Logger.Model;
using ImageService.Logger;

namespace ImageService.Model
{

    public class ImageServiceModel : IImageServiceModel
    {
        #region Members
        private string mOutputFolder;
        private int mThumbnailSize;
        private ILoggingService mLoggingService;
        #endregion

        public ImageServiceModel(string outputFolder, int thumbnailSize, ILoggingService loggingService)
        {
            mOutputFolder = outputFolder;
            mThumbnailSize = thumbnailSize;
            mLoggingService = loggingService;
        }

        private static Regex r = new Regex(":");

        private DateTime GetDateTakenFromImage(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(306);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                mLoggingService.Log("dateTakenString: " + dateTaken, MessageTypeEnum.INFO);
                return DateTime.Parse(dateTaken);
            }
        }
        public string AddFile(string path, out MessageTypeEnum result)
        {
            result = MessageTypeEnum.FAILURE;

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

                string pathSuffix = dateTime.Year + "\\" + dateTime.Month + "\\" + Path.GetFileName(path);
                string outputFilePath = mOutputFolder + "\\" + pathSuffix;
                string thumbnailPath = mOutputFolder + "\\thumbnails\\" + pathSuffix;

                // Creates thumbnail
                using (Image image = Image.FromFile(path))
                using (Image thumb = image.GetThumbnailImage(mThumbnailSize, mThumbnailSize, () => false, IntPtr.Zero))
                    thumb.Save(Path.ChangeExtension(thumbnailPath, "thumb"));

                // Copies file to output folder
                System.IO.File.Copy(path, outputFilePath, true);

                result = MessageTypeEnum.INFO;
                return outputFilePath;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private void CreateDirectoriesStructure(DateTime dateTime)
        {
            // Creates image folder
            Directory.CreateDirectory(mOutputFolder + "\\" + dateTime.Year + "\\" + dateTime.Month);

            // Creates thumbnail folder
            Directory.CreateDirectory(mOutputFolder + "\\thumbnails\\" + dateTime.Year + "\\" + dateTime.Month);
        }
    }
}

