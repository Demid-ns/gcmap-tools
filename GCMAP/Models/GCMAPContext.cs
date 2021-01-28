using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GCMAP.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GCMAP.Models
{
    public class GCMAPContext : IdentityDbContext<GCMAPUser>
    {
        public DbSet<News> News { get; set; } //таблица новостей
        public DbSet<Photo> Photos { get; set; } //таблица фото
        public DbSet<Connection> Connections { get; set; } //таблица подключений
        public DbSet<Request> Requests { get; set; } //таблица заявок разных типов
        public DbSet<Map> Maps { get; set; } //таблица карт

        public GCMAPContext(DbContextOptions<GCMAPContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
