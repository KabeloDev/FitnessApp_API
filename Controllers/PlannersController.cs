using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlannersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlannersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Planners/GetPlannersByUserId/{userId}
        [HttpGet("GetPlannersByUserId/{userId}")]
        public async Task<ActionResult<List<Planner>>> GetPlannersByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var planners = await _context.Planners
                                         .Where(p => p.UserId == userId)
                                         .OrderBy(p => p.Date)
                                         .ToListAsync();

            return Ok(planners);
        }

        // POST: api/Planners/PostPlanner
        [HttpPost("PostPlanner")]
        public async Task<ActionResult<Planner>> PostPlanner(Planner planner)
        {
            if (planner == null || string.IsNullOrWhiteSpace(planner.UserId))
            {
                return BadRequest("Invalid planner data.");
            }

            _context.Planners.Add(planner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlannersByUserId), new { userId = planner.UserId }, planner);
        }

        // PUT: api/Planners/PutPlanner/{id}
        [HttpPut("PutPlanner/{id}")]
        public async Task<IActionResult> PutPlanner(int id, Planner planner)
        {
            if (id != planner.Id)
            {
                return BadRequest("Planner ID mismatch.");
            }

            var existingPlanner = await _context.Planners.FindAsync(id);
            if (existingPlanner == null)
            {
                return NotFound("Planner not found.");
            }

            if (existingPlanner.UserId != planner.UserId)
            {
                return Unauthorized("You can only edit your own planners.");
            }

            existingPlanner.Title = planner.Title;
            existingPlanner.Description = planner.Description;
            existingPlanner.Date = planner.Date;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlannerExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Planners/DeletePlanner/{id}
        [HttpDelete("DeletePlanner/{id}")]
        public async Task<IActionResult> DeletePlanner(int id)
        {
            var planner = await _context.Planners.FindAsync(id);
            if (planner == null)
            {
                return NotFound("Planner not found.");
            }

            _context.Planners.Remove(planner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlannerExists(int id)
        {
            return _context.Planners.Any(e => e.Id == id);
        }
    }
}
