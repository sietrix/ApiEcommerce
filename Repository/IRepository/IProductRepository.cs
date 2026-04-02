using System;
using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{
  public ICollection<Product> GetProducts();
  public ICollection<Product> GetProductsForCategory(int categoryId);
  public ICollection<Product> SearchProducts(string searchTerm);
  public Product? GetProduct(int id);
  public bool BuyProduct(string nameProduct, int quantity);
  public bool ProductExists(int id);
  public bool ProductExists(string nameProduct);
  public bool CreateProduct(Product product);
  public bool UpdateProduct(Product product);
  public bool DeleteProduct(Product product);
  public bool Save();
}
