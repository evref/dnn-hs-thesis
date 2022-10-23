using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NeuronMadness3._2ElectricBoogaloo
{
    public class DigitImage
    {
        public int width; // 28
        public int height; // 28
        public int[][] pixels; // 0(white) - 255(black)
        public int label; // '0' - '9'
        public float[] correctActivation; // array where correct activation has value 1, others have 0
        public float[] activationData; // net input, pixels reduced to 1 dim float array with values from 0(black) - 1(white)


        public DigitImage(int aWidth, int aHeight, int[][] somePixels, int aLabel)
        {
            width = aWidth;
            height = aHeight;
            pixels = new int[height][];
            label = aLabel;
            activationData = new float[width * height];


            correctActivation = new float[10];
            correctActivation[label] = 1;


            // set pixel values
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = new int[width];
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pixels[i][j] = somePixels[i][j];
                }
            }
        }



        public static DigitImage[] LoadData(string pixelFile, string labelFile, int aNumImages)
        {
            int numImages = aNumImages;

            DigitImage[] result = new DigitImage[numImages];
            int[][] pixels = new int[28][];
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = new int[28];
            }


            FileStream ifsPixels = new FileStream(pixelFile, FileMode.Open);
            FileStream ifsLabels = new FileStream(labelFile, FileMode.Open);
            BinaryReader brImages = new BinaryReader(ifsPixels);
            BinaryReader brLabels = new BinaryReader(ifsLabels);
            int magic1 = brImages.ReadInt32(); // stored as a big median
            magic1 = ReverseBytes(magic1); // convert to intel format
            int imageCount = brImages.ReadInt32();
            imageCount = ReverseBytes(imageCount);
            int numRows = brImages.ReadInt32();
            numRows = ReverseBytes(numRows);
            int numCols = brImages.ReadInt32();
            numCols = ReverseBytes(numCols);
            int magic2 = brLabels.ReadInt32();
            magic2 = ReverseBytes(magic2);
            int numLabels = brLabels.ReadInt32();
            numLabels = ReverseBytes(numLabels);


            for (int di = 0; di < numImages; ++di)
            {
                for (int i = 0; i < 28; ++i) // get 28x28 pixel values
                {
                    for (int j = 0; j < 28; ++j)
                    {
                        byte b = brImages.ReadByte();
                        pixels[i][j] = b;
                    }
                }

                byte lbl = brLabels.ReadByte(); // get the label
                DigitImage dImage = new DigitImage(28, 28, pixels, lbl);
                result[di] = dImage;
            } // repeat for each image

            ifsPixels.Close();
            brImages.Close();

            ifsLabels.Close();
            brLabels.Close();

            SetActivationData(result);
            return result;
        }
        static int ReverseBytes(int v)
        {
            byte[] intAsBytes = BitConverter.GetBytes(v);
            Array.Reverse(intAsBytes);
            return BitConverter.ToInt32(intAsBytes, 0);
        }
        static void SetActivationData(DigitImage[] someDigits)
        {
            for (int i = 0; i < someDigits.Length; i++) // repeat for each digit
            {
                for (int j = 0; j < someDigits[i].pixels.Length; j++)
                {
                    for (int k = 0; k < someDigits[i].pixels[j].Length; k++)
                    {
                        int n = someDigits[i].pixels[j][k];
                        float a = n / 255f;

                        someDigits[i].activationData[(someDigits[i].pixels.Length * j) + k] = a;
                    }
                }
            }
        }
    }
}
