using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronMadness3._2ElectricBoogaloo
{
    public static class RandomF
    {
        static float seed1, seed2, seed3;
        public static void Init(string seed)
        {
            seed1 = Math.Abs(seed.GetHashCode()) % 30269f;
            seed2 = seed1 % 172;
            seed3 = seed2 % 170;
        }

        public static float Float()
        {
            seed1 = (seed1 * 171f) % 30269f;
            seed2 = (seed2 * 172f) % 30307f;
            seed3 = (seed3 * 170f) % 30323f;

            return (seed1 / 30269f + seed2 / 30307f + seed3 / 30323f) % 1;
        }
    }
}
