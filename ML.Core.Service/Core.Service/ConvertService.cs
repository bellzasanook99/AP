using Core.Database.MLBufferDB;
using Core.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SautinSoft;
using System.IO;
using OpenCvSharp;
using System.Security.Cryptography;
using Core.Models;
using SkiaSharp;

namespace Core.Service
{
    public class ConvertService: IConvertService
    {

        public List<TransFile> ConvertPdfToImge2(List<FilesList> PathList, string ProjectNo)
        {
            List<TransFile> transFiles = new List<TransFile>();
            PdfFocus f = new PdfFocus();
            var filePath = Path.Combine("D:\\", "wwwroot\\uploads");
            foreach (var item in PathList)
            {
                f.OpenPdf(@"" + item.FullPath);

                if (f.PageCount > 0)
                {
                    f.ImageOptions.ImageFormat = SautinSoft.PdfFocus.CImageOptions.ImageFormats.Jpeg;
                    f.ImageOptions.Dpi = 400;
                    f.ImageOptions.JpegQuality = 100;

                    int count = 1;
                    for (int page = 1; page <= f.PageCount; page++)
                    {

                        string jpegFile = Path.Combine(filePath, String.Format("Page{0}_{1}_ReSize.bmp", page, Guid.NewGuid()));

                        int result = f.ToImage(jpegFile);
                        transFiles.Add(new TransFile { DocNo = ProjectNo, FilePath = jpegFile, ItemNo = count, FileName = item.FileName });
                        //   int result = f.ToImages(jpegFile, page);
                        count++;
                    }


                  
                }
            }
            f.ClosePdf();
            foreach (var item in PathList)
            {
                FileInfo fileInfo = new FileInfo(item.FullPath);
                fileInfo.Delete();
            }
            return transFiles;
        }

        public List<TransFile> ConvertPdfToImge(List<IFormFile> formFile,string ProjectNo)
        {
             List<TransFile> transFiles = new List<TransFile>();
             PdfFocus f = new PdfFocus();
        
             //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
             var filePath = Path.Combine("D:\\", "wwwroot\\uploads");





            foreach (var item in formFile)
            {
                Stream fileStream = item.OpenReadStream();
                f.OpenPdf(fileStream);
                //using (var stream = new FileStream("path_to_save_file", FileMode.Create))
                //{
                //    await fileStream.CopyToAsync(stream);
                //}

                if (f.PageCount > 0)
                {
                    f.ImageOptions.ImageFormat = SautinSoft.PdfFocus.CImageOptions.ImageFormats.Jpeg;
                    f.ImageOptions.Dpi = 400;
                    f.ImageOptions.JpegQuality = 100;

                    int count = 1;
                    for (int page = 1; page <= f.PageCount; page++)
                    {

                        string jpegFile = Path.Combine(filePath, String.Format("Page{0}_{1}_ReSize.bmp", page, Guid.NewGuid()));

                        int result = f.ToImage(jpegFile);

                        transFiles.Add(new TransFile { DocNo = ProjectNo, FilePath = jpegFile, ItemNo = count, FileName = item.FileName });

               
                        count++;
                    }
                }
            }
            f.ClosePdf();




            return transFiles;
        }

    }
}
