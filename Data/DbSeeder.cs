using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HajurKoCarRental.Data
{
    public class DbSeeder
    {
        public static async Task SeedUser(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                roleManager.CreateAsync(new IdentityRole(SD.Role_User_Admin)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_User_Staff)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_User_Cust)).GetAwaiter().GetResult();

                await CreateUserIfNotExist(userManager, "nabinshrestha348@gmail.com", "Nabin Shrestha", SD.Role_User_Admin, "9869064300");
                await CreateUserIfNotExist(userManager, "customer123@gmail.com", "Nabin Shrestha", SD.Role_User_Cust, "123456789");
            }
        }

        private static async Task CreateUserIfNotExist(UserManager<IdentityUser> userManager, string email, string name, string roleName, string phoneNumber)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Name = name,
                    IsRegular = true,
                    IsActive = true,
                    Email = email,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Verified = true,
                    PaymentDue = false,
                    PhoneNumber = phoneNumber
                };

                var result = await userManager.CreateAsync(newUser, "@Nabin123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, roleName);
                }
            }
        }

        //seed car to db if there are no any car at start. this is written to add car in db when the application is first started.
        public static void SeedCar(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) //creating a new scope for resolving services
            {
                var db = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                db.Database.EnsureCreated(); // ensure db is created.

                if (!db.Cars.Any()) //checks if there are any data in cars db or not.
                {
                    db.Cars.AddRange(new List<Car>
                    {
                        new()
                        {
                            Manufacturer = "Toyota",
                            Model = "Camry",
                            Color = "Black",
                            RentalRate = 30000,
                            VehicleNo = "Ba 2 Ka 5678",
                            IsAvailable = true,
                            CarImageUrl = "https://hips.hearstapps.com/popularmechanics/assets/17/24/1497377943-dsc-1196asdfa.jpg"

                        },
                        new()
                        {
                            Manufacturer = "Ford",
                            Model = "Mustang",
                            Color = "Red",
                            RentalRate = 35000,
                            VehicleNo = "Ba 3 La 9012",
                            IsAvailable = true,
                            CarImageUrl = "https://www.bugatti.com/fileadmin/_processed_/sei/p1/se-image-2180c3d181555154d1bc13ffbbf05f29.jpg"

                        },
                        new()
                        {
                            Manufacturer = "Chevrolet",
                            Model = "Corvette",
                            Color = "Blue",
                            RentalRate = 40000,
                            VehicleNo = "Ba 4 Ma 3456",
                            IsAvailable = true,
                            CarImageUrl = "https://images.unsplash.com/photo-1525609004556-c46c7d6cf023?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1000&q=80"

                        },
                        new()
                        {
                            Manufacturer = "Nissan",
                            Model = "Altima",
                            Color = "Silver",
                            RentalRate = 28000,
                            VehicleNo = "Ba 5 Na 7890",
                            IsAvailable = true,
                             CarImageUrl = "https://www.motorbeam.com/wp-content/uploads/Hyperion-XP1-672x414.jpg"

                        },
                       new()
                        {
                            Manufacturer = "BMW",
                            Model = "X5",
                            Color = "Black",
                            RentalRate = 50000,
                            VehicleNo = "Ba 6 Oa 2345",
                            IsAvailable = true,
                            CarImageUrl = "https://images.ecestaticos.com/ebdegeb0L4ZmU_R3jEQsDcWkHBg=/0x0:0x0/1200x1200/filters:fill(white):format(jpg)/f.elconfidencial.com%2Foriginal%2F641%2F4ed%2Fd51%2F6414edd5121a3789fd5c075ff7df9fe8.jpg"

                        },
                         new()
                        {
                            Manufacturer = "Audi",
                            Model = "A6",
                            Color = "Gray",
                            RentalRate = 45000,
                            VehicleNo = "Ba 7 Pa 6789",
                            IsAvailable = true,
                              CarImageUrl = "https://imgd.aeplcdn.com/642x336/n/cw/ec/115149/maybach-s-class-exterior-right-front-three-quarter.jpeg?isig=0&q=75&fbclid=IwAR1GSGcGu_N2Ex_NmiPDW6jJ3hwrWIKMpO-dDyO4Fhe6lRXOyXywjkrcJUc"
                        },
                         new()
                        {
                             Manufacturer = "Mercedes-Benz",
                            Model = "C-Class",
                            Color = "White",
                            RentalRate = 55000,
                            VehicleNo = "Ba 8 Ra 0123",
                            IsAvailable = true,
                              CarImageUrl = "https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxzZWFyY2h8MXx8bWVyY2VkZXMlMjBjYXJ8ZW58MHx8MHx8&w=1000&q=80"
                        },
                         new()
                        {
                            Manufacturer = "Lamborghini",
                            Model = "Aventador",
                            Color = "Yellow",
                            RentalRate = 100000,
                            VehicleNo = "Ba 9 Sa 4567",
                            IsAvailable = true,
                              CarImageUrl = "https://stimg.cardekho.com/images/carexteriorimages/930x620/Bentley/Continental/7771/1676965640042/front-left-side-47.jpg?fbclid=IwAR2ENcpb5xb8FEpRloBvC5viT-QgkOPaXTgTfi3Zm4soU6G5F8kb-Onao9Q"
                        },
                        new()
                        {
                            Manufacturer = "Ferrari",
                            Model = "458 Italia",
                            Color = "Red",
                            RentalRate = 90000,
                            VehicleNo = "Ba 10 Ta 8901",
                            IsAvailable = true,
                            CarImageUrl = "https://media.evo.co.uk/image/upload/v1578500236/evo/2020/01/Bugatti%20Veyron-25.jpg"

                        }
                    });
                    db.SaveChanges();
                }
            }
        }
    }
}
