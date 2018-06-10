using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Web
{
    public class PhotoInfo
    {
        public PhotoInfo(string thumbnailPath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(thumbnailPath);
                Name = fileInfo.Name.Replace(fileInfo.Extension, "");
                Year = fileInfo.Directory?.Parent?.Name;
                Month = fileInfo.Directory?.Name;
                CreationDate = Month + "-" + Year;
                ThumbnailPath = thumbnailPath;
                ImagePath = ThumbnailPath.Replace(@"thumbnails\", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ImagePath")]
        public string ImagePath { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ThumbnailPath")]
        public string ThumbnailPath { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Year")]
        public string Year { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Month")]
        public string Month { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "CreationDate")]
        public string CreationDate { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}