using Microsoft.AspNetCore.Mvc;
using MyProject.Model;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Website.Controllers;

namespace MyProject.Controllers
{
	public class CandidateJobApplicationController : SurfaceController
	{
		public CandidateJobApplicationController(
		   IUmbracoContextAccessor umbracoContextAccessor,
		   IUmbracoDatabaseFactory databaseFactory,
		   ServiceContext services,
		   AppCaches appCaches,
		   IProfilingLogger profilingLogger,
		   IPublishedUrlProvider publishedUrlProvider)
		   : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
		{ }
		public ActionResult Render()
		{
			var CandidateJobApplicationFormViewModel = new CandidateJobApplicationFormViewModel();
			return PartialView("Form", CandidateJobApplicationFormViewModel);
		}
		[HttpPost]
		public IActionResult Submit(CandidateJobApplicationFormViewModel model, IFormFile resumeFile)
		{
			

			if (resumeFile != null && resumeFile.Length > 0)
			{
				string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media", "fzanusae"); // Change the folder path as needed
				string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(resumeFile.FileName);
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
			

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					resumeFile.CopyTo(stream);
				}
				string resumeFilePath = "/media/fzanusae/" + uniqueFileName;




				var CandidateJobApplicationFormViewModel = new CandidateJobApplicationFormViewModel
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					Email = model.Email,
					Phone = model.Phone,
					JobTitle = model.JobTitle,
					CoverLetter = model.CoverLetter,
					ResumeFile = resumeFilePath


					// Other properties...
				};
				var contentService = Services.ContentService;
				var parent = contentService.GetById(1104); // ID of the parent node dbb97f39-a43c-4c66-b3ad-75c4d23193d5  candidateFormList
				var prentID = new Guid("eb693c7a-b3b6-4c47-98df-d8bb2d701da2");
				var newContent = contentService.Create("ContactPage", prentID, CandidateJobApplicationForm.ModelTypeAlias);

				newContent.SetValue("FirstName", CandidateJobApplicationFormViewModel.FirstName);
				newContent.SetValue("LastName", CandidateJobApplicationFormViewModel.LastName);

				newContent.SetValue("Email", CandidateJobApplicationFormViewModel.Email);

				newContent.SetValue("phoneNumber", CandidateJobApplicationFormViewModel.Phone); // Set phone number as string

				newContent.SetValue("jobTitle", CandidateJobApplicationFormViewModel.JobTitle);

				newContent.SetValue("coverLetter", CandidateJobApplicationFormViewModel.CoverLetter);


				newContent.SetValue("resume", CandidateJobApplicationFormViewModel.ResumeFile); // Use the property here


				newContent.SetValue("Date", DateTime.UtcNow);

				newContent.SetValue("Status", "Submitted");

				// Set other property values...

				contentService.SaveAndPublish(newContent);


			}
			return RedirectToCurrentUmbracoPage();
		}
        [HttpPost]
        public IActionResult Edit(int id, CandidateJobApplicationFormViewModel model, IFormFile resumeFile)
        {
            if (ModelState.IsValid)
            {
                var contentService = Services.ContentService;
                var existingContent = contentService.GetById(id); // Fetch the existing content by its ID

                if (existingContent != null)
                {
                    existingContent.SetValue("FirstName", model.FirstName);
                    existingContent.SetValue("LastName", model.LastName);
                    existingContent.SetValue("Email", model.Email);
                    existingContent.SetValue("phoneNumber", model.Phone); // Set phone number as string
                    existingContent.SetValue("jobTitle", model.JobTitle);
                    existingContent.SetValue("coverLetter", model.CoverLetter);

                    // Update the resume file if a new one is uploaded
                    if (resumeFile != null && resumeFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media", "fzanusae");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(resumeFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            resumeFile.CopyTo(stream);
                        }

                        string resumeFilePath = "/media/fzanusae/" + uniqueFileName;
                        existingContent.SetValue("resume", resumeFilePath);
                    }

                    existingContent.SetValue("Date", DateTime.UtcNow);
                    existingContent.SetValue("Status", "Updated");

                    contentService.SaveAndPublish(existingContent);
                }
            }

            return RedirectToAction("Index"); // Redirect to the list or details page
        }


    }
}