using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webshop.Model;
using Webshop.Repository.Common;

namespace Webshop.Repository
{
	public class OrderRepository : IOrderRepository
	{
		ProductOrderRepository ProductOrderRepo = new ProductOrderRepository();

		static readonly string connectionString = "Data Source=DESKTOP-4HPED1P;Initial Catalog=WebshopDB;Integrated Security=True";


		public async Task CreateNewOrder(List<ProductOrder> productOrders, PaymentModel payment)
		{
			Order order = new Order
			{
				Id = new Guid()
			};

			

			SqlConnection connection = new SqlConnection(connectionString);
			string sqlQuery = $"INSERT INTO Orders (Id, PaymentId, TotalPrice) VALUES ('{order.Id}', '{payment.Id}', '0');";
			decimal totalPrice = 0;

			using (connection)
			{
				connection.Open();
				SqlTransaction transaction;
				transaction = connection.BeginTransaction("UpdateTransaction");
				SqlCommand command = new SqlCommand(sqlQuery, connection)
				{
					Transaction = transaction
				};

				SqlDataReader reader = command.ExecuteReader();

				try
				{
					foreach (ProductOrder productOrder in productOrders)
					{
						await ProductOrderRepo.CreateNewProductOrder(productOrder, order);
					}

					command.CommandText = $"SELECT Product.Price, ProductOrder.Quantity FROM Product INNER JOIN ProductOrder ON ProductOrder.ProductID = Product.Id WHERE ProductOrder.OrderId = '{order.Id}';";
					command.ExecuteReader();

					while (reader.Read())
					{
						totalPrice += reader.GetDecimal(0) * reader.GetInt32(1);
					}
					reader.Close();

					command.CommandText = $"UPDATE Orders SET TotalPrice = {totalPrice} WHERE Orders.Id = {order.Id}";
					command.ExecuteNonQuery();

					transaction.Commit();
				}
				catch (Exception exception)
				{
					string exceptionMessage = exception.Message;
					try
					{
						transaction.Rollback();
					}
					catch (Exception exception2)
					{
						string exceptionMessage2 = exception2.Message;
					}
				}

				connection.Close();
			}
		}

		public async Task RemoveOrder(Order order)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			string deleteQuery = $"DELETE FROM Orders WHERE Id = '{order.Id}';";

			using (connection)
			{
				connection.Open();
				SqlTransaction transaction;
				transaction = connection.BeginTransaction("DeleteTransaction");
				SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection)
				{
					Transaction = transaction
				};

				try
				{
					foreach (ProductOrder productOrder in await ProductOrderRepo.GetProductsOnOrder(order))
						await ProductOrderRepo.RemoveProductFromOrder(productOrder, order);

					await deleteCommand.ExecuteNonQueryAsync();
					transaction.Commit();
				}
				catch (Exception exception)
				{
					string exceptionMessage = exception.Message;
					try
					{
						transaction.Rollback();						
					}
					catch (Exception exception2)
					{
						string exceptionMessage2 = exception2.Message;
					}
				}

					connection.Close();
				}
			}
		}
	}
