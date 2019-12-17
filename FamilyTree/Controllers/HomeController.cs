using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FamilyTree.Models;
using FamilyTree.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace FamilyTree.Controllers
{
    [Route("Home/")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly FamilyTreeContext _context;
        private IMemoryCache _cache;

        public HomeController(FamilyTreeContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        // All information is stored in LocalDB.

          
        // Get a parent by id and his/her child. 

        [HttpGet]
        [Route("GetFamilyTree/{id}")]
        public ActionResult<Person> GetFamilyTree(int id)
        {
            var person = _context.Person.Where(m => m.Id == id).Include("Child").FirstOrDefault();
            var cacheEntry = new Person();

            // Look for cache key.
            if (!_cache.TryGetValue(CacheKeys.Entry, out cacheEntry))
            {
                cacheEntry = person;
                 // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for 60 seconds, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60));

                // Save data in cache.
                _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);

                return person;
            } else
            {
                return _cache.Get<Person>(CacheKeys.Entry);
            }
                        
        }
        
        // Add a new person to the database. 

        [HttpPost]
        [Route("AddPerson")]
        public async Task<ActionResult<Person>> AddPerson([FromBody] Person person)
        {
            var tempPerson = new Person
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Age = person.Age
            };

            await _context.AddAsync(tempPerson);
            await _context.SaveChangesAsync();

            return Ok();        
        }


        // Add a child to person by id. 

        [HttpPost]
        [Route("AddChild")]
        public async Task<ActionResult<Person>> AddChild(int id, [FromBody] Person child)
        {
            // Age gets a random number if empty. 
            if (child.Age == null)
            {
                child.Age = await RandomNumber();
            }

            var tempPerson = _context.Person.Where(m => m.Id == id).Include("Child").FirstOrDefault();

            tempPerson.Child = new Person
            {
                FirstName = child.FirstName,
                LastName = child.LastName,
                Age = child.Age
            };

            await _context.AddAsync(tempPerson.Child);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Update a person info. 

        [HttpPatch]
        [Route("UpdatePerson")]
        public async Task<ActionResult<Person>> UpdatePerson(int id, [FromBody] Person person)
        {
            var tempPerson = _context.Person.Where(m => m.Id == id).FirstOrDefault();

            tempPerson.FirstName = person.FirstName;
            tempPerson.LastName = person.LastName;
            tempPerson.Age = person.Age;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // Update child info. 

        [HttpPatch]
        [Route("UpdateChild")]
        public async Task<ActionResult<Person>> UpdateChild(int id, [FromBody] Person child)
        {
            var tempPerson = _context.Person.Where(m => m.Id == id).FirstOrDefault();

            if (tempPerson.Child != null)
            {
                tempPerson.FirstName = child.FirstName;
                tempPerson.LastName = child.LastName;
                tempPerson.Age = child.Age;
            }
                       
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Get a random number from HttpClient. It should be between 1 and 100. 

        public async Task<int> RandomNumber()
        {
            var client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync("https://www.random.org/integers/?num=1&min=1&max=100&col=1&base=10&format=plain&rnd=new");
            response.EnsureSuccessStatusCode();

            return int.Parse(await response.Content.ReadAsStringAsync());
        }

    }
}
