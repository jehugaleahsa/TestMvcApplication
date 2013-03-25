using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Adapters.Binders;

namespace Adapters.Models
{
    public class CustomerData
    {
        [HiddenInput(DisplayValue=false)]
        [FieldName("hdnCustomerId")]
        public string CustomerId { get; set; }

        [Required]
        [FieldName("txtName")]
        public string Name { get; set; }

        [Required]
        [FieldName("dpBirthDate")]
        public string BirthDate { get; set; }

        [Required]
        [FieldName("txtHeight")]
        public int Height { get; set; }
    }
}
