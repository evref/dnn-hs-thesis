using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NeuronMadness3._2ElectricBoogaloo
{
    public static class TrainingManager
    {
        public static string seed, savePath;
        static int miniBatchSize;
        static DigitImage[] testImages, trainImages;


        public static void Initialize(int aMiniBatchSize, string aSeed)
        {
            // initialize random method
            RandomF.Init(seed = aSeed); // remember not to ever use special characters in the seed as it will be used as net file name (and no hashtags)
            savePath =
                @"../../data/nets/";

            miniBatchSize = aMiniBatchSize;

            LoadData();
        }
        static void LoadData()
        {
            trainImages = new DigitImage[60000];
            testImages = new DigitImage[10000];
            string path =
                @"../../data/images/";

            string imageFile =
                path + "train-images.idx3-ubyte";
            string labelFile =
                path + "train-labels.idx1-ubyte";
            trainImages = DigitImage.LoadData(imageFile, labelFile, 60000);

            imageFile =
                path + "t10k-images.idx3-ubyte";
            labelFile =
                path + "t10k-labels.idx1-ubyte";
            testImages = DigitImage.LoadData(imageFile, labelFile, 10000);
        }


        public static NetData Train(NetData aNet, int aLearningRate, bool aDebugFlag = false)
        {
            string path = savePath + NetData.CreateFileName(seed, aNet.GetSize);
            if (File.Exists(path))
            {
                return NetData.Load(seed, aNet.GetSize);
            }


            NetData trainedNet = aNet.Clone();
            DigitImage[][] miniBatches = CreateMinibatches(trainImages);
            NetData[] someNets = new NetData[trainImages.Length / miniBatchSize];

            for (int i = 0; i < miniBatches.Length; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Currently at minibatch {0} out of {1}", i + 1, miniBatches.Length);

                NetData stepChange = new NetData(aNet.GetSize);

                for (int j = 0; j < miniBatchSize; j++)
                {
                    stepChange.AddData(NetManager.GetValueNudges(aNet, miniBatches[i][j].activationData, miniBatches[i][j].correctActivation));
                }

                stepChange.DivideData(miniBatchSize);

                // test
                stepChange.MultiplyData(aLearningRate);

                trainedNet.AddData(stepChange);
            }


            return trainedNet;
        }
        static DigitImage[][] CreateMinibatches(DigitImage[] someData) // not random as of yet
        {
            DigitImage[][] miniBatches = new DigitImage[someData.Length / miniBatchSize][];
            for (int i = 0; i < miniBatches.Length; i++)
            {
                miniBatches[i] = new DigitImage[miniBatchSize];
                for (int j = 0; j < miniBatchSize; j++)
                {
                    miniBatches[i][j] = someData[(i * miniBatchSize) + j];
                }
            }

            return miniBatches;
        }


        public static float Test(NetData aNet, bool aQuickFlag)
        {
            int numOfTests;
            if (aQuickFlag)
                numOfTests = testImages.Length / 10;
            else
                numOfTests = testImages.Length;

            int score = 0;
            for (int i = 0; i < numOfTests; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Currently at test {0} out of {1}", i + 1, numOfTests);
                bool result = TestDigit(aNet, testImages[i]);
                if (result)
                    score++;
            }

            return (float)Math.Round((double)score / numOfTests, 2);
        }
        static bool TestDigit(NetData aNet, DigitImage aDigit)
        {
            float[] result = NetManager.Activate(aNet, aDigit.activationData);
            int highestIdx = GetIdxOfHighest(result);

            return aDigit.correctActivation[highestIdx] >= 0.9f; // -0.1f to be safe from unpredictable float values
        }
        static int GetIdxOfHighest(float[] anArray)
        {
            int idx = 0;
            for (int i = 1; i < anArray.Length; i++)
            {
                if (anArray[i] > anArray[idx])
                    idx = i;
            }

            return idx;
        }
    }
}
