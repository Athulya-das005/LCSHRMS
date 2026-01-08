
using LcsTaskTracker.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace LcsTaskTracker.Api.Data
{
    public static class AdminSeeder
    {
        public static void SeedAdmin(AppDbContext context)
        {
            // 🔹 Admin
            if (!context.Users.Any(x => x.Username == "tl_admin"))
            {
                context.Users.Add(new User
                {
                    Username = "tl_admin",
                    PasswordHash = HashPassword("Admin@123"),
                    Role = "Admin",
                    IsActive = true
                });
            }

            // 🔹 Employees
            SeedEmployee(context, "sidha");
            SeedEmployee(context, "athulya");
            SeedEmployee(context, "angad");
            SeedEmployee(context, "ayush");
            SeedEmployee(context, "ali");

            context.SaveChanges();
        }

        private static void SeedEmployee(AppDbContext context, string username)
        {
            if (context.Users.Any(x => x.Username == username))
                return;

            context.Users.Add(new User
            {
                Username = username,
                PasswordHash = HashPassword("User@123"),
                Role = "Employee",
                IsActive = true
            });
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
