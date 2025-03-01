using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ML.Core.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // สร้าง path สำหรับบันทึกไฟล์
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", model.File.FileName);

            // บันทึกไฟล์ลงดิสก์
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            return Ok(new { message = "File uploaded successfully!", filePath });
        }


        [RequestSizeLimit(52428800)] // 50 MB
        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultipleFiles([FromForm] MultipleFileUploadModel model)
        {
            if (model.Files == null || model.Files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            var uploadedFiles = new List<string>();

            foreach (var file in model.Files)
            {
                if (file.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    // สร้าง path สำหรับบันทึกไฟล์
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);

                    // บันทึกไฟล์ลงดิสก์
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadedFiles.Add(filePath);
                }
            }

            return Ok(new { message = "Files uploaded successfully!", uploadedFiles });
        }
    }
}
