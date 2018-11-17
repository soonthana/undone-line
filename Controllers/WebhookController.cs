using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Undone.Line.BusinessModel;
using Undone.Line.Services;
using Undone.Line.Utils;

namespace Undone.Line.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class WebhookController : ControllerBase
  {
    private IConfiguration _config;
    private IHostingEnvironment _hostingEnv;
    private Firebase _fireObj;
    private string _SYSTEM_NAME = string.Empty;
    private string _LINE_CHANNEL_ICON = string.Empty;
    private string _LINE_CHANNEL_ACCESS_TOKEN = string.Empty;
    private string _LINE_MESSAGING_API_BASE_ADDRESS = string.Empty;
    private string _MTL_EMPLOYEE_API_BASE_ADDRESS = string.Empty;
    private string _MTL_AUTH_TOKEN_BASE_ADDRESS = string.Empty;
    private string _MTL_CLIENT_ID = string.Empty;
    private string _SENDGRID_API_KEY = string.Empty;
    private string _SENDGRID_API_BASE_ADDRESS = string.Empty;
    private string _NOTIFY_BASE_ADDRESS = string.Empty;
    private string _WORDING_EMAIL_OTP = "<html><img src=\"#[LINECHANNELICON]#\" title=\"รูปโลโก้ MTL-IT-Notify\" style=\"width: 100px; height: 100px;\" /><p>มีการลงทะเบียนเพื่อที่จะใช้งาน MTL-IT-Notify ด้วย e-mail address นี้</p><p>กรุณานำรหัส One-Time Password <b>#[OTP]#</b> นี้ ไปกรอกที่ MTL-IT-Notify (รหัสมีอายุการใช้งานภายใน #[EXPIRYDATETIME]# เท่านั้น)</p><p>ต้องขออภัย ถ้าหากคุณไม่ได้เป็นผู้ร้องขอรหัส One-Time Password ในครั้งนี้ โปรดคลิก <a href = \"#[NOTIFYADMINURL]#\">ปฏิเสธการลงทะเบียนนี้</a> เพื่อให้เรารู้</p></html>";
    private string _OFFICER_PICTURE_URL = "https://firebasestorage.googleapis.com/v0/b/stntestproj.appspot.com/o/#[OBJECTNAME]#?alt=media";

    public WebhookController(IConfiguration config, IHostingEnvironment hostingEnv)
    {
      _config = config;
      _hostingEnv = hostingEnv;
      _fireObj = new Firebase(_config);

      _SYSTEM_NAME = _config["SystemName"];

      var fileLineJson = System.IO.File.ReadAllText(_config["Line:Key"]);
      var lineJson = JsonConvert.DeserializeObject<KeyLine>(fileLineJson);
      _LINE_CHANNEL_ICON = lineJson.Providers.SOONTHANA.Channels.MTLITNOTIFY.IconUrl;
      _LINE_CHANNEL_ACCESS_TOKEN = lineJson.Providers.SOONTHANA.Channels.MTLITNOTIFY.AccessToken;
      _LINE_MESSAGING_API_BASE_ADDRESS = _config["Line:MessagingApiUrl"];

      var fileMtlJson = System.IO.File.ReadAllText(_config["MTL:Key"]);
      var mtlJson = JsonConvert.DeserializeObject<KeyMTL>(fileMtlJson);
      _MTL_EMPLOYEE_API_BASE_ADDRESS = _config["MTL:EmployeeApiUrl"];
      _MTL_AUTH_TOKEN_BASE_ADDRESS = _config["MTL:AuthApiUrl"];
      _MTL_CLIENT_ID = mtlJson.ClientId;

      var fileSgJson = System.IO.File.ReadAllText(_config["SendGrid:Key"]);
      var sgJson = JsonConvert.DeserializeObject<KeySendGrid>(fileSgJson);
      _SENDGRID_API_KEY = sgJson.Key;
      _SENDGRID_API_BASE_ADDRESS = _config["SendGrid:ApiUrl"];
      _NOTIFY_BASE_ADDRESS = _config[""];
    }

    // GET api/webhook
    [HttpGet]
    public ActionResult Get()
    {
      var result = DateTimes.GetCurrentUtcDateTimeInThaiTimeZone(DateTimes.DateTimeFormat.YearMonthDayByDashTHourMinuteSecondByColon, DateTimes.LanguageCultureName.ENGLISH_UNITED_STATES, DateTimes.DateTimeUtcOffset.HHMMByColon);
      return Ok(result);
    }


    // POST api/webhook
    [HttpPost]
    public async Task<ActionResult> Post()
    {
      var replyToken = string.Empty;
      var result = new object();

      var jsonContent = string.Empty;
      using (var reader = new StreamReader(Request.Body))
      {
        jsonContent = reader.ReadToEnd();
      }

      var obj = JsonConvert.DeserializeObject<LineMessagingApi.Webhook>(jsonContent);

      var userProfile = new UserProfiles();
      var lineProfile = new LineMessagingApi.Profile();

      await SendAsyncPushMessageToAdmin(jsonContent); // TODO: Unneccessary use after testing completed.
      try
      {
        foreach (var item in obj.Events)
        {
          replyToken = item.ReplyToken;

          if (item.Source.Type == "user")
          {
            var profile = await GetAsyncProfile(item.Source.UserId);
            lineProfile.UserId = profile.UserId;
            lineProfile.DisplayName = profile.DisplayName;
            lineProfile.PictureUrl = profile.PictureUrl;
            lineProfile.StatusMessage = profile.StatusMessage;

            var resp = await FindAsyncUserProfileByLineUserId(item.Source.UserId);

            if (resp.StatusCode == HttpStatusCode.OK)
            {
              var jsonExistingUserProfileContent = resp.Content.ReadAsStringAsync().Result;
              var existingUserProfile = JsonConvert.DeserializeObject<UserProfiles>(jsonExistingUserProfileContent);

              if (lineProfile.UserId != null && (lineProfile.DisplayName != existingUserProfile.LineDisplayName || lineProfile.PictureUrl != existingUserProfile.LinePictureUrl || lineProfile.StatusMessage != existingUserProfile.LineStatusMessage))
              {
                // Updating UserProfile
                existingUserProfile.LineDisplayName = lineProfile.DisplayName;
                existingUserProfile.LinePictureUrl = lineProfile.PictureUrl;
                existingUserProfile.LineStatusMessage = lineProfile.StatusMessage;
                await UpdateAsyncUserProfile(existingUserProfile);
              }

              // Setting userProfile object
              userProfile.Id = existingUserProfile.Id;
              userProfile.LineUserId = existingUserProfile.LineUserId;
              userProfile.LineDisplayName = existingUserProfile.LineDisplayName;
              userProfile.LinePictureUrl = existingUserProfile.LinePictureUrl;
              userProfile.LineStatusMessage = existingUserProfile.LineStatusMessage;
              userProfile.OfficerId = existingUserProfile.OfficerId;
              userProfile.OfficerName = existingUserProfile.OfficerName;
              userProfile.OfficerEmail = existingUserProfile.OfficerEmail;
              userProfile.OfficerPictureUrl = existingUserProfile.OfficerPictureUrl;
              userProfile.RegisteringDateTime = existingUserProfile.RegisteringDateTime;
              userProfile.RegisteredDateTime = existingUserProfile.RegisteredDateTime;
              userProfile.Status = existingUserProfile.Status;
              userProfile.Role = existingUserProfile.Role;
            }
            else
            {
              userProfile.Id = Guid.NewGuid();
              userProfile.LineUserId = lineProfile.UserId;
              userProfile.LineDisplayName = lineProfile.DisplayName;
              userProfile.LineStatusMessage = lineProfile.StatusMessage;
              userProfile.LinePictureUrl = lineProfile.PictureUrl;

              var respAddUserProfile = await AddAsyncUserProfile(userProfile);
            }
          }

          var replyMessage = new LineMessagingApi.ReplyMessage();
          replyMessage.replyToken = replyToken;

          var messages = new List<LineMessagingApi.SendMessage>();
          var sendmsgText = new LineMessagingApi.SendMessage();
          var sendmsgSticker = new LineMessagingApi.SendMessage();
          var sendmsgLocation = new LineMessagingApi.SendMessage();
          var sendmsgImage = new LineMessagingApi.SendMessage();
          var sendmsgVideo = new LineMessagingApi.SendMessage();
          var sendmsgAudio = new LineMessagingApi.SendMessage();
          var sendmsgImagemap = new LineMessagingApi.SendMessage();
          var sendmsgTemplate = new LineMessagingApi.SendMessage();

          switch (item.Type.ToLower())
          {
            #region Message Event
            case "message":
              switch (item.Message.Type.ToLower())
              {
                #region Text message
                case "text":
                  if (userProfile.OfficerId == null && userProfile.RegisteringDateTime == null)
                  {
                    // Checking Officer Id
                    if (Regex.IsMatch(item.Message.Text, @"^\d{6}$", RegexOptions.IgnoreCase) == true)
                    {
                      var resp = await GetEmployeeById(item.Message.Text);

                      if (resp.StatusCode == HttpStatusCode.OK)
                      {
                        var jsonEmployeeContent = resp.Content.ReadAsStringAsync().Result;
                        var emp = JsonConvert.DeserializeObject<Employee>(jsonEmployeeContent);

                        if (emp.EmployeeId == null || emp.EmployeeStatus.ToLower() != "a")
                        {
                          result = "ขออภัย รหัสพนักงานที่คุณ " + lineProfile.DisplayName + " กรอกไม่ถูกต้อง" + "\r\n\r\n" + "กรุณากรอกรหัสพนักงานของคุณอีกครั้ง " + "\uD83C\uDD94";
                        }
                        else
                        {
                          if (emp.Email == null || emp.Email == "")
                          {
                            result = "ขออภัย ไม่พบ e-mail address ในฐานข้อมูลพนักงาน กรุณาอัพเดตก่อน";
                          }
                          else
                          {
                            // Generating One-Time Password
                            var otp = new OneTimePasswords();
                            otp.UserProfileId = userProfile.Id;
                            otp.OTP = GetRandom8NumericOnly();
                            otp.CreatedDateTime = DateTime.Now;
                            otp.ExpiryDateTime = DateTime.Now.AddMinutes(15);
                            otp.Type = "Register";
                            otp.Status = true;

                            // Sending OTP via e-mail
                            var mailsend = new SendGridMailSendApi.Mail();

                            var listPersonalizations = new List<SendGridMailSendApi.Personalization>();
                            var personalization = new SendGridMailSendApi.Personalization();

                            var listTo = new List<SendGridMailSendApi.To>();
                            var to = new SendGridMailSendApi.To();
                            to.email = emp.Email.Trim().ToLower();
                            to.name = emp.EmployeeNameEn + " " + emp.EmployeeSurnameEn;
                            listTo.Add(to);

                            personalization.to = listTo;
                            listPersonalizations.Add(personalization);

                            var from = new SendGridMailSendApi.From();
                            from.email = "soonthana@gmail.com";
                            from.name = _SYSTEM_NAME;

                            var listContents = new List<SendGridMailSendApi.Content>();
                            var contenet = new SendGridMailSendApi.Content();
                            contenet.type = "text/html";
                            var emailOtp = otp.OTP;
                            var emailExpiryDateTime = DateTimes.ConvertToUtcDateTimeInThaiTimeZone(otp.ExpiryDateTime.Value, DateTimes.DateTimeFormat.DayMonthYearByForwardSlash_HourMinuteByColon, DateTimes.LanguageCultureName.ENGLISH_UNITED_STATES, DateTimes.DateTimeUtcOffset.Null);
                            var emailNotifyAdminUrl = _NOTIFY_BASE_ADDRESS + "optout/delete/" + userProfile.Id.ToString("N");
                            contenet.value = _WORDING_EMAIL_OTP.Replace("#[LINECHANNELICON]#", _LINE_CHANNEL_ICON).Replace("#[OTP]#", emailOtp).Replace("#[EXPIRYDATETIME]#", emailExpiryDateTime).Replace("#[NOTIFYADMINURL]#", emailNotifyAdminUrl);
                            listContents.Add(contenet);

                            mailsend.personalizations = listPersonalizations;
                            mailsend.from = from;
                            mailsend.subject = "รหัส OTP สำหรับการลงทะเบียนใช้งาน " + _SYSTEM_NAME;
                            mailsend.content = listContents;

                            var respMailSend = await SendAsyncEmail(mailsend);

                            if (respMailSend.StatusCode == HttpStatusCode.Accepted)
                            {
                              // Updating UserProfile
                              userProfile.OfficerId = emp.EmployeeId;
                              userProfile.OfficerName = emp.EmployeeNameTh + " " + emp.EmployeeSurnameTh;
                              userProfile.OfficerEmail = emp.Email.ToLower();
                              if (emp.ImageBase64Info.ImageBase64 != "")
                              {
                                var officerPicture = new OfficerPicture();
                                officerPicture.PictureBase64String = emp.ImageBase64Info.ImageBase64;
                                officerPicture.PictureExtension = emp.ImageBase64Info.ImageFileExtension.ToLower();
                                officerPicture.PictureFileName = userProfile.Id.ToString("N");

                                // Saving Officer Picture
                                var respSaveOfficerPicture = await SaveAsyncOfficerPicture(userProfile.Id, officerPicture);

                                if (respSaveOfficerPicture.StatusCode == HttpStatusCode.OK)
                                {
                                  userProfile.OfficerPictureUrl = _OFFICER_PICTURE_URL.Replace("#[OBJECTNAME]#", userProfile.Id.ToString());
                                }
                              }
                              userProfile.RegisteringDateTime = DateTime.Now;
                              userProfile.Role = userProfile.OfficerId == "083116" ? "Admin" : "User";
                              await UpdateAsyncUserProfile(userProfile);

                              // Adding Officer User Profile
                              var officer = new OfficerUserProfiles();
                              officer.UserProfileId = userProfile.Id;
                              officer.OfficerId = emp.EmployeeId;
                              await AddAsyncOfficerUserProfile(officer);

                              // Adding One-Time Password
                              await AddAsyncOneTimePassword(otp);

                              result = "คุณ " + lineProfile.DisplayName + " กรุณากรอกรหัส OTP ที่คุณจะได้รับทาง e-mail " + "\uD83D\uDC8C" + " " + "\u23F3";
                            }
                            else
                            {
                              // แจ้ง Admin
                              var textToAdmin = string.Empty;
                              if (respMailSend.StatusCode == HttpStatusCode.BadRequest)
                              {
                                textToAdmin = "Register Problem - SendGrid 400-Bad Request" + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName;
                              }
                              else if (respMailSend.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                              {
                                textToAdmin = "Register Problem - SendGrid 413-Request Entity Too Large" + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName;
                              }
                              else if (respMailSend.StatusCode == HttpStatusCode.Unauthorized)
                              {
                                textToAdmin = "Register Problem - SendGrid 401-Unauthorized" + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName;
                              }
                              else
                              {
                                textToAdmin = "Register Problem - SendGrid " + respMailSend.StatusCode.ToString() + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName;
                              }
                              await SendAsyncPushMessageToAdmin(textToAdmin);

                              // Update UserProfile  (Clear data to be not yet registering)
                              userProfile.OfficerId = null;
                              userProfile.OfficerName = null;
                              userProfile.OfficerEmail = null;
                              userProfile.OfficerPictureUrl = null;
                              userProfile.RegisteringDateTime = null;
                              userProfile.Status = false;
                              userProfile.Role = null;
                              await UpdateAsyncUserProfile(userProfile);

                              result = "ระบบมีปัญหาบางประการ" + "\r\n\r\n" + "กรุณากรอกรหัสพนักงานของคุณอีกครั้ง " + "\uD83C\uDD94";
                            }
                          }
                        }
                      }
                      else
                      {
                        // แจ้ง Admin
                        var textToAdmin = string.Empty;
                        if (resp.StatusCode == HttpStatusCode.NotFound)
                        {
                          textToAdmin = "Register Problem - Employee 404-Not Found" + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName + "\r\n\r\n" + item.Message.Text;
                        }
                        else if (resp.StatusCode == HttpStatusCode.InternalServerError)
                        {
                          textToAdmin = "Register Problem - Employee 500-Internal Server Error" + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName + "\r\n\r\n" + item.Message.Text;
                        }
                        else
                        {
                          textToAdmin = "Register Problem - Employee " + resp.StatusCode.ToString() + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName + "\r\n\r\n" + item.Message.Text;
                        }
                        await SendAsyncPushMessageToAdmin(textToAdmin);

                        result = "ระบบมีปัญหาบางประการ" + "\r\n\r\n" + "กรุณากรอกรหัสพนักงานของคุณอีกครั้ง " + "\uD83C\uDD94";
                      }
                    }
                    else
                    {
                      result = "ขอโทษ คุณ " + lineProfile.DisplayName + " ระบุรหัสพนักงานไม่ถูก" + "\r\n\r\n" + "กรุณากรอกรหัสพนักงานของคุณอีกครั้ง " + "\uD83C\uDD94";
                    }

                    sendmsgText.type = "text";
                    sendmsgText.text = result.ToString();
                    messages.Add(sendmsgText);
                  }
                  else if (userProfile.RegisteredDateTime == null)
                  {
                    // Checking One-Time Password
                    if (Regex.IsMatch(item.Message.Text, @"^\d{8}$", RegexOptions.IgnoreCase) == true)
                    {
                      var resp = await FindAsyncOneTimePassword(userProfile.Id);

                      if (resp.StatusCode == HttpStatusCode.OK)
                      {
                        var jsonOtpContent = resp.Content.ReadAsStringAsync().Result;
                        var otp = JsonConvert.DeserializeObject<OneTimePasswords>(jsonOtpContent);

                        if (otp.Status == true)
                        {
                          if (otp.ExpiryDateTime > DateTime.Now)
                          {
                            if (item.Message.Text.Trim() == otp.OTP.Trim())
                            {
                              // แจ้ง Admin
                              var textToAdmin = userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName + " ลงทะเบียนเป็นผู้ใช้งานระบบ " + _SYSTEM_NAME + " เรียบร้อยแล้ว";
                              await SendAsyncPushMessageToAdmin(textToAdmin);

                              // Update UserProfile
                              userProfile.RegisteredDateTime = DateTime.Now;
                              userProfile.Status = true;
                              await UpdateAsyncUserProfile(userProfile);

                              // Update OneTimePassword
                              otp.Status = false;
                              await UpdateAsyncOneTimePassword(otp);

                              result = "ยินดีด้วย คุณ " + lineProfile.DisplayName + " ลงทะเบียนเป็นผู้ใช้งานระบบ " + _SYSTEM_NAME + " เรียบร้อยแล้ว";
                            }
                            else
                            {
                              result = "ขอโทษ รหัส OTP ที่กรอกไม่ถูกต้อง" + "\r\n\r\n" + "กรุณากรอกรหัส OTP ที่คุณได้รับทาง e-mail อีกครั้ง " + "\uD83C\uDD94";
                            }
                          }
                          else
                          {
                            // Update UserProfile (Clear data to be not yet registering)
                            userProfile.OfficerId = null;
                            userProfile.OfficerName = null;
                            userProfile.OfficerEmail = null;
                            userProfile.OfficerPictureUrl = null;
                            userProfile.RegisteringDateTime = null;
                            userProfile.Status = false;
                            userProfile.Role = null;
                            await UpdateAsyncUserProfile(userProfile);

                            result = "รหัส OTP หมดอายุแล้ว คุณทำรายการช้าเกินไป" + "\r\n\r\n" + "กรุณากรอกรหัสพนักงานของคุณอีกครั้ง " + "\uD83C\uDD94";
                          }
                        }
                        else
                        {
                          // Update UserProfile (Clear data to be not yet registering)
                          userProfile.OfficerId = null;
                          userProfile.OfficerName = null;
                          userProfile.OfficerEmail = null;
                          userProfile.OfficerPictureUrl = null;
                          userProfile.RegisteringDateTime = null;
                          userProfile.Status = false;
                          userProfile.Role = null;
                          await UpdateAsyncUserProfile(userProfile);

                          result = "รหัส OTP ถูกใช้งานไปแล้ว" + "\r\n\r\n" + "กรุณากรอกรหัสพนักงานของคุณอีกครั้ง " + "\uD83C\uDD94";
                        }
                      }
                      else
                      {
                        // แจ้ง Admin
                        var textToAdmin = string.Empty;
                        if (resp.StatusCode == HttpStatusCode.BadRequest)
                        {
                          textToAdmin = "Register Problem - OTP 400-Bad Request" + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName;
                        }
                        else if (resp.StatusCode == HttpStatusCode.ServiceUnavailable)
                        {
                          textToAdmin = "Register Problem - OTP 503-Service Unavailable" + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName;
                        }
                        else if (resp.StatusCode == HttpStatusCode.NotFound)
                        {
                          textToAdmin = "Register Problem - OTP 404-Not Found" + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName;
                        }
                        else
                        {
                          textToAdmin = "Register Problem - OTP " + resp.StatusCode.ToString() + "\r\n\r\n" + userProfile.LineDisplayName + "\r\n" + "(" + userProfile.OfficerId + ") " + userProfile.OfficerName;
                        }
                        await SendAsyncPushMessageToAdmin(textToAdmin + "\r\n\r\n" + resp.ToString() + "\r\n\r\n" + userProfile.Id.ToString("N"));

                        result = "ระบบมีปัญหาบางประการ" + "\r\n\r\n" + "กรุณากรอกรหัส OTP ที่คุณได้รับทาง e-mail อีกครั้ง " + "\uD83C\uDD94";
                      }
                    }
                    else
                    {
                      result = "ขอโทษ คุณ " + lineProfile.DisplayName + " ระบุ OTP ไม่ถูก" + "\r\n\r\n" + "กรุณากรอกรหัส OTP ที่คุณได้รับทาง e-mail อีกครั้ง " + "\uD83C\uDD94";
                    }

                    sendmsgText.type = "text";
                    sendmsgText.text = result.ToString();
                    messages.Add(sendmsgText);
                  }
                  else
                  {
                    #region EXAMPLE OF MESSAGE TYPES
                    if (item.Message.Text.ToLower() == "$1")
                    {
                      /* Sticker */
                      sendmsgSticker.type = "sticker";
                      sendmsgSticker.packageId = "1";
                      sendmsgSticker.stickerId = "2";
                      messages.Add(sendmsgSticker);
                    }
                    else if (item.Message.Text.ToLower() == "$2")
                    {
                      /* Template Confirm */
                      sendmsgTemplate.type = "template";
                      sendmsgTemplate.altText = "Template ประเภท Confirm";
                      LineMessagingApi.Template tmp = new LineMessagingApi.Template();
                      tmp.type = "confirm";
                      tmp.text = "Are you sure?";
                      List<LineMessagingApi.TemplateAction> listTmpact = new List<LineMessagingApi.TemplateAction>();
                      LineMessagingApi.TemplateAction tmpactYes = new LineMessagingApi.TemplateAction();
                      tmpactYes.type = "message";
                      tmpactYes.label = "Yes";
                      tmpactYes.text = "yes_textvalue";
                      listTmpact.Add(tmpactYes);
                      LineMessagingApi.TemplateAction tmpactNo = new LineMessagingApi.TemplateAction();
                      tmpactNo.type = "message";
                      tmpactNo.label = "No";
                      tmpactNo.text = "no_textvalue";
                      listTmpact.Add(tmpactNo);
                      tmp.actions = listTmpact;
                      sendmsgTemplate.template = tmp;
                      messages.Add(sendmsgTemplate);
                    }
                    else if (item.Message.Text.ToLower() == "$3")
                    {
                      /* Template Buttons */
                      sendmsgTemplate.type = "template";
                      sendmsgTemplate.altText = "Template ประเภท Buttons";
                      LineMessagingApi.Template tmp = new LineMessagingApi.Template();
                      tmp.type = "buttons";
                      tmp.text = "กรุณาเลือก";
                      tmp.title = "ฝ่ายต้นสังกัด";
                      List<LineMessagingApi.TemplateAction> listTmpact = new List<LineMessagingApi.TemplateAction>();
                      LineMessagingApi.TemplateAction tmpact1 = new LineMessagingApi.TemplateAction();
                      tmpact1.type = "postback";
                      tmpact1.label = "ITS";
                      tmpact1.data = "action=select_dept&deptid=10500";
                      listTmpact.Add(tmpact1);
                      LineMessagingApi.TemplateAction tmpact2 = new LineMessagingApi.TemplateAction();
                      tmpact2.type = "postback";
                      tmpact2.label = "ITO";
                      tmpact2.data = "action=select_dept&deptid=10100";
                      listTmpact.Add(tmpact2);
                      LineMessagingApi.TemplateAction tmpact3 = new LineMessagingApi.TemplateAction();
                      tmpact3.type = "postback";
                      tmpact3.label = "ITN";
                      tmpact3.data = "action=select_dept&deptid=30400";
                      listTmpact.Add(tmpact3);
                      LineMessagingApi.TemplateAction tmpactView = new LineMessagingApi.TemplateAction();
                      tmpactView.type = "uri";
                      tmpactView.label = "View MTL";
                      tmpactView.uri = "http://www.muangthai.co.th";
                      listTmpact.Add(tmpactView);
                      tmp.actions = listTmpact;
                      sendmsgTemplate.template = tmp;
                      messages.Add(sendmsgTemplate);
                    }
                    else if (item.Message.Text.ToLower() == "$4")
                    {
                      /* Template ImageCarousel */
                      sendmsgTemplate.type = "template";
                      sendmsgTemplate.altText = "Template ประเภท Image Carousel";
                      LineMessagingApi.Template tmp = new LineMessagingApi.Template();
                      tmp.type = "image_carousel";
                      List<LineMessagingApi.Columns> listCols = new List<LineMessagingApi.Columns>();
                      LineMessagingApi.Columns col1 = new LineMessagingApi.Columns();
                      col1.imageUrl = "https://2.bp.blogspot.com/-Zd1L8TSmdE8/WM1MbfMyaqI/AAAAAAAAAY4/QxFxB8pKcxEyoXL09VbBJVmUm8g3eAvcgCLcB/s1600/Bayern-M%25C3%25BCnchen-logo.png";
                      LineMessagingApi.TemplateAction tmpact1 = new LineMessagingApi.TemplateAction();
                      tmpact1.type = "postback";
                      tmpact1.label = "Bayern";
                      tmpact1.data = "action=select_image&imgid=1";
                      col1.action = tmpact1;
                      listCols.Add(col1);
                      LineMessagingApi.Columns col2 = new LineMessagingApi.Columns();
                      col2.imageUrl = "https://www.seeklogo.net/wp-content/uploads/2015/10/leicester-city-fc-vector-logo-eps-svg.png";
                      LineMessagingApi.TemplateAction tmpact2 = new LineMessagingApi.TemplateAction();
                      tmpact2.type = "postback";
                      tmpact2.label = "Leicester";
                      tmpact2.data = "action=select_image&imgid=2";
                      col2.action = tmpact2;
                      listCols.Add(col2);
                      LineMessagingApi.Columns col3 = new LineMessagingApi.Columns();
                      col3.imageUrl = "https://i.imgur.com/HSU2Cm0.png";
                      LineMessagingApi.TemplateAction tmpact3 = new LineMessagingApi.TemplateAction();
                      tmpact3.type = "postback";
                      tmpact3.label = "Liverpool";
                      tmpact3.data = "action=select_image&imgid=3";
                      col3.action = tmpact3;
                      listCols.Add(col3);
                      LineMessagingApi.Columns col4 = new LineMessagingApi.Columns();
                      col4.imageUrl = "https://i.imgur.com/kJlLXuY.png";
                      LineMessagingApi.TemplateAction tmpact4 = new LineMessagingApi.TemplateAction();
                      tmpact4.type = "postback";
                      tmpact4.label = "Chelsea";
                      tmpact4.data = "action=select_image&imgid=4";
                      col4.action = tmpact4;
                      listCols.Add(col4);
                      LineMessagingApi.Columns col5 = new LineMessagingApi.Columns();
                      col5.imageUrl = "https://s-media-cache-ak0.pinimg.com/originals/2e/f7/6c/2ef76c6125263727028499700aecd104.png";
                      LineMessagingApi.TemplateAction tmpact5 = new LineMessagingApi.TemplateAction();
                      tmpact5.type = "postback";
                      tmpact5.label = "Real Madrid";
                      tmpact5.data = "action=select_image&imgid=5";
                      col5.action = tmpact5;
                      listCols.Add(col5);
                      tmp.columns = listCols;
                      sendmsgTemplate.template = tmp;
                      messages.Add(sendmsgTemplate);
                    }
                    else if (item.Message.Text.ToLower() == "$5")
                    {
                      /* Template Carousel */
                      sendmsgTemplate.type = "template";
                      sendmsgTemplate.altText = "Template ประเภท Carousel";
                      LineMessagingApi.Template tmp = new LineMessagingApi.Template();
                      tmp.type = "carousel";
                      List<LineMessagingApi.Columns> listCols = new List<LineMessagingApi.Columns>();
                      LineMessagingApi.Columns col1 = new LineMessagingApi.Columns();
                      col1.thumbnailImageUrl = "https://s-media-cache-ak0.pinimg.com/originals/2e/f7/6c/2ef76c6125263727028499700aecd104.png";
                      col1.text = "text1";
                      col1.title = "title1";
                      List<LineMessagingApi.TemplateAction> listTmpact1 = new List<LineMessagingApi.TemplateAction>();
                      LineMessagingApi.TemplateAction tmpact1 = new LineMessagingApi.TemplateAction();
                      tmpact1.type = "postback";
                      tmpact1.label = "Go Madrid";
                      tmpact1.data = "action=select_something&id=1";
                      listTmpact1.Add(tmpact1);
                      col1.actions = listTmpact1;
                      listCols.Add(col1);
                      LineMessagingApi.Columns col2 = new LineMessagingApi.Columns();
                      col2.thumbnailImageUrl = "https://i.imgur.com/kJlLXuY.png";
                      col2.text = "text2";
                      col2.title = "title2";
                      List<LineMessagingApi.TemplateAction> listTmpact2 = new List<LineMessagingApi.TemplateAction>();
                      LineMessagingApi.TemplateAction tmpact2 = new LineMessagingApi.TemplateAction();
                      tmpact2.type = "postback";
                      tmpact2.label = "Go Chelsea ";
                      tmpact2.data = "action=select_something&id=2";
                      listTmpact2.Add(tmpact2);
                      col2.actions = listTmpact2;
                      listCols.Add(col2);
                      LineMessagingApi.Columns col3 = new LineMessagingApi.Columns();
                      col3.thumbnailImageUrl = "https://2.bp.blogspot.com/-Zd1L8TSmdE8/WM1MbfMyaqI/AAAAAAAAAY4/QxFxB8pKcxEyoXL09VbBJVmUm8g3eAvcgCLcB/s1600/Bayern-M%25C3%25BCnchen-logo.png";
                      col3.text = "text3";
                      col3.title = "title3";
                      List<LineMessagingApi.TemplateAction> listTmpact3 = new List<LineMessagingApi.TemplateAction>();
                      LineMessagingApi.TemplateAction tmpact3 = new LineMessagingApi.TemplateAction();
                      tmpact3.type = "postback";
                      tmpact3.label = "Go Bayern";
                      tmpact3.data = "action=select_something&id=3";
                      listTmpact3.Add(tmpact3);
                      col3.actions = listTmpact3;
                      listCols.Add(col3);
                      tmp.columns = listCols;
                      sendmsgTemplate.template = tmp;
                      messages.Add(sendmsgTemplate);
                    }
                    else if (item.Message.Text.ToLower() == "$6")
                    {
                      sendmsgLocation.type = "location";
                      sendmsgLocation.title = _SYSTEM_NAME + " Location";
                      sendmsgLocation.address = "250 Rachadaphisek Rd., Huaykwang, Bangkok 10310";
                      sendmsgLocation.latitude = 13.786782407279155M;
                      sendmsgLocation.longitude = 100.57417687028646M;
                      messages.Add(sendmsgLocation);
                    }
                    else if (item.Message.Text.ToLower() == "$7")
                    {
                      // Emoji ใช้เป็น UTF-16 https://r12a.github.io/apps/conversion/ โดยเอา Unicode Emoji มาใช้ได้ http://unicode.org/emoji/charts/full-emoji-list.html
                      result = lineProfile.DisplayName + " \uD83D\uDC68\u200D\uD83D\uDCBB " + item.Message.Text + " " + "\u270B" + "\uDBC0\uDC88" + " " + "\uD83D\uDC36" + "\r\n" + "\uD83C\uDDF9\uD83C\uDDED";

                      sendmsgText.type = "text";
                      sendmsgText.text = result.ToString();
                      messages.Add(sendmsgText);
                    }
                    else if (item.Message.Text.StartsWith("$8"))
                    {
                      sendmsgText.type = "text";
                      sendmsgText.text = item.Message.Text.Replace("$8", "").ToString();
                      messages.Add(sendmsgText);

                      var listString = new List<string>();

                      var resp = await GetAsyncUserProfiles();
                      var content = resp.Content.ReadAsStringAsync().Result;
                      var jObj = JObject.Parse(content);

                      foreach (var jItem in jObj)
                      {
                        var up = jItem.Value.ToObject<UserProfiles>();
                        
                        listString.Add(up.LineUserId);
                      }

                      var multicastObj = new LineMessagingApi.MulticastMessage();
                      multicastObj.to = listString;
                      multicastObj.messages = messages;

                      await SendAsyncMulticastMessage(multicastObj);
                    }
                    else
                    {
                      result = "คุณ " + lineProfile.DisplayName + " เป็นผู้ใช้งานระบบ " + _SYSTEM_NAME + " อยู่แล้ว";

                      sendmsgText.type = "text";
                      sendmsgText.text = result.ToString();
                      messages.Add(sendmsgText);
                    }
                    #endregion
                  }

                  replyMessage.messages = messages;

                  await SendAsyncReplyMessage(replyMessage);
                  break;
                #endregion

                #region Image message
                case "image":
                  result = "ขอโทษ ฉันยังไม่เข้าใจ image ที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id;

                  sendmsgText.type = "text";
                  sendmsgText.text = result.ToString();
                  messages.Add(sendmsgText);

                  sendmsgSticker.type = "sticker";
                  sendmsgSticker.packageId = "1";
                  sendmsgSticker.stickerId = "422";
                  messages.Add(sendmsgSticker);

                  replyMessage.messages = messages;

                  await SendAsyncReplyMessage(replyMessage);
                  break;
                #endregion

                #region Video message
                case "video":
                  result = "ขอโทษ ฉันยังไม่เข้าใจ video ที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id;

                  sendmsgText.type = "text";
                  sendmsgText.text = result.ToString();
                  messages.Add(sendmsgText);

                  sendmsgSticker.type = "sticker";
                  sendmsgSticker.packageId = "1";
                  sendmsgSticker.stickerId = "422";
                  messages.Add(sendmsgSticker);

                  replyMessage.messages = messages;

                  await SendAsyncReplyMessage(replyMessage);
                  break;
                #endregion

                #region Audio message
                case "audio":
                  result = "ขอโทษ ฉันยังไม่เข้าใจ audio ที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id;

                  sendmsgText.type = "text";
                  sendmsgText.text = result.ToString();
                  messages.Add(sendmsgText);

                  sendmsgSticker.type = "sticker";
                  sendmsgSticker.packageId = "1";
                  sendmsgSticker.stickerId = "422";
                  messages.Add(sendmsgSticker);

                  replyMessage.messages = messages;

                  await SendAsyncReplyMessage(replyMessage);
                  break;
                #endregion

                #region File message
                case "file":
                  result = "ขอโทษ ฉันยังไม่เข้าใจ file ที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id + "\r\n" + "FileName: " + item.Message.FileName + "\r\n" + "FileSize: " + item.Message.FileSize;

                  sendmsgText.type = "text";
                  sendmsgText.text = result.ToString();
                  messages.Add(sendmsgText);

                  sendmsgSticker.type = "sticker";
                  sendmsgSticker.packageId = "1";
                  sendmsgSticker.stickerId = "422";
                  messages.Add(sendmsgSticker);

                  replyMessage.messages = messages;

                  await SendAsyncReplyMessage(replyMessage);
                  break;
                #endregion

                #region Location message
                case "location":
                  result = "ขอโทษ ฉันยังไม่เข้าใจ location ที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id + "\r\n" + "Title: " + item.Message.Title + "\r\n" + "Address: " + item.Message.Address + "\r\n" + "Lat: " + item.Message.Latitude + "\r\n" + "Lon: " + item.Message.Longitude;

                  sendmsgText.type = "text";
                  sendmsgText.text = result.ToString();
                  messages.Add(sendmsgText);

                  sendmsgSticker.type = "sticker";
                  sendmsgSticker.packageId = "1";
                  sendmsgSticker.stickerId = "422";
                  messages.Add(sendmsgSticker);

                  replyMessage.messages = messages;

                  await SendAsyncReplyMessage(replyMessage);
                  break;
                #endregion

                #region Sticker message
                case "sticker":
                  result = "ขอโทษ ฉันยังไม่เข้าใจ sticker ที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id + "\r\n" + "PackageId: " + item.Message.PackageId + "\r\n" + "StickerId: " + item.Message.StickerId;

                  sendmsgText.type = "text";
                  sendmsgText.text = result.ToString();
                  messages.Add(sendmsgText);

                  sendmsgSticker.type = "sticker";
                  sendmsgSticker.packageId = "1";
                  sendmsgSticker.stickerId = "422";
                  messages.Add(sendmsgSticker);

                  replyMessage.messages = messages;

                  await SendAsyncReplyMessage(replyMessage);
                  break;
                #endregion
                default:
                  result = "ฉันยังไม่เข้าใจประเภทของ Message ที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id;

                  sendmsgText.type = "text";
                  sendmsgText.text = result.ToString();
                  messages.Add(sendmsgText);

                  replyMessage.messages = messages;

                  await SendAsyncReplyMessage(replyMessage);
                  break;
              }
              break;
            #endregion

            #region Follow Event
            case "follow":
              sendmsgText.type = "text";
              sendmsgText.text = "\uD83C\uDF89" + "\uD83C\uDF8A" + "ยินดีต้อนรับคุณ " + lineProfile.DisplayName + "\r\n" + "เข้าสู่ " + _SYSTEM_NAME + " ไลน์สำหรับแจ้งเตือนปัญหาจากระบบงานที่คุณดูแล " + "\uD83D\uDCE3" + "\uD83D\uDD14" + "\u26A0" + "\uD83C\uDD98" + "\r\n\r\n" + "กรุณากรอกรหัสพนักงานของคุณ " + "\uD83C\uDD94";
              messages.Add(sendmsgText);

              replyMessage.messages = messages;

              await SendAsyncReplyMessage(replyMessage);
              break;
            #endregion

            #region Unfollow Event
            case "unfollow":
              // Deleting UserProfile
              await DeleteAsyncUserProfile(userProfile);

              /* เนื่องจาก Line User Id ได้ทำการ unfollow ไปแล้ว ทำให้ไม่สามารถที่จะ Get Profile, ส่ง Push ไปหา Line User Id นั้นได้อีก 
              result = "คุณ " + lineProfile.DisplayName + " unfollow แล้วกลับมาใหม่นะ";

              sendmsgText.type = "text";
              sendmsgText.text = result.ToString();
              messages.Add(sendmsgText);

              sendmsgSticker.type = "sticker";
              sendmsgSticker.packageId = "1";
              sendmsgSticker.stickerId = "408";
              messages.Add(sendmsgSticker);

              var pushMessage = new LineMessagingApi.PushMessage();
              pushMessage.to = userProfile.LineUserId;
              pushMessage.messages = messages;

              await SendAsyncPushMessage(pushMessage);
              */
              break;
            #endregion

            #region Join Event
            case "join":
              result = "ยินดีต้อนรับคุณ " + lineProfile.DisplayName + " ที่ join กลุ่ม";

              sendmsgText.type = "text";
              sendmsgText.text = result.ToString();
              messages.Add(sendmsgText);

              sendmsgSticker.type = "sticker";
              sendmsgSticker.packageId = "1";
              sendmsgSticker.stickerId = "138";
              messages.Add(sendmsgSticker);

              replyMessage.messages = messages;

              await SendAsyncReplyMessage(replyMessage);
              break;
            #endregion

            #region Leave Event
            case "leave":
              result = "เสียใจที่คุณ " + lineProfile.DisplayName + " ออกจากกลุ่ม";

              sendmsgText.type = "text";
              sendmsgText.text = result.ToString();
              messages.Add(sendmsgText);

              sendmsgSticker.type = "sticker";
              sendmsgSticker.packageId = "1";
              sendmsgSticker.stickerId = "131";
              messages.Add(sendmsgSticker);

              replyMessage.messages = messages;

              await SendAsyncReplyMessage(replyMessage);
              break;
            #endregion

            #region Postback Event
            case "postback":
              if (item.Postback.Data == "" || item.Postback.Data == null)
              {
                sendmsgSticker.type = "sticker";
                sendmsgSticker.packageId = "1";
                sendmsgSticker.stickerId = "135";
                messages.Add(sendmsgSticker);

                result = "ขอโทษที คุณไม่ได้ส่งข้อมูลอะไรมานะ";

                sendmsgText.type = "text";
                sendmsgText.text = result.ToString();
                messages.Add(sendmsgText);
              }
              else
              {
                string[] arrData = item.Postback.Data.Split('&');

                var firstData = new DataInKeyValue();
                var secondData = new DataInKeyValue();
                firstData = GetKeyValue(arrData[0]);

                if (firstData.Key != "action")
                {
                  sendmsgSticker.type = "sticker";
                  sendmsgSticker.packageId = "1";
                  sendmsgSticker.stickerId = "422";
                  messages.Add(sendmsgSticker);

                  result = "ขอโทษที ฉันยังไม่รู้จักข้อมูลที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id + "\r\n" + "Postback: " + item.Postback.Data;

                  sendmsgText.type = "text";
                  sendmsgText.text = result.ToString();
                  messages.Add(sendmsgText);
                }
                else
                {
                  switch (firstData.Value)
                  {
                    case "select_image":
                      // TODO:
                      secondData = GetKeyValue(arrData[1]);

                      result = "คุณเลือกรูป " + secondData.Key + " = " + secondData.Value;

                      sendmsgText.type = "text";
                      sendmsgText.text = result.ToString();
                      messages.Add(sendmsgText);
                      break;
                    case "select_dept":
                      // TODO:
                      secondData = GetKeyValue(arrData[1]);

                      result = "คุณเลือกฝ่าย " + secondData.Key + " = " + secondData.Value;

                      sendmsgText.type = "text";
                      sendmsgText.text = result.ToString();
                      messages.Add(sendmsgText);
                      break;
                    case "select_something":
                      // TODO:
                      secondData = GetKeyValue(arrData[1]);

                      result = "คุณเลือกอะไรบางอย่าง " + secondData.Key + " = " + secondData.Value;

                      sendmsgText.type = "text";
                      sendmsgText.text = result.ToString();
                      messages.Add(sendmsgText);
                      break;
                    default:
                      sendmsgSticker.type = "sticker";
                      sendmsgSticker.packageId = "1";
                      sendmsgSticker.stickerId = "422";
                      messages.Add(sendmsgSticker);

                      result = "ฉันยังไม่เข้าใจประเภทของ Action ที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id + "\r\n" + "Postback: " + item.Postback.Data;

                      sendmsgText.type = "text";
                      sendmsgText.text = result.ToString();
                      messages.Add(sendmsgText);
                      break;
                  }
                }
              }

              replyMessage.messages = messages;

              await SendAsyncReplyMessage(replyMessage);
              break;
            #endregion

            #region Beacon Event
            case "beacon":
              // TODO: Beacon Event

              break;
            #endregion
            default:
              sendmsgSticker.type = "sticker";
              sendmsgSticker.packageId = "1";
              sendmsgSticker.stickerId = "422";
              messages.Add(sendmsgSticker);

              result = "ฉันไม่เข้าใจสิ่งที่คุณส่งมา" + "\r\n" + "MsgId: " + item.Message.Id;

              sendmsgText.type = "text";
              sendmsgText.text = result.ToString();
              messages.Add(sendmsgText);

              replyMessage.messages = messages;

              await SendAsyncReplyMessage(replyMessage);
              break;
          }
        }
      }
      catch (Exception ex)
      {
        await SendAsyncPushMessageToAdmin(ex.ToString());
      }

      return Ok(result);
    }





    #region Private methods for data sources - Firebase Storage
    /// <summary>
    /// Save Officer Picture to MTLITNotify "POST /api/saveofficerpicture"
    /// </summary>
    /// <param name="userProfileId"></param>
    /// <param name="picture"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> SaveAsyncOfficerPicture(Guid userProfileId, OfficerPicture picture)
    {
      var bytes = Convert.FromBase64String(picture.PictureBase64String);

      var resp = await _fireObj.UploadFile(userProfileId.ToString(), "image/jpeg", bytes);

      return resp;
    }
    #endregion



    #region Private methods for data sources - Firebase Real-time Database
    /// <summary>
    /// List all user profile from data sources
    /// </summary>
    /// <returns></returns>
    private async Task<HttpResponseMessage> GetAsyncUserProfiles()
    {
      return await _fireObj.GetUserProfiles();
    }

    /// <summary>
    /// Get a user profile by Line user id from data sources
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> FindAsyncUserProfileByLineUserId(string id)
    {
      var respLineUserProfile = await _fireObj.GetLineUserProfilesById(id);

      if (respLineUserProfile.StatusCode == HttpStatusCode.OK)
      {
        var jsonLineUserProfileContent = respLineUserProfile.Content.ReadAsStringAsync().Result;
        var lineUserProfile = JsonConvert.DeserializeObject<LineUserProfiles>(jsonLineUserProfileContent);

        if (lineUserProfile != null)
        {
          return await _fireObj.GetUserProfilesById(lineUserProfile.UserProfileId);
        }
        else
        {
          return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
      }
      else
      {
        return new HttpResponseMessage(HttpStatusCode.NotFound);
      }
    }

    /// <summary>
    /// Add a new user profile to data sources
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> AddAsyncUserProfile(UserProfiles user)
    {
      try
      {
        await _fireObj.PutUserProfiles(user);

        var lup = new LineUserProfiles();
        lup.UserProfileId = user.Id;
        lup.LineUserId = user.LineUserId;
        await _fireObj.PutLineUserProfiles(lup);

        return new HttpResponseMessage(HttpStatusCode.OK);
      }
      catch
      {
        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
      }
    }

    /// <summary>
    /// Update an existing user profile to data sources
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> UpdateAsyncUserProfile(UserProfiles user)
    {
      return await _fireObj.PatchUserProfiles(user);
    }

    /// <summary>
    /// Delete an existing user profile from data sources
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> DeleteAsyncUserProfile(UserProfiles user)
    {
      return await _fireObj.DeleteUserProfiles(user.Id.ToString());
    }

    /// <summary>
    /// Add a new officer user profile to data sources
    /// </summary>
    /// <param name="officer"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> AddAsyncOfficerUserProfile(OfficerUserProfiles officer)
    {
      return await _fireObj.PutOfficerUserProfiles(officer);
    }

    /// <summary>
    /// Get a one-time password by User profile id from data sources
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> FindAsyncOneTimePassword(Guid id)
    {
      return await _fireObj.GetOneTimePasswordsById(id);
    }

    /// <summary>
    /// Add a new one-time password to data sources
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> AddAsyncOneTimePassword(OneTimePasswords otp)
    {
      return await _fireObj.PutOneTimePasswords(otp);
    }

    /// <summary>
    /// Update an existing one-time password to data sources
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> UpdateAsyncOneTimePassword(OneTimePasswords otp)
    {
      return await _fireObj.PatchOneTimePasswords(otp);
    }
    #endregion



    #region Private methods for LINE Messaging Api
    /// <summary>
    /// Get Line Profile from LINE "GET https://api.line.me/v2/bot/profile/{userId}"
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<LineMessagingApi.Profile> GetAsyncProfile(string userId)
    {
      var obj = new LineMessagingApi.Profile();

      var http = new HttpClient();
      http.BaseAddress = new Uri(_LINE_MESSAGING_API_BASE_ADDRESS);
      http.DefaultRequestHeaders.Accept.Clear();
      http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _LINE_CHANNEL_ACCESS_TOKEN);

      var response = await http.GetAsync("profile/" + userId);
      var jsonContent = response.Content.ReadAsStringAsync().Result;
      obj = JsonConvert.DeserializeObject<LineMessagingApi.Profile>(jsonContent);

      return obj;
    }

    /// <summary>
    /// Get Content (image, video and audio) sent by users from LINE "GET https://api.line.me/v2/bot/message/{messageId}/content
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    private async Task<Object> GetAsyncContent(string messageId)
    {
      var obj = new Object();

      var http = new HttpClient();
      http.BaseAddress = new Uri(_LINE_MESSAGING_API_BASE_ADDRESS);
      http.DefaultRequestHeaders.Accept.Clear();
      http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _LINE_CHANNEL_ACCESS_TOKEN);

      var response = await http.GetAsync("message/" + messageId + "/content");
      var jsonContent = response.Content.ReadAsStringAsync().Result;
      obj = jsonContent;
      // TODO: Convert binary string to something to display content

      return obj;
    }

    /// <summary>
    /// Send ReplyMessage to LINE "POST https://api.line.me/v2/bot/message/reply"
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task<bool> SendAsyncReplyMessage(LineMessagingApi.ReplyMessage message)
    {
      var http = new HttpClient();
      http.BaseAddress = new Uri(_LINE_MESSAGING_API_BASE_ADDRESS);
      http.DefaultRequestHeaders.Accept.Clear();
      http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _LINE_CHANNEL_ACCESS_TOKEN);

      var jsonString = JsonConvert.SerializeObject(message);

      var response = await http.PostAsync("message/reply", new StringContent(jsonString, Encoding.UTF8, "application/json"));

      if (response.StatusCode == HttpStatusCode.OK)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Send PushMessage to LINE "POST https://api.line.me/v2/bot/message/push"
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task<bool> SendAsyncPushMessage(LineMessagingApi.PushMessage message)
    {
      var http = new HttpClient();
      http.BaseAddress = new Uri(_LINE_MESSAGING_API_BASE_ADDRESS);
      http.DefaultRequestHeaders.Accept.Clear();
      http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _LINE_CHANNEL_ACCESS_TOKEN);

      var jsonString = JsonConvert.SerializeObject(message);

      var response = await http.PostAsync("message/push", new StringContent(jsonString, Encoding.UTF8, "application/json"));

      if (response.StatusCode == HttpStatusCode.OK)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Send MulticastMessage to LINE "POST https://api.line.me/v2/bot/message/multicast"
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task<bool> SendAsyncMulticastMessage(LineMessagingApi.MulticastMessage message)
    {
      var http = new HttpClient();
      http.BaseAddress = new Uri(_LINE_MESSAGING_API_BASE_ADDRESS);
      http.DefaultRequestHeaders.Accept.Clear();
      http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _LINE_CHANNEL_ACCESS_TOKEN);

      var jsonString = JsonConvert.SerializeObject(message);

      var response = await http.PostAsync("message/multicast", new StringContent(jsonString, Encoding.UTF8, "application/json"));

      if (response.StatusCode == HttpStatusCode.OK)
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    #endregion



    #region Private methods for MTL Employee Api
    /// <summary>
    /// Get Employee Profile from MTL "GET http://api.muangthai.co.th/MTLEmployeeAPI/api/employees/{employeeId}"
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> GetEmployeeById(string employeeId)
    {
      var mtlAuthToken = await GetMTLAuthToken();

      var http = new HttpClient();
      http.BaseAddress = new Uri(_MTL_EMPLOYEE_API_BASE_ADDRESS);
      http.DefaultRequestHeaders.Accept.Clear();
      http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", mtlAuthToken.Access_Token);

      return await http.GetAsync(employeeId);
    }

    /// <summary>
    /// Get MTL Authorization Token from MTL "POST https://mtlauthapp.azurewebsites.net/oauth2/token"
    /// </summary>
    /// <returns></returns>
    private async Task<MTLAuthToken> GetMTLAuthToken()
    {
      var obj = new MTLAuthToken();

      var http = new HttpClient();
      http.BaseAddress = new Uri(_MTL_AUTH_TOKEN_BASE_ADDRESS);
      http.DefaultRequestHeaders.Accept.Clear();
      http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var body = "grant_type=password&username=test&client_id=" + _MTL_CLIENT_ID;

      var response = await http.PostAsync("oauth2/token", new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
      var jsonContent = response.Content.ReadAsStringAsync().Result;
      obj = JsonConvert.DeserializeObject<MTLAuthToken>(jsonContent);

      return obj;
    }
    #endregion



    #region Private methods for SendGrid Api
    /// <summary>
    /// Send Mail to SendGrid "POST https://api.sendgrid.com/v3/mail/send"
    /// </summary>
    /// <param name="mailsend"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> SendAsyncEmail(SendGridMailSendApi.Mail mailsend)
    {
      var http = new HttpClient();
      http.BaseAddress = new Uri(_SENDGRID_API_BASE_ADDRESS);
      http.DefaultRequestHeaders.Accept.Clear();
      http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _SENDGRID_API_KEY);

      var jsonString = JsonConvert.SerializeObject(mailsend);

      return await http.PostAsync("send", new StringContent(jsonString, Encoding.UTF8, "application/json"));
    }
    #endregion



    #region Private methods and classes
    /// <summary>
    /// Data in Key / Value class model
    /// </summary>
    private class DataInKeyValue
    {
      public string Key { get; set; }
      public string Value { get; set; }
    }

    /// <summary>
    /// MTL Authorization Token class model
    /// </summary>
    private class MTLAuthToken
    {
      public string Access_Token { get; set; }
      public string Token_Type { get; set; }
      public int Expires_In { get; set; }
      public string Refresh_Token { get; set; }
    }

    /// <summary>
    /// Send PushMessage to Admin "POST https://api.line.me/v2/bot/message/push"
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task<bool> SendAsyncPushMessageToAdmin(string message)
    {
      var messages = new List<LineMessagingApi.SendMessage>();
      var sendmsgText = new LineMessagingApi.SendMessage();
      var sendmsgSticker = new LineMessagingApi.SendMessage();
      var pushMessage = new LineMessagingApi.PushMessage();

      sendmsgSticker.type = "sticker";
      sendmsgSticker.packageId = "1";
      sendmsgSticker.stickerId = "134";
      messages.Add(sendmsgSticker);

      sendmsgText.type = "text";
      sendmsgText.text = message;
      messages.Add(sendmsgText);

      pushMessage.to = "Uec530c6d7d661b431dcc4e14d851c7c7"; // Admin's LINE Id
      pushMessage.messages = messages;

      return await SendAsyncPushMessage(pushMessage);
    }

    /// <summary>
    /// Get Key / Value splited by "="
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private DataInKeyValue GetKeyValue(string data)
    {
      var result = new DataInKeyValue();
      string[] arrData = data.Split('=');

      result.Key = arrData[0].ToString();
      result.Value = arrData[1].ToString();

      return result;
    }

    /// <summary>
    /// ใช้สำหรับ Random ตัวอักษร
    /// </summary>
    /// <param name="numChars">จำนวนตัวอักษรที่ต้องการให้ Random ออกมา</param>
    /// <param name="seed">จำนวน seed ที่จะใช้</param>
    /// <returns></returns>
    private string GetRandomCharacter(int numChars, int seed)
    {
      string[] chars = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "P", "Q", "R", "S",
                        "T", "U", "V", "W", "X", "Y", "Z", "2", "3", "4", "5", "6", "7", "8", "9" };

      Random rnd = new Random(seed);
      string random = string.Empty;
      for (int i = 0; i < numChars; i++)
      {
        random = random + chars[rnd.Next(0, chars.Length)];
      }

      return random;
    }

    /// <summary>
    /// ใช้สำหรับ Random ตัวเลข
    /// </summary>
    /// <param name="numChars">จำนวนตัวอักษรที่ต้องการให้ Random ออกมา</param>
    /// <param name="seed">จำนวน seed ที่จะใช้</param>
    /// <returns></returns>
    private string GetRandomNumericOnly(int numChars, int seed)
    {
      string[] chars = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

      Random rnd = new Random(seed);
      string random = string.Empty;
      for (int i = 0; i < numChars; i++)
      {
        random = random + chars[rnd.Next(0, chars.Length)];
      }

      return random;
    }

    /// <summary>
    /// ใช้สำหรับ Random ตัวเลขยาว 8 หลักเท่านั้น
    /// </summary>
    /// <returns></returns>
    private string GetRandom8NumericOnly()
    {
      Random rnd = new Random();
      var result = rnd.Next(1, 99999999);
      return result.ToString().PadLeft(8, '0');
    }
    #endregion
  }
}