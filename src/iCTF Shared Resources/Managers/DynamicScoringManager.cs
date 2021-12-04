using iCTF_Shared_Resources.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Shared_Resources.Managers
{
    public static class DynamicScoringManager
    {
        public static Expression<Func<int, int>> SolvePoints { get; } = 
            x => Math.Max(Convert.ToInt32((100 - 500) / 5000f * Convert.ToInt32(Math.Pow(x, 2)) + 500), 100);

        public static int GetPointsFromSolvesCount(int solvesCount) => SolvePoints.Compile().Invoke(solvesCount);
    }
}
