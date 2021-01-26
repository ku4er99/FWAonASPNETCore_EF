using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCompany.Domain.Entities;

namespace MyCompany.Domain.Repositories.Abstract
{
    public interface IServiceItemsRepository
    {
        IQueryable<ServiceItem> getServiceItems();
        ServiceItem getServiceItemById(Guid id);
        void SeveServiceItem(ServiceItem entity);
        void DeleteServiceItem(Guid id);
    }
}
