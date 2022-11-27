using AdsMarketSharing.Entities.Keyless;
using Microsoft.EntityFrameworkCore;

namespace AdsMarketSharing.Entities.Functions
{
    public static class Scalars
    {
        public static int CalcSoldQuantity(int productId)
        {
            return 0;
        }
        public static void RegisterFunction(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(() => CalcSoldQuantity(0));
        }

        public static SellerDashboard GetSellerDashboard(int sellerId)
        {
            return new SellerDashboard();
        }
    }
}
