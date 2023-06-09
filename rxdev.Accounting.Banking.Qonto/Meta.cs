using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace rxdev.Accounting.Banking.Qonto;
internal class Meta
{
    [JsonPropertyName("next_page")]
    public int? NextPage { get; set; }
}
