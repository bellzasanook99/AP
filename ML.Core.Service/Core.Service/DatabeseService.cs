using Core.Database.MLBufferDB;
using Core.Interface;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
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


        public List<TransFile> GetTransFiles(int FoleId)
        {
            using (var con = new MlbufferDbContext())
            {
                var tf = con.TransFiles.Where(m => m.Id == FoleId).ToList();
                foreach (var item1 in tf)
                {
                    item1.TransPages = con.TransPages.Where(m => m.FileId == item1.Id).ToList();
                }

                return tf.ToList();
            }
        }


        public List<TransPage> GetTransPages(int FoleId)
        {
            using (var con = new MlbufferDbContext())
            {
                return con.TransPages.Where(m=>m.FileId == FoleId).ToList();
            }
        }

        public  Aproject GetAproject(string ProjectNo) {



            using (var con = new MlbufferDbContext())
            {

                var ap = con.Aprojects.Where(m => m.ProjectNo == ProjectNo).FirstOrDefault();

                if (ap != null)
                {
                    ap.Transactions = con.Transactions.Where(m=> m.ProjectId == ap.Id).ToList();
                    if(ap.Transactions.Count >0)
                    {
                        foreach (var item in ap.Transactions)
                        {
                            item.TransFiles = con.TransFiles.Where(m => m.TransId == item.Id).ToList();

                            foreach (var item1 in item.TransFiles)
                            {
                                item1.TransPages = con.TransPages.Where(m => m.FileId == item1.Id).ToList();
                            }

                        }
                    }
                }

         

                return ap;
            }
        }
    }
}
