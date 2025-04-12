using Core.Database.MLBufferDB;
using Core.Interface;
using Core.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using ML.Core.MVC.Models;
using OpenCvSharp;
using System.Diagnostics;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System;
using System.Collections;
using Microsoft.AspNetCore.Http;
using SautinSoft;
using SkiaSharp;

namespace ML.Core.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDatabeseService _databeseService;
        private readonly IProcressingService _procressingService;
        private readonly IConvertService _convertService;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IDatabeseService databeseService, IProcressingService procressingService, IConvertService convertService, IWebHostEnvironment env)
        {
            _logger = logger;
            _databeseService = databeseService;
            _procressingService = procressingService;
            _convertService = convertService;
            _env = env;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
       
        // GET: My/Modal
        public ActionResult Modal()
        {
            var model = new MyModel(); // Initialize your model
            return PartialView("_ModalContent", model);
        }

        [HttpPost]
        public ActionResult SubmitForm(MyModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the form data (e.g., save to database)
           

                // Return the updated partial view
                return PartialView("_ModalContent", model);
            }

            // If the model state is invalid, return the partial view with validation errors
            return PartialView("_ModalContent", model);
        }



        public IActionResult Result(string ProjectNo)
        {
         

          var aproject1 = _databeseService.GetAproject(ProjectNo);

            return View(aproject1);
        }


        [HttpPost]
        public IActionResult Index([FromForm] MultipleFileUploadModel model)
        {
            if (model.Files == null || model.Files.Count == 0)
            {
                ViewBag.Message = "No files uploaded.";
                //return View();
                return Json(new { success = false, redirectUrl = Url.Action("Index", "Home") });
            }






            var uploadedFiles = new List<string>();

            Guid myuuid = Guid.NewGuid();
            Aproject aproject = new Aproject();
            aproject.ProjectNo = myuuid.ToString();
            var transFiles = _convertService.ConvertPdfToImge(model.Files, myuuid.ToString());
            var tfnew = _procressingService.Procressingtotf(transFiles);
            aproject.Transactions = new List<Transaction>();
            aproject.Transactions.Add(new Transaction { TransFiles = tfnew });
            _databeseService.SaveDocFile(aproject);

            //Aproject aproject = _databeseService.GetAproject("4081673b-d08a-4534-bee2-e169000496c8");  //sim

            //foreach (var item in aproject.Transactions)
            //{
            //    foreach (var item1 in item.TransFiles)
            //    {
            //        _procressingService.SimmulatorData(item1.TransPages);

            //    }
            //}



            return Json(new { success = true, redirectUrl = Url.Action("Result", "Home", new { ProjectNo = aproject.ProjectNo }) });
        }

        [HttpPost]
        public ActionResult simmulator()
        {
            Aproject aproject = _databeseService.GetAproject("4081673b-d08a-4534-bee2-e169000496c8");  //sim

            //foreach (var item in aproject.Transactions)
            //{
            //    foreach (var item1 in item.TransFiles)
            //    {
            //        _procressingService.SimmulatorData(item1);

            //    }
            //}

            return RedirectToAction("Result", "Home",new { ProjectNo = aproject.ProjectNo });
        }

        public ActionResult DownloadSimulator(int fileId)
        {
            var transFiles = _databeseService.GetTransFiles(fileId).FirstOrDefault();
            if(transFiles != null)
            {
               var imageb = _procressingService.SimmulatorData(transFiles);
                return File(imageb, "image/jpeg");
            }
            return Json(new { success = false, message = "Error" });
        }

        public ActionResult DownloadFileAny(int fileId)
        {
            var transPages = _databeseService.GetTransPages(fileId);
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                foreach (var pager in transPages)
                {
                    streamWriter.WriteLine($"{pager.Id},{pager.Px},{pager.Py},{pager.Hocr},{pager.Locr}");
                }
                var filename = $"data_{DateTime.Now:yyyyMMddHHmmss}.csv";
                Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");
                return File(memoryStream.ToArray(), "text/csv");

            }

            return Json(new { success = true, message = "File uploaded successfully." });
        }

        [HttpPost]
        public ActionResult DownloadFile(int fileId)
        {
            var transPages = _databeseService.GetTransPages(fileId);
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                foreach (var pager in transPages)
                {
                    streamWriter.WriteLine($"{pager.Id},{pager.Px},{pager.Py},{pager.Hocr},{pager.Locr}");
                }
                  var filename = $"data_{DateTime.Now:yyyyMMddHHmmss}.csv";
                   Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");
                return File(memoryStream.ToArray(), "text/csv");
                
            }

            return Json(new { success = true, message = "File uploaded successfully." });
        }

        [HttpPost]
        //public async Task<FileContentResult> Upload(MultipleFileUploadModel model)
        public async Task<IActionResult> Upload(MultipleFileUploadModel model)
        {
            if (model.Files == null || model.Files.Count == 0)
            {
                ViewBag.Message = "No files uploaded.";
                //  return new FileContentResult();
                //  return View();
            }


         


            var uploadedFiles = new List<string>();
            //List<TransFile> transFiles = new List<TransFile>();


            Guid myuuid = Guid.NewGuid();
            Aproject aproject = new Aproject();
            aproject.ProjectNo = myuuid.ToString();


            var transFiles = _convertService.ConvertPdfToImge(model.Files, aproject.ProjectNo);


            //foreach (var item in transFiles)
            //{
            //    List<TransPage> transPage = new List<TransPage>();
            //    FileInfo fileInfo = new FileInfo(item.FilePath);

            //    // The byte[] to save the data in
            //    byte[] data = new byte[fileInfo.Length];

            //    // Load a filestream and put its content into the byte[]
            //    using (FileStream fs = fileInfo.OpenRead())
            //    {
            //        fs.Read(data, 0, data.Length);
            //    }

            //    var output = _procressingService.Procressing(data);
            //    fileInfo.Delete();
            //    output.ForEach(m =>
            //    {
            //        try
            //        {
            //            transPage.Add(new TransPage { Px = m.rectangle.X, Py = m.rectangle.Y, Height = m.rectangle.Height, Width = m.rectangle.Width, Hocr = m.ocrstr.Split('_')[0], Locr = m.ocrstr.Split('_')[1] });

            //        }
            //        catch (Exception ex)
            //        {

            //        }

            //    });
            //    item.TransPages = transPage;
            //}





            aproject.Transactions = new List<Transaction>();
            aproject.Transactions.Add(new Transaction { TransFiles = transFiles });

            _databeseService.SaveDocFile(aproject);

            //     ViewBag.UploadedFiles = uploadedFiles;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/test.csv");
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            //using (var streamWriter = new StreamWriter(filePath))
            {

                foreach (var row in aproject.Transactions)
                {
                    streamWriter.WriteLine($"{row.Project.ProjectNo}");
                    foreach (var filer in row.TransFiles)
                    {
                        streamWriter.WriteLine($"{filer.Id},{filer.TransId},{filer.DocNo}");
                        foreach (var pager in filer.TransPages)
                        {
                            streamWriter.WriteLine($"{pager.Id},{pager.Px},{pager.Py},{pager.Hocr},{pager.Locr}");
                        }
                    }


                }

                // Ensure all data is written to the stream
                streamWriter.Flush();
                ViewBag.Message = "Files uploaded successfully!";
               // Convert the stream to a byte array\
               return File(memoryStream.ToArray(), "text/csv", aproject.ProjectNo + ".csv");
              //  return File(new byte[4], "text/csv", aproject.ProjectNo + ".csv");

            }
            //   return View("Index");
            return Ok(transFiles);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
