using System.ComponentModel.DataAnnotations;
namespace QuizMaster.Models
{
    public class QuizQuestion
    {
        [Key]
        public int Id { get; set; }



        public int Score { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Content { get; set; }

        // Navigation property to represent the parent quiz
        // Foreign key to identify the parent test/quiz
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }

        public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
        public ICollection<TakeAnswer> TakeAnswers { get; set; } = new List<TakeAnswer>();
    }
}
