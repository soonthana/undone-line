using System;

namespace Undone.Line.BusinessModel
{
  public class UserProfiles
  {
    public Guid Id { get; set; }
    public string LineUserId { get; set; }
    public string LineDisplayName { get; set; }
    public string LinePictureUrl { get; set; }
    public string LineStatusMessage { get; set; }
    public string OfficerId { get; set; }
    public string OfficerName { get; set; }
    public string OfficerEmail { get; set; }
    public string OfficerPictureUrl { get; set; }
    public Nullable<DateTime> RegisteringDateTime { get; set; }
    public Nullable<DateTime> RegisteredDateTime { get; set; }
    public bool Status { get; set; }
    public string Role { get; set; }
  }
}