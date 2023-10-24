using System.ComponentModel.DataAnnotations;
namespace QuizMaster.Models
{
    public class QuizAnswer
    {
        [Key]
        public int Id { get; set; }


        public bool Correct { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Content { get; set; }

        // Foreign key to identify the parent question
        public int QuestionId { get; set; }
        public QuizQuestion? Question { get; set; }

        public ICollection<TakeAnswer> TakeAnswers { get; set; } = new List<TakeAnswer>();
    }
}
