using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
    }
    public class MultipleFileUploadModel
    {
        public string Description { get; set; }
        public List<IFormFile> Files { get; set; }
    }
    public class FilesList
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
    }
}
