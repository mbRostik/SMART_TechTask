using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Infrastructure.Data
{
    public class IdentityServerDbContext : IdentityDbContext, IDataProtectionKeyContext
    {
        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options) : base(options)
        {

        }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}
