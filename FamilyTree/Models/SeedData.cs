using FamilyTree.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTree.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FamilyTreeContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<FamilyTreeContext>>()))
            {
                // Look for any movies.
                if (context.Person.Any())
                {
                    return;   // DB has been seeded
                }

                context.Person.AddRange(
                    new Person
                    {
                        FirstName = "Harry",
                        LastName = "Smith",
                        Age = 80,
                        Child = new Person
                        {
                            FirstName = "Andrew",
                            LastName = "Smith",
                            Age = 50,
                            Child = new Person
                            {
                                FirstName = "Victoria",
                                LastName = "Smith",
                                Age = 27
                            }
                        }
                    },
                    new Person
                    {
                        FirstName = "Teresa",
                        LastName = "Thompson",
                        Age = 20
                    },
                    new Person
                    {
                        FirstName = "Ken",
                        LastName = "Black",
                        Age = 29,
                        Child = new Person
                        {
                            FirstName = "Nicholas",
                            LastName = "Black",
                            Age = 3
                        }
                    }
                ); ; 

                context.SaveChanges();
            }

        }
    }
}
