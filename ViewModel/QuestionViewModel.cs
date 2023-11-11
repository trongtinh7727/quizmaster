namespace QuizMaster.ViewModel
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
        public int Score { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
