using System;

namespace Undone.Line.BusinessModel
{
  public class OneTimePasswords
  {
    public Guid UserProfileId { get; set; }
    public string OTP { get; set; }
    public Nullable<DateTime> CreatedDateTime { get; set; }
    public Nullable<DateTime> ExpiryDateTime { get; set; }
    public string Type { get; set; }
    public Nullable<bool> Status { get; set; }
  }
}