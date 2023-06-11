using Microsoft.AspNetCore.Mvc;
using TestsApi.Models.ViewModels;
using TestsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Cors;
using webapi.Database;
using webapi.Models.Tests;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;
using Vers333.Models.ViewModels;

namespace TestsApi.Controllers
{
    //[Authorize]
    public class TestController : Controller
    {
        public static int PositionId = -1;
        private ApplicationContext db;

        public TestController(ApplicationContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult tests()
        {
            return Redirect("/test/GetTest/1");
        }



        [HttpGet]
        public IActionResult GetTest(int id)
        {
            // Находим тест из БД
            var test = db.Tests.FirstOrDefault(u => u.Id == id);


            if (test == null) return NotFound(new { message = "Тест не найден" });

            // Инициализируем вьюмодель
            TestViewModel vm = new TestViewModel(db, test);

            // Отправляем вьюмодель теста(вопросы, ответы) в виде json

            return View("MBTI", vm);
        }

        [HttpGet]
        public IActionResult GetDataTest(int id)
        {
            // Находим тест из БД
            var test = db.Tests.FirstOrDefault(u => u.Id == id);


            if (test == null) return NotFound(new { message = "Тест не найден" });

            // Инициализируем вьюмодель
            TestViewModel vm = new TestViewModel(db, test);

            // Отправляем вьюмодель теста(вопросы, ответы) в виде json

            return Json(vm);
        }


        [HttpGet]
        public IActionResult InitResultMbti(int testId)
        {
            Result newResult = new Result {
                OpenTestTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 
                Status = "Started", 
                TestId = testId,
                UserId = MainController.IdUser
            };

            db.Results.Add(newResult);
            db.SaveChanges();
            
            int newId = db.Results.OrderBy(x => x.Id).Max(x => x.Id);
            return Json(new { id = newId });
        }

        [HttpPost("BreakMbtiInit")]
        public async Task<IActionResult> BreakMbtiInit(int resultId)
        {
            Result? resDB = db.Results.FirstOrDefault(x => x.Id == resultId);

            bool fFindedRes = false;
            if(resDB !=null)
            {
                resDB.ClosedTestTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                resDB.Status = "Cancelled";
                fFindedRes = true;
                await db.SaveChangesAsync();
            }
            return Ok(fFindedRes);
        }




        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> TestMBTI([FromBody] Answer[] answers, [FromQuery] int idResult)
        {
            int IntrovertCount = 0; // I
            int ExtravertCount = 0; // E
            int IntuitCount = 0; //N 
            int SensoricCount = 0; //S 
            int FellCount = 0; // F
            int BrainCount = 0; // T
            int GibkostCount = 0; // P
            int SuzhdenieCount = 0; // J

            foreach (Answer a in answers)
            {
                switch (a.Scale)
                {
                    case "I":
                        IntrovertCount++;
                        break;
                    case "E":
                        ExtravertCount++;
                        break;
                    case "N":
                        IntuitCount++;
                        break;
                    case "S":
                        SensoricCount++;
                        break;
                    case "F":
                        FellCount++;
                        break;
                    case "T":
                        BrainCount++;
                        break;
                    case "P":
                        GibkostCount++;
                        break;
                    case "J":
                        SuzhdenieCount++;
                        break;
                }
            }

            string FirstPair = IntrovertCount >= ExtravertCount ? "I" : "E";
            string SecondPair = IntuitCount >= SensoricCount ? "N" : "S";
            string ThirdPair = FellCount >= BrainCount ? "F" : "T";
            string FouthPair = GibkostCount >= SuzhdenieCount ? "P" : "J";

            string res = FirstPair + SecondPair + ThirdPair + FouthPair;


            Result? resDB = db.Results.FirstOrDefault(x => x.Id == idResult);

            if (resDB == null)
                return NotFound();

            resDB.ResultContent = res;
            resDB.ClosedTestTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            resDB.Status = "Completed";
            resDB.TestId = 1;

            await db.SaveChangesAsync();

            return Ok(true);
        }



        // Тест PAIE

        [HttpPost]
        public IActionResult paie() 
        {
            // Находим тест из БД
            var test = db.Tests.FirstOrDefault(u => u.Id == 2);

            if (test == null) return NotFound(new { message = "Тест не найден" });

            // Инициализируем вьюмодель
            TestViewModel vm = new TestViewModel(db, test);

            // Отправляем вьюмодель теста(вопросы, ответы) в виде json

            return View("PAIE", vm);
        }

        [HttpGet]
        public IActionResult GetPaieTest()
        {
            // Находим тест из БД
            var test = db.Tests.FirstOrDefault(u => u.Id == 2);

            if (test == null) return NotFound(new { message = "Тест не найден" });

            // Инициализируем вьюмодель
            TestViewModel vm = new TestViewModel(db, test);

            // Отправляем вьюмодель теста(вопросы, ответы) в виде json

            return View("PAIE", vm);
        }

        [HttpPost]
        public async Task<IActionResult> FinalPaie([FromBody] int[] scores, [FromQuery] int idResult)
        {
            try
            {
                var reses = db.Results;
                string res = "";
                if (scores.Length == 4)
                {
                    res += scores[0] > 26 ? "P" : scores[0] < 20 ? "-" : "p";
                    res += scores[1] > 26 ? "A" : scores[1] < 20 ? "-" : "a";
                    res += scores[2] > 26 ? "E" : scores[2] < 20 ? "-" : "e";
                    res += scores[3] > 26 ? "I" : scores[3] < 20 ? "-" : "i";
                }

                Result? resDB = db.Results.FirstOrDefault(x => x.Id == idResult);

                if (resDB == null)
                    return NotFound();

                resDB.ResultContent = res;
                resDB.ClosedTestTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                resDB.Status = "Completed";
                resDB.TestId = 2;

                await db.SaveChangesAsync();

                char[] pok = res.ToArray();
                var pokes = db.ScalesPAIE;

                List<string> opisi = new List<string>();

                foreach (char p in pok)
                {
                    string? opis = pokes.Where(x => x.ResScale == p.ToString()).Select(x => x.Description).First();
                    if (opis != null)
                    {
                        opisi.Add(opis);
                    }
                }

                Result? resultMBTI = reses.Where(x => x.TestId == 1 && x.UserId == MainController.IdUser).OrderBy(x=> x.Id)
                    .LastOrDefault();


                string? resMBTI = "";
                if (resultMBTI != null)
                {
                    resMBTI = resultMBTI.ResultContent;
                }

                string urlMBTI = "/";
                if (resMBTI != null)
                {
                    switch (resMBTI)
                    {
                        case "ISTJ":
                            urlMBTI = "https://my-type.ru/istj";
                            break;
                        case "ISTP":
                            urlMBTI = "https://my-type.ru/istp";
                            break;
                        case "ESTP":
                            urlMBTI = "https://my-type.ru/estp";
                            break;
                        case "ESTJ":
                            urlMBTI = "https://my-type.ru/estj";
                            break;
                        case "ISFP":
                            urlMBTI = "https://my-type.ru/isfp";
                            break;
                        case "ISFJ":
                            urlMBTI = "https://my-type.ru/isfj";
                            break;
                        case "ESFP":
                            urlMBTI = "https://my-type.ru/esfp";
                            break;
                        case "ESFJ":
                            urlMBTI = "https://my-type.ru/esfj";
                            break;
                        case "INTP":
                            urlMBTI = "https://my-type.ru/intp";
                            break;
                        case "INTJ":
                            urlMBTI = "https://my-type.ru/intj";
                            break;
                        case "ENTP":
                            urlMBTI = "https://my-type.ru/entp";
                            break;
                        case "ENTJ":
                            urlMBTI = "https://my-type.ru/entj";
                            break;
                        case "ENFJ":
                            urlMBTI = "https://my-type.ru/enfj";
                            break;
                        case "INFJ":
                            urlMBTI = "https://my-type.ru/infj";
                            break;
                        case "ENFP":
                            urlMBTI = "https://my-type.ru/enfp";
                            break;
                        case "INFP":
                            urlMBTI = "https://my-type.ru/infp";
                            break;
                    }
                }

                UserAnswersData data = new UserAnswersData()
                {
                    UrlMBTI = urlMBTI,
                    PAIEResults = opisi.ToArray()
                };
                dGlobal.PAIEResults = data.PAIEResults;
                dGlobal.UrlMBTI = data.UrlMBTI;

                return Ok(true);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static UserAnswersData dGlobal = new UserAnswersData();

        [HttpPost]
        public IActionResult GetViewResults()
        {
            return View("UserResults", dGlobal);
        }
    }
}
