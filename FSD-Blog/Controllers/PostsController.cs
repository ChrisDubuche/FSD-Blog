using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FSD_Blog.Models;
using FSD_Blog.Helpers;
using System.IO;
using PagedList;
using PagedList.Mvc;


namespace FSD_Blog.Controllers
{
    [RequireHttps]
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index(int? page, string searchStr)        
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            var pageSize = 3; //this displayes three blog posts at a time on this page
            var pageNumber = (page ?? 1);
        
            return View(blogList.OrderByDescending(p=>p.Created).ToPagedList(pageNumber, pageSize)); 
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

        // GET: Posts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Body,Slug,MediaURL,Published")] Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(post.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(post);
                }

                if(db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(post);
                }

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var fullName = DateTime.Now.ToString("hh.mm.ss.ffffff") + "_" + fileName;
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fullName));
                    post.MediaURL = "/Uploads/" + fullName;
                }

                post.Slug = Slug;
                //programmatically adding the Created date - avoids user error
                post.Created = DateTimeOffset.Now;
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Slug,Created,MediaURL,Published")] Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var fullName = DateTime.Now.ToString("hh.mm.ss.ffffff") + "_" + fileName;
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fullName));
                    post.MediaURL = "/Uploads/" + fullName;
                }

                //the user can no longer add an Updated Date during an Edit - we'll store it programmatically
                post.Updated = DateTimeOffset.Now;
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
