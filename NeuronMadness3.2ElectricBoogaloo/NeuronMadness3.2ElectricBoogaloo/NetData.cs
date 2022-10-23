using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NeuronMadness3._2ElectricBoogaloo
{
    public class NetData
    {
        public float[][,] Weights { get => myWeights; set => myWeights = value; }
        public float[][] Biases { get => myBiases; set => myBiases = value; }
        public int[] GetSize { get => mySize; }

        public float[][] LastActivations { get => myLastActivations; set => myLastActivations = value; }
        public float[][] LastZActivations { get => myLastZActivations; set => myLastZActivations = value; }


        float[][,] myWeights; // [l = L idx][n = idx of neuron in L-1, k = idx of neuron in L]
        float[][] myBiases; // [l = L idx][k = idx of neuron in L]
        int[] mySize; // [L idx] = amount of neurons in L

        float[][] myLastZActivations; // [l = L idx][k = idx of neuron in L]
        float[][] myLastActivations; // [l = L idx][k = idx of neuron in L]
        // where L is the currently focused layer
        // k is always L's neuron count
        // n is always L-1's neuron count


        public NetData(params int[] aSize)
        {
            // Create a reference model
            mySize = aSize;

            // Set all values to zero
            Reset();
        }


        #region Reset

        public void Reset()
        {
            ResetBiases();
            ResetWeights();
            ResetLastActivations();
            ResetLastZActivations();
        }
        void ResetBiases()
        {
            myBiases = new float[mySize.Length - 1][];
            for (int l = 0; l < myBiases.Length; l++)
            {
                myBiases[l] = new float[mySize[l + 1]];
            }
        }
        void ResetWeights()
        {
            myWeights = new float[mySize.Length - 1][,];
            for (int l = 0; l < myWeights.Length; l++)
            {
                myWeights[l] = new float[mySize[l], mySize[l + 1]];
            }
        }
        void ResetLastActivations()
        {
            myLastActivations = new float[mySize.Length][];
            for (int l = 0; l < myLastActivations.Length; l++)
            {
                myLastActivations[l] = new float[mySize[l]];
            }
        }
        void ResetLastZActivations()
        {
            myLastZActivations = new float[mySize.Length - 1][];
            for (int l = 0; l < myLastZActivations.Length; l++)
            {
                myLastZActivations[l] = new float[mySize[l]];
            }
        }

        #endregion

        #region Math Operations


        public void AddData(NetData someDataToAdd)
        {
            // add biases
            for (int l = 0; l < myBiases.Length; l++)
            {
                for (int k = 0; k < myBiases[l].Length; k++)
                {
                    myBiases[l][k] += someDataToAdd.Biases[l][k];
                }
            }


            // add weights
            for (int l = 0; l < myWeights.Length; l++)
            {
                for (int n = 0; n < myWeights[l].GetLength(0); n++)
                {
                    for (int k = 0; k < myWeights[l].GetLength(1); k++)
                    {
                        myWeights[l][n, k] += someDataToAdd.Weights[l][n, k];
                    }
                }
            }
        }
        public void DivideData(int aNum)
        {
            // divide biases
            for (int l = 0; l < myBiases.Length; l++)
            {
                for (int k = 0; k < myBiases[l].Length; k++)
                {
                    myBiases[l][k] /= aNum;
                }
            }


            // divide weights
            for (int i = 0; i < myWeights.Length; i++)
            {
                for (int n = 0; n < myWeights[i].GetLength(0); n++)
                {
                    for (int k = 0; k < myWeights[i].GetLength(1); k++)
                    {
                        myWeights[i][n, k] /= aNum;
                    }
                }
            }
        }
        public void MultiplyData(int aNum)
        {
            // biases
            for (int l = 0; l < myBiases.Length; l++)
            {
                for (int k = 0; k < myBiases[l].Length; k++)
                {
                    myBiases[l][k] *= aNum;
                }
            }


            // weights
            for (int l = 0; l < myWeights.Length; l++)
            {
                for (int n = 0; n < myWeights[l].GetLength(0); n++)
                {
                    for (int k = 0; k < myWeights[l].GetLength(1); k++)
                    {
                        myWeights[l][n, k] *= aNum;
                    }
                }
            }
        }
        public NetData Clone()
        {
            NetData netToReturn = new NetData(GetSize);


            // copy biases
            netToReturn.Biases = Biases;


            // copy weights
            netToReturn.Weights = Weights;


            return netToReturn;
        }


        #endregion

        #region Saving and loading network data


        public void Save()
        {
            // create file name: SEED#SIZE (- between every array point) ex: seed#1-2-3-4
            string name = CreateFileName(TrainingManager.seed, GetSize);

            // create path
            string path = TrainingManager.savePath + name;

            // same seed and layout would mean same network data
            if (File.Exists(path))
                return;
            StreamWriter sw = new StreamWriter(path);

            // write down the biases
            string line = "";
            for (int l = 0; l < Biases.Length; l++)
            {
                for (int k = 0; k < Biases[l].Length; k++)
                {
                    line += Biases[l][k];
                    if (k != Biases[l].Length - 1)
                        line += "#";
                }

                sw.WriteLine(line);
                line = "";
            }
            sw.WriteLine("w");

            // write down the weights
            for (int l = 0; l < Weights.Length; l++)
            {
                for (int n = 0; n < Weights[l].GetLength(0); n++)
                {
                    for (int k = 0; k < Weights[l].GetLength(1); k++)
                    {
                        line += Weights[l][n, k];
                        if (k != Weights[l].GetLength(1) - 1)
                            line += "#";

                        sw.Write(line);
                        line = "";
                    }
                    sw.WriteLine();
                }
            }

            sw.Close();
        }
        public static NetData Load(string seed, params int[] aSize)
        {
            // create file name: SEED#SIZE (- between every array point) ex: seed#1-2-3-4
            string name = CreateFileName(seed, aSize);

            // create path
            string path = TrainingManager.savePath + name;

            // return null if the file does not exist
            if (!File.Exists(path))
                return null;

            StreamReader sr = new StreamReader(path);
            NetData netToReturn = new NetData(aSize);
            string line, stringVal = "";
            float floatVal = 0;

            // decryption
            // biases
            for (int l = 0; l < netToReturn.Biases.Length; l++)
            {
                int k = 0, i = 0;
                line = sr.ReadLine();
                foreach (char c in line)
                {
                    if (c == '#' || i == line.Length - 1)
                    {
                        bool result = float.TryParse(stringVal, out floatVal);
                        if (!result)
                            Console.WriteLine("Parsing failed on line {0}, file {1}", l, name);

                        netToReturn.Biases[l][k] = floatVal;
                        floatVal = 0;
                        stringVal = "";

                        k++;
                    }
                    else
                        stringVal += c;
                    i++;
                }

                stringVal = "";
            }

            // weights
            sr.ReadLine(); // getting rid of the 'w'
            stringVal = "";
            for (int l = 0; l < netToReturn.Weights.Length; l++)
            {
                for (int n = 0; n < netToReturn.Weights[l].GetLength(0); n++)
                {
                    int k = 0, i = 0;
                    stringVal = "";
                    line = sr.ReadLine();
                    foreach (char c in line)
                    {
                        if (c == '#' || i == line.Length - 1)
                        {
                            bool result = float.TryParse(stringVal, out floatVal);
                            if (!result)
                                Console.WriteLine("Parsing failed on line {0}, file {1}", line, name);

                            netToReturn.Weights[l][n, k] = floatVal;
                            stringVal = "";

                            k++;
                        }
                        else
                            stringVal += c;
                        i++;
                    }
                }
            }

            sr.Close();


            return netToReturn;
        }
        public static string CreateFileName(string seed, int[] aSize)
        {
            string name = seed + "#";
            for (int i = 0; i < aSize.Length; i++)
            {
                name += aSize[i];
                if (i != aSize.Length - 1)
                    name += "-";
            }

            return name;
        }


        #endregion
    }
}
