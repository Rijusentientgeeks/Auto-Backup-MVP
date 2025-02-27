using System.ComponentModel.DataAnnotations;

namespace GeekathonAutoSync.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}