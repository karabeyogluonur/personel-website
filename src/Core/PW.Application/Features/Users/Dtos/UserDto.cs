namespace PW.Application.Features.Users.Dtos;

public class UserDto
{
   public class UserSummaryDto
   {
      public int Id { get; set; }
      public string FirstName { get; set; } = string.Empty;
      public string LastName { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public List<string> Roles { get; set; } = new();
   }

   public class UserDetailDto
   {
      public int Id { get; set; }
      public string FirstName { get; set; } = string.Empty;
      public string LastName { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public List<string> Roles { get; set; } = new();
   }

   public class UserCreateDto
   {
      public string FirstName { get; set; } = string.Empty;
      public string LastName { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public string Password { get; set; } = string.Empty;
      public List<string> Roles { get; set; } = new();
   }

   public class UserUpdateDto
   {
      public int Id { get; set; }
      public string FirstName { get; set; } = string.Empty;
      public string LastName { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public List<string> SelectedRoles { get; set; } = new();
      public bool IsPasswordChangeRequested { get; set; }
      public string? NewPassword { get; set; }
   }
}
