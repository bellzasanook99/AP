using System;
using System.Collections.Generic;

namespace Core.Database.MLBufferDB;

public partial class Aproject
{
    public int Id { get; set; }

    public string? ProjectNo { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
