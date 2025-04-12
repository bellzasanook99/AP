using Core.Database.MLBufferDB;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IProcressingService
    {
        MdlGenPage Procressing(byte[] bytes);
        List<TransFile> Procressingtotf(List<TransFile> transFiles);
        byte[] SimmulatorData(TransFile transPage);
      
    }
}
