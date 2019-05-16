using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLite;

namespace UserMigrationApp
{
    class Program
    {
        static readonly string applicationId = ConfigurationManager.AppSettings["applicationId"]; 
        static readonly string applicationSecret = ConfigurationManager.AppSettings["applicationSecret"]; 
        static readonly string tenant = ConfigurationManager.AppSettings["tenant"];
        static GraphApiClient client;

        static async Task Main(string[] args)
        {
            Console.Write("Checking GraphAPI client credentials...");
            client = new GraphApiClient(applicationId, applicationSecret, tenant);
            await client.EnsureInitAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("DONE.");
            Console.ForegroundColor = ConsoleColor.White;

            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            await TestUpdates();

            await DeleteTestUsers();

            // Load local users
            var userRepository = new UserRepository(connectionString);
            var users = await userRepository.GetUsersAsync();

            // Migrate first 20 user to B2C
            var createdUsers = new List<User>();
            var timespans = new List<long>();
            foreach (var user in users.Take(20))
            {
                user.DisplayName = "test " + user.DisplayName;
                var sw = Stopwatch.StartNew();
                var newUser = await client.UserCreateAsync(user);
                var elapsedMs = sw.ElapsedMilliseconds;
                Console.WriteLine($"User added: ({elapsedMs} ms): {user.DisplayName} ({user.SignInNames.First().Value})");
                timespans.Add(elapsedMs);
                createdUsers.Add(newUser);
            }

            Console.WriteLine($"Users created: {users.Count} -Avg creation: {timespans.Average()} ms.");
            Console.WriteLine();
            Console.WriteLine("Do you want to delete the created users? (Y/N)");
            var confirmation = Console.ReadLine();
            if (!string.Equals(confirmation, "Y", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            foreach (var user in createdUsers)
            {
                var sw = Stopwatch.StartNew();
                client.UserDeleteAsync(user.ObjectId).Wait();
                Console.WriteLine($"User Deleted: ({sw.ElapsedMilliseconds} ms): {user.DisplayName} ({user.SignInNames.First().Value})");
            }
        }

        static async Task TestUpdates()
        {
            var users = await client.UserGetListAsync();
            var user = users.Items[14];

            await client.UserUpdateAsync(user.ObjectId, new
            {
                signInNames = new[] {
                        new SignInName { Value = "n.ia.n.ton@gmail.com", Type = "emailAddress" }
                    }
            });

            user = await client.UserGetAsync(user.ObjectId);
        }

        static bool IsTestUser(User user)
        {
            return user.DisplayName.StartsWith("test")
                || user.DisplayName.StartsWith("[test]");
        }

        static async Task DeleteTestUsers()
        {
            Console.WriteLine("Are you sure you want to delete test users? (Y/N)");
            var confirmation = Console.ReadLine();
            if (!string.Equals(confirmation, "Y", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            var allUsers = client.UserGetAllAsync().Result;
            var testUsers = allUsers.Where(IsTestUser);
            foreach (var user in testUsers)
            {
                var sw = Stopwatch.StartNew();
                await client.UserDeleteAsync(user.ObjectId);
                Console.WriteLine($"User Deleted: ({sw.ElapsedMilliseconds} ms): {user.DisplayName} ({user.SignInNames.First().Value})");
            }
        }
    }
}
 