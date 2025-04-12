using Core.Database.MLBufferDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Order
    {
        public string ProjectNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Transaction> transaction { get; set; }
    }
}
