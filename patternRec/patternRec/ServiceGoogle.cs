using Newtonsoft.Json;
using OpenCvSharp;
using patternRec.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;


namespace patternRec
{
    public class ServiceGoogle
    {

        static int num = 0;

        static public string DrawingNoGoogleOCR(string base64, Bitmap bitmap, CvRect cvRect)
        {


            UriBuilder _url = new UriBuilder();
            
            string urlquery = "https://vision.googleapis.com/v1/images:annotate?key=AIzaSyDB98jskniFMSH-03EO7-07e7x6d6QuyVk";//_url.ToString().Replace("[", "").Replace("]", "");


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlquery);
            req.PreAuthenticate = true;
            req.Headers.Add("Content-Transfer-Encoding", "8bit");
            req.ContentType = "application/json";
            req.KeepAlive = true;
            req.Method = "POST";
            req.Timeout = Timeout.Infinite;



            Root2 root2 = new Root2()
            {
                requests = new List<Request2>
                {
                    new Request2()
                    {
                        image = new Models.Image2
                        {
                            content = base64
                        },
                        features = new List<Feature2> { new Feature2 { type = "TEXT_DETECTION" } }
                    }
                }
            };





            string datasent = JsonConvert.SerializeObject(root2, Formatting.Indented);

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(datasent);
            req.ContentLength = byte1.Length;
            var newStream = req.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);

            var httpResponse = (HttpWebResponse)req.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //  i++;
                JavaScriptSerializer js = new JavaScriptSerializer();
                var result = streamReader.ReadToEnd();
                // textBox1.Text = result;
                try
                {

                    RootRes json = JsonConvert.DeserializeObject<RootRes>(result);

                    if (json != null)
                    {
                        json.responses.ForEach(m =>
                        {
                            m.fullTextAnnotation.text = m.fullTextAnnotation.text.Replace("\n", "_").Replace("/", "#");
                           // bitmap.Save("output\\" + num + "_" + cvRect.X + "_" + cvRect.Y + "_" + cvRect.Width + "_" + cvRect.Height + "_" + m.fullTextAnnotation.text + ".bmp");
                        });
                        

                    }


                    return json.responses.FirstOrDefault().fullTextAnnotation.text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
            return "";
        }

        static public void GoogleOCR(string base64,Bitmap bitmap, CvRect cvRect,string OcrDrawingNo,string DocRef)
    {
           // bitmap.Save("output\\"+Guid.NewGuid()+".bmp");

            UriBuilder _url = new UriBuilder();
            //_url.Scheme = "https";
            //_url.Host = "vision.googleapis.com/v1/images:annotate?key=AIzaSyDM6z3VyqPG7TiMEzXSeb-5S0Aa12pUyAM";
            string urlquery = "https://vision.googleapis.com/v1/images:annotate?key=AIzaSyDB98jskniFMSH-03EO7-07e7x6d6QuyVk";//_url.ToString().Replace("[", "").Replace("]", "");


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlquery);
            req.PreAuthenticate = true;
            req.Headers.Add("Content-Transfer-Encoding", "8bit");
            req.ContentType = "application/json";
            req.KeepAlive = true;
            req.Method = "POST";
            req.Timeout = Timeout.Infinite;

            //Root  root = new Root() {  requests =
            // new List<Request>
            //{
            //     new Request()
            //    {
            //         image = new Models.Image()
            //         { source = new Source()
            //         { imageUri = "http://124.121.94.134:8095/0e4a6187-e7e3-45db-9a39-b86e56ceeabd.bmp"}},
            //          features = new List<Feature>(){ new Feature { type = "TEXT_DETECTION" } }

            //    }
            //}
            //};


            Root2 root2 = new Root2()
            {
                requests = new List<Request2>
                {
                    new Request2()
                    {
                        image = new Models.Image2
                        {
                            content = base64
                        },
                        features = new List<Feature2> { new Feature2 { type = "TEXT_DETECTION" } }
                    }
                }
            };





            string datasent = JsonConvert.SerializeObject(root2, Formatting.Indented);

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(datasent);
            req.ContentLength = byte1.Length;
            var newStream = req.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);

            var httpResponse = (HttpWebResponse)req.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //  i++;
                JavaScriptSerializer js = new JavaScriptSerializer();
                var result = streamReader.ReadToEnd();
                // textBox1.Text = result;
                try
                {

                    RootRes json = JsonConvert.DeserializeObject<RootRes>(result);

                    if (json != null)
                    {
                        json.responses.ForEach(m =>
                        {
                            m.fullTextAnnotation.text = m.fullTextAnnotation.text.Replace("\n", "_").Replace("/","#");
                            bitmap.Save("output\\" + num + "_" + cvRect.X + "_" + cvRect.Y + "_" + cvRect.Width + "_" + cvRect.Height + "_" + m.fullTextAnnotation.text + ".bmp");
                        });
                        num++;

                    }



                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                //    date = json["datetime"];
                //    logs_stamp.stampslogs.stamp("CheckServer : " + date, "checkserver");
                // label3.Text = i + "";
            }

        }
        static public string ConvertBitmapToString(Bitmap bitmap)
        {
            // Convert the bitmap to a byte array
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] byteImage = ms.ToArray();

            // Convert the byte array to a base64 string
            string base64String = Convert.ToBase64String(byteImage);

            return base64String;
        }
    }
}
