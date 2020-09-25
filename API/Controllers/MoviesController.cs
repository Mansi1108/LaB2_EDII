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

        [HttpGet]
        [Route("{traversal}")]
        public List<Movies> GetMovies(string traversal)
        {
            switch (traversal)
            {
                case "preorden":
                    //return Storage.Instance.MoviesTree.GetPathing(0);
                case "inorden":
                    //return Storage.Instance.MoviesTree.GetPathing(1);
                case "postorden":
                    //return Storage.Instance.MoviesTree.GetPathing(2);
                    return new List<Movies>();
            }
            return new List<Movies>();
        }

        // POST api/<MoviesController>
        [HttpPost]
        public IActionResult PostTreeOrder([FromForm] IFormFile file)
        {
            using var content = new MemoryStream();
            try
            {
                file.CopyToAsync(content); 
                var text = Encoding.ASCII.GetString(content.ToArray());
                var order = JsonSerializer.Deserialize<int>(text);
                Storage.Instance.MoviesTree = new BTree<Movies>(Environment.ContentRootPath , order);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // ELIMINAR ÁRBOL
        [HttpDelete]
        public void DeleteTree()
        {
            //Borrar el archivo donde tenemos el árbol environment/Btree.txt
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
                if (true)//Aquí debemos ejecutar la función del árbol y dependiendo del resultado ya devolver not found u ok;
                {
                    return NotFound();
                }
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
