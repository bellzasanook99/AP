using System;
using System.Collections.Generic;

namespace Core.Database.MLBufferDB;

public partial class TransPage
{
    public int Id { get; set; }

    public int? FileId { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? Px { get; set; }

    public int? Py { get; set; }

    public string? SymbolType { get; set; }

    public string? Hocr { get; set; }

    public string? Locr { get; set; }

    public string? RecDrawingNo { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual TransFile? File { get; set; }
}
