using System;
using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{
  ICollection<Product> GetProducts();
  ICollection<Product> GetProductsInPages(int pageNumber, int pageSize);
  int GetTotalProducts();
  ICollection<Product> GetProductsForCategory(int categoryId);
  ICollection<Product> SearchProducts(string searchTerm);
  Product? GetProduct(int id);
  bool BuyProduct(string nameProduct, int quantity);
  bool ProductExists(int id);
  bool ProductExists(string nameProduct);
  bool CreateProduct(Product product);
  bool UpdateProduct(Product product);
  bool DeleteProduct(Product product);
  bool Save();
}
