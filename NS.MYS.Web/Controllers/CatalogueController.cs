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
using NS.MYS.Web.ViewModel;

namespace NS.MYS.Web.Controllers
{
    public class CatalogueController : Controller
    {
        private NSMYSWebContext db = new NSMYSWebContext();

        //private List<Photo> Photos
        //{
        //    get { if (Session["photos"] == null) { return new List<Photo>(); } else return Photos; }
        //    set { Session["photos"] = value; }
        //}

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

            //        model.Item = await db.Items.Include(i => i.ItemVerifications)
            //.FirstOrDefaultAsync(i => i.Id == id.Value);

            Catalogue catalogue = await db.Catalogues.Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.CatalogueId == id.Value);

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
                if (Session["photos"] == null) { }
                else
                {
                    db.Photos.AddRange((IEnumerable<Photo>)Session["photos"]);
                    catalogue.Photos = (ICollection<Photo>)Session["photos"];
                    db.Catalogues.Add(catalogue);
                    await db.SaveChangesAsync();
                    Session["photos"] = null;
                }
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
            List<Photo> photos = null;
            if (Session["photos"] == null)
            {
                photos = new List<Photo>();
            }
            else
            {
                photos = (List<Photo>)Session["photos"];
            }
            string filePath = null;
            foreach (var file in files)
            {
                //guid = Guid.NewGuid().ToString();
                filePath = Guid.NewGuid() + Path.GetExtension(file.FileName);
                file.SaveAs(Path.Combine(Server.MapPath("~/UploadedFiles"), filePath));

                //Here you can write code for save this information in your database if you want
                photos.Add(new Photo { PhotoId = filePath, Description = "descripción genérica", Order = 0 });
            }

            Session["photos"] = photos;

            return Json(filePath);
        }


        [HttpPost]
        public ActionResult DeletePhoto(string photoId)
        {
            List<Photo> photos = (List<Photo>)Session["photos"];

            Session["photos"] = photos.Where(x => x.PhotoId != photoId).ToList();

            System.IO.File.Delete(Path.Combine(Server.MapPath("~/UploadedFiles"), photoId));

            ViewBag.Photos = photos.Where(x => x.PhotoId != photoId).ToList();
            ViewBag.EliminaPhoto = true;
            return PartialView("PhotoPartial");
        }

        public async Task<ActionResult> Catalogue()
        {
            //var catalogos = await db.Catalogues.Include(x => x.Photos).ToListAsync();
            // catalogos.Select(x => new SelectListItem { Value = x.CatalogueId.ToString(), Text = x.Description });
            CatalogueViewModel catalogueViewModel = new CatalogueViewModel
            {
                Catalogues = await db.Catalogues.ToListAsync()
            };
            return View(catalogueViewModel);
        }

        //[HttpPost]
        public async Task<ActionResult> CatalogoVarios(int selectedUserCatalogueId)
        {
            Catalogue Catalogues = await db.Catalogues.Include(i => i.Photos).Where(x => x.CatalogueId == selectedUserCatalogueId).FirstOrDefaultAsync();

            ViewBag.Photos = Catalogues.Photos;
            ViewBag.EliminaPhoto = false;

            //return View(catalogueViewModel);
            return PartialView("PhotoPartial");
        }

        //POST: Catalogue/DestroyFiles
        //TODO: hay que enviarle el antiforgerytoken...
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DestroyFiles()
        {
            if (Session["photos"] != null)
            {
                foreach (Photo photo in (List<Photo>)Session["photos"])
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/UploadedFiles"), photo.PhotoId));
                }
            }
            return Json("Archivos destruídos correctamente");
        }
    }
}
