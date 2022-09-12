using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context){
            //if we do have any users we will return
            if(await context.MyProperty.AnyAsync()) return;

            //we do not have any users then populate data from the json file we added earlier
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach(var user in users){
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();

                //we are hard coding password for users created in seed data
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("User@123"));
                user.PasswordSalt = hmac.Key;

                context.MyProperty.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}