using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestsApi.Models;
using webapi.Database;
using webapi.Models.Tests;

namespace TestsApi.Controllers
{
    //[Authorize]
    public class ResultsController : Controller
    {
        private ApplicationContext db;

        public ResultsController(ApplicationContext db)
        {
            this.db = db;
        }

        // Список результатов
        [HttpGet]
        public async Task<IList<Result>> GetResults()
        {
            return await db.Results.ToListAsync();
        }

        // Возвращаем один результат
        [HttpGet("id:int")]
        public async Task<IActionResult> GetResult(int id)
        {
            Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == id);

            if (result == null) return NotFound(new { message = "Результат не найден" });

            return Json(result);
        }

        // Удаление результата
        [HttpDelete("id:int")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == id);
            if (result == null) return NotFound(new { message = "Результат не найден" });

            db.Results.Remove(result);
            await db.SaveChangesAsync();
            return Json(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateResult(Result result)
        {
            if (result == null) return NotFound(new { message = "нечего обновлять" });

            Result? original = db.Results.Where(x=> x.Id == result.Id).FirstOrDefault();
            if (original == null) return NotFound(new { message = "нечего обновлять" });

            original.Status = result.Status;
            original.ResultContent = result.ResultContent;

            db.Results.Update(original);
            await db.SaveChangesAsync();

            return Ok(true);
        }

    }
}
