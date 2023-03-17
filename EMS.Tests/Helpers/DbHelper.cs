using EMS.DL.DataContext;
using EMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EMS.Tests.Helpers
{
    internal static class DbHelper
    {
        internal static DbContextOptions<EMSDbContext> TestDbContextOptions()
        {
            //create a new service provider to create a new in-memory database.
            ServiceProvider serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            DbContextOptionsBuilder<EMSDbContext> builder = new DbContextOptionsBuilder<EMSDbContext>().UseInMemoryDatabase("TestDb").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        internal static void SetupDatabase(EMSDbContext dbContext)
        {
            AddDepartments(dbContext);
            AddEmployees(dbContext);
        }

        internal static void AddDepartments(EMSDbContext dbContext)
        {
            bool departmentsExists = dbContext.Department.Any();
            if (departmentsExists)
                return;

            dbContext.Department.AddRange(DataHelper.GetDepartments());
            dbContext.SaveChanges();
        }

        internal static void AddEmployees(EMSDbContext dbContext)
        {
            bool employeesExists = dbContext.Employee.Any();
            if (employeesExists)
                return;

            dbContext.Employee.AddRange(DataHelper.GetEmployees());
            dbContext.SaveChanges();
        }
    }
}