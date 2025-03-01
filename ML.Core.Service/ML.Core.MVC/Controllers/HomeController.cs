using Core.Database.MLBufferDB;
using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ML.Core.MVC.Models;
using System.Diagnostics;

namespace ML.Core.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDatabeseService _databeseService;

        public HomeController(ILogger<HomeController> logger, IDatabeseService databeseService)
        {
            _logger = logger;
            _databeseService = databeseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(MultipleFileUploadModel model)
        {
            if (model.Files == null || model.Files.Count == 0)
            {
                ViewBag.Message = "No files uploaded.";
                return View();
            }






            var uploadedFiles = new List<string>();
            List<TransFile> transFiles = new List<TransFile>();


            Guid myuuid = Guid.NewGuid();
            Aproject aproject = new Aproject();
            aproject.ProjectNo = myuuid.ToString();

            int count = 1;
            foreach (var file in model.Files)
            {
                if (file.Length > 0)
                {
                    List<TransPage> transPage = new List<TransPage>() { 
                        new TransPage { Px =1 , Py =23,Height = 26,Width = 20 }
                    ,new TransPage { Px =2 , Py =32,Height = 26,Width = 20 }
                    ,new TransPage { Px =3 , Py =44,Height = 26,Width = 20 }

                    };
            
                    // สร้าง path สำหรับบันทึกไฟล์
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);
                    transFiles.Add(new TransFile { DocNo = aproject.ProjectNo, FilePath = filePath, ItemNo = count, FileName = file.FileName, TransPages  = transPage });
                    // บันทึกไฟล์ลงดิสก์
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadedFiles.Add(filePath);
                    count++;
                }
            }


            aproject.Transactions = new List<Transaction>();
            aproject.Transactions.Add(new Transaction {TransFiles = transFiles });
        
            _databeseService.SaveDocFile(aproject);
            ViewBag.Message = "Files uploaded successfully!";
       //     ViewBag.UploadedFiles = uploadedFiles;
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
