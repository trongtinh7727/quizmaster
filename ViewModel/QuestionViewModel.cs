namespace QuizMaster.ViewModel
{
    public class QuestionViewModel
    {
        public string QuestionText { get; set; }
        public List<string> Answers { get; set; }
        public int Score { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
