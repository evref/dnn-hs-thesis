using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronMadness3._2ElectricBoogaloo
{
    static class NetManager
    {
        #region Setup


        // Data generation
        public static void GenNewNetworkData(NetData aNetwork)
        {
            GenWeights(aNetwork);
            GenBiases(aNetwork, 10);
        }
        static void GenWeights(NetData aNetwork)
        {
            for (int l = 0; l < aNetwork.Weights.Length; l++)
            {
                for (int n = 0; n < aNetwork.Weights[l].GetLength(0); n++)
                {
                    for (int k = 0; k < aNetwork.Weights[l].GetLength(1); k++)
                    {
                        float tempWeight = (2 * RandomF.Float()) - 1;
                        if (tempWeight == 0)
                            tempWeight = 0.1f;
                        aNetwork.Weights[l][n, k] = tempWeight;
                    }
                }
            }
        }
        static void GenBiases(NetData aNetwork, int aMaxBias)
        {
            for (int l = 0; l < aNetwork.Biases.Length; l++)
            {
                for (int k = 0; k < aNetwork.Biases[l].Length; k++)
                {
                    float tempBias = (2 * aMaxBias * RandomF.Float()) - aMaxBias;
                    if (tempBias == 0)
                        tempBias = 0.1f;
                    aNetwork.Biases[l][k] = tempBias;
                }
            }
        }
        public static float[] GenNewActivations(int aNeuronAmount)
        {
            float[] tempNeuronsToReturn = new float[aNeuronAmount];
            for (int i = 0; i < aNeuronAmount; i++)
            {
                tempNeuronsToReturn[i] = RandomF.Float();
            }

            return tempNeuronsToReturn;
        }


        #endregion 


        #region Activation


        // Activation
        public static float[] Activate(NetData aNetwork, float[] someActivationNeurons)
        {
            aNetwork.LastActivations[0] = someActivationNeurons;
            return ActivateLayer(aNetwork, 0, someActivationNeurons);
        }
        static float[] ActivateLayer(NetData aNetwork, int aLayer, float[] someNeurons)
        {
            // check if last layer is reached
            if (aLayer >= aNetwork.GetSize.Length - 1)
                return someNeurons;

            // activate current layer
            float[,] tempWeighted = MultiplyVectors(someNeurons, aNetwork.Weights[aLayer]);
            float[] tempSum = SumVector2(tempWeighted);
            float[] tempZ = AddVectors(tempSum, aNetwork.Biases[aLayer]);

            // add non-sigmoided activations to storage
            aNetwork.LastZActivations[aLayer] = tempZ;

            float[] tempActiveNeurons = Sigmoid(tempZ);

            // add activations to data storage
            aNetwork.LastActivations[aLayer + 1] = tempActiveNeurons;

            // proceed to next layer
            return ActivateLayer(aNetwork, ++aLayer, tempActiveNeurons);
        }


        // the required vector math
        static float[,] MultiplyVectors(float[] aVector, float[,] aVector2)
        {
            int d0 = aVector2.GetLength(0);
            int d1 = aVector2.GetLength(1);

            float[,] tempVector2 = new float[d0, d1];
            for (int n = 0; n < d0; n++)
            {
                for (int k = 0; k < d1; k++)
                {
                    tempVector2[n, k] = aVector[n] * aVector2[n, k];
                }
            }

            return tempVector2;
        }
        static float[] SumVector2(float[,] aVector2)
        {
            int d0 = aVector2.GetLength(0);
            int d1 = aVector2.GetLength(1);


            float[] tempSum = new float[d1];

            for (int k = 0; k < d1; k++) // repeat for each activation in layer L
            {
                for (int n = 0; n < d0; n++) // repeat for each activation in layer L-1
                {
                    tempSum[k] += aVector2[n, k];
                }
            }

            return tempSum;
        }
        static float[] AddVectors(float[] aVector, float[] aVectorToAdd)
        {
            float[] tempVector = new float[aVector.Length];
            for (int i = 0; i < tempVector.Length; i++)
            {
                tempVector[i] = aVector[i] + aVectorToAdd[i];
            }

            return tempVector;
        }


        // function returning a float between 0 and 1
        static float[] Sigmoid(float[] aVector)
        {
            float[] sigmoid = new float[aVector.Length];
            for (int i = 0; i < sigmoid.Length; i++)
            {
                sigmoid[i] = (float)(1 / (1 + Math.Pow(Math.E, -aVector[i])));
            }

            return sigmoid;
        }



        #endregion


        #region Back Propagation


        // aL = Activations for the last layer, L
        // aL1 = Activations for the second last layer, L - 1
        // wL = Weights for the last layer, L
        // bL = Biases for the last layer, L
        // zL = aL1 * wL + bL
        // y = Desired activations for the network
        // yL = Desired activations for the current layer
        // yL1 = Desired activations for the layer before the current layer
        // C0 = The cost of the network (how far from the desired outputs were we?)

        public static NetData GetValueNudges(NetData aNetwork, float[] someActivations, float[] y)
        {
            // get the activations for the network
            Activate(aNetwork, someActivations);

            // return the suggested changes
            return BackProp(aNetwork, y, 0);
        }

        static NetData BackProp(NetData aNetwork, float[] yL, int anIteration, NetData aNetworkToReturn = null)
        {
            // get ID
            int i = aNetwork.GetSize.Length - anIteration - 1;

            // prepare current activations
            float[] aL = aNetwork.LastActivations[i];
            float[] aL1 = aNetwork.LastActivations[i - 1];
            int d0 = aL1.Length;
            int d1 = aL.Length;
            float[] zL = aNetwork.LastZActivations[i - 1];

            // create network to return
            NetData nToReturn;
            if (aNetworkToReturn != null) nToReturn = aNetworkToReturn;
            else
                nToReturn = new NetData(aNetwork.GetSize);


            // aL1 wanted activations
            float[] yL1 = new float[aL1.Length];

            // iterate for each neuron in aL
            for (int k = 0; k < d1; k++)
            {
                // get cost function
                float C0 = GetC0(aL[k], yL[k]); // not necessary

                // iterate for each connection of aL1 to neuron
                for (int n = 0; n < d0; n++)
                {
                    // weight nudge
                    float weightNudge = aL1[n] * ConstantChainRule(aL[k], yL[k], zL[k]);
                    nToReturn.Weights[i - 1][n, k] -= weightNudge;

                    // yL1
                    float yL1Nudge = aNetwork.Weights[i - 1][n, k] * ConstantChainRule(aL[k], yL[k], zL[k]);
                    yL1[n] -= yL1Nudge;
                }

                // bias
                float biasNudge = ConstantChainRule(aL[k], yL[k], zL[k]);
                nToReturn.Biases[i - 1][k] -= biasNudge;
            }

            // loop exiting conditions
            if (i <= 1)
                return nToReturn;


            // add yL1 to aL1 to get a more optimal value for aL1
            for (int j = 0; j < aL1.Length; j++)
            {
                yL1[j] += aL1[j];
                if (yL1[j] < 0)
                    yL1[j] = 0;
                else if (yL1[j] > 1)
                    yL1[j] = 1;
            }


            // repeat process for next layer
            return BackProp(aNetwork, yL1, ++anIteration, nToReturn);
        }

        static float ConstantChainRule(float activation, float desiredActivation, float nonSigmoidActivation)
        {
            float z = nonSigmoidActivation;
            float sigZ = dSigmoid(z);
            float dCost = 2 * (activation - desiredActivation);
            float numToReturn = sigZ * dCost;


            return numToReturn;
        }
        static float GetC0(float a, float y)
        {
            return (float)Math.Pow(a - y, 2);
        }
        static float dSigmoid(float aValue)
        {
            float e = (float)Math.E;
            float x = aValue;

            float dSigmoid = (float)(Math.Pow(e, x) / Math.Pow(Math.Pow(e, x) + 1, 2));

            return dSigmoid;
        }


        #endregion


        #region Debug


        public static void PrintWeights(float[][,] someWeights)
        {
            Console.WriteLine("Weights:");
            string line = "";
            for (int l = 0; l < someWeights.Length; l++)
            {
                for (int n = 0; n < someWeights[l].GetLength(0); n++)
                {
                    for (int k = 0; k < someWeights[l].GetLength(1); k++)
                    {
                        line += Math.Round(someWeights[l][n, k], 2);
                        if (k != someWeights[l].GetLength(1) - 1)
                            line += '#';
                    }
                    Console.WriteLine(line);
                    line = "";
                }
            }
        }
        public static void PrintBiases(float[][] someBiases)
        {
            Console.WriteLine("Biases:");
            string line = "";
            for (int l = 0; l < someBiases.Length; l++)
            {
                for (int k = 0; k < someBiases[l].Length; k++)
                {
                    line += someBiases[l][k];
                    if (k != someBiases[l].Length - 1)
                        line += '#';
                }
                Console.WriteLine(line);
                line = "";
            }
        }
        public static void CheckForNaN(NetData aNet)
        {
            float[][,] weights = aNet.Weights;

            for (int l = 0; l < weights.Length; l++)
            {
                for (int n = 0; n < weights[l].GetLength(0); n++)
                {
                    for (int k = 0; k < weights[l].GetLength(1); k++)
                    {
                        if (float.IsNaN(weights[l][n, k]))
                            Console.WriteLine("NaN detected");
                    }
                }
            }


            float[][] biases = aNet.Biases;

            for (int l = 0; l < biases.Length; l++)
            {
                for (int k = 0; k < biases[l].Length; k++)
                {
                    if (float.IsNaN(biases[l][k]))
                        Console.WriteLine("NaN detected");
                }
            }
        }


        #endregion
    }
}
