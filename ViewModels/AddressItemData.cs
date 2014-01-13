using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ViewModels
{
    [DataContract]
    public class AddressItemData
    {
        [DataMember]
        public string AddressItemId { get; set; }

        [DataMember]
        public string CustomerId { get; set; }

        [Required]
        [DataMember(IsRequired=true)]
        public string Key { get; set; }

        [Required]
        [DataMember(IsRequired=true)]
        public string Value { get; set; }
    }
}
