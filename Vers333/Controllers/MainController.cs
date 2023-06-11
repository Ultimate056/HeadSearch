using Aspose.Words;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Principal;
using TestsApi.Models;
using Vers333.Models;
using Vers333.Models.ViewModels;
using webapi.Database;
using webapi.Models.Anketa;
using webapi.Models.Persons;
using webapi.Models.Tests;

namespace TestsApi.Controllers
{
    public class MainController : Controller
    {
        public static int IdUser = -1;
        private ApplicationContext db;
        Position[] poses;
        public MainController(ApplicationContext db)
        {
            this.db = db;
            poses = db.Positions.Where(x => x.fPoisk == 1).ToArray();
        }

        [HttpGet]
        [Route("/")]
        public IActionResult EntryInApp()
        {
            return IdUser == -1 ? View("Login") : View("Main", poses);
        }


        [HttpPost]
        public async Task<IActionResult> SignIn()
        {
            var email = HttpContext.Request.Form["Login"].ToString();
            var password = HttpContext.Request.Form["Password"].ToString();

            var user = db.Users.FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user is null)
            {
                Error err = new Error("Внимание", "EntryInApp", "Main", "Пользователя с таким email или паролем не существует");
                return View("errorModel", err);
            }
            // аутенфикация

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Id.ToString()) };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            IdUser = user.Id;

            return View("Main", poses);
        }


        [HttpPost]
        public async Task<IActionResult> SignInAdmin()
        {
            var email = HttpContext.Request.Form["Login"].ToString();
            var password = HttpContext.Request.Form["Password"].ToString();

            var user = db.Admins.FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user is null)
            {
                Error err = new Error("Внимание", "Admin", "Main", "Пользователя с таким email или паролем не существует");
                return View("errorModel", err);
            }
            // аутенфикация

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Id.ToString()) };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            IdUser = user.Id;

            List<ResultsViewModel> vm = new List<ResultsViewModel>();

            var results = db.Results.ToList();
            var users = db.Users.ToList();
            var tests = db.Tests.ToList();

            foreach(Result result in results)
            {
                if (result.Status != "Completed")
                    continue;

                Test? test = tests.Where(x => x.Id == result.TestId).FirstOrDefault();
                User? userWhoDo = users.Where(x => x.Id == result.UserId).FirstOrDefault();
                if(test != null && userWhoDo != null)
                {
                    vm.Add(new ResultsViewModel
                    {
                        IdResult = result.Id,
                        nameTest = test.Name,
                        ContentResult = result.ResultContent,
                        FullName = userWhoDo.SecondName + " " + userWhoDo.FirstName + " " + userWhoDo.ThirdName,
                        DateEnd = result.ClosedTestTime,
                        DateStart = result.OpenTestTime,
                        PhoneNumber = userWhoDo.PhoneNumber,
                        PlaceOfLife = userWhoDo.PlaceOfLife,
                        IdUser = userWhoDo.Id
                    });
                }
            }


            return View("ViewResults", vm);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            IdUser = -1;
            return Ok(true);
        }


        [HttpGet]
        //[Route("/registration")]
        public IActionResult Registration()
        {
            return View("Registration");
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpGet("/admin")]
        public IActionResult Admin()
        {
            return View("LoginAdmin");
        }

        [HttpGet]
        public IActionResult Main()
        {
            return View("Main", poses);
        }


        [HttpPost]
        public async Task<IActionResult> AddUser(UserViewModel newUser)
        {

            if (ModelState.IsValid)
            {
                User newUs = new User
                {
                    Email = newUser.Email,
                    Password = newUser.Password,
                    PhoneNumber = newUser.PhoneNumber,
                    FirstName = newUser.FirstName,
                    SecondName = newUser.SecondName,
                    ThirdName = newUser.ThirdName,
                    PlaceOfLife = newUser.PlaceOfLife
                };
                db.Users.Add(newUs);
                await db.SaveChangesAsync();
                return View("Login");
            }
            else
            {
                Error err = new Error("Внимание", "Registration", "Main", "Вы не прошли валидацию! Повторите еще раз");
                return View("errorModel", err);
            }
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckEmail(string email)
        {
            if (db.Users.Any(x => x.Email == email))
            {
                return Json(false);
            }
            return Json(true);
        }


        [HttpPost]
        public IActionResult GetDataUseFilter()
        {
            int testNeed = Convert.ToInt32(HttpContext.Request.Form["FilterTest"]);
            string SearchContentFIO = HttpContext.Request.Form["SearchUser"].ToString();



            List<Result> results = new List<Result>(); 
            switch(testNeed)
            {
                case 0:
                    results = db.Results.ToList();
                    break;
                case 1:
                    results = db.Results.Where(x => x.TestId == 1).ToList();
                    break;
                case 2:
                    results = db.Results.Where(x => x.TestId == 2).ToList();
                    break;
            }

            List<ResultsViewModel> vm = new List<ResultsViewModel>();


            List<User> users = new List<User>();
            if(SearchContentFIO != null && SearchContentFIO.Trim() != "")
            {
                foreach (User user in db.Users)
                {
                    string fName = user.SecondName + " " + user.FirstName + " " + user.ThirdName;
                    if (fName.Contains(SearchContentFIO))
                    {
                        users.Add(user);
                    }
                }
            }
            else
            {
                users = db.Users.ToList();
            }

            var tests = db.Tests.ToList();

            foreach (Result result in results)
            {
                if (result.Status != "Completed")
                    continue;

                Test? test = tests.Where(x => x.Id == result.TestId).FirstOrDefault();
                User? userWhoDo = users.Where(x => x.Id == result.UserId).FirstOrDefault();
                if (test != null && userWhoDo != null)
                {
                    vm.Add(new ResultsViewModel
                    {
                        IdResult = result.Id,
                        nameTest = test.Name,
                        ContentResult = result.ResultContent,
                        FullName = userWhoDo.SecondName + " " + userWhoDo.FirstName + " " + userWhoDo.ThirdName,
                        DateEnd = result.ClosedTestTime,
                        DateStart = result.OpenTestTime,
                        PhoneNumber = userWhoDo.PhoneNumber,
                        PlaceOfLife = userWhoDo.PlaceOfLife,
                        IdUser = userWhoDo.Id
                    });
                }
            }


            return View("ViewResults", vm);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult GenerateWord(int idUser)
        {
            User? user = db.Users.Where(x=> x.Id == idUser).FirstOrDefault();

            if(user != null)
            {
                var answers = db.UserAnswersForAnketa.Where(x => x.UserId == idUser).ToList();
                var answersWithQuest = answers.Join(
                    db.AnketaQuestions, x => x.QuestionAnketaId, y => y.Id, (x, y) => new { Vopros = y.Content, Otvet = x.Content }
                    ).ToList();

                var fistQuest = db.AnketaQuestions.Where(x => x.Id == answers[0].QuestionAnketaId).First();

                var namePosition = db.Anketas.Where(x => x.Id == fistQuest.AnketaId).Join(db.Positions,
                    x => x.PositionId, y => y.Id, (x, y) => y.NamePosition).First();


                var resMBTI = db.Results.Where(x => x.UserId == user.Id && x.TestId == 1 && x.Status == "Completed")
                    .Select(x => x.ResultContent).First();

                var resPAIE = db.Results.Where(x => x.UserId == user.Id && x.TestId == 2 && x.Status == "Completed")
                    .Select(x => x.ResultContent).First();

                Document doc = new Document();
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.Font.Size = 14;
                
                builder.Writeln("Характеристика");
                builder.Writeln($"Кандидат: {user.SecondName} {user.FirstName} {user.ThirdName}");
                builder.InsertHorizontalRule();
                builder.Writeln();
                builder.Writeln("Основные данные");
                builder.Writeln("Место проживания: " + user.PlaceOfLife);
                builder.Writeln("Контактный телефон: " + user.PhoneNumber);
                builder.Writeln("Электронная почта: " + user.Email);
                builder.Writeln();
                builder.Writeln($"Ответы на анкету для позиции {namePosition}");
                builder.Writeln();
                foreach (var p in answersWithQuest)
                {
                    builder.Writeln(p.Vopros + ": ");
                    builder.Writeln();
                    builder.Writeln("- " + p.Otvet);
                    builder.Writeln();
                    builder.InsertHorizontalRule();
                }

                builder.Writeln($"Результат MBTI теста: {resMBTI}");
                builder.Writeln();
                builder.Writeln($"Результат PAIE теста: {resPAIE}");
                builder.Writeln();
                builder.Writeln($"Желаемая должность: {namePosition}");

                string uniqPath = user.SecondName + "_" + user.FirstName + "_" + user.PlaceOfLife;
                string path = $"C:\\Users\\79204\\Desktop\\Frontend\\{uniqPath}.docx";
                doc.Save(path);
                return Json(true);
            }
            return Json(true);
        }
        //[AcceptVerbs("Get", "Post")]
        //public IActionResult OpenUserAnketa(int idUser)
        //{
        //    AnketaQuestion[] ankquest = db.AnketaQuestions.Where(x => x.AnketaId == AnketaId).ToArray();

        //    return View("FillAnketaForm", ankquest);
        //}
        
    }
}
