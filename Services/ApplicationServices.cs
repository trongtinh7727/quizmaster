using System;
using System.Security.Cryptography;
using System.Text;

namespace QuizMaster.Services
{
    public class ApplicationServices
    {
        private static readonly string AllowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public static string GenerateEnrollCode(string id,int length = 8)
        {
           
                Random random = new Random();

                StringBuilder code = new StringBuilder(length);

                for (int i = 0; i < length; i++)
                {
                    int randomNumber = random.Next(AllowedCharacters.Length);
                    char randomChar = AllowedCharacters[randomNumber];
                    code.Append(randomChar);
                }

                return code.ToString()+id;
            
        }
    }
}
