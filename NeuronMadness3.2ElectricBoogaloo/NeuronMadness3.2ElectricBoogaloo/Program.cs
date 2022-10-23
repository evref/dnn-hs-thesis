using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NeuronMadness3._2ElectricBoogaloo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the Training manager, takes minibatch-sizes and seed as inputs
            TrainingManager.Initialize(100, "seed");

            // Create a new neural network, input and output layers should always be the same size
            // Example: (784, 16, 16, 10)
            NetData net = new NetData(784, 16, 16, 10);

            // Randomize start data for the network
            NetManager.GenNewNetworkData(net);

            NetData trainedNet = net;


            // Här kan du välja att repetera inlärningsprocessen flera gånger vilket tenderar att leda till bättre resultat (fler epoker)

            // Repeat the learning process, the variable i here is the number of epochs
            for (int i = 0; i < 1; i++)
            {
                // Train the network on the data using the TrainingManager which we initialized earlier
                // If a network with the same parameters already exist, the program will load that network instead and skip the training process
                trainedNet = TrainingManager.Train(trainedNet, 1, true);

                // Test the network on the test set of the data
                float res = TrainingManager.Test(trainedNet, false);


                // Print network accuracy
                Console.SetCursorPosition(0, 10 + i);
                Console.WriteLine("The neural network scored: {0}% on the test data", res * 100);
                Console.SetCursorPosition(0, 0);
            }

            // Saves the net to the "data" folder
            // The system only keeps track of networks with differing seeds and artificial neuron structures
            // An already saved net can be loaded by trying to train a network with the same parameters, which will load the existing one instead
            trainedNet.Save();

            Console.ReadLine();
        }
    }
}