using System;

namespace Undone.Line.BusinessModel
{
  public class ErrorPayload
  {
    public string errorId { get; set; }
    public string code { get; set; }
    public string messageToDeveloper { get; set; }
    public ErrorPayloadMessageToUserDetail messageToUser { get; set; }
    public string created { get; set; }
  }

  public class ErrorPayloadMessageToUserDetail
  {
    public string langTh { get; set; }
    public string langEn { get; set; }
  }
}