using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModuleAI.Commons
{
    public static class HttpListenerService
    {
        private static Task _mainLoop;
        public static int Port { get; set; }
        private static bool _keepGoing = true;
        private static HttpListener Listener = new HttpListener { Prefixes = { $"http://*:{Port}/" } };

        public static void StartWebServer()
        {
            Listener = new HttpListener { Prefixes = { $"http://*:{Port}/" } };
            if (_mainLoop != null && !_mainLoop.IsCompleted)
            {
                return; //Already started
            }
            _mainLoop = MainLoop();
        }

        public static void StopWebServer()
        {

            _keepGoing = false;
            lock (Listener)
            {
                //Use a lock so we don't kill a request that's currently being processed
                Listener.Stop();
            }
            try
            {
                _mainLoop.Wait();
            }
            catch { /* je ne care pas */ }
        }

        private static async Task MainLoop()
        {

            Listener.Start();
            Listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            Listener.UnsafeConnectionNtlmAuthentication = true;
            Listener.IgnoreWriteExceptions = true;
            while (_keepGoing)
            {
                try
                {
                    //GetContextAsync() returns when a new request come in
                    var context = await Listener.GetContextAsync();
                    lock (Listener)
                    {


                        if (_keepGoing)
                        {
                            //ProcessRequest(context);
                            ThreadPool.QueueUserWorkItem(ProcessRequest, context);

                        }

                    }
                }
                catch (Exception e)
                {

                    if (e is HttpListenerException) return; //this gets thrown when the listener is stopped
                    //TODO: Log the exception
                }
            }
        }


        public static void ProcessRequest(object contextx)
        {
            new Thread(() =>
            {
                HttpListenerContext context = (HttpListenerContext)contextx;
                HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)context.User.Identity;
                if (identity.Name == "admin" && identity.Password == "admin")
                {
                    using (var response = context.Response)
                    {

                        try
                        {
                            ServicePointManager.Expect100Continue = false;
                            ServicePointManager.DefaultConnectionLimit = 200;
                            var handled = false;

                            switch (context.Request.Url.AbsolutePath)
                            {
                                case "/api/gettime":
                                    switch (context.Request.HttpMethod)
                                    {
                                        case "GET":

                                            using (var body = context.Request.InputStream)
                                            using (var reader = new StreamReader(body, Encoding.UTF8))
                                            {
                                                //Get the data that was sent to us
                                                var json = reader.ReadToEnd();


                                                var responseBody = JsonConvert.SerializeObject(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                                                //Write it to the response stream
                                                var buffer = Encoding.UTF8.GetBytes(responseBody);
                                                response.ContentLength64 = buffer.Length;
                                                // Thread.Sleep(100);
                                                response.OutputStream.Write(buffer, 0, buffer.Length);
                                                handled = true;
                                            }
                                            break;
                                    }
                                    break;

                                case "/api/PortalSMS":
                                    switch (context.Request.HttpMethod)
                                    {
                                        case "POST":


                                            //using (var body = context.Request.InputStream)
                                            //using (var reader = new StreamReader(body, Encoding.UTF8))
                                            //{
                                            //    var json = reader.ReadToEnd();
                                            //    MdlSMSModule rootObject = JsonConvert.DeserializeObject<MdlSMSModule>(json);

                                            //    Form1.mdlSMSModule = rootObject;

                                            //    var responseBody = JsonConvert.SerializeObject(rootObject);

                                            //    //Write it to the response stream
                                            //    var buffer = Encoding.UTF8.GetBytes(responseBody);
                                            //    response.ContentLength64 = buffer.Length;
                                            //    // Thread.Sleep(100);
                                            //    response.OutputStream.Write(buffer, 0, buffer.Length);
                                            //    handled = true;
                                            //}

                                            break;
                                    }

                                    handled = true;
                                    break;

                                case "/api/Upload":
                                    switch (context.Request.HttpMethod)
                                    {
                                        case "POST":

                                            if (context.Request.QueryString.Count > 0)
                                            {
                                                //string data = context.Request.QueryString.Get(0);
                                                ////     response.ContentType = "image/png";
                                                //string path = "C:\\Upload\\";
                                                ////DirectoryInfo dirx = new DirectoryInfo(path);
                                                ////foreach (FileInfo xxxx in dirx.GetFiles())
                                                ////{
                                                ////    xxxx.Delete();
                                                ////}

                                                //SaveFile(data, path, context.Request.ContentEncoding, GetBoundary(context.Request.ContentType), context.Request.InputStream);
                                                //MdlDataRes mdlDataRes = null;
                                                //new Thread(() =>
                                                //{
                                                //    mdlDataRes = Form1.Loadimg(path + data);

                                                //}).Start();

                                                //while (mdlDataRes == null)
                                                //{
                                                //    Thread.Sleep(1000);
                                                //}

                                                //var responseBody = JsonConvert.SerializeObject(mdlDataRes);
                                                //var buffer = Encoding.UTF8.GetBytes(responseBody);
                                                //response.ContentLength64 = buffer.Length;
                                                //// Thread.Sleep(100);
                                                //response.OutputStream.Write(buffer, 0, buffer.Length);
                                            }
                                            else
                                            {
                                                //var buffer = Encoding.UTF8.GetBytes("Require : /api/Upload?filename=xxxx.xxx");
                                                //response.ContentLength64 = buffer.Length;
                                                //// Thread.Sleep(100);
                                                //response.OutputStream.Write(buffer, 0, buffer.Length);
                                            }

                                            //using (StreamWriter writer = new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
                                            //{

                                            //}
                                            //     writer.WriteLine("File Uploaded");
                                            break;
                                    }
                                    handled = true;
                                    break;

                            }


                        }
                        catch (Exception e)
                        {
                            try
                            {
                                //Return the exception details the client - you may or may not want to do this
                                response.StatusCode = 500;
                                response.ContentType = "application/json";
                                // var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
                                var buffer = Encoding.UTF8.GetBytes(e.Message);
                                response.ContentLength64 = buffer.Length;
                                response.OutputStream.Write(buffer, 0, buffer.Length);
                            }
                            catch (Exception ex) { }

                            //TODO: Log the exception
                        }
                    }
                }

            }).Start();
        }


        private static String GetBoundary(String ctype)
        {
            return "--" + ctype.Split(';')[1].Split('=')[1];
        }

        private static void SaveFile(string filename, string path, Encoding enc, String boundary, Stream input)
        {
            Byte[] boundaryBytes = enc.GetBytes(boundary);
            Int32 boundaryLen = boundaryBytes.Length;
            byte[] bytes;
            using (FileStream output = new FileStream(path + filename, FileMode.Create, FileAccess.Write))
            {
                Byte[] buffer = new Byte[1024];
                Int32 len = input.Read(buffer, 0, 1024);
                Int32 startPos = -1;

                // Find start boundary
                while (true)
                {
                    if (len == 0)
                    {
                        throw new Exception("Start Boundaray Not Found");
                    }

                    startPos = IndexOf(buffer, len, boundaryBytes);
                    if (startPos >= 0)
                    {
                        break;
                    }
                    else
                    {
                        Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
                        len = input.Read(buffer, boundaryLen, 1024 - boundaryLen);
                    }
                }

                // Skip four lines (Boundary, Content-Disposition, Content-Type, and a blank)
                for (Int32 i = 0; i < 4; i++)
                {
                    while (true)
                    {
                        if (len == 0)
                        {
                            throw new Exception("Preamble not Found.");
                        }

                        startPos = Array.IndexOf(buffer, enc.GetBytes("\n")[0], startPos);
                        if (startPos >= 0)
                        {
                            startPos++;
                            break;
                        }
                        else
                        {
                            len = input.Read(buffer, 0, 1024);
                        }
                    }
                }

                Array.Copy(buffer, startPos, buffer, 0, len - startPos);
                len = len - startPos;

                while (true)
                {
                    Int32 endPos = IndexOf(buffer, len, boundaryBytes);
                    if (endPos >= 0)
                    {
                        if (endPos > 0) output.Write(buffer, 0, endPos - 2);
                        break;
                    }
                    else if (len <= boundaryLen)
                    {
                        throw new Exception("End Boundaray Not Found");
                    }
                    else
                    {
                        output.Write(buffer, 0, len - boundaryLen);
                        Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
                        len = input.Read(buffer, boundaryLen, 1024 - boundaryLen) + boundaryLen;


                    }
                }
            }

        }

        private static Int32 IndexOf(Byte[] buffer, Int32 len, Byte[] boundaryBytes)
        {
            for (Int32 i = 0; i <= len - boundaryBytes.Length; i++)
            {
                Boolean match = true;
                for (Int32 j = 0; j < boundaryBytes.Length && match; j++)
                {
                    match = buffer[i + j] == boundaryBytes[j];
                }

                if (match)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
