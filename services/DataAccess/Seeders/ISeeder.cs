using Microsoft.EntityFrameworkCore;

namespace DataAccess.Seeders;

public interface ISeeder<T>
{
    public void SeedForBuilder(ModelBuilder builder);
}