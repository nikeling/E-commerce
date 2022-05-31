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
	public class ProductOrderRepository : IProductOrderRepository
	{
		static readonly string connectionString = "Data Source=DESKTOP-4HPED1P;Initial Catalog=WebshopDb;Integrated Security=True;";

		public async Task<List<ProductOrder>> GetProductsOnOrder(Order order)
		{
			SqlConnection connection = new SqlConnection(connectionString);

			using (connection)
			{
				SqlCommand sqlCommand = new SqlCommand($"SELECT Id, ProductId, Quantity, OrderId FROM dbo.ProductOrder WHERE OrderId = '{order.Id}';", connection);
				connection.Open();
				SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
				List<ProductOrder> tempProductOrders = new List<ProductOrder>();

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						ProductOrder productOrder = new ProductOrder
						{
							Id = reader.GetGuid(0),
							ProductId = reader.GetGuid(1),
							Quantity = reader.GetInt32(2),
							OrderId = reader.GetGuid(3)
						};
						tempProductOrders.Add(productOrder);
					}
					reader.Close();
					return tempProductOrders;
				}
				else
					return null;
			}
		}

		public async Task CreateNewProductOrder(ProductOrder productOrder, Order order)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			string sqlQuery = $"SELECT Quantity FROM dbo.Product WHERE Product.Id = '{productOrder.ProductId}';";

			using (connection)
			{
				connection.Open();
				SqlTransaction transaction;
				transaction = connection.BeginTransaction("UpdateTransaction");
				SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection)
				{
					Transaction = transaction
				};
				SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

				try
				{
					await reader.ReadAsync();
					if ((reader.GetDecimal(0) == 0) && (reader.GetDecimal(0) < productOrder.Quantity))
					{
						reader.Close();
						return; //invalid request; not ehnough products in stock
					}
					else
					{
						reader.Close();
						sqlCommand.CommandText = $"INSERT INTO dbo.ProductOrder (ProductId, OrderID, Quantity) VALUES ('{productOrder.ProductId}', '{order.Id}', '{productOrder.Quantity}');";
						await sqlCommand.ExecuteNonQueryAsync();

						sqlCommand.CommandText = $"UPDATE dbo.Product SET Product.Quantity -= '{productOrder.Quantity}' WHERE Product.Id = '{productOrder.ProductId}';";
						await sqlCommand.ExecuteNonQueryAsync();

						transaction.Commit();
					}
				}
				catch (Exception exception)
				{
					System.Diagnostics.Debug.WriteLine("Commit Exception Type: {0}", exception.GetType());
					System.Diagnostics.Debug.WriteLine("  Message: {0}", exception.Message);
					try
					{
						transaction.Rollback();
						string exceptionMessage = exception.Message;
					}
					catch (Exception exception2)
					{
						System.Diagnostics.Debug.WriteLine("Rollback Exception Type: {0}", exception2.GetType());
						System.Diagnostics.Debug.WriteLine("  Message: {0}", exception2.Message);
					}
				}
				connection.Close();
			}
		}

		public async Task ChangeProductQuantity(ProductOrder productOrder, Order order)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			string sqlQuery = $"SELECT Product.Quantity FROM Product WHERE Product.Id = '{productOrder.ProductId}';";
			decimal totalPrice = 0;

			using (connection)
			{
				connection.Open();
				SqlTransaction transaction;
				transaction = connection.BeginTransaction("UpdateTransaction");
				SqlCommand updateCommand = new SqlCommand(sqlQuery, connection)
				{
					Transaction = transaction
				};

				SqlDataReader reader = await updateCommand.ExecuteReaderAsync();

				try
				{
					await reader.ReadAsync();
					if ((reader.GetDecimal(0) == 0) && (reader.GetDecimal(0) < productOrder.Quantity))
					{
						reader.Close();
						return; //invalid request; not ehnough products in stock
					}
					else
					{
						updateCommand.CommandText = $"UPDATE ProductOrder SET Quantity = '{productOrder.Quantity}' WHERE OrderId = '{order.Id}' AND ProductId = '{productOrder.ProductId}';";
						await updateCommand.ExecuteNonQueryAsync();

						updateCommand.CommandText = $"UPDATE Orders SET TotalPrice = {totalPrice} WHERE Orders.Id = {order.Id}";
						await updateCommand.ExecuteNonQueryAsync();

						while (reader.Read())
						{
							totalPrice += reader.GetDecimal(0) * reader.GetInt32(1);
						}
						reader.Close();

						updateCommand.CommandText = $"UPDATE Orders SET TotalPrice = {totalPrice} WHERE Orders.Id = {order.Id}";
						await updateCommand.ExecuteNonQueryAsync();

						transaction.Commit();
					}
				}
				catch (Exception exception)
				{
					System.Diagnostics.Debug.WriteLine("Commit Exception Type: {0}", exception.GetType());
					System.Diagnostics.Debug.WriteLine("  Message: {0}", exception.Message);
					try
					{
						transaction.Rollback();
						string exceptionMessage = exception.Message;
					}
					catch (Exception exception2)
					{
						System.Diagnostics.Debug.WriteLine("Rollback Exception Type: {0}", exception2.GetType());
						System.Diagnostics.Debug.WriteLine("  Message: {0}", exception2.Message);
					}
				}

				connection.Close();
			}
		}

		public async Task RemoveProductFromOrder(ProductOrder productOrder, Order order)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			string sqlQuery = $"SELECT Product.ProductPrice, ProductOrder.Quantity FROM Product INNER JOIN ProductOrder ON ProductOrder.ProductID = Product.Id WHERE ProductOrder.OrderId = '{order.Id}' AND ProductOrder.Id = '{productOrder.Id}'; ";
			decimal totalPrice;
			int tempQuantity;

			using (connection)
			{
				connection.Open();
				SqlTransaction transaction;
				transaction = connection.BeginTransaction("DeleteTransaction");
				SqlCommand deleteCommand = new SqlCommand(sqlQuery, connection)
				{
					Transaction = transaction
				};

				SqlDataReader reader = await deleteCommand.ExecuteReaderAsync();

				try
				{
					await reader.ReadAsync();
					tempQuantity = reader.GetInt32(1);
					totalPrice = reader.GetDecimal(0) * reader.GetInt32(1);
					reader.Close();


					deleteCommand.CommandText = $"UPDATE Product SET Product.Quantity += '{tempQuantity}' WHERE Product.Id = '{productOrder.ProductId}';";
					await deleteCommand.ExecuteNonQueryAsync();

					deleteCommand.CommandText = $"DELETE FROM ProductOrder WHERE Id = '{productOrder.Id}' AND OrderID = '{order.Id}';";
					await deleteCommand.ExecuteNonQueryAsync();

					deleteCommand.CommandText = $"UPDATE Orders SET TotalPrice -= '{totalPrice}' WHERE Orders.Id = '{order.Id}'";
					await deleteCommand.ExecuteNonQueryAsync();

					transaction.Commit();
				}
				catch (Exception exception)
				{
					System.Diagnostics.Debug.WriteLine("Commit Exception Type: {0}", exception.GetType());
					System.Diagnostics.Debug.WriteLine("  Message: {0}", exception.Message);
					try
					{
						transaction.Rollback();
					}
					catch (Exception exception2)
					{
						System.Diagnostics.Debug.WriteLine("Rollback Exception Type: {0}", exception2.GetType());
						System.Diagnostics.Debug.WriteLine("  Message: {0}", exception2.Message);
					}
				}

				connection.Close();
			}
		}
	}
}
