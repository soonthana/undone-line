using System;
using System.Collections.Generic;

namespace Undone.Line.BusinessModel
{
  public class SendGridMailSendApi
  {
    public class Mail
    {
      public List<Personalization> personalizations { get; set; }
      public From from { get; set; }
      public ReplyTo reply_to { get; set; }
      public string subject { get; set; }
      public List<Content> content { get; set; }
      public List<Attachments> attachments { get; set; }
      public string template_id { get; set; }
      public object sections { get; set; }
      public object headers { get; set; }
      public List<string> categories { get; set; }
      public object custom_args { get; set; }
      public int send_at { get; set; }
      public string batch_id { get; set; }
      public Asm asm { get; set; }
      public string ip_pool_name { get; set; }
      public MailSettings mail_settings { get; set; }
      public TrackingSettings tracking_settings { get; set; }
    }

    public class Personalization
    {
      public List<To> to { get; set; }
      public List<Cc> cc { get; set; }
      public List<Bcc> bcc { get; set; }
      public string subject { get; set; }
      public object headers { get; set; }
      public object substitutions { get; set; }
      public object custom_args { get; set; }
      public int send_at { get; set; }
    }

    public class To
    {
      public string email { get; set; }
      public string name { get; set; }
    }

    public class Cc
    {
      public string email { get; set; }
      public string name { get; set; }
    }

    public class Bcc
    {
      public string email { get; set; }
      public string name { get; set; }
      public bool enable { get; set; }
    }

    public class From
    {
      public string email { get; set; }
      public string name { get; set; }
    }

    public class ReplyTo
    {
      public string email { get; set; }
      public string name { get; set; }
    }

    public class Content
    {
      public string type { get; set; }
      public string value { get; set; }
    }

    public class Attachments
    {
      public string content { get; set; }
      public string type { get; set; }
      public string filename { get; set; }
      public string disposition { get; set; }
      public string content_id { get; set; }
    }

    public class Asm
    {
      public int group_id { get; set; }
      public List<int> groups_to_display { get; set; }
    }

    public class MailSettings
    {
      public Bcc bcc { get; set; }
      public ByPassListManagement bypass_list_management { get; set; }
      public Footer footer { get; set; }
      public SandboxMode sandbox_mode { get; set; }
      public SpamCheck spam_check { get; set; }
    }

    public class ByPassListManagement
    {
      public bool enable { get; set; }
    }

    public class Footer
    {
      public bool enable { get; set; }
      public string text { get; set; }
      public string html { get; set; }
    }

    public class SandboxMode
    {
      public bool enable { get; set; }
    }

    public class SpamCheck
    {
      public bool enable { get; set; }
      public int threshold { get; set; }
      public string post_to_url { get; set; }
    }

    public class TrackingSettings
    {
      public ClickTracking click_tracking { get; set; }
      public OpenTracking open_tracking { get; set; }
      public SubscriptionTracking subscription_tracking { get; set; }
      public Ganalytics ganalytics { get; set; }
    }

    public class ClickTracking
    {
      public bool enable { get; set; }
      public bool enable_text { get; set; }
    }

    public class OpenTracking
    {
      public bool enable { get; set; }
      public string substitution_tag { get; set; }
    }

    public class SubscriptionTracking
    {
      public bool enable { get; set; }
      public string text { get; set; }
      public string html { get; set; }
      public string substitution_tag { get; set; }
    }

    public class Ganalytics
    {
      public bool enable { get; set; }
      public string utm_source { get; set; }
      public string utm_medium { get; set; }
      public string utm_term { get; set; }
      public string utm_content { get; set; }
      public string utm_campaign { get; set; }
    }
  }
}