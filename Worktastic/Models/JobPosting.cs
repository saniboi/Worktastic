using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Worktastic.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string JobLocation { get; set; }
        public string Description { get; set; }
        public int Salary { get; set; }
        public DateTime StartDate { get; set; }
       
        //[Required(AllowEmptyStrings = false)]
        public string? CompanyName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactMail { get; set; }
        public string ContactWebsite { get; set; }
        public byte[] CompanyImage { get; set; }
        public string OwnerUsername { get; set; }
    }
}
