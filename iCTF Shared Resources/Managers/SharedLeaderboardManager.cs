using iCTF_Shared_Resources.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Shared_Resources.Managers
{
    public class SharedLeaderboardManager
    {
        public static async Task<List<User>> GetTopPlayers(DatabaseContext context, int max = 10)
        {
            return await context.Users.Where(x => x.Score > 0).OrderByDescending(x => x.Score).ThenBy(x => x.LastUpdated).Take(max).ToListAsync();
        }

        public static async Task<int> GetPlayerPosition(DatabaseContext context, User user)
        {
            return await context.Users.Where(x => (x.Score > user.Score) || (x.Score == user.Score && x.LastUpdated < user.LastUpdated)).CountAsync() + 1;
        }
    }
}
