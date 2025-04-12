using Azure.Core;
using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
     static public class GoogleService
    {
        static public string GoogleOCR(string base64)
        {
            // bitmap.Save("output\\"+Guid.NewGuid()+".bmp");
            MdlGoogle.RootRes json = new MdlGoogle.RootRes();
            UriBuilder _url = new UriBuilder();
            string? strocr = null;
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



            MdlGoogle.Root2 root2 = new MdlGoogle.Root2()
            {
                requests = new List<MdlGoogle.Request2>
                {
                    new MdlGoogle.Request2()
                    {
                        image = new MdlGoogle.Image2
                        {
                            content = base64
                        },
                        features = new List<MdlGoogle.Feature2> { new MdlGoogle.Feature2 { type = "TEXT_DETECTION" } }
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
              //  JavaScriptSerializer js = new JavaScriptSerializer();
                var result = streamReader.ReadToEnd();
                // textBox1.Text = result;
                try
                {

                 json = JsonConvert.DeserializeObject<MdlGoogle.RootRes>(result);

                    if (json != null)
                    {
                        json.responses.ForEach(m =>
                        {
                            if (m.fullTextAnnotation != null)
                            {
                                m.fullTextAnnotation.text = m.fullTextAnnotation.text.Replace("\n", "_").Replace("/", "#");
                                // bitmap.Save("output\\" + num + "_" + cvRect.X + "_" + cvRect.Y + "_" + cvRect.Width + "_" + cvRect.Height + "_" + m.fullTextAnnotation.text + ".bmp");
                                strocr = m.fullTextAnnotation.text;
                            }
                        });
                      //  num++;

                    }



                }
                catch (Exception ex)
                {
                    json = null;
                    strocr = null;
                  //  MessageBox.Show(ex.Message);
                }


                //    date = json["datetime"];
                //    logs_stamp.stampslogs.stamp("CheckServer : " + date, "checkserver");
                // label3.Text = i + "";
            }
            return strocr;

        }
    }
}
