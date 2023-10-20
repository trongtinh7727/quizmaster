using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace QuizMaster.Models;

// Add profile data for application users by adding properties to the QuizMasterUser class
public class QuizMasterUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }

    public ICollection<Quiz> Quizzes { get; set; }

    public ICollection<TakeQuiz> TakeQuizs { get; set; }


}

