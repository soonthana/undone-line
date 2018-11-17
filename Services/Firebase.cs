using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Undone.Line.BusinessModel;
using Undone.Line.Framework;
using Undone.Line.Utils;

namespace Undone.Line.Services
{
  public class Firebase
  {
    private IConfiguration _config;
    private string accessToken = string.Empty;
    private string googleApiUrl = string.Empty;
    private string projectUrl = string.Empty;
    private string requestTokenUrl = string.Empty;
    private string serviceAccount = string.Empty;
    private string scope = string.Empty;
    private string rs256PrivateKeyXml = string.Empty;
    private string storageProject = string.Empty;
    private string storageBucket = string.Empty;

    public Firebase(IConfiguration config)
    {
      _config = config;
      googleApiUrl = _config["GoogleApi:ApiUrl"];
      projectUrl = _config["GoogleApi:Firebase:UndoneResources:ProjectUrl"];
      requestTokenUrl = _config["GoogleApi:Firebase:UndoneResources:RequestTokenUrl"];
      serviceAccount = _config["GoogleApi:Firebase:UndoneResources:ServiceAccount"];
      scope = _config["GoogleApi:Firebase:UndoneResources:Scope"];
      rs256PrivateKeyXml = _config["GoogleApi:Firebase:UndoneResources:Key:RS256:PrivateKeyXml"];
      storageProject = _config["GoogleApi:Firebase:UndoneResources:Storage:Project"];
      storageBucket = _config["GoogleApi:Firebase:UndoneResources:Storage:Bucket"];

      accessToken = GetAccessToken().Result;
    }

    #region PUBLIC METHODS
    public string TestGetAccessToken()
    {
      return GetAccessToken().Result;
    }

    #region Firebase Storage Resources
    public async Task<HttpResponseMessage> GetBuckets()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(googleApiUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

      var response = await client.GetAsync("storage/v1/b?project=" + storageProject);

      return response;
    }

    public async Task<HttpResponseMessage> GetObjectByObjectName(string objectName)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(googleApiUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

      var response = await client.GetAsync("storage/v1/b/" + storageBucket + "/o/" + objectName);

      return response;
    }

    public async Task<HttpResponseMessage> UploadFile(string objectName, string contentType, byte[] data)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(googleApiUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
      client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
      var streamContent = new StreamContent(new MemoryStream(data));
      streamContent.Headers.Add("Content-Type", contentType);
      var response = await client.PostAsync("upload/storage/v1/b/" + storageBucket + "/o?uploadType=media&name=" + objectName, streamContent);

      return response;
    }
    #endregion

    #region Firebase Real-time Database Resources UserProfiles
    // GET https://undone-resources.firebaseio.com/UserProfiles.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetUserProfiles()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("UserProfiles.json?access_token=" + accessToken);

      return response;
    }

    // GET https://undone-resources.firebaseio.com/UserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetUserProfilesById(Guid node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("UserProfiles/" + node + ".json?access_token=" + accessToken);

