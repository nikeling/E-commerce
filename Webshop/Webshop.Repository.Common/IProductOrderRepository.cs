using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop.Model;

namespace Webshop.Repository.Common
{
	public interface IProductOrderRepository
	{
		Task<List<ProductOrder>> GetProductsOnOrder(Order order);
		Task CreateNewProductOrder(ProductOrder productOrder, Order order);
		Task ChangeProductQuantity(ProductOrder productOrder, Order order);
		Task RemoveProductFromOrder(ProductOrder productOrder, Order order);
	}
}
