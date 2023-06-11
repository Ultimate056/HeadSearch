using webapi.Database;
using webapi.Models.Tests;

namespace TestsApi.Models.ViewModels
{
    public class TestViewModel
    {
        public Test Test { get; set; }

        public List<TestQuestion> Questions { get; set; }

        public List<Answer> AllAnswers { get; set; }

        public TestViewModel(ApplicationContext db, Test test)
        {
            Questions = new List<TestQuestion>();
            AllAnswers = new List<Answer>();
            Test = test;
            Questions.AddRange(db.TestQuestions.Where(u => u.TestId == test.Id));
            foreach (var q in Questions)
            {
                AllAnswers?.AddRange(db.Answers.Where(u => u.QuestionId == q.Id));
            }
        }

    }
}
