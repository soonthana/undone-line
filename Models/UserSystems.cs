using System;

namespace Undone.Line.BusinessModel
{
  public class UserSystems
  {
    public Guid SystemId { get; set; }
    public string LineUserId { get; set; }
    public Guid UserProfileId { get; set; }
  }
}