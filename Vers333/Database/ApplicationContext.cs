using Microsoft.EntityFrameworkCore;
using Vers333.Models.Tests;
using webapi.Models.Anketa;
using webapi.Models.Persons;
using webapi.Models.Tests;

namespace webapi.Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Anketa> Anketas { get; set; }
        public DbSet<AnketaQuestion> AnketaQuestions { get; set; }
        public DbSet<UserAnswerForAnketa> UserAnswersForAnketa { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<ScalePAIE> ScalesPAIE { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) {
            
        }
        
    }
}
