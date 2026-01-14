namespace PW.Application.Interfaces.Identity.Dtos;

public class IdentityUserDto
{
   public int Id { get; set; }
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string Email { get; set; } = string.Empty;
   public List<string> Roles { get; set; } = new List<string>();
}

public class IdentityCreateUserDto
{
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string Email { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
   public List<string> Roles { get; set; } = new List<string>();
}

public class IdentityLoginDto
{
   public string Email { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
   public bool RememberMe { get; set; }
}

public class IdentityCreateRoleDto
{
   public string Name { get; set; } = string.Empty;
   public string? Description { get; set; }
}

public class IdentitySetPasswordDto
{
   public int UserId { get; set; }
   public string NewPassword { get; set; } = string.Empty;
}

public class IdentityUserRoleAssignmentDto
{
   public int UserId { get; set; }
   public List<string> RoleNames { get; set; } = new List<string>();
}
