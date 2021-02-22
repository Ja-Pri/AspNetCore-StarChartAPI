using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route(""), ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            celestialObject.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(n => n.Name == name);
            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            foreach(var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestial = _context.CelestialObjects.Find(id);
            if (celestial == null)
            {
                return NotFound();
            }
            celestial.Name = celestialObject.Name;
            celestial.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestial.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.Update(celestial);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestial = _context.CelestialObjects.Find(id);
            if (celestial == null)
            {
                return NotFound();
            }
            celestial.Name = name;

            _context.Update(celestial);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestial = _context.CelestialObjects.Find(id);
            if (celestial == null)
            {
                return NotFound();
            }
            _context.RemoveRange(celestial);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
