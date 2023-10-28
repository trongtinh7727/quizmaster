using System.ComponentModel.DataAnnotations;
namespace QuizMaster.Models
{
    public class TakeQuiz
    {
        [Key]
        public int Id { get; set; }


        public int Score { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        // Foreign key to identify the user taking the quiz
        public string? UserId { get; set; }

        // Foreign key to identify the associated quiz
        public int? QuizId { get; set; }
        // Navigation property to represent the user taking the quiz
        public QuizMasterUser? User { get; set; }

        // Navigation property to represent the associated quiz
        public Quiz? Quiz { get; set; }

        public ICollection<TakeAnswer> TakeAnswers { get; set; } = new List<TakeAnswer>();

    }
}
