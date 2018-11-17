using System;
using System.Collections.Generic;

namespace Undone.Line.BusinessModel
{
  public class UserProfilesPayload : UserProfiles
  {
    public UserProfilesPayloadLinksDetail links { get; set; }
  }

  public class UserProfilesPayloadLinksDetail
  {
    public string rel { get; set; }
    public string href { get; set; }
    public string action { get; set; }
    public List<string> types { get; set; }
  }
}