using QuizMaster.Models;

namespace QuizMaster.ViewModel
{
    public class TakeQuizViewModel
    {
        public DateTime? StartedAt { get; set; }
        public int? QuizID { get; set; }
        public Quiz? Quiz { get; set; }
        public IDictionary<int, int> Answers { get; set; } = new Dictionary<int, int>();

    }
}
