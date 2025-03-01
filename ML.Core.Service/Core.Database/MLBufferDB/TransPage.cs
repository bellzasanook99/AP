using System;
using System.Collections.Generic;

namespace Core.Database.MLBufferDB;

public partial class TransPage
{
    public int Id { get; set; }

    public int? TransId { get; set; }

    public int? ItemNo { get; set; }

    public int? FileId { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? Px { get; set; }

    public int? Py { get; set; }

    public string? RecDrawingNo { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual TransFile? File { get; set; }
}
