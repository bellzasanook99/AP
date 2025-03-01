using System;
using System.Collections.Generic;

namespace Core.Database.MLBufferDB;

public partial class TransFile
{
    public int Id { get; set; }

    public int? TransId { get; set; }

    public string? DocNo { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public int? ItemNo { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreaterDate { get; set; }

    public virtual Transaction? Trans { get; set; }

    public virtual ICollection<TransPage> TransPages { get; set; } = new List<TransPage>();
}
