using Microsoft.EntityFrameworkCore;
using hometask6.Models;

namespace hometask6.Data;

public class ApplicationContext: DbContext
{
    public DbSet<Expense> Expenses { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options) {}
}