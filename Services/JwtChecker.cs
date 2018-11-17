using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Undone.Line.Services
{
  public class JwtChecker
  {
    private IConfiguration _config;

    public JwtChecker(IConfiguration config)
    {
      _config = config;
    }

    public async Task<bool> Check(string token)
    {
      var result = false;

      try
      {
        var http = new HttpClient();
        http.BaseAddress = new Uri(_config["AuthUrl"]);
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        http.DefaultRequestHeaders.Add("Auth-Jwt", token);

        var response = await http.PostAsync("/api/auth", new StringContent(""));

        if (response.StatusCode == HttpStatusCode.OK)
        {
          result = true;
        }

        return result;
      }
      catch
      {
        return result;
      }
    }
  }
}