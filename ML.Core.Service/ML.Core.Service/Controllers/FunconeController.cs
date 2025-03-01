using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

using System.Drawing;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System;

namespace ML.Core.Service.Comtrollers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class FunconeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Auth()
        {
            string imagePath = "path_to_your_image.jpg";

            // ใช้ Cv2.ImRead เพื่ออ่านภาพ
            Mat image = Cv2.ImRead(imagePath, ImreadModes.Color);

            return Ok();
        }

       
        [HttpGet]
        public IActionResult Procressing()
        {
            string imagePath = @"C:\Users\s5801073810058\source\repos\patternRec\patternRec\bin\Debug\Result\Page1_0c432026-4ddd-40c6-a8e1-d4879feacd4f_ReSize.bmp";
          
            int Scales = 2;
            // ใช้ Cv2.ImRead เพื่ออ่านภาพ
            Mat iplImage = Cv2.ImRead(imagePath, ImreadModes.Color);
            Mat dstLinear = new Mat();
            Cv2.Resize(iplImage, dstLinear, new OpenCvSharp.Size(iplImage.Width / Scales, iplImage.Height / Scales));

            Mat gray = new Mat();
            Cv2.CvtColor(dstLinear, gray, ColorConversionCodes.BGR2GRAY);

            string haarPath = @"C:\Users\s5801073810058\source\repos\patternRec\patternRec\bin\Debug\A13.xml"; // ไฟล์ Haar Cascade
            var cascade = new CascadeClassifier(haarPath);
            const double ScaleFactor = 1.0850;
            const int MinNeighbors = 2;
            Rect[] faces = cascade.DetectMultiScale(gray, ScaleFactor, MinNeighbors, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(20, 20));
            List<Rect> rects = new List<Rect>();
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
                    // dstLinear.Circle(center, radius, CvColor.Red, 3, LineType.AntiAlias, 0);
                    dstLinear.Line(center, center, Scalar.Red, 10);
                    //dstLinear.Rectangle(r, CvColor.Green, 1);

                    Rect rec = new Rect(r.X * Scales, r.Y * Scales, r.Width * Scales, r.Height * Scales);
                    Cv2.Rectangle(iplImage, rec, Scalar.Red, 2); // วาดสี่เหลี่ยมสีแดง
                                                               // iplImage.Rectangle(rec, Scalar.Green, 2);
                }
       
              
               
            }
            byte[] imageBytes = iplImage.ToBytes(".jpg");
            return File(imageBytes, "image/jpeg");


            return Ok();
        }
    }
}