      return response;
    }

    // PUT https://undone-resources.firebaseio.com/UserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PutUserProfiles(UserProfiles user)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(user);
      var uniqueId = user.Id;
      var response = await client.PutAsync("UserProfiles/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // PATCH https://undone-resources.firebaseio.com/UserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PatchUserProfiles(UserProfiles user)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(user);
      var uniqueId = user.Id;
      var response = await client.PatchAsync("UserProfiles/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // DELETE https://undone-resources.firebaseio.com/UserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> DeleteUserProfiles(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.DeleteAsync("UserProfiles/" + node + ".json?access_token=" + accessToken);

      return response;
    }
    #endregion

    #region Firebase Real-time Database Resources LineUserProfiles
    // GET https://undone-resources.firebaseio.com/LineUserProfiles.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetLineUserProfiles()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("LineUserProfiles.json?access_token=" + accessToken);

      return response;
    }

    // GET https://undone-resources.firebaseio.com/LineUserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetLineUserProfilesById(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("LineUserProfiles/" + node + ".json?access_token=" + accessToken);

      return response;
    }

    // PUT https://undone-resources.firebaseio.com/LineUserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PutLineUserProfiles(LineUserProfiles user)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(user);
      var uniqueId = user.LineUserId;
      var response = await client.PutAsync("LineUserProfiles/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // PATCH https://undone-resources.firebaseio.com/LineUserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PatchLineUserProfiles(LineUserProfiles user)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(user);
      var uniqueId = user.LineUserId;
      var response = await client.PatchAsync("LineUserProfiles/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // DELETE https://undone-resources.firebaseio.com/LineUserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> DeleteLineUserProfiles(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.DeleteAsync("LineUserProfiles/" + node + ".json?access_token=" + accessToken);

      return response;
    }
    #endregion

    #region Firebase Real-time Database Resources OfficerUserProfiles
    // GET https://undone-resources.firebaseio.com/OfficerUserProfiles.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetOfficerUserProfiles()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("OfficerUserProfiles.json?access_token=" + accessToken);

      return response;
    }

    // GET https://undone-resources.firebaseio.com/OfficerUserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetOfficerUserProfilesById(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("OfficerUserProfiles/" + node + ".json?access_token=" + accessToken);

      return response;
    }

    // PUT https://undone-resources.firebaseio.com/OfficerUserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PutOfficerUserProfiles(OfficerUserProfiles user)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(user);
      var uniqueId = user.OfficerId;
      var response = await client.PutAsync("OfficerUserProfiles/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // PATCH https://undone-resources.firebaseio.com/OfficerUserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PatchOfficerUserProfiles(OfficerUserProfiles user)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(user);
      var uniqueId = user.OfficerId;
      var response = await client.PatchAsync("OfficerUserProfiles/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // DELETE https://undone-resources.firebaseio.com/OfficerUserProfiles/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> DeleteOfficerUserProfiles(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.DeleteAsync("OfficerUserProfiles/" + node + ".json?access_token=" + accessToken);

      return response;
    }
    #endregion

    #region Firebase Real-time Database Resources OneTimePasswords
    // GET https://undone-resources.firebaseio.com/OneTimePasswords.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetOneTimePasswords()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("OneTimePasswords.json?access_token=" + accessToken);

      return response;
    }

    // GET https://undone-resources.firebaseio.com/OneTimePasswords/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetOneTimePasswordsById(Guid node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("OneTimePasswords/" + node + ".json?access_token=" + accessToken);

      return response;
    }

    // PUT https://undone-resources.firebaseio.com/OneTimePasswords/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PutOneTimePasswords(OneTimePasswords otp)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(otp);
      var uniqueId = otp.UserProfileId;
      var response = await client.PutAsync("OneTimePasswords/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // PATCH https://undone-resources.firebaseio.com/OneTimePasswords/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PatchOneTimePasswords(OneTimePasswords otp)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(otp);
      var uniqueId = otp.UserProfileId;
      var response = await client.PatchAsync("OneTimePasswords/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // DELETE https://undone-resources.firebaseio.com/OneTimePasswords/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> DeleteOneTimePasswords(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.DeleteAsync("OneTimePasswords/" + node + ".json?access_token=" + accessToken);

      return response;
    }
    #endregion

    #region Firebase Real-time Database Resources Systems
    // GET https://undone-resources.firebaseio.com/Systems.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetSystems()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("Systems.json?access_token=" + accessToken);

      return response;
    }

    // GET https://undone-resources.firebaseio.com/Systems/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetSystemsById(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("Systems/" + node + ".json?access_token=" + accessToken);

      return response;
    }

    // PUT https://undone-resources.firebaseio.com/Systems/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PutSystems(Systems system)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(system);
      var uniqueId = system.Id;
      var response = await client.PutAsync("Systems/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // PATCH https://undone-resources.firebaseio.com/Systems/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PatchSystems(Systems system)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(system);
      var uniqueId = system.Id;
      var response = await client.PatchAsync("Systems/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // DELETE https://undone-resources.firebaseio.com/Systems/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> DeleteSystems(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.DeleteAsync("Systems/" + node + ".json?access_token=" + accessToken);

      return response;
    }
    #endregion

    #region Firebase Real-time Database Resources UserSystems
    // GET https://undone-resources.firebaseio.com/UserSystems.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetUserSystems()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("UserSystems.json?access_token=" + accessToken);

      return response;
    }

    // GET https://undone-resources.firebaseio.com/UserSystems/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> GetUserSystemsById(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.GetAsync("UserSystems/" + node + ".json?access_token=" + accessToken);

      return response;
    }

    // PUT https://undone-resources.firebaseio.com/UserSystems/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PutUserSystems(UserSystems user)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(user);
      var uniqueId = user.UserProfileId;
      var response = await client.PutAsync("UserSystems/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // PATCH https://undone-resources.firebaseio.com/UserSystems/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> PatchUserSystems(UserSystems user)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var jsonString = JsonConvert.SerializeObject(user);
      var uniqueId = user.UserProfileId;
      var response = await client.PatchAsync("UserSystems/" + uniqueId + ".json?access_token=" + accessToken, new StringContent(jsonString, Encoding.UTF8, "application/json"));

      return response;
    }

    // DELETE https://undone-resources.firebaseio.com/UserSystems/<SPECIFIC_NODE>.json?access_token=<ACCESS_TOKEN>
    public async Task<HttpResponseMessage> DeleteUserSystems(string node)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(projectUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.DeleteAsync("UserSystems/" + node + ".json?access_token=" + accessToken);

      return response;
    }
    #endregion

    #endregion


    #region PRIVATE METHODS
    // POST https://www.googleapis.com/oauth2/v4/token
    private async Task<string> GetAccessToken()
    {
      var jwtRequest = GenerateJwtRequestByRSAKey();

      var body = "grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion=" + jwtRequest;

      var client = new HttpClient();
      client.BaseAddress = new Uri(requestTokenUrl);
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

      var response = await client.PostAsync("token", new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));

      if (response.StatusCode == HttpStatusCode.OK)
      {
        var jsonContent = response.Content.ReadAsStringAsync().Result;
        var obj = JsonConvert.DeserializeObject<GoogleAccessToken>(jsonContent);

        return obj.access_token;
      }
      else
      {
        return "";
      }
    }

    private string GenerateJwtRequestByRSAKey()
    {
      var payloadObj = new Payload();
      payloadObj.iss = serviceAccount;
      payloadObj.scope = scope;
      payloadObj.aud = requestTokenUrl + "token";
      payloadObj.exp = Convert.ToInt32(DateTimes.ConvertToUnixTimeByDateTime(DateTime.UtcNow.AddMinutes(60)));
      payloadObj.iat = Convert.ToInt32(DateTimes.ConvertToUnixTimeByDateTime(DateTime.UtcNow));

      SigningCredentials creds;

      using (RSA privateRsa = RSA.Create())
      {
        var privateKeyXml = File.ReadAllText(rs256PrivateKeyXml);
        privateRsa.fromXmlString(privateKeyXml);
        var privateKey = new RsaSecurityKey(privateRsa);
        creds = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
      }

      var claims = new[] {
        new Claim("scope", payloadObj.scope),
        new Claim(JwtRegisteredClaimNames.Iat, payloadObj.iat.ToString()),
        new Claim(JwtRegisteredClaimNames.Exp, payloadObj.exp.ToString())
      };
      var token = new JwtSecurityToken(
        payloadObj.iss,
        payloadObj.aud,
        claims,
        signingCredentials: creds
      );

      var result = new JwtSecurityTokenHandler().WriteToken(token);

      return result;
    }

    private class Payload
    {
      public string iss { get; set; }
      public string scope { get; set; }
      public string aud { get; set; }
      public int exp { get; set; }
      public int iat { get; set; }
    }

    private class GoogleAccessToken
    {
      public string access_token { get; set; }
      public string token_type { get; set; }
      public int expires_in { get; set; }
    }
    #endregion
  }
}