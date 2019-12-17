using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FamilyTree.Models;

namespace FamilyTree.Data
{
    public class FamilyTreeContext : DbContext
    {
        public FamilyTreeContext(DbContextOptions<FamilyTreeContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Person { get; set; }
    }
}
