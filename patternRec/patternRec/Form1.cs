using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Imaging.Converters;
using Accord.MachineLearning;
using Accord.Statistics.Analysis;
using Microsoft.Office.Interop.Excel;
using OpenCvSharp;
using OpenCvSharp.Blob;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.LinkLabel;
using static OpenCvSharp.CvHaarFeature;
using Excel = Microsoft.Office.Interop.Excel;

namespace patternRec
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            initial();
         //  SautinSoft.PdfFocus.SetLicense("02/16/25TTSu7jriACjaMZOb6x0h6HMoMr/XYxv305");
        }


        private static List<Bitmap> Listpic = new List<Bitmap>();
        private static List<string> Listname = new List<string>();
        private static double[][] inputs = new double[4][];
        private static double[][] features = new double[4][];
        // double[][] reversion = new double[4][];
        private static PrincipalComponentAnalysis pca;
        private static MinimumMeanDistanceClassifier classifier;
        private static ImageToArray imageToArray1 = new ImageToArray(-1.0, 1.0);
        private static DescriptiveAnalysis sda;

        static int dim = 50; ///eigenvecs corlum

        static void initial()
        {
            //  textBox2.Text += "initial \r\n";
            int i = 0;
            // StreamReader streamReader = new StreamReader("datatrain.txt"); //label
            //  CvFileStorage cvFileStorage = new CvFileStorage("datatrain.xml", null, FileStorageMode.Read);  //image
            StreamReader streamReader = new StreamReader("datatrain.txt"); //label
            CvFileStorage cvFileStorage = new CvFileStorage("datatrain.xml", null, FileStorageMode.Read);  //image
            string strstream;
            while ((strstream = streamReader.ReadLine()) != null)
            {
                if (!strstream.Equals(""))
                {
                    Listname.Add(strstream);
                    CvFileNode fileNodeByName = cvFileStorage.GetFileNodeByName(null, "_" + i + "_");
                    IplImage img1 = cvFileStorage.Read<IplImage>(fileNodeByName);
                    //  img1.SaveImage(i+".jpg");

                    Listpic.Add(img1.ToBitmap());
                    ++i;
                    img1.Dispose();
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

        private string Matching(IplImage img)
        {
            long timesave = DateTime.Now.Ticks / 10000L;
            int sizew = Convert.ToInt16(112);
            int sizeh = Convert.ToInt16(112);
            IplImage img2 = new IplImage(sizew, sizeh, img.Depth, img.NChannels);
            Cv.Resize(img, img2, Interpolation.Linear);
            IplImage gray = new IplImage(img2.Width, img2.Height, BitDepth.U8, 1);
            Cv.CvtColor(img2, gray, ColorConversion.BgrToGray);

            double[] output;
            imageToArray1.Convert(img2.ToBitmap(), out output);
            double[] distances;
            int index = classifier.Compute(pca.Transform(output), out distances);


            string str = Listname[index];

            return str;

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


        private void button1_Click(object sender, EventArgs e)
        {
            string pdfFile = Path.GetFullPath(@"ex_\");
            string jpegDir = new DirectoryInfo(Directory.GetCurrentDirectory()).CreateSubdirectory("Result").FullName;
          
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            //f.Serial = "02/16/25TTSu7jriACjaMZOb6x0h6HMoMr/XYxv305";
            var listfile = Directory.EnumerateFiles(pdfFile, "*.pdf");
            foreach (string file in listfile)
            {
               

                f.OpenPdf(file);

                if (f.PageCount > 0)
                {
                    f.ImageOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    f.ImageOptions.Dpi = 400;
                    f.ImageOptions.JpegQuality = 100;


                    for (int page = 1; page <= f.PageCount; page++)
                    {
                        string jpegFile = Path.Combine(jpegDir, String.Format("Page{0}_{1}_ReSize.bmp", page, Guid.NewGuid()));
                         int result = f.ToImage(jpegFile, page);

                    //    Bitmap input = new Bitmap(f.ToDrawingImage(page));
                    //    int cropint = 200;
                    //    int cropintx = 0;
                    //    Bitmap cropimage = input.Clone(new Rectangle(cropintx, cropint, input.Width - cropintx, input.Height - cropint), input.PixelFormat);
                    //    IplImage image = new IplImage(cropimage.Width, cropimage.Height, BitDepth.U8, 3);
                    //    image.CopyFrom(cropimage);
                    //    cropimage.Dispose();
                    ////    CvRect cvRect = new CvRect(new CvPoint(0, 0), new CvSize(image.Width, image.Height));
                    ////    image.Rectangle(cvRect, CvColor.Green, 2);
                    //     IplImage dstLinear = new IplImage(new CvSize(image.Width / 2, image.Height / 2), image.Depth, image.NChannels);
                    //     Cv.Resize(image, dstLinear, Interpolation.Linear);

                      //  subimg(dstLinear,4);
                      //   dstLinear.SaveImage(jpegFile);
                        //Cv.ShowImage("img", image);

                    }
                    Cv.WaitKey(0);
                }
            }
         }

        private void button2_Click(object sender, EventArgs e)
        {
            string pdfFile = Path.GetFullPath(@"Result\");
            var listfile = Directory.EnumerateFiles(pdfFile, "*.bmp");
            listBox1.Items.Clear();
            foreach (string file in listfile)
            {
                listBox1.Items.Add(file);
            }

         }

        void subimg(IplImage iplImage,int block)
        {
            IplImage crop = iplImage.Clone();
            CvPoint[] pnt;
            CvRect[] ret;
            int lines = block;

            int x = 0;
            int y = 0;
            int xSpace = Convert.ToInt32(iplImage.Width / lines);
            int ySpace = Convert.ToInt32(iplImage.Height / lines);

            pnt = new CvPoint[(lines * lines) + 1];
            ret = new CvRect[(lines * lines) + 1];
            for (int i = 0; i < lines; i++)
            {

                //  Cv.Line(iplImage, new CvPoint(x, y), new CvPoint(x, ySpace * lines), CvColor.Green);
                x += xSpace;
            }
            x = 0;
            for (int i = 0; i < lines; i++)
            {

                //    Cv.Line(iplImage, new CvPoint(x, y), new CvPoint(xSpace * lines,               y), CvColor.Green);
                y += ySpace;
            }
            x = 0;
            y = 0;
            int counter = 1;
            for (int r = 0; r < lines; r++)
            {
                for (int c = 0; c < lines; c++)
                {
                    ret[counter] = new CvRect(new CvPoint((int)x, (int)y), new CvSize((int)xSpace, (int)ySpace));
                    pnt[counter] = new CvPoint((int)((int)x + xSpace * 0.5), (int)((int)y + ySpace * 0.5));
                    crop.SetROI(ret[counter]);

  

                    crop.SaveImage("subimg\\"+Guid.NewGuid() + ".bmp");
                    crop.ResetROI();
                    //IplImage gray = new IplImage(iplImage.Size, BitDepth.U8, 1);
                    //Cv.CvtColor(iplImage, gray, ColorConversion.BgrToGray);
                    // iplImage.Rectangle(ret[counter], CvColor.Green, 2);

                    x += xSpace;
                    counter++;
                }
                y += ySpace;
                x = 0;
            }
        }


        IplImage Rotation(IplImage input,double angle)
        {
            CvPoint2D32f centerimg = new CvPoint2D32f(input.Width / 2, input.Height / 2);
         //   double angle = -90;
            double scale = 1.0;
            double angleRad = angle * Math.PI / 180; // แปลงมุมเป็นเรเดียน
            CvMat rotation_matrix = Cv.GetRotationMatrix2D(centerimg, angle, scale);
            int newW = Convert.ToInt32( Math.Abs(input.Width * Math.Cos(angleRad)) + Math.Abs(input.Height * Math.Sin(angleRad)));
            int newH = Convert.ToInt32(Math.Abs(input.Height * Math.Cos(angleRad)) + Math.Abs(input.Width * Math.Sin(angleRad)));
            rotation_matrix[0, 2] += (newW - input.Width) / 2;
            rotation_matrix[1, 2] += (newH - input.Height) / 2;
            IplImage rotated_image = Cv.CreateImage( new CvSize( newW, newH), input.Depth, input.NChannels);
            Cv.WarpAffine(input, rotated_image, rotation_matrix);
            CvSize cvSize= new CvSize(input.Width, input.Height);



            return rotated_image;
          //  rotated_image.SaveImage("test.jpg");
        }

        int Scales = 2;
        CvFont font = new CvFont(FontFace.HersheyComplex, 0.2, 0.2, 0.5, 1, LineType.AntiAlias);
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(listBox1.Text))
            {

                if (btnname != "ReadChar")
                {
                    CvHaarClassifierCascade cascade = CvHaarClassifierCascade.FromFile("A13.xml");
                    CvHaarClassifierCascade cascade2 = CvHaarClassifierCascade.FromFile("A17_R.xml");
                    CvHaarClassifierCascade cascade3 = CvHaarClassifierCascade.FromFile("A16_C.xml");
                    CvHaarClassifierCascade cascade4 = CvHaarClassifierCascade.FromFile("A16_C.xml");
                    IplImage iplImage = Cv.LoadImage(listBox1.SelectedItem.ToString());

                  //  iplImage = Rotation(iplImage,-90);
                    iplImage = Rotation(iplImage, 0);
                    IplImage dstLinear = new IplImage(new CvSize(iplImage.Width / Scales, iplImage.Height / Scales), iplImage.Depth, iplImage.NChannels);
                    Cv.Resize(iplImage, dstLinear, Interpolation.Linear);

                    IplImage crop = iplImage.Clone();
              
             
                    IplImage gray = new IplImage(dstLinear.Size, BitDepth.U8, 1);
                    Cv.CvtColor(dstLinear, gray, ColorConversion.BgrToGray);

                    CvRect RecDrawingNo = new CvRect(2570 , 2150 , 480 , 70 );
                    IplImage RecDrawingCorp = dstLinear.Clone();
                    RecDrawingCorp.SetROI(RecDrawingNo);
                      Bitmap RecDrawingCorpbitmap = RecDrawingCorp.ToBitmap();
                      string RecDrawingStr = ServiceGoogle.ConvertBitmapToString(RecDrawingCorpbitmap);
                    //  RecDrawingCorp.Rectangle(RecDrawingNo, CvColor.Green, 1);
                 //    Cv.ShowImage("xxx", RecDrawingCorp);
                  //   Cv.WaitKey(0);
                  //  string OcrDrawingNo = ServiceGoogle.DrawingNoGoogleOCR(RecDrawingStr, RecDrawingCorpbitmap, RecDrawingNo);
                    RecDrawingCorp.ResetROI();
                    // RecDrawingCorp.ToBitmap().Save("c.jpg");
                  

                    //  RecDrawingCorp.ResetROI();

                    // Cv.ShowImage("sss", dst);
                    CvMemStorage storage = new CvMemStorage();
                    const double ScaleFactor = 1.0850;
                    const int MinNeighbors = 2;
                    //  const double Scale = 1.14; 
                    CvSeq<CvAvgComp> faces = Cv.HaarDetectObjects(gray, cascade, storage, ScaleFactor, MinNeighbors, HaarDetectionType.ScaleImage, new CvSize(20, 20));
                     List<CvRect> rects = new List<CvRect>();
                    //for (int i = 0; i < faces.Total; i++)
                    //{
                    //    CvRect r = faces[i].Value.Rect;
                    //    int rechw = (r.Width * r.Height);
                    //    if (rechw < 4000)
                    //    {
                    //        rects.Add(r);
                    //        CvRect rec = new CvRect(r.X * Scales, r.Y * Scales, r.Width * Scales, r.Height * Scales);
                    //        iplImage.Rectangle(rec, CvColor.Green, 1);
                    //    }
                    //}

                    //storage = new CvMemStorage();
                    //CvSeq<CvAvgComp> faces1 = Cv.HaarDetectObjects(gray, cascade2, storage, ScaleFactor, 10, HaarDetectionType.ScaleImage, new CvSize(110, 20));

                    //for (int i = 0; i < faces1.Total; i++)
                    //{
                    //    CvRect r = faces1[i].Value.Rect;
                    //    CvRect rec = new CvRect(r.X * Scales, r.Y * Scales, r.Width * Scales, r.Height * Scales);
                    //    //int rechw = (r.Width * r.Height);
                    //    //if (rechw < 4000)
                    //    //{
                    //    //    rects.Add(r);
                    //    //}
                    //    CvPoint center = new CvPoint
                    //    {
                    //        X = Cv.Round((r.X + r.Width * 0.5)),
                    //        Y = Cv.Round((r.Y + r.Height * 0.5))
                    //    };
                    //    iplImage.Rectangle(rec, CvColor.Red, 1);
                    //    int radius = Cv.Round((r.Width + r.Height) * 0.25);
                    //    //    dstLinear.Circle(center, radius, CvColor.Red, 3, LineType.AntiAlias, 0);
                    //}


                    // gray.SaveImage("g.jpg");
                  
           


                    for (int i = 0; i < faces.Total; i++)
                    {
                        CvRect r = faces[i].Value.Rect;
                        int rechw = (r.Width * r.Height);
                        CvPoint center = new CvPoint
                        {
                            X = Cv.Round((r.X + r.Width * 0.5)),
                            Y = Cv.Round((r.Y + r.Height * 0.5))
                        };
                        //    listBox2.Items.Add(rechw);
                        if (rechw < 4000)
                        {
                            int radius = Cv.Round((r.Width + r.Height) * 0.25);
                            // dstLinear.Circle(center, radius, CvColor.Red, 3, LineType.AntiAlias, 0);
                            dstLinear.Line(center, center, CvColor.Red, 10);
                            //dstLinear.Rectangle(r, CvColor.Green, 1);

                            CvRect rec = new CvRect(r.X * Scales, r.Y * Scales, r.Width * Scales, r.Height * Scales);
                            iplImage.Rectangle(rec, CvColor.Green, 1);


                            crop.SetROI(rec);


                            IplImage dstLinear1 = new IplImage(new CvSize(crop.ROI.Width, crop.ROI.Height), crop.Depth, crop.NChannels);
                            Cv.Resize(crop, dstLinear1, Interpolation.Linear);
                            string xxx = Matching(dstLinear1);



                            ////Google Procressing

                                    Bitmap bitmap = dstLinear1.ToBitmap();


                             string imgstring = ServiceGoogle.ConvertBitmapToString(bitmap);
                          //   ServiceGoogle.GoogleOCR(imgstring, bitmap, rec);


                            dstLinear1.SaveImage("temp3\\" + xxx + "_" + Guid.NewGuid() + ".bmp");
                            crop.ResetROI();
                        }

                        // crop.SetROI(r);
                        //  string output = Matching(crop);
                        //  iplImage.PutText(output, new CvPoint(r.X, r.Y - 5), new CvFont(FontFace.HersheyComplex, 0.4, 0.4), CvColor.Red);
                        // crop.ResetROI();

                    }

                    //pictureBox1.Image = dstLinear.ToBitmap();
                    //   Cv.ShowImage("xx", dstLinear);
                    //  Cv.WaitKey(0);
                    // subimg(dstLinear, 4);
                    iplImage.SaveImage("temp2\\" + Guid.NewGuid() + ".jpg");
                //   dstLinear.SaveImage("temp2\\" + Guid.NewGuid() + "_.jpg");
                    dstLinear.Dispose();
                    gray.Dispose();
                    iplImage.Dispose();
                    storage.Dispose();
                    crop.Dispose();
                    GC.Collect();
                }
                else
                {
                    IplImage imagein = Cv.LoadImage(listBox1.SelectedItem.ToString(),LoadMode.AnyColor);
                    IplImage correspond = imagein.Clone();
                    IplImage roi1 = correspond.Clone();
                
                    CvPoint center = new CvPoint
                    {
                        X = Cv.Round((0 + imagein.Width * 0.5)),
                        Y = Cv.Round((0 + imagein.Height * 0.5))
                    };
                
                    CvRect rect1 = new CvRect(imagein.Width *10 /100, center.Y, imagein.Width * 80 / 100, 50);
                    CvRect rect2 = new CvRect(imagein.Width * 10 / 100, center.Y - rect1.Height, imagein.Width * 80 / 100, 50);
                    roi1.SetROI(rect2);
                    IplImage Originalimg = roi1.Clone();
                
                    IplImage gray = new IplImage(roi1.GetSize(), BitDepth.U8, 1);
                    roi1.CvtColor(gray, ColorConversion.BgrToGray);
                    IplImage newimageline = gray.Clone();
                    newimageline.SetZero();
                    gray.Smooth(gray, SmoothType.Gaussian, 3);

                    IplImage tmpImg = gray.Clone();
                    //IplImage dstImgOpening = gray.Clone();
                    //IplConvKernel element = Cv.CreateStructuringElementEx(2, 2, 0, 0, ElementShape.Ellipse, null);
                    //Cv.MorphologyEx(gray, dstImgOpening, tmpImg, element, MorphologyOperation.Open, 2);
                    //   gray.Dispose();
                    IplImage dstImgClosing = gray.Clone();
                    IplConvKernel element1 = Cv.CreateStructuringElementEx(2, 2, 0, 0, ElementShape.Rect, null);
                    Cv.MorphologyEx(gray, dstImgClosing, tmpImg, element1, MorphologyOperation.Close, 1);

                    

                    IplImage Thresholdx = new IplImage(dstImgClosing.GetSize(), BitDepth.U8, 1);
                    dstImgClosing.AdaptiveThreshold(Thresholdx, 255, AdaptiveThresholdType.GaussianC, ThresholdType.BinaryInv, 5, 5);


                 //   Cv.ShowImage("Thresholdx", Thresholdx);
                   pictureBox3.Image = Thresholdx.ToBitmap();
                    CvSeq<CvPoint> contours;
                    CvMemStorage storage = new CvMemStorage();
                    Cv.FindContours(Thresholdx, storage, out contours, CvContour.SizeOf, ContourRetrieval.External, ContourChain.ApproxSimple);
                    try
                    {
                        Cv.DrawContours(newimageline, contours, CvColor.White, CvColor.White, 1, 1, LineType.Link4);
                    }
                    catch (Exception ex)
                    { }

                    List<int> listx = new List<int>();
                    List<int> listy = new List<int>();
                    List<int> Sizeh = new List<int>();
                    List<int> Sizew = new List<int>();
                    List<CvRect> rec = new List<CvRect>();
                    List<int> Sizea = new List<int>();
                    listBox2.Items.Clear();
                    CvContourScanner ContourScanner = new CvContourScanner(newimageline, storage, CvContour.SizeOf, ContourRetrieval.External, ContourChain.ApproxSimple);


                    CvPoint h1 = new CvPoint
                    {
                        X = Cv.Round(0),
                        Y = Cv.Round(newimageline.Height * 0.5)
                    };

                    CvPoint h2 = new CvPoint
                    {
                        X = Cv.Round((0 + newimageline.Width)),
                        Y = h1.Y
                    };



                    CvPoint v1 = new CvPoint
                    {
                        X = Cv.Round(newimageline.Width * 0.5),
                        Y = Cv.Round(0)
                    };

                    CvPoint v2 = new CvPoint
                    {
                        X = v1.X,
                        Y = newimageline.Height
                    };

                    Originalimg.Line(v1, v2, CvColor.Orange, 1);

                    Originalimg.Line(h1, h2, CvColor.Orange, 1);


                    foreach (CvArr Contour in ContourScanner)
                    {
                        CvRect conrecx = Contour.BoundingRect();
                      
                        //conrecx.Y = conrecx.Y - 10;
                        //conrecx.Height = conrecx.Height + 5;
                        int Areax = conrecx.Width * conrecx.Height;

                        CvPoint ct = new CvPoint
                        {
                            X = Cv.Round((conrecx.X + conrecx.Width * 0.5)),
                            Y = Cv.Round((conrecx.Y + conrecx.Height * 0.5))
                        };

               


                        CvPoint2D32f Intersectionh = lineIntersection(h1,h2, ct, new CvPoint(ct.X, ct.Y + 10));

                        CvPoint2D32f Intersectionv = lineIntersection(v1, v2, ct, new CvPoint(ct.X+10, ct.Y));

                        // Originalimg.PutText(Intersection.Y+"", new CvPoint(ct.X, (int)ct.Y), font, CvColor.Green);

                        double dish = Math.Sqrt(Math.Pow((double)Intersectionh.X  - ct.X, 2.0) + Math.Pow((double)(Intersectionh.Y )  - ct.Y, 2.0));

                        double disv= Math.Sqrt(Math.Pow((double)Intersectionv.X - ct.X, 2.0) + Math.Pow((double)(Intersectionv.Y) - ct.Y, 2.0));

                        listBox2.Items.Add(Areax+"_"+Intersectionh.Y + "_" + ct.Y +"_"+ dish+"_"+disv);


                        if (Areax > 45 && Areax < 1000 && dish < 10 && disv < 30)
                        {
                            Sizea.Add(Areax);
                            rec.Add(conrecx);
                            Sizeh.Add(conrecx.Height);
                            listy.Add(conrecx.Y);
                          //  listBox2.Items.Add(Areax + "_i");
                            Originalimg.Rectangle(conrecx, CvColor.Green, 1);
                        }
                        else
                        {
                         //   listBox2.Items.Add(Areax + "_o");
                        }
                    }


                    //double avh = Sizeh.Average();
                    //label1.Text = avh.ToString();
                    //listBox2.Items.Clear();
                    //double av = avh * 2.5;
                    //rec.ForEach(m => { 

                    //    int s = m.Width * m.Height;


                    //    if ( m.Height > 5 && m.Height < av)
                    //    {
                    //        listBox2.Items.Add(m.Height + "_i");
                    //        Originalimg.Rectangle(m, CvColor.Green, 1);
                    //    }
                    //    else
                    //    {
                    //        listBox2.Items.Add(m.Height + "_o");

                    //    }


                    //});

                    //   double stdDev1 = CalculateStdDev(listy);
                    //    Cv.ShowImage("newimageline", Originalimg);

          
                    roi1.ResetROI();

                    if (pictureBox2.Image != null)
                    {
                        pictureBox2.Image.Dispose();
                    }
                    pictureBox2.Image = Originalimg.ToBitmap();

                    Cv.WaitKey(0);
         

                }
         
           

            }


        }

      

        CvPoint lineIntersection(CvPoint o1, CvPoint p1, CvPoint o2, CvPoint p2)
        {

            CvPoint x, d1, d2;
            CvPoint r;

            x.X = o2.X - o1.X;
            x.Y = o2.Y - o1.Y;

            d1.X = p1.X - o1.X;
            d1.Y = p1.Y - o1.Y;

            d2.X = p2.X - o2.X;
            d2.Y = p2.Y - o2.Y;

            float cross = d1.X * d2.Y - d1.Y * d2.X;
            double t1 = (x.X * d2.Y - x.Y * d2.X) / cross;


            r.X = Convert.ToInt32(o1.X + (d1.X * t1));
            r.Y = Convert.ToInt32(o1.Y + (d1.Y * t1));

            return r;

        }

        double CalculateStdDev(List<int> values)
        {
            double num = 0.0;
            if (values.Count<int>() > 0)
            {
                double avg = values.Average();
                num = Math.Sqrt(values.Sum<int>((Func<int, double>)(d => Math.Pow((double)d - avg, 2.0))) / (double)(values.Count<int>() - 1));
            }
            return num;
        }
        string btnname;
        private void button4_Click(object sender, EventArgs e)
        {
            //var da = sender as Button;
            //btnname = da.Text;
            string pdfFile = Path.GetFullPath(@"temp3\");
            var listfile = Directory.EnumerateFiles(pdfFile, "*.bmp");
            listBox1.Items.Clear();
            foreach (string file in listfile)
            {
                listBox1.Items.Add(file);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //var da = sender as Button;
            //btnname = da.Text;
            string pdfFile = Path.GetFullPath(@"output\");
            var listfile = Directory.EnumerateFiles(pdfFile, "*.bmp");
            listBox1.Items.Clear();
            IplImage dstLinear = new IplImage(new CvSize(6619, 4680), BitDepth.U8, 3);
            dstLinear.SetZero();
            //   dstLinear.Not(dstLinear);


            //Create the data set and table
            DataSet NewDataSet = new DataSet("New_DataSet");
            NewDataSet.Tables.Add("TEST");
            NewDataSet.Tables["TEST"].Columns.Add("Up");
            NewDataSet.Tables["TEST"].Columns.Add("Low");
            //DataTable dt = new DataTable("New_DataTable");

            ////Set the locale for each
            //ds.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
            //dt.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;

            ////Open a DB connection (in this example with OleDB)
            //OleDbConnection con = new OleDbConnection(dbConnectionString);
            //con.Open();

            ////Create a query and fill the data table with the data from the DB
            //string sql = "SELECT Whatever FROM MyDBTable;";
            //OleDbCommand cmd = new OleDbCommand(sql, con);
            //OleDbDataAdapter adptr = new OleDbDataAdapter();

            //adptr.SelectCommand = cmd;
            //adptr.Fill(dt);
            //con.Close();

            ////Add the table to the data set
            //ds.Tables.Add(dt);

            ////Here's the easy part. Create the Excel worksheet from the data set
            //ExcelLibrary.DataSetHelper.CreateWorkbook("MyExcelFile.xls", ds);

            foreach (string file in listfile)
            {
             FileInfo fileInfo = new FileInfo(file);

                int x = Convert.ToInt16(fileInfo.Name.Split('_')[1]);
                int y = Convert.ToInt16(fileInfo.Name.Split('_')[2]);
                int w = Convert.ToInt16(fileInfo.Name.Split('_')[3]);
                int h = Convert.ToInt16(fileInfo.Name.Split('_')[4]);
                CvRect rect = new CvRect(x, y, w, h);
                CvPoint ct = new CvPoint
                {
                    X = Cv.Round((rect.X + rect.Width * 0.2)),
                    Y = Cv.Round((rect.Y + rect.Height * 0.5))
                };
                String STRT = fileInfo.Name.Split('_')[5];
                String STRL = fileInfo.Name.Split('_')[6].Replace(".bmp","");

                DataRow toInsert = NewDataSet.Tables["TEST"].NewRow();
                toInsert[0] = STRT;
                toInsert[1] = STRL;
                NewDataSet.Tables["TEST"].Rows.Add(toInsert);

                //   dstLinear.Circle(ct, 2, CvColor.White);
                dstLinear.PutText(STRT, ct, new CvFont(FontFace.HersheyComplex, 0.8, 0.8), CvColor.White);
                dstLinear.PutText(STRL, new CvPoint(ct.X,ct.Y+35), new CvFont(FontFace.HersheyComplex, 0.8, 0.8), CvColor.White);
                dstLinear.Rectangle(rect, CvColor.Green, 2);




            }
            dataGridView1.DataSource = NewDataSet.Tables["TEST"];
            dstLinear.SaveImage("check.jpg", new JpegEncodingParam(50));


        //    Cv.ShowImage("dstLinear", dstLinear);
         //   Cv.WaitKey(0);
        }

        private void button6_Click(object sender, EventArgs e)
        {


            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"D:\NEW2024\A Project\Instrument.xlsx");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= colCount; j++)
                {
                    //new line
                    if (j == 1)
                        Console.Write("\r\n");

                    //write the value to the console
                    if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                        Console.Write(xlRange.Cells[i, j].Value2.ToString() + "\t");
                }
            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(@"C:\Users\s5801073810058\source\repos\patternRec\patternRec\bin\Debug\temp3\e14fa32f-f162-4a42-a15d-25bbc7a6b75d.bmp");
            string imgstring = ServiceGoogle.ConvertBitmapToString(bitmap);
           // ServiceGoogle.GoogleOCR(imgstring, bitmap,new CvRect());
        }
    }
}
