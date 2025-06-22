using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using BulkyBook.DataAccess.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>{
    public ApplicationDbContext CreateDbContext(string[] args){
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Set your real connection string here
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=Bulky;User Id=BulkyUser;Password=Your#SecureP@ssw0rd;TrustServerCertificate=True;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}