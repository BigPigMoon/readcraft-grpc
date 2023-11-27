using Microsoft.EntityFrameworkCore;
using Storage.Models;

namespace Storage
{
    public class Sqlite : DbContext
    {
        private string DbPath { get; }
        public DbSet<User> Users { get; set; }

        public Sqlite()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "storage.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source = {DbPath}");
        }
    }
}
