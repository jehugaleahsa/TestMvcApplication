using System.Runtime.Serialization;
using System.Web.Mvc;
using MvcUtilities.Binders;
using System.ComponentModel.DataAnnotations;

namespace Adapters.Models
{
    [DataContract]
    public class SettingData
    {
        [DataMember]
        [FieldName("hdnSettingId")]
        [HiddenInput(DisplayValue = false)]
        public string SettingId { get; set; }

        [DataMember]
        [HiddenInput(DisplayValue = false)]
        public string CustomerId { get; set; }

        [Required]
        [DataMember(IsRequired=true)]
        public string Key { get; set; }

        [Required]
        [DataMember(IsRequired=true)]
        public string Value { get; set; }
    }
}
