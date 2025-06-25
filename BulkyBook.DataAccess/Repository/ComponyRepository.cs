using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository;

public class ComponyRepository : Repository<Compony>, IComponyRepository{
    private readonly ApplicationDbContext _db;

    public ComponyRepository(ApplicationDbContext db) : base(db){
        _db = db;
    }

    public void Update(Compony compony){
        _db.Componies.Update(compony);
    }
}