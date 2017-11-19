using FSD_Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FSD_Blog.Helpers;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace FSD_Blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller       
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            var pageSize = 3; //this displayes three blog posts at a time on this page
            var pageNumber = (page ?? 1);

            return View(blogList.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
            // TODO: add "Where(p => p.Published == true)." after I checked the published box for the blogs 
        }
    public IQueryable<Post> IndexSearch(string searchStr)
    {
        IQueryable<Post> result = null;
        if (searchStr != null)
        {
            result = db.Posts.AsQueryable();
            result = result.Where(p => p.Title.Contains(searchStr) ||
            p.Body.Contains(searchStr) ||
            p.Comments.Any(c => c.Body.Contains(searchStr) ||
            c.Author.FirstName.Contains(searchStr) ||
            c.Author.LastName.Contains(searchStr) ||
            c.Author.DisplayName.Contains(searchStr) ||
            c.Author.Email.Contains(searchStr)));
        }

        else
        {
            result = db.Posts.AsQueryable();
        }

        return result.OrderByDescending(p => p.Created);
    }
    public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Sent()
        {
            ViewBag.Message = "Your email was successfully sent!";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>My message to you:</p><p>{2}</p> <p>This is a message from your blog site. The name and the email of the contacting person is above.</p>";
                    var from = "Christian Dubuche's Blog<cdubuche@outlook.com>";
                   /* model.Body = "This is a message from your portfolio site. The name and the email of the contacting person is above."*/;

                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = model.Subject,
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return RedirectToAction("Sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }
    }
}