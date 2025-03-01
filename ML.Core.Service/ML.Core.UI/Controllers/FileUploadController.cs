
using Core.Models;
using Microsoft.AspNetCore.Mvc;
 

namespace ML.Core.UI.Controllers
{
    public class FileUploadController : Controller
    {
        [HttpGet]
        public IActionResult Upload()
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

            foreach (var file in model.Files)
            {
                if (file.Length > 0)
                {
                    // สร้าง path สำหรับบันทึกไฟล์
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);

                    // บันทึกไฟล์ลงดิสก์
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadedFiles.Add(filePath);
                }
            }

            ViewBag.Message = "Files uploaded successfully!";
            ViewBag.UploadedFiles = uploadedFiles;
            return View();
        }
    }
}
