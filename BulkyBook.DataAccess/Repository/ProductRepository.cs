using System.Linq.Expressions;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db) : base(db){
        _db = db;
    }

    public void Update(Product product){
        var entity = _db.Products.FirstOrDefault(u => u.Id == product.Id);
        if (entity != null){
            entity.Title = product.Title;
            entity.Description = product.Description;
            entity.Price = product.Price;
            entity.CategoryId = product.CategoryId;
            entity.Author = product.Author;
            entity.Price50 = product.Price50;
            entity.Price100 = product.Price100;
            entity.ListPrice = product.ListPrice;
            entity.ISBN = product.ISBN;

            if (entity.ImageUrl != null){
                entity.ImageUrl = product.ImageUrl;
            }
        }
    }
}