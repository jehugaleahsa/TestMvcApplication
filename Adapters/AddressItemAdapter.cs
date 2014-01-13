using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Adapters.Mappers;
using ViewModels;
using Policies;
using DataObjects;
using ServiceInterfaces;

namespace Adapters
{
    public interface IAddressItemAdapter
    {
        AddressItemData AddAddressItem(AddressItemData data);

        IEnumerable<AddressItemData> GetAddressItems(string customerId);

        void RemoveAddressItem(string settingId);
    }

    public class AddressItemAdapter : IAddressItemAdapter
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IAddressItemRepository itemRepository;

        public AddressItemAdapter(
            ICustomerRepository customerRepository, 
            IAddressItemRepository itemRepository)
        {
            if (customerRepository == null)
            {
                throw new ArgumentNullException("customerRepository");
            }
            if (itemRepository == null)
            {
                throw new ArgumentNullException("itemRepository");
            }
            this.customerRepository = customerRepository;
            this.itemRepository = itemRepository;
            AddressItemMapper = new AddressItemMapper();
        }

        public IAddressItemMapper AddressItemMapper { get; set; }

        [Log]
        [ErrorMessage("An error occurred while retrieving the customer's address items.")]
        public IEnumerable<AddressItemData> GetAddressItems(string customerId)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            Customer customer = customerRepository.GetCustomer(mapper.ToGuid(customerId));
            if (customer == null)
            {
                throw new AdapterException(HttpStatusCode.NotFound, "A customer with the given ID was not found.");
            }
            return itemRepository.GetAddressItems(customer).Select(s => AddressItemMapper.Convert(s)).ToArray();
        }

        [Log]
        [ErrorMessage("An error occurred while adding the address item.")]
        public AddressItemData AddAddressItem(AddressItemData data)
        {
            AddressItem item = AddressItemMapper.Convert(data);
            itemRepository.Add(item);
            return AddressItemMapper.Convert(item);
        }

        [Log]
        [ErrorMessage("An error occurred while removing the address item.")]
        public void RemoveAddressItem(string settingId)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            AddressItem item = itemRepository.GetAddressItem(mapper.ToGuid(settingId));
            if (item == null)
            {
                throw new AdapterException(HttpStatusCode.NotFound, "An address item with the given ID was not found.");
            }
            itemRepository.Remove(item);
        }
    }
}
