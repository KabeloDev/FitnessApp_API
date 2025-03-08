using FitnessApp.Data;
using FitnessApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FitnessApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkoutsController(AppDbContext context)
        {
            _context = context;
        }

        // Create a new workout
        [HttpPost("CreateWorkout")]
        public async Task<IActionResult> CreateWorkout([FromBody] Workout workout)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return Ok(workout);
        }

        // Get all workouts for a specific user
        [HttpGet("GetWorkouts/{userId}")]
        public async Task<IActionResult> GetWorkouts(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .Include(w => w.Exercises)
                .AsNoTracking()
                .ToListAsync();

            if (!workouts.Any())
            {
                return NotFound("No workouts found for this user.");
            }

            return Ok(workouts);
        }

        // Update a workout
        [HttpPut("UpdateWorkout/{id}")]
        public async Task<IActionResult> UpdateWorkout(int id, [FromBody] Workout updatedWorkout)
        {
            if (updatedWorkout == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid workout data" });
            }

            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound(new { message = "Workout not found" });
            }

            workout.Name = updatedWorkout.Name;
            //workout.Date = updatedWorkout.Date;
            workout.Status = updatedWorkout.Status;

            await _context.SaveChangesAsync();
            return Ok(workout);
        }

        // Delete a workout
        [HttpDelete("DeleteWorkout/{id}")]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            var workout = await _context.Workouts
                .Include(w => w.Exercises)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workout == null)
            {
                return NotFound(new { message = "Workout not found" });
            }

            _context.ExerciseEntries.RemoveRange(workout.Exercises);
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Workout and related exercises deleted successfully" });
        }

        [HttpPost("AddExercise/{workoutId}")]
        public async Task<IActionResult> AddExercise(int workoutId, [FromBody] ExerciseEntry exercise)
        {
            if (exercise == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid exercise data" });
            }

            var workout = await _context.Workouts.Include(w => w.Exercises).FirstOrDefaultAsync(w => w.Id == workoutId);
            if (workout == null)
            {
                return NotFound(new { message = "Workout not found" });
            }

            workout.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise added successfully", exercise });
        }

        [HttpGet("GetExercise/{exerciseId}")]
        public async Task<IActionResult> GetExercise(int exerciseId)
        {
            var exercise = await _context.ExerciseEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == exerciseId);

            if (exercise == null)
            {
                return NotFound(new { message = "Exercise not found" });
            }

            return Ok(exercise);
        }


        [HttpPut("EditExercise/{exerciseId}")]
        public async Task<IActionResult> EditExercise(int exerciseId, [FromBody] ExerciseEntry updatedExercise)
        {
            if (updatedExercise == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid exercise data" });
            }

            var exercise = await _context.ExerciseEntries.FirstOrDefaultAsync(e => e.Id == exerciseId);
            if (exercise == null)
            {
                return NotFound(new { message = "Exercise not found" });
            }

            exercise.Name = updatedExercise.Name;
            exercise.Sets = updatedExercise.Sets;
            exercise.Reps = updatedExercise.Reps;
            exercise.Time = updatedExercise.Time;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Exercise updated successfully", exercise });
        }

        [HttpDelete("DeleteExercise/{exerciseId}")]
        public async Task<IActionResult> DeleteExercise(int exerciseId)
        {
            var exercise = await _context.ExerciseEntries.FindAsync(exerciseId);
            if (exercise == null)
            {
                return NotFound(new { message = "Exercise not found" });
            }

            _context.ExerciseEntries.Remove(exercise);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise deleted successfully" });
        }
    }
}
