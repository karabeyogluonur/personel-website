namespace PW.Application.Models.Dtos.Identity
{
    public class UserRoleAssignmentDto
    {
        public int UserId { get; set; }
        public List<string> RoleNames { get; set; } = new();
    }
}
