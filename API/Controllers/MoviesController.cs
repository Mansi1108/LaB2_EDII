using System;
using API.Helpers;
using API.Models;
using System.IO;
using System.Text;
using System.Text.Json;
using CustomGenerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Server.HttpSys;
using System.Runtime.InteropServices.WindowsRuntime;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    
    public class MoviesController : ControllerBase
    {
        private IWebHostEnvironment Environment;

        public MoviesController(IWebHostEnvironment env)
        {
            Environment = env;
        }

        // GET: api/<MoviesController>
        [HttpGet]
        public List<Movies> GetMovies()
        {
            return new List<Movies>();
        }

        enum Pathings
        {
            preorden,
            inorden,
            postorden
        }

        [HttpGet]
        [Route("{traversal}")]
        public List<Movies> GetMovies(string traversal)
        {
            switch (traversal)
            {
                case "preorden":
                    return Storage.Instance.MoviesTree.Pathing(1);
                case "inorden":
                    return Storage.Instance.MoviesTree.Pathing(2);
                case "postorden":
                    return Storage.Instance.MoviesTree.Pathing(3);
            }
            return new List<Movies>();
        }

        // POST api/<MoviesController>
        [HttpPost]
        public IActionResult PostTreeOrder([FromBody] string order)
        {
            try
            {
                Movies movie = new Movies();
                Storage.Instance.MoviesTree = new BTree<Movies>(Environment.ContentRootPath , Convert.ToInt32(order));
                if (Storage.Instance.MoviesTree.TreeOrder != Convert.ToInt32(order))
                {
                    return StatusCode(500);
                }
                else
                {
                    return Ok();
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // ELIMINAR ÁRBOL
        [HttpDelete]
        public IActionResult DeleteTree()
        {
            if (System.IO.File.Exists($"{Environment.ContentRootPath}/BTree.txt"))
            {
                System.IO.File.Delete($"{Environment.ContentRootPath}/BTree.txt");
                return Ok();
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("populate")]
        public IActionResult PostMovies([FromForm] IFormFile file)
        {
            using var content = new MemoryStream();
            try
            {
                file.CopyToAsync(content);
                var text = Encoding.ASCII.GetString(content.ToArray());
                var Movies = JsonSerializer.Deserialize<List<Movies>>(text, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                foreach (var movie in Movies)
                {
                    movie.SetID();
                    Storage.Instance.MoviesTree.AddValue(movie);
                }
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // DELETE api/<MoviesController>/5
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteValue(string id)
        {
            //manejar todas las funciones como tipo bool, así podemos diferencias entre cuál debería ser el resultado.
            try
            {
                if (Storage.Instance.MoviesTree.DeleteValue(new Movies() { ID = id }))//Aquí debemos ejecutar la función del árbol y dependiendo del resultado ya devolver not found u ok;
                {
                    return Ok();
                }
                return NotFound();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
