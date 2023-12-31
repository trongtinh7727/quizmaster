﻿using QuizMaster.Models;

namespace QuizMaster.ViewModel
{
    public class QuizViewModel
    {

        public int? Id { get; set; }
        public string QuizTitle { get; set; }

        public string QuizSummary { get; set; }
        public string? EnrollCode { get; set; }
        public string QuizTag { get; set; }
        public bool QuizPublished { get; set; }
        public int QuizLevel { get; set; }
        public Quiz? quiz { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
    }
}