using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronMadness3._2ElectricBoogaloo
{
    class TestNet : NetData
    {
        public TestNet() : base(new int[] { 1, 2, 2, 1 })
        {
            Biases[0] = new float[] { 1, 1 };
            Biases[1] = new float[] { 1, 1 };
            Biases[2] = new float[] { 1 };

            Weights[0] = new float[,]
            {
                { 0.5f, 0.5f }
            };
            Weights[1] = new float[,]
            {
                { 0.5f, 0.5f },
                { 0.5f, 0.5f }
            };
            Weights[2] = new float[,]
            {
                { 0.5f },
                { 0.5f }
            };
        }
    }
}
