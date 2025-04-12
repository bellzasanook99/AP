using Core.Database.MLBufferDB;
using Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IConvertService
    {
        // List<TransFile> ConvertPdfToImge(List<IFormFile> formFile, string ProjectNo);
        List<TransFile> ConvertPdfToImge(List<IFormFile> formFile, string ProjectNo);
        List<TransFile> ConvertPdfToImge2(List<FilesList> PathList, string ProjectNo);
    }
}
