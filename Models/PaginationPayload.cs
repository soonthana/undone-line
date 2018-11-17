using System;

namespace Undone.Line.BusinessModel
{
  public class PaginationPayload
  {
    public int offset { get; set; }
    public int limit { get; set; }
    public int total { get; set; }
    public PaginationPayloadLinksDetail links { get; set; }
  }

  public class PaginationPayloadLinksDetail
  {
    public string next { get; set; }
    public string prev { get; set; }
    public string first { get; set; }
    public string last { get; set; }
  }
}