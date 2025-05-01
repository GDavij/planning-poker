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
            new Role(8, "Tech Lead", "TL"),
            new Role(9, "DevOps Engineer", "DevOps"),
            new Role(10, "Data Scientist", "DS"),
            new Role(11, "Machine Learning Engineer", "MLE"),
            new Role(12, "Solutions Architect", "SA"),
            new Role(13, "Software Engineer", "SE"),
            new Role(14, "Database Administrator", "DBA"),
            new Role(15, "Generic Participant", "NONE"));
    }
}