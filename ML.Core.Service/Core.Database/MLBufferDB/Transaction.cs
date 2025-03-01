using System;
using System.Collections.Generic;

namespace Core.Database.MLBufferDB;

public partial class Transaction
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Aproject? Project { get; set; }

    public virtual ICollection<TransFile> TransFiles { get; set; } = new List<TransFile>();
}
