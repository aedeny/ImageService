using System.ComponentModel.DataAnnotations;

namespace Infrastructure
{
    public class StudentInfo
    {
        public static string FirstNameJsonName = "FirstName";
        public static string IdJsonName = "Id";
        public static string LastNameJsonName = "LastName";

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}