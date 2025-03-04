using FitnessApp.Data;
using FitnessApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return Ok(workout);
        }

        // Get all workouts for a specific user
        [HttpGet("GetWorkouts")]
        public async Task<IActionResult> GetWorkouts(string userId)
        {
            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .Include(w => w.Exercises)
                .ToListAsync();

            return Ok(workouts);
        }

        // Update a workout
        [HttpPut("UpdateWorkout/{id}")]
        public async Task<IActionResult> UpdateWorkout(int id, [FromBody] Workout updatedWorkout)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound(new { message = "Workout not found" });
            }

            // Update fields
            workout.Name = updatedWorkout.Name;
            workout.Date = updatedWorkout.Date;

            await _context.SaveChangesAsync();
            return Ok(workout);
        }

        // Delete a workout
        [HttpDelete("DeleteWorkout/{id}")]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            // Find the workout with its related exercises
            var workout = await _context.Workouts
                .Include(w => w.Exercises)  // Include related ExerciseEntries
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workout == null)
            {
                return NotFound(new { message = "Workout not found" });
            }

            // Manually delete the related ExerciseEntries
            _context.ExerciseEntries.RemoveRange(workout.Exercises);

            // Now delete the workout itself
            _context.Workouts.Remove(workout);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Workout and related exercise entries deleted successfully" });
        }

        [HttpPost("AddExercise/{workoutId}")]
        public async Task<IActionResult> AddExercise(int workoutId, [FromBody] ExerciseEntry exercise)
        {
            // Find the workout to which we want to add the exercise
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == workoutId);

            if (workout == null)
            {
                return NotFound(new { message = "Workout not found" });
            }

            // Add the exercise to the workout’s exercise list
            workout.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise added successfully", exercise });
        }

        [HttpPut("EditExercise/{exerciseId}")]
        public async Task<IActionResult> EditExercise(int exerciseId, [FromBody] ExerciseEntry updatedExercise)
        {
            // Find the exercise by its ID
            var exercise = await _context.ExerciseEntries
                .FirstOrDefaultAsync(e => e.Id == exerciseId);

            if (exercise == null)
            {
                return NotFound(new { message = "Exercise not found" });
            }

            // Update the exercise fields
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
            // Find the exercise to delete
            var exercise = await _context.ExerciseEntries
                .FirstOrDefaultAsync(e => e.Id == exerciseId);

            if (exercise == null)
            {
                return NotFound(new { message = "Exercise not found" });
            }

            _context.ExerciseEntries.Remove(exercise);  // Remove the exercise from the context
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise deleted successfully" });
        }

    }
}
