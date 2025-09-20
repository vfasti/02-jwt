using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthenticationTemplateNet9.Infrastructure;

public class ToDosDbContext : IdentityDbContext  // instead of DbContext - Schritt 0
{
    public ToDosDbContext(DbContextOptions<ToDosDbContext> options) : base(options)
    {
    }
}
