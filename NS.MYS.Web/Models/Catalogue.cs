using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NS.MYS.Web.Models
{
    public class Catalogue
    {
        public int CatalogueId { get; set; }

        [DisplayName("Descripción")]
        [MaxLength(100, ErrorMessage = "La descripción del catálogo no puede tener más de 100 caracteres")]
        public string Description { get; set; }

        [DisplayName("Observación")]
        [MaxLength(1000, ErrorMessage = "El campo observación del catálogo no puede tener más de 1000 caracteres")]
        public string Observation { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}