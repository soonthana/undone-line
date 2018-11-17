using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Undone.Line.BusinessModel;
using Undone.Line.Services;

namespace Undone.Line.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestController : ControllerBase
  {
    private IConfiguration _config;
    private Firebase _fbObj;

    public TestController(IConfiguration config)
    {
      _config = config;
      _fbObj = new Firebase(_config);
    }

    // GET api/test
    [HttpGet]
    public ActionResult<string> Get()
    {
      for (var i = 1; i <= 29; i++)
      {
        var pNo = i.ToString().PadLeft(2, '0');

        var up = new UserProfiles();
        up.Id = Guid.NewGuid();
        up.LineDisplayName = "LineDisplayName" + " " + pNo;
        up.LinePictureUrl = "https://line/pic/" + pNo;
        up.LineStatusMessage = "LineStatusMessage" + " " + pNo;
        up.LineUserId = "LineUserId" + " " + pNo;
        up.OfficerEmail = "OfficerEmail" + " " + pNo;
        up.OfficerId = pNo;
        up.OfficerName = "OfficerName" + " " + pNo;
        up.OfficerPictureUrl = "https://officer/pic/" + pNo;
        up.RegisteredDateTime = DateTime.Now;
        up.RegisteringDateTime = DateTime.Now;
        up.Role = "USER";
        up.Status = true;

        var resp = _fbObj.PutUserProfiles(up);
        var result = resp.Result.Content.ReadAsStringAsync().Result;
      }

      return Ok("FINISHED");
    }



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
      http.BaseAddress = new Uri("https://api.muangthai.co.th/MTLEmployeeAPI/api/v1/employee/");
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
      http.BaseAddress = new Uri("https://mtlauthapp.azurewebsites.net/");
      http.DefaultRequestHeaders.Accept.Clear();
      http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var body = "grant_type=password&username=test&client_id=9ad41ad0594546e891f52f8707406328";

      var response = await http.PostAsync("oauth2/token", new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
      var jsonContent = response.Content.ReadAsStringAsync().Result;
      obj = JsonConvert.DeserializeObject<MTLAuthToken>(jsonContent);

      return obj;
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
    #endregion
  }
}