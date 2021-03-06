﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web.Mvc;
using MvcUtilities.Binders;
using MvcUtilities.ValidationAttributes;

namespace ViewModels
{
    [DataContract]
    public class CustomerData
    {
        [HiddenInput(DisplayValue=false)]
        [FieldName("hdnCustomerId")]
        [Guid]
        [DataMember]
        public string CustomerId { get; set; }

        [Required]
        [FieldName("txtName")]
        [DataMember(IsRequired=true)]
        public string Name { get; set; }

        [Required]
        [Date]
        [FieldName("dpBirthDate")]
        [DataMember(IsRequired = true)]
        public string BirthDate { get; set; }

        [Required]
        [FieldName("txtHeight")]
        [DataMember(IsRequired = true)]
        public int Height { get; set; }
    }
}
