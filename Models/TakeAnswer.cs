using System.ComponentModel.DataAnnotations;
namespace QuizMaster.Models
{
    public class TakeAnswer
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Content { get; set; }

        // Navigation property to represent the quiz attempt (take)
        // Foreign key to identify the quiz attempt (take)
        public int TakeId { get; set; }

        // Foreign key to identify the quiz answer
        public int AnswerId { get; set; }

        public int QuestionId { get; set; }
        public TakeQuiz TakeQuiz { get; set; }

        // Navigation property to represent the associated quiz answer
        public QuizAnswer Answer { get; set; }

        public QuizQuestion QuizQuestion { get; set; }
    }
}
