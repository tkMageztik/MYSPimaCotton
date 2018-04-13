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
    [Authorize]
    public class CatalogueController : Controller
    {
        private NSMYSWebContext db = new NSMYSWebContext();

        //private List<Photo> Photos
        //{
        //    get { if (TempData["photos"] == null) { return new List<Photo>(); } else return Photos; }
        //    set { TempData["photos"] = value; }
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
            TempData["photos"] = null;
            return View();
        }

        // POST: Catalogue/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CatalogueId,Description,Observation,Photos")] Catalogue catalogue)
        {
            if (ModelState.IsValid)
            {
                if (TempData["photos"] == null) { }
                else
                {
                    db.Photos.AddRange((IEnumerable<Photo>)TempData["photos"]);
                    catalogue.Photos = (ICollection<Photo>)TempData["photos"];
                    db.Catalogues.Add(catalogue);
                    await db.SaveChangesAsync();
                    TempData["photos"] = null;
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
            Catalogue catalogue = await db.Catalogues.Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.CatalogueId == id.Value);

            TempData["photos"] = catalogue.Photos.ToList<Photo>();

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
        public async Task<ActionResult> Edit([Bind(Include = "CatalogueId,Description,Observation,Photos")] Catalogue catalogue)
        {
            if (ModelState.IsValid)
            {
                //TODO: crear transacción
                if (TempData["photosEdit"] != null)
                {
                    foreach (Photo photo in (List<Photo>)TempData["photosEdit"])
                    {
                        photo.CatalogueId = catalogue.CatalogueId;
                        db.Entry(photo).State = EntityState.Added;
                    }
                    TempData["photosEdit"] = null;
                }

                if (TempData["tempDeleted"] != null)
                {
                    foreach (Photo photo in ((List<Photo>)TempData["tempDeleted"]))
                    {
                        Photo photoBD = db.Photos.Where(x => x.PhotoId == photo.PhotoId).FirstOrDefault();
                        if (photoBD != null)
                        {
                            db.Entry(photoBD).State = EntityState.Deleted;
                        }

                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/UploadedFiles"), photo.PhotoId));
                    }

                    //await db.SaveChangesAsync();
                    TempData["tempDeleted"] = null;
                }

                var local = db.Set<Catalogue>()
                         .Local
                         .FirstOrDefault(f => f.CatalogueId == catalogue.CatalogueId);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

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
            Catalogue catalogue = await db.Catalogues.Include(i => i.Photos).Where(x => x.CatalogueId == id).FirstOrDefaultAsync();

            //TODO incluir transaction
            foreach (Photo photo in catalogue.Photos)
            {
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/UploadedFiles"), photo.PhotoId));
            }
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
        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files, string view, Catalogue model)
        {
            List<Photo> photos = null;
            List<Photo> photosEdit = null;
            string filePath = null;
            if (TempData["photos"] == null)
            {
                photos = new List<Photo>();
            }
            else
            {
                photos = (List<Photo>)TempData["photos"];
            }

            if (view == "Edit")
            {
                if (TempData["photosEdit"] == null)
                {
                    photosEdit = new List<Photo>();
                }
                else
                {
                    photosEdit = (List<Photo>)TempData["photosEdit"];
                }
            }

            foreach (var file in files)
            {
                //guid = Guid.NewGuid().ToString();
                filePath = Guid.NewGuid() + Path.GetExtension(file.FileName);
                file.SaveAs(Path.Combine(Server.MapPath("~/UploadedFiles"), filePath));

                //Here you can write code for save this information in your database if you want
                photos.Add(new Photo { PhotoId = filePath, Description = "descripción genérica", Order = 0 });

                if (view == "Edit")
                {
                    photosEdit.Add(new Photo { PhotoId = filePath, Description = "descripción genérica", Order = 0 });
                }
            }
            TempData["photos"] = photos;
            TempData["photosEdit"] = photosEdit;

            //return Json(filePath);
            ViewBag.Photos = photos;
            ViewBag.EliminaPhoto = true;
            return PartialView("_PhotoPartial", model);
        }


        [HttpPost]
        public ActionResult DeletePhoto(string photoId, string view)
        {
            List<Photo> photos = (List<Photo>)TempData["photos"];

            //List<Photo> photosEdit = (List<Photo>)TempData["photosEdit"];
            ////TODO: incluir transaccion
            //if (photosEdit != null)
            //{
            //    photosEdit = photosEdit.Where(x => x.PhotoId != photoId).ToList();
            //    TempData["photosEdit"] = photosEdit;
            //    photos = photos.Concat(photosEdit).ToList();
            //}

            if (view == "Create")
            {
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/UploadedFiles"), photoId));
            }
            else if (view == "Edit")
            {
                List<Photo> tempDeleted = null;
                if (TempData["tempDeleted"] != null)
                {
                    tempDeleted = (List<Photo>)TempData["tempDeleted"];
                }
                else
                {
                    tempDeleted = new List<Photo>();
                }
                tempDeleted.Add(photos.Where(x => x.PhotoId == photoId).FirstOrDefault());
                TempData["tempDeleted"] = tempDeleted;

                List<Photo> photosEdit = null;
                if (TempData["photosEdit"] != null)
                {
                    photosEdit = (List<Photo>)TempData["photosEdit"];
                }
                else
                {
                    photosEdit = new List<Photo>();
                }

                TempData["photosEdit"] = photosEdit.Where(x => x.PhotoId != photoId).ToList();
            }

            //TODO: incluir transaccion
            photos = photos.Where(x => x.PhotoId != photoId).ToList();
            TempData["photos"] = photos;

            ViewBag.Photos = photos;
            ViewBag.EliminaPhoto = true;
            return PartialView("_PhotoPartial");
        }

        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<ActionResult> CatalogoVarios(int selectedUserCatalogueId)
        {
            Catalogue Catalogues = await db.Catalogues.Include(i => i.Photos).Where(x => x.CatalogueId == selectedUserCatalogueId).FirstOrDefaultAsync();

            ViewBag.Photos = Catalogues.Photos;
            ViewBag.EliminaPhoto = false;

            //return View(catalogueViewModel);
            return PartialView("_PhotoPartial");
        }

        //POST: Catalogue/DestroyFiles
        //TODO: hay que enviarle el antiforgerytoken...
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DestroyFiles(string view)
        {
            if (view == "Edit")
            {
                if (TempData["photosEdit"] != null)
                {
                    foreach (Photo photo in (List<Photo>)TempData["photosEdit"])
                    {
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/UploadedFiles"), photo.PhotoId));
                    }
                    TempData["photosEdit"] = null;
                }
            }
            else if (view == "Create")
            {
                if (TempData["photos"] != null)
                {
                    foreach (Photo photo in (List<Photo>)TempData["photos"])
                    {
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/UploadedFiles"), photo.PhotoId));
                    }
                    TempData["photos"] = null;
                }
            }
            return Json("Archivos destruidos correctamente");
        }
    }
}
