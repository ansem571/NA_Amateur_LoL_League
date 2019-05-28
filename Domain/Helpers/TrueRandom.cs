using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Helpers
{
    public static class TrueRandom
    {
        public static bool TrueRandomGenerator()
        {
            Random rand = new Random();
            var seed = rand.Next(int.MaxValue-1);
            var newRandom = new Random(seed);

            return newRandom.Next(100) % 2 == 1;
        }
    }
}
