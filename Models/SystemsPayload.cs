using System;
using System.Collections.Generic;

namespace Undone.Line.BusinessModel
{
  public class SystemsPayload : Systems
  {
    public SystemsPayloadLinksDetail links { get; set; }
  }

  public class SystemsPayloadLinksDetail
  {
    public string rel { get; set; }
    public string href { get; set; }
    public string action { get; set; }
    public List<string> types { get; set; }
  }
}