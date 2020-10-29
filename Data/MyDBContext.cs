using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serversideprogrammering.Models;

namespace Serversideprogrammering.Data
{
    public class MyDBContext:DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<TodoItem> TodoItem { get; set; }       
        public IConfiguration Configuration { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration["SqlConnectionString"]);
        }

        public MyDBContext(DbContextOptions<MyDBContext> options, IConfiguration configuration) :base(options) 
        { 
            Configuration = configuration; 
        }   
    }
}
