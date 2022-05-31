using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop.Model;
using Webshop.Repository;
using Webshop.Repository.Common;
using Webshop.Service.Common;

namespace Webshop.Service
{
	public class ProductOrderService : IProductOrderService
	{
		IProductOrderRepository ProductOrderRepo;

		public ProductOrderService (IProductOrderRepository productOrderRepository)
		{
			this.ProductOrderRepo = productOrderRepository;
		}

		public async Task<List<ProductOrder>> GetProductsOnOrder(Order order)
		{
			return await ProductOrderRepo.GetProductsOnOrder(order);
		}

		public async Task CreateNewProductOrder(ProductOrder productOrder, Order order)
		{
			await ProductOrderRepo.CreateNewProductOrder(productOrder, order);
		}

		public async Task ChangeProductQuantity(ProductOrder productOrder, Order order)
		{
			await ProductOrderRepo.ChangeProductQuantity(productOrder, order);
		}

		public async Task RemoveProductFromOrder(ProductOrder productOrder, Order order)
		{
			await ProductOrderRepo.RemoveProductFromOrder(productOrder, order);
		}
	}
}
