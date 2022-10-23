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
            // Här kan du ändra både minibatch-storlekarna och seedet
            // Problemet jag nämner senare appliceras på int-värdet i den här metoden
            TrainingManager.Initialize(100, "seed");

            // Här kan du ändra antalet gömda noder 
            // Mellan första och sista kan du lägga till hur många lager du vill, du kommer dock märka att träningstiden inte kommer bli kortare av detta :)
            // PS: Ändrar du input-lagret (784) eller output-lagret (10) så kommer nätverket sluta fungera
            NetData net = new NetData(784, 16, 16, 10);

            // Slumpar fram ny startdata till nätverket (ändra seed för att få annorlunda)
            NetManager.GenNewNetworkData(net);

            NetData trainedNet = net;


            // Här kan du välja att repetera inlärningsprocessen flera gånger vilket tenderar att leda till bättre resultat (fler epoker)
            for (int i = 0; i < 1; i++)
            {
                // Metoden som tränar nätverket på den givna datan
                // Ifall nätverket redan finns kommer detta steget hoppas över då nätverket ändå hade slutat identiskt
                // PS: Det är värt att notera att ovanstående påstående inte är helt sant då förändringar i träningsmetoden kan orsaka olikheter, detta är dessvärre en baksida med systemet
                trainedNet = TrainingManager.Train(trainedNet, 1, true);

                // Ger oss hur många procent rätt nätverket gissade
                float res = TrainingManager.Test(trainedNet, false);


                Console.SetCursorPosition(0, 10 + i);
                Console.WriteLine("The net scored: {0}", res);
                Console.SetCursorPosition(0, 0);
            }

            // Sparar nätverket till "data"-mappen
            // Systemet sparar endast nätverk med olika seeds eller struktur (antal gömda lager, nodantal, osv.)
            // Du kan ladda ett redan skapat nätverk genom att försöka skapa ett nätverk med samma struktur och seed som det nätverk du försöker ladda
            trainedNet.Save();

            Console.ReadLine();
        }
    }
}