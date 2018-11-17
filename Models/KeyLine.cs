using System;

namespace Undone.Line.BusinessModel
{
  public class KeyLine
  {
    public Providers Providers { get; set; }
  }

  public class ST17
  {
    public string AccessToken { get; set; }
    public string IconUrl { get; set; }
  }

  public class MTLITNOTIFY
  {
    public string AccessToken { get; set; }
    public string IconUrl { get; set; }
  }

  public class Channels
  {
    public ST17 ST17 { get; set; }
    public MTLITNOTIFY MTLITNOTIFY { get; set; }
  }

  public class SOONTHANA
  {
    public Channels Channels { get; set; }
  }

  public class Providers
  {
    public SOONTHANA SOONTHANA { get; set; }
  }
}