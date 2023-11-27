using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace buy3i.api.Models
{

   public class User
   {
      [Key] public int id_user { get; set; }
      public string? first_name { get; set; }
      public string? last_name { get; set; }
      public string? email { get; set; }
      public string? confirm_email { get; set; }
      public string? password { get; set; }
      public string? confirm_password { get; set; }
      public DateTime? date_add { get; set; }
      public DateTime? date_update { get; set; }
      public byte indicator_del { get; set; }
      public DateTime? date_del { get; set; }
   }

   public class UsersDbContext : DbContext
   {
      protected readonly IConfiguration Configuration;

      public UsersDbContext(IConfiguration configuration) 
      {
         Configuration = configuration;
      }   

      protected override void OnConfiguring(DbContextOptionsBuilder options)
      {
         options.UseSqlServer(Configuration.GetConnectionString("UsersDbContext"));
      }

      public DbSet<User> TBUsers { get; set; } = null!;
   }

}
