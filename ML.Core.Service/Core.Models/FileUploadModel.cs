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
        public List<IFormFile> Files { get; set; }
    }
}
