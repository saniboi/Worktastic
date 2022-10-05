using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Worktastic.Data;
using Worktastic.Models;

namespace Worktastic.Controllers
{
    [Authorize]
    public class JobPostingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public JobPostingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var allPostings = _context.JobPostings.ToList();
                return View(allPostings);
            }

            var jobPostingsFromDb= _context.JobPostings.Where(x => x.OwnerUsername == User.Identity.Name).ToList();
            return View(jobPostingsFromDb);
        }

        public IActionResult CreateEditJobPosting(int id)
        {
            if (id != 0)
            {
                var jobPostingFromDb = _context.JobPostings.SingleOrDefault(x => x.Id == id);

                if ((jobPostingFromDb.OwnerUsername != User.Identity.Name) && !User.IsInRole("Admin"))
                {
                    return Unauthorized();
                }
                
                if (jobPostingFromDb != null)
                {
                    return View(jobPostingFromDb);
                }else
                {
                    return NotFound();
                }
            }
            return View();
        }

        public IActionResult CreateEditJob(JobPosting jobPosting, IFormFile file2)
        {
            jobPosting.OwnerUsername = User.Identity.Name;
           
            if (file2 != null)
            {
                using (var ms = new MemoryStream())
                {
                    file2.CopyTo(ms);
                    var bytes = ms.ToArray();
                    jobPosting.CompanyImage = bytes;
                }
                
            }
            else
            {
                jobPosting.CompanyImage = Array.Empty<byte>();
            }

            if (jobPosting.Id == 0)
            {
                _context.JobPostings.Add(jobPosting);
            }
            else
            {
                //var entry = _context.Entry(jobPosting);
                //entry.State = EntityState.Modified;
                //entry.Property(x => x.CompanyImage).IsModified = false;
                //_context.JobPostings.Update(jobPosting);
                //_context.SaveChanges()

                var jobFromDb = _context.JobPostings.SingleOrDefault(x => x.Id == jobPosting.Id);

                if (jobFromDb == null)
                {
                    return NotFound();
                }

                jobFromDb.CompanyImage = jobPosting.CompanyImage;
                jobFromDb.CompanyName = jobPosting.CompanyName;
                jobFromDb.ContactMail = jobPosting.ContactMail;
                jobFromDb.ContactPhone = jobPosting.ContactPhone;
                jobFromDb.Description = jobPosting.Description;
                jobFromDb.JobLocation = jobPosting.JobLocation;
                jobFromDb.JobTitle = jobPosting.JobTitle;
                jobFromDb.Salary = jobPosting.Salary;
                jobFromDb.StartDate = jobPosting.StartDate;
                //jobFromDb.OwnerUsername = jobPosting.OwnerUsername;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteJobPostingById(int id)   
        {
            if (id == 0)
            {
                return NotFound();
            }
            var jobPostingFromDb = _context.JobPostings.SingleOrDefault(x => x.Id == id);

            if (jobPostingFromDb == null)
            {
                return NotFound();
            }

            _context.JobPostings.Remove(jobPostingFromDb);
            _context.SaveChanges();

            return Ok();
        }
    }
}
