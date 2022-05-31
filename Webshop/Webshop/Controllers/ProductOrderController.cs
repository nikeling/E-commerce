using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Webshop.Model;
using Webshop.Service;
using Webshop.Service.Common;

namespace Webshop.Controllers
{
    public class ProductOrderController : ApiController
    {
        IProductOrderService ProductOrderService;
        public ProductOrderController (IProductOrderService productOrderService)
        {
            this.ProductOrderService = productOrderService;
        }

        [HttpGet]
        [Route("api/productOrder/")]
        public async Task<HttpResponseMessage> GetProductsOnOrder([FromBody]Order order)
        {
            return Request.CreateResponse(HttpStatusCode.OK, await ProductOrderService.GetProductsOnOrder(order));
        }

        [HttpPost]
        [Route("api/productOrder/")]
        public async Task<HttpResponseMessage> CreateNewProductOrder([FromBody]ProductOrder productOrder, [FromUri]Order order)
        {
            await ProductOrderService.CreateNewProductOrder(productOrder, order);
            return Request.CreateResponse(HttpStatusCode.OK, "New product added to the order.");
        }

        [HttpDelete]
        [Route("api/productOrder/")]
        public async Task<HttpResponseMessage> RemoveProductFromOrder([FromBody]ProductOrder productOrder, Order order)
        {
            await ProductOrderService.RemoveProductFromOrder(productOrder, order);
            return Request.CreateResponse(HttpStatusCode.OK, "Product removed from the order.");
        }
    }
}
