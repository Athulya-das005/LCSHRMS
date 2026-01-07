using LcsTaskTracker.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace LcsTaskTracker.Api.Data
{
    public static class AdminSeeder
    {
        public static void SeedAdmin(AppDbContext context)
        {
            if (context.Users.Any(x => x.Username == "tl_admin"))
                return;

            var admin = new User
            {
                Username = "tl_admin",
                PasswordHash = HashPassword("Admin@123"),
                Role = "Admin",
                IsActive = true
            };

            context.Users.Add(admin);
            context.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
