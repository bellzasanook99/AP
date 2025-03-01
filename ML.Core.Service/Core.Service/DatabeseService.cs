using Core.Database.MLBufferDB;
using Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public class DatabeseService: IDatabeseService
    {

        public void SaveDocFile(Aproject aproject)
        {
            using (var con = new MlbufferDbContext())
            {
               con.Aprojects.Add(aproject).Context.SaveChanges();
            }
            
        }
    }
}
