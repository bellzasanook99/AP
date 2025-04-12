using Core.Database.MLBufferDB;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IDatabeseService
    {
        void SaveDocFile(Aproject aproject);
        Aproject GetAproject(string ProjectNo);
        List<TransPage> GetTransPages(int FoleId);
        List<TransFile> GetTransFiles(int FoleId);
    }
}
