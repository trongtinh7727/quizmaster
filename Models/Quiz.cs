using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizMaster.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }


        public string? Title { get; set; }

        public string? Summary { get; set; }
        public string? Tag { get; set; }

        public int Score { get; set; }

        public bool Published { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime PublishedAt { get; set; }

        // Foreign key to identify the quiz host
        public string? AuthorId { get; set; }

        // Navigation property to represent the quiz host (Author)
        public QuizMasterUser? Author { get; set; }

        public ICollection<TakeQuiz> TakeQuizs { get; set; } = new List<TakeQuiz>();
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();

    }
}
