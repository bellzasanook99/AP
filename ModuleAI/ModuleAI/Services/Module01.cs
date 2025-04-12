using Accord.Imaging.Converters;
using Accord.MachineLearning;
using Accord.Statistics.Analysis;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace ModuleAI.Services
{
    static public class Module01
    {


        private static List<Bitmap> Listpic = new List<Bitmap>();
        private static List<string> Listname = new List<string>();
        private static double[][] inputs = new double[4][];
        private static double[][] features = new double[4][];
        // double[][] reversion = new double[4][];
        private static PrincipalComponentAnalysis pca;
        private static MinimumMeanDistanceClassifier classifier;
        private static ImageToArray imageToArray1 = new ImageToArray(-1.0, 1.0);
        private static DescriptiveAnalysis sda;


        static public void train()
        {
            string path = @"D:\NEW2024\devai\PortAI\bin\Debug\Database\15122024\datatrin\";
            string[] filename = Directory.GetFiles(path, "*jpg", SearchOption.AllDirectories);
         
            int count = -1;
            int sizew = Convert.ToInt16(1280);
            int sizeh = Convert.ToInt16(1024);
            OpenCvSharp.Size newSize = new OpenCvSharp.Size(sizew, sizeh);
            string[] shotname = new string[filename.Length];
            using (var fs = new FileStorage("datatrain.xml", FileStorage.Modes.Write))
            {
                for (int i = 0; i < filename.Length; i++)
            {
                count++;
                shotname[i] = filename[i].Split('\\')[filename[i].Split('\\').Length - 2];
                Mat load = Cv2.ImRead(filename[i], ImreadModes.Grayscale);
                Mat resizedImage = new Mat();
                Cv2.Resize(load, resizedImage, newSize);
            //    resizedImage.ImWrite("test.jpg");
                File.WriteAllLines("datatrain.txt", shotname);
              
                    fs.Write("_" + count + "_", resizedImage);
                



            }
            }

        }

        static private Bitmap convertbytetobitmap(byte[] data)
        {
            Bitmap bitmap;
            using (var ms = new MemoryStream(data))
            {
                bitmap = (Bitmap)Image.FromStream(ms);
            }
            return bitmap;
        }

        static public void initial()
        {
            //  textBox2.Text += "initial \r\n";
            int i = 0;
            // StreamReader streamReader = new StreamReader("datatrain.txt"); //label
            //  CvFileStorage cvFileStorage = new CvFileStorage("datatrain.xml", null, FileStorageMode.Read);  //image
            StreamReader streamReader = new StreamReader("Database\\datatrain.txt"); //label
            FileStorage cvFileStorage = new FileStorage("datatrain.xml", FileStorage.Modes.Read);

            //using (var fs = new FileStorage("datatrain.xml", FileStorage.Modes.Read))
            //{
            //}


                string strstream;
            while ((strstream = streamReader.ReadLine()) != null)
            {
                if (!strstream.Equals(""))
                {
                    Listname.Add(strstream);
                    Mat image = cvFileStorage["_" + i + "_"].ToMat();
                    //FileNode fileNodeByName = cvFileStorage.GetFileNodeByName(null, "_" + i + "_");


                    //IplImage img1 = cvFileStorage.Read<IplImage>(fileNodeByName);
                    //  img1.SaveImage(i+".jpg");

                    Listpic.Add(convertbytetobitmap(image.ToBytes()));
                    ++i;
                    image.Dispose();
                }
            }
            streamReader.Close();
            inputs = new double[Listname.Count][];
            features = new double[Listname.Count][];
            pca = new PrincipalComponentAnalysis(extract(), AnalysisMethod.Center);
            pca.Compute();

            int[] outputs = new int[Listpic.Count];
            for (int index = 0; index < Listpic.Count; ++index)
            {
                double[] output;
                imageToArray1.Convert(Listpic[index], out output); //size ::w*h : pic
                double[] numArray = pca.Transform(output);
                inputs[index] = output;
                features[index] = numArray;
                outputs[index] = index;
                Listpic[index].Dispose();
            }


            classifier = new MinimumMeanDistanceClassifier(features, outputs);
            GC.Collect();

        }

        static public string Matching(Bitmap img)
        {
            long timesave = DateTime.Now.Ticks / 10000L;
            int sizew = Convert.ToInt16(1280);
            int sizeh = Convert.ToInt16(1024);
            OpenCvSharp.Size newSize = new OpenCvSharp.Size(sizew, sizeh);
            Mat mat = BitmapToMat(img);
            Mat resizedImage = new Mat();
            Cv2.Resize(mat, resizedImage, newSize);
            Mat grayImage = new Mat();
            Cv2.CvtColor(resizedImage, grayImage, ColorConversionCodes.BGR2GRAY);


            double[] output;
            imageToArray1.Convert(convertbytetobitmap(grayImage.ToBytes()), out output);
            double[] distances;
            int index = classifier.Compute(pca.Transform(output), out distances);


            string str = Listname[index];

            return str;

        }

        static Mat BitmapToMat(Bitmap bitmap)
        {
            // Ensure the Bitmap is in a supported format (e.g., 24bppRgb or 32bppArgb)
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb &&
                bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException("Unsupported pixel format. Use 24bppRgb or 32bppArgb.");
            }

            // Get the Bitmap's dimensions
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Create a Mat with the same dimensions
            Mat mat = new Mat(height, width, MatType.CV_8UC3); // 3-channel (BGR) image

            // Lock the Bitmap's bits
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat
            );

            try
            {
                // Copy the pixel data from the Bitmap to the Mat
                unsafe
                {
                    byte* srcPtr = (byte*)bitmapData.Scan0; // Pointer to the Bitmap's pixel data
                    byte* dstPtr = (byte*)mat.DataPointer;  // Pointer to the Mat's pixel data

                    int srcStride = bitmapData.Stride; // Number of bytes per row in the Bitmap
                    int dstStride = (int)mat.Step();  // Number of bytes per row in the Mat

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Copy BGR values (skip alpha channel if present)
                            dstPtr[y * dstStride + x * 3] = srcPtr[y * srcStride + x * 3];     // Blue
                            dstPtr[y * dstStride + x * 3 + 1] = srcPtr[y * srcStride + x * 3 + 1]; // Green
                            dstPtr[y * dstStride + x * 3 + 2] = srcPtr[y * srcStride + x * 3 + 2]; // Red
                        }
                    }
                }
            }
            finally
            {
                // Unlock the Bitmap's bits
                bitmap.UnlockBits(bitmapData);
            }

            return mat;
        }
        static Mat BitmapToMatGrayscale(Bitmap bitmap)
        {
            // Ensure the Bitmap is in a supported format (8bppIndexed)
            if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new ArgumentException("Unsupported pixel format. Use 8bppIndexed.");
            }

            // Get the Bitmap's dimensions
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Create a Mat with the same dimensions
            Mat mat = new Mat(height, width, MatType.CV_8UC1); // Single-channel (grayscale) image

            // Lock the Bitmap's bits
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat
            );

            try
            {
                // Copy the pixel data from the Bitmap to the Mat
                unsafe
                {
                    byte* srcPtr = (byte*)bitmapData.Scan0; // Pointer to the Bitmap's pixel data
                    byte* dstPtr = (byte*)mat.DataPointer;  // Pointer to the Mat's pixel data

                    int srcStride = bitmapData.Stride; // Number of bytes per row in the Bitmap
                    int dstStride = (int)mat.Step();  // Number of bytes per row in the Mat

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Copy grayscale value
                            dstPtr[y * dstStride + x] = srcPtr[y * srcStride + x];
                        }
                    }
                }
            }
            finally
            {
                // Unlock the Bitmap's bits
                bitmap.UnlockBits(bitmapData);
            }

            return mat;
        }

        private static double[][] extract()
        {
            double[][] numArray = new double[Listpic.Count][];
            ImageToArray imageToArray = new ImageToArray(-1, 1);
            for (int index = 0; index < Listpic.Count; ++index)
            {
                imageToArray.Convert(Listpic[index], out numArray[index]);
            }
            return numArray;
        }
    }
}
