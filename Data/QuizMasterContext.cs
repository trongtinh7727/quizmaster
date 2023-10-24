using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Models;
using System.Reflection.Emit;

namespace QuizMaster.Data;

public class QuizMasterContext : IdentityDbContext<QuizMasterUser>
{
    public DbSet<Quiz> Quizzes  { set; get; }
    public DbSet<QuizAnswer> Answers { set; get; }
    public DbSet<QuizQuestion> Questions { set; get; }
    public DbSet<TakeQuiz> TakeQuizzes { set; get; }
    public DbSet<TakeAnswer> TakeAnswers { set; get; }

    public QuizMasterContext(DbContextOptions<QuizMasterContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // moi quiz co 1 author, 1 author co the tao nhieu quiz
        modelBuilder.Entity<Quiz>()
            .HasOne(q=>q.Author)
            .WithMany(a => a.Quizzes)
            .HasForeignKey(q => q.AuthorId);

        // moi question nam trong 1 quiz, 1 quiz co nhieu cau hoi
        modelBuilder.Entity<QuizQuestion>()
            .HasOne(q=>q.Quiz)
            .WithMany(q => q.QuizQuestions)
            .HasForeignKey(q=> q.QuizId);

        // moi dap an ung voi 1 question, 1 question co nhieu dap an
        modelBuilder.Entity<QuizAnswer>()
            .HasOne(a=> a.Question)
            .WithMany(a=>a.Answers)
            .HasForeignKey(a=>a.QuestionId);

        // moi user co the lam nhieu quiz
        modelBuilder.Entity<TakeQuiz>()
            .HasOne(uq => uq.User)
            .WithMany(u => u.TakeQuizs)
            .HasForeignKey(uq => uq.UserId);
        // moi quiz co the duoc lam boi nhieu user
        modelBuilder.Entity<TakeQuiz>()
            .HasOne(uq => uq.Quiz)
            .WithMany(q => q.TakeQuizs)
            .HasForeignKey(uq => uq.QuizId)
            .OnDelete(DeleteBehavior.NoAction);

        // ....
        modelBuilder.Entity<TakeAnswer>()
            .HasOne(ta => ta.TakeQuiz)
            .WithMany(t => t.TakeAnswers)
            .HasForeignKey(ta => ta.TakeId);

        modelBuilder.Entity<TakeAnswer>()
            .HasOne(ta => ta.Answer)
            .WithMany(a => a.TakeAnswers)
            .HasForeignKey(ta => ta.AnswerId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<TakeAnswer>()
            .HasOne(ta => ta.QuizQuestion)
            .WithMany(a => a.TakeAnswers)
            .HasForeignKey(ta => ta.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(modelBuilder);
    }
}
