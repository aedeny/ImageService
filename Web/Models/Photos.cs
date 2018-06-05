using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class Photos
    {
        [DataType(DataType.Text)]
        [Display(Name = "Thumbnails")]
        public ObservableCollection<string> Thumbnails { get; set; }
    }
}