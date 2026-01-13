using Microsoft.AspNetCore.Identity;

namespace PW.Identity.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public string Description { get; set; }
}
