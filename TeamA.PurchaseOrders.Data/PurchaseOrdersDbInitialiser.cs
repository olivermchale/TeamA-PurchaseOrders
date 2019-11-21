using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamA.PurchaseOrders.Data
{
    public class PurchaseOrdersDbInitialiser
    {
        public static async Task SeedTestData(PurchaseOrdersDb context, IServiceProvider services)
        {
            if (context.PurchaseOrders.Any())
            {
                //db is seeded
                return;
            }
            await context.SaveChangesAsync();
        }
    }
}
