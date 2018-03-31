using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NS.MYS.Web.Models
{
    public class Photo
    {
        public string PhotoId { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public int CatalogueId { get; set; }
        public Catalogue Catalogue { get; set; }
    }
}