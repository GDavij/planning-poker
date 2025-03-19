using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Seeders;

internal class RoleSeeder : ISeeder<Role>
{
    public void SeedForBuilder(ModelBuilder builder)
    {
        builder.Entity<Role>().HasData(
            new Role(1, "Product Owner", "PO"),
            new Role(2, "Scrum Master", "SM"),
            new Role(3, "Software Developer", "Dev"),
            new Role(4, "Quality Assurance", "QA"),
            new Role(5, "UX UI Designer", "UX/UI"),
            new Role(6, "Business Analyst", "BA"),
            new Role(7, "Project Manager", "PM"),
            new Role(7, "Tech Lead", "TL"),
            new Role(8, "DevOps Engineer", "DevOps"),
            new Role(9, "Data Scientist", "DS"),
            new Role(10, "Machine Learning Engineer", "MLE"),
            new Role(11, "Solutions Architect", "SA"),
            new Role(12, "Software Engineer", "SE"),
            new Role(13, "Database Administrator", "DBA"));
    }
}