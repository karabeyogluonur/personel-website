namespace PW.Application.Models.Dtos.Identity
{
    public class SetPasswordDto
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
