using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AdsMarketSharing.Entities;

namespace AdsMarketSharing.Entities.Configurations
{
    public class AccountRoleConfig : IEntityTypeConfiguration<AccountRole>
    {
        public void Configure(EntityTypeBuilder<AccountRole> builder)
        {
            builder.HasKey(accountRole => new { accountRole.AccountId, accountRole.RoleId });
        }
    }
}
