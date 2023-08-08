using System.ComponentModel.DataAnnotations;

namespace MyProject.Model
{
	public class CandidateJobApplicationFormViewModel
	{
		[Key]
		public int CandidateID { get; set; }

		[Required]
		[MaxLength(50)]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(50)]
		public string LastName { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
	
		public string Phone { get; set; }

		[Required]
		public string ResumeFile { get; set; } // You may choose to store the file path or binary data here

		public string CoverLetter { get; set; }
        public string JobTitle { get; set; }
        public string Status { get; set; }

        [Required]
		public DateTime DateApplied { get; set; }
	}
}
