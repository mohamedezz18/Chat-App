using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DatingApp.Infrastructure.DbContext
{
    public class Seed
    {
        private record SeedUserDto
        {
            public Guid Id { get; init; } = default!;
            public string Email { get; init; } = default!;
            public DateOnly DateOfBirth { get; init; }
            public string? ImageUrl { get; init; }
            public string DisplayName { get; init; } = default!;
            public DateTime Created { get; init; }
            public DateTime LastActive { get; init; }
            public string Gender { get; init; } = default!;
            public string? Description { get; init; }
            public string City { get; init; } = default!;
            public string Country { get; init; } = default!;
        }

        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;
            var memberData = await File.ReadAllTextAsync("../DatingApp.Infrastructure/DbContext/UserSeedData.json");
            var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);


            if (members == null)
            {
                Console.WriteLine("No members in seed data");
                return;
            }

            foreach (var member in members)
            {
                var user = new AppUser
                {
                    Id = member.Id,
                    Email = member.Email,
                    UserName = member.Email,
                    DisplayName = member.DisplayName,
                    ImageUrl = member.ImageUrl,
                    Member = new Member
                    {
                        Id = member.Id,
                        DisplayName = member.DisplayName,
                        Description = member.Description,
                        DateOfBirth = member.DateOfBirth,
                        ImageUrl = member.ImageUrl,
                        Gender = member.Gender,
                        City = member.City,
                        Country = member.Country,
                        LastActive = member.LastActive,
                        Created = member.Created
                    }
                };

                user.Member.Photos.Add(new Photo
                {
                    Url = member.ImageUrl!,
                    MemberId = member.Id,
                });

                var result = await userManager.CreateAsync(user, "ezz123@");
                if (!result.Succeeded)
                {
                    Console.WriteLine(result.Errors.First().Description);
                }
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = "admin@test.com",
                Email = "admin@test.com",
                DisplayName = "Admin"
            };

            await userManager.CreateAsync(admin, "ezz123@");
            await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);




        }
    }
}
