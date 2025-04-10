using System;
using System.Collections.Generic;

namespace AgencyPlatform.Core.Entities;

public partial class foto
{
    public int id { get; set; }

    public int acompanante_id { get; set; }

    public string url { get; set; } = null!;

    public bool? es_principal { get; set; }

    public int? orden { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual acompanante acompanante { get; set; } = null!;
}
