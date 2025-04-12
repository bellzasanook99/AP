using Accord.Imaging.Converters;
using Accord.MachineLearning;
using Accord.Statistics.Analysis;
using Core.Database.MLBufferDB;
using Core.Interface;
using Core.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Core.Service
{
    public class ProcressingService : IProcressingService
    {


        public byte[] SimmulatorData (TransFile transFiles)
        {
            int MaxWidth = transFiles.MaxWidth.Value;
            int MaxHeight = transFiles.MaxHeight.Value;
            var outputFilePath = Path.Combine("wwwroot/uploads", "processed_.jpg");
            Mat blackImage = new Mat(MaxHeight, MaxWidth, MatType.CV_8UC3, Scalar.All(0));

            HersheyFonts font = HersheyFonts.HersheySimplex;
            double fontScale = 0.7;
            Scalar textColor = new Scalar(0, 255, 0);
            int thickness = 1;
            foreach (var item2 in transFiles.TransPages)
            {
                Rect rec = new Rect(item2.Px.Value, item2.Py.Value, item2.Width.Value, item2.Height.Value);
                OpenCvSharp.Point center = new OpenCvSharp.Point
                {

                    X = (int)Math.Round(rec.X + rec.Width * 0.5),
                    Y = (int)Math.Round(rec.Y + rec.Height * 0.5)
                };

                Cv2.Rectangle(blackImage, rec, Scalar.Green, 2);
                Cv2.PutText(blackImage, item2.Hocr, new OpenCvSharp.Point(center.X-(rec.Width/4), center.Y), font, fontScale, textColor, thickness);
                Cv2.PutText(blackImage, item2.Locr, new OpenCvSharp.Point(center.X- (rec.Width/4), center.Y+35), font, fontScale, textColor, thickness);
            }
             Cv2.ImWrite(outputFilePath, blackImage); //save image
            return blackImage.ToBytes();
        }

        public List<TransFile> Procressingtotf(List<TransFile> transFiles)
        {
            foreach (var item in transFiles)
            {
                List<TransPage> transPage = new List<TransPage>();
                FileInfo fileInfo = new FileInfo(item.FilePath);

                // The byte[] to save the data in
                byte[] data = new byte[fileInfo.Length];

                // Load a filestream and put its content into the byte[]
                using (FileStream fs = fileInfo.OpenRead())
                {
                    fs.Read(data, 0, data.Length);
                }

                var output = Procressing(data);
                item.MaxWidth = output.MaxWidth;
                item.MaxHeight = output.MaxHeight;
                fileInfo.Delete();
                output.mdlGRes.ForEach(m =>
                {
                    try
                    {
                       transPage.Add(new TransPage { Px = m.rectangle.X, Py = m.rectangle.Y, Height = m.rectangle.Height, Width = m.rectangle.Width, Hocr = m.ocrstr.Split('_')[0], Locr = m.ocrstr.Split('_')[1] });
                    //    transPage.Add(new TransPage { Px = m.rectangle.X, Py = m.rectangle.Y, Height = m.rectangle.Height, Width = m.rectangle.Width });
                    }
                    catch (Exception ex)
                    {

                    }

                });
                item.TransPages = transPage;
            }

            return transFiles;
        }

        public List<MdlGRes> Procressing_Update1(Mat gray, List<MdlGRes> mdlGRes)
        {
         
            string haarPath = @"D:\HAA\A13.xml"; // ไฟล์ Haar Cascade
            var cascade = new CascadeClassifier(haarPath);
            const double ScaleFactor = 1.0850;
            const int MinNeighbors = 2;

            int Scales = 2;
            Rect[] faces = cascade.DetectMultiScale(gray, ScaleFactor, MinNeighbors, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(20, 20));

            foreach (Rect r in faces)
            {
                int rechw = (r.Width * r.Height);
                OpenCvSharp.Point center = new OpenCvSharp.Point
                {

                    X = (int)Math.Round(r.X + r.Width * 0.5),
                    Y = (int)Math.Round(r.Y + r.Height * 0.5)
                };

                if (rechw < 4000)
                {

                    int radius = (int)Math.Round((r.Width + r.Height) * 0.25);


                    Rect rec = new Rect(r.X * Scales, r.Y * Scales, r.Width * Scales, r.Height * Scales);

                    mdlGRes.Add(new MdlGRes { rectangle = new Rectangle { X = rec.X, Y = rec.Y, Height = rec.Height, Width = rec.Width } });

                }


                //byte[] imageBytes = iplImage.ToBytes(".jpg");
            }


            return mdlGRes;
        }


        public void initial()
        {

            //List<Bitmap> Listpic = new List<Bitmap>();
            List<string> Listname = new List<string>();
            double[][] inputs = new double[4][];
            double[][] features = new double[4][];
            PrincipalComponentAnalysis pca;
            MinimumMeanDistanceClassifier classifier;
            ImageToArray imageToArray1 = new ImageToArray(-1.0, 1.0);
            DescriptiveAnalysis sda;

            //StreamReader streamReader = new StreamReader(@"D:\HAA\datatrain.txt"); //label
            //FileStorage cvFileStorage = new FileStorage(@"D:\HAA\datatrain.xml", FileStorage.Modes.Read);  //image

            //   string filePath = @"D:\HAA\datatrain.xml";

            //   string strstream;
            //   int i = 0;

            //using (var fs = new FileStorage(filePath, FileStorage.Modes.Read))
            //{
            //    FileNode fileNodeByName = fs.GetFirstTopLevelNode("_" + i + "_");
            //}

            //    while ((strstream = streamReader.ReadLine()) != null)
            //{
            //    if (!strstream.Equals(""))
            //    {
            //        Listname.Add(strstream);
            //        FileNode fileNodeByName = cvFileStorage.GetFileNodeByName( "_" + i + "_");
            //        Mat img1 = cvFileStorage.Read<Mat>(fileNodeByName);
            //        //  img1.SaveImage(i+".jpg");

            //      //  Listpic.Add(img1.ToBitmap());
            //        ++i;
            //        img1.Dispose();
            //    }
            //}

        }


        public MdlGenPage Procressing(byte[] bytes)
        {
           // initial();
            MdlGenPage mdlGenPage = new MdlGenPage();
            mdlGenPage.mdlGRes = new List<MdlGRes>();
            var outputFilePath = Path.Combine("wwwroot/uploads", "processed1_.jpg");
            int Scales = 2;
            // ใช้ Cv2.ImRead เพื่ออ่านภาพ
            //  Mat iplImage = Cv2.ImRead(imagePath, ImreadModes.Color);
            Mat iplImage = Cv2.ImDecode(bytes, ImreadModes.Color);


          //  Cv2.ImWrite(outputFilePath, iplImage); //save image
            mdlGenPage.MaxHeight = iplImage.Height;
            mdlGenPage.MaxWidth = iplImage.Width;
            Mat dstLinear = new Mat();
            Cv2.Resize(iplImage, dstLinear, new OpenCvSharp.Size(iplImage.Width / Scales, iplImage.Height / Scales));

            Mat gray = new Mat();
            Cv2.CvtColor(dstLinear, gray, ColorConversionCodes.BGR2GRAY);
         
            string haarPath = @"D:\HAA\A13.xml"; // ไฟล์ Haar Cascade
            var cascade = new CascadeClassifier(haarPath);
            const double ScaleFactor = 1.0850;
            const int MinNeighbors = 2;



            Rect[] faces = cascade.DetectMultiScale(gray, ScaleFactor, MinNeighbors, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(20, 20));
            //List<Rectangle> rects = new List<Rectangle>();
            List<MdlGRes> mdlGRes = new List<MdlGRes>();
            //  Mat crop = iplImage.Clone();

            foreach (Rect r in faces)
            {
                int rechw = (r.Width * r.Height);
                OpenCvSharp.Point center = new OpenCvSharp.Point
                {

                    X = (int)Math.Round(r.X + r.Width * 0.5),
                    Y = (int)Math.Round(r.Y + r.Height * 0.5)
                };

                if (rechw < 4000)
                {

                    int radius = (int)Math.Round((r.Width + r.Height) * 0.25);
 

                    Rect rec = new Rect(r.X * Scales, r.Y * Scales, r.Width * Scales, r.Height * Scales);
                    Mat crop = new Mat(iplImage, rec).Clone();
                    var str = ConvertByteToString(crop.ToBytes());
                    var gres = GoogleService.GoogleOCR(str);

                    if (gres != null)
                    {
                        mdlGenPage.mdlGRes.Add(new MdlGRes { ocrstr = gres, rectangle = new Rectangle { X = rec.X, Y = rec.Y, Height = rec.Height, Width = rec.Width } });
                    }
                //    mdlGenPage.mdlGRes.Add(new MdlGRes {  rectangle = new Rectangle { X = rec.X, Y = rec.Y, Height = rec.Height, Width = rec.Width } });
                    Cv2.Rectangle(iplImage, rec, Scalar.Green, 2);

                }


                //byte[] imageBytes = iplImage.ToBytes(".jpg");
            }

              Cv2.ImWrite(outputFilePath, iplImage); //save image
            //   mdlGenPage.mdlGRes = Procressing_Update1(gray, mdlGenPage.mdlGRes);


            return mdlGenPage;
        }

        private string ConvertByteToString(byte[] byteImage)
        {
            

            // Convert the byte array to a base64 string
            string base64String = Convert.ToBase64String(byteImage);

            return base64String;
        }
    }
}
