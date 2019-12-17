using FamilyTree;
using FamilyTree.Controllers;
using FamilyTree.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FamilyTreeTests
{
    public class Tests
    {            
        private static readonly FamilyTreeContext _context;
        private static IMemoryCache _cache;

        HomeController homeController = new HomeController(_context,_cache);

        public Tests()
        {           

        }
        
        [SetUp]
        public void Setup()
        {
            
        }

        //RandomNumber function in the HomeController should return an integer between 1 and 100

        [Test]
        public async Task RandomTest()
        {
            int i = await homeController.RandomNumber();
            
            Assert.GreaterOrEqual(i,1);
            Assert.LessOrEqual(i, 100);
        }

        [Test]
        public async Task IntegrationTest()
        {
            // Arrange
            var appFactory = new WebApplicationFactory<Startup>();

            var client = appFactory.CreateClient();

            client.BaseAddress = new Uri("https://localhost:44387");

            //Act
            var response = await client.GetAsync("Home/GetFamilyTree/{id}".Replace("{id}","1"));

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}