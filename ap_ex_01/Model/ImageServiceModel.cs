using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using ImageService.Logging.Model;
//using static System.Net.Mime.MediaTypeNames;



namespace ImageService.Model
{

    public class ImageServiceModel : IImageServiceModel
    {
        #region Members
        private string mOutputFolder;
        private int mThumbnailSize;


        public ImageServiceModel(string outputFolder, int thumbnailSize)
        {
            mOutputFolder = outputFolder;
            mThumbnailSize = thumbnailSize;
        }

        // We init this once so that if the function is repeatedly called
        // it isn't stressing the garbage man
        private static Regex r = new Regex(":");

        // Retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
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

                // Checks if output folder exists
                if (!Directory.Exists(mOutputFolder))
                {
                    Directory.CreateDirectory(mOutputFolder);
                }

                DateTime dateTime = GetDateTakenFromImage(path);

                string pathInOutputDirSuffix = "\\" + dateTime.Year;
                if (!Directory.Exists(mOutputFolder + pathInOutputDirSuffix))
                {
                    Directory.CreateDirectory(mOutputFolder + pathInOutputDirSuffix);
                }

                pathInOutputDirSuffix += "\\" + dateTime.Month;
                string destDir = mOutputFolder + pathInOutputDirSuffix;

                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                // Creates thumbnail
                string thumbnailDir = mOutputFolder + "\\Thumbnails" + pathInOutputDirSuffix;
                Image image = Image.FromFile(path);
                Image thumb = image.GetThumbnailImage(mThumbnailSize, mThumbnailSize, () => false, IntPtr.Zero);
                thumb.Save(Path.ChangeExtension(thumbnailDir, "thumb"));

                System.IO.File.Copy(path, destDir, true);
                result = MessageTypeEnum.INFO;
                return destDir;
            }
            catch (Exception)
            {
                return "Could not add file.";
            }
        }
        // TODO Change to more specific error discriptions
        // TODO Create Dir if doesn't exist function


    }


    #endregion

}

