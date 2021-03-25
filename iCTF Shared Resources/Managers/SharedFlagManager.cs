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
        public static async Task<Challenge> GetChallByFlag(DatabaseContext context, string flag)
        {
            Challenge chall = await context.Challenges.Where(x => x.Flag == flag && x.State == 2).FirstOrDefaultAsync();
            return chall;
        }
    }
}
