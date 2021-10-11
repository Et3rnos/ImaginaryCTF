using iCTF_Shared_Resources.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Shared_Resources.Managers
{
    public class DynamicScoringManager
    {
        public static int GetPointsFromSolvesCount(int solvesCount)
        {
            return Math.Max(Convert.ToInt32((100 - 500) / 5000f * Convert.ToInt32(Math.Pow(solvesCount, 2)) + 500), 100);
        }
    }
}
