using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
//using static System.Net.Mime.MediaTypeNames;



namespace ImageService.Model
{

    public class ImageServiceModel : IImageServiceModel
    {
        #region Members
        private string mOutputFolder;
        private int mThumbnailSize;

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
        public string AddFile(string path, out bool result)
        {
            result = false;
            try
            {
                if (!File.Exists(path))
                {
                    return "File does not exist.";
                }

                // Check if output folder exists
                if (!Directory.Exists(mOutputFolder))
                {
                    Directory.CreateDirectory(mOutputFolder);
                }

                DateTime dateTime = GetDateTakenFromImage(path);
                if (!Directory.Exists(mOutputFolder + '\\' + dateTime.Year))
                {
                    Directory.CreateDirectory(mOutputFolder + '\\' + dateTime.Year);
                }

                string destDir = mOutputFolder + '\\' + dateTime.Year + '\\' + dateTime.Month;

                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                System.IO.File.Copy(path, destDir, true);
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

