using API.Models;
using API.Utilities;

namespace API.Data.Seed;

public class UserSeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = serviceProvider.GetRequiredService<AppDbContext>())
        {
            CryptoUtils _crypto = new CryptoUtils();
            string passwordSalt = _crypto.GenerateSalt();

            if (!context.Users.Any(u => u.Username == "Admin"))
            {
                var adminUser = new User
                {
                    Username = "Admin",
                    Email = "test@test.com",
                    Password = _crypto.HashPassword("admin@123", passwordSalt),
                    PasswordSalt = passwordSalt,
                    DateCreated = DateTime.Now,
                    CreatedBy = 0,
                    ModifiedBy = 0,
                    IsActive = true
                };
                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}