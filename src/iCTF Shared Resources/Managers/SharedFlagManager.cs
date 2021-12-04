using iCTF_Shared_Resources.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Shared_Resources.Managers
{
    public class SharedFlagManager
    {
        public static async Task<Challenge> GetChallByFlag(DatabaseContext context, string flag, bool includeArchived = false)
        {
            if (string.IsNullOrEmpty(flag)) return null;
            if (includeArchived) {
                return await context.Challenges.FirstOrDefaultAsync(x => x.Flag == flag && (x.State == 2 || x.State == 3));
            } else {
                return await context.Challenges.FirstOrDefaultAsync(x => x.Flag == flag && x.State == 2);
            }
        }
    }
}
