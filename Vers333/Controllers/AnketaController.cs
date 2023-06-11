using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using TestsApi.Controllers;
using Vers333.Models.ViewModels;
using webapi.Database;
using webapi.Models.Anketa;

namespace Vers333.Controllers
{
    public class AnketaController : Controller
    {
        private ApplicationContext db;

        public AnketaController(ApplicationContext db)
        {
            this.db = db;
        }
        [HttpPost]
        public IActionResult OpenAnketa()
        {
            var PositionId = HttpContext.Request.Form["position"].ToString();
            int posId = int.Parse(PositionId);
            TestController.PositionId = int.Parse(PositionId);

            Anketa ank = db.Anketas.Where(x => x.PositionId == posId).FirstOrDefault();

            if (ank == null)
                return NotFound("Анкеты и тестов для этой должности нет");

            int AnketaId = ank.Id;


            AnketaQuestion[] ankquest = db.AnketaQuestions.Where(x => x.AnketaId == AnketaId).ToArray();

            return View("FillAnketaForm", ankquest);
        }

        [HttpPost]
        public async Task<IActionResult> AddAnswers([FromBody] AnAnswersViewModel[] answers)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                db.UserAnswersForAnketa.Add(new UserAnswerForAnketa
                {
                    QuestionAnketaId = answers[i].Id,
                    Content = answers[i].Content,
                    UserId = MainController.IdUser
                });
            }
            await db.SaveChangesAsync();
            return Ok();
        }


    }
}
