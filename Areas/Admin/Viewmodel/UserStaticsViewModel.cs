using QuizMaster.Models;

namespace QuizMaster.Areas.Admin.Viewmodel
{
    public class UserStaticsViewModel
    {
        public int NewAccountsIn7Days { get; set; }
        public int ActiveAccounts { get; set; }
        public int LockedAccounts { get; set; }
        public List<int> AccountCountsByMonth { get; set; }
    }
}
