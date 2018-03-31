using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NS.MYS.Web.Data;
using NS.MYS.Web.Models;
using System.IO;

namespace NS.MYS.Web.Controllers
{
    public class CatalogueController : Controller
    {
        private NSMYSWebContext db = new NSMYSWebContext();

        private ICollection<Photo> photos
        {
            get { return Session["photos"] as ICollection<Photo>; }
            set { Session["photos"] = value; }
        }

        // GET: Catalogue
        public async Task<ActionResult> Index()
        {
            return View(await db.Catalogues.ToListAsync());
        }

        // GET: Catalogue/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Catalogue catalogue = await db.Catalogues.FindAsync(id);
            if (catalogue == null)
            {
                return HttpNotFound();
            }
            return View(catalogue);
        }

        // GET: Catalogue/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Catalogue/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CatalogueId,Description,Observation")] Catalogue catalogue)
        {
            if (ModelState.IsValid)
            {
                catalogue.Photos = photos;
                db.Catalogues.Add(catalogue);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(catalogue);
        }

        // GET: Catalogue/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Catalogue catalogue = await db.Catalogues.FindAsync(id);
            if (catalogue == null)
            {
                return HttpNotFound();
            }
            return View(catalogue);
        }

        // POST: Catalogue/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CatalogueId,Description,Observation")] Catalogue catalogue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalogue).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(catalogue);
        }

        // GET: Catalogue/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Catalogue catalogue = await db.Catalogues.FindAsync(id);
            if (catalogue == null)
            {
                return HttpNotFound();
            }
            return View(catalogue);
        }

        // POST: Catalogue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Catalogue catalogue = await db.Catalogues.FindAsync(id);
            db.Catalogues.Remove(catalogue);
            await db.SaveChangesAsync();
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

        [HttpPost]
        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            string guid;

            if (photos == null)
            {
                photos = new List<Photo>();
            }

            foreach (var file in files)
            {
                guid = Guid.NewGuid().ToString();
                string filePath = guid + Path.GetExtension(file.FileName);
                file.SaveAs(Path.Combine(Server.MapPath("~/UploadedFiles"), filePath));
                //Here you can write code for save this information in your database if you want
                photos.Add(new Photo { PhotoId = guid, Description = "descripción genérica", Order = 0 });
            }

            return Json("file uploaded successfully");
        }
    }
}
