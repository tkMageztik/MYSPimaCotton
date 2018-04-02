using NS.MYS.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NS.MYS.Web.ViewModel
{
    public class CatalogueViewModel
    {
        public IEnumerable<Catalogue> Catalogues { get; set; }
        public int SelectedUserCatalogueId { get; set; }
        public IEnumerable<Photo> Photos { get; set; }
        

        //public IEnumerable<SelectListItem> SelectListCatalogues { get; set; }  
        //public ICollection<Photo> Photos { get; set; }
    }
}