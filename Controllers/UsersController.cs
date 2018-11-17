using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Undone.Line.BusinessModel;
using Undone.Line.Services;
using Undone.Line.Utils;

namespace Undone.Line.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private IConfiguration _config;
    private Firebase _fireObj;
    private const int DEFAULT_LIMIT = 10;

    public UsersController(IConfiguration config)
    {
      _config = config;
      _fireObj = new Firebase(_config);
    }

    // GET api/users
    // GET api/users?offset=1&limit=1
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] int? offset, int? limit)
    {
      if (CheckIsValidJwt())
      {
        // Data Payload
        var users = new List<UserProfilesPayload>();

        var resp = await _fireObj.GetUserProfiles();
        var content = resp.Content.ReadAsStringAsync().Result;

        if (resp.StatusCode == HttpStatusCode.OK && (content != "null" && content != null && content != ""))
        {
          var jObj = JObject.Parse(content);

          foreach (var jItem in jObj)
          {
            var up = jItem.Value.ToObject<UserProfilesPayload>();

            var listTypes = new List<string>();
            listTypes.Add("application/json");

            up.links = new UserProfilesPayloadLinksDetail
            {
              rel = "User " + up.OfficerId + " " + up.OfficerName,
              href = Request.Scheme + "://" + Request.Host + Request.Path.Value + "/" + up.Id,
              action = "GET",
              types = listTypes
            };

            users.Add(up);
          }

          // Pagination Payload
          var pTotal = users.Count;

          var pOffset = 0;
          var pLimit = 0;
          var pNext = 0;
          var pPrev = 0;
          var pFirst = 0;
          var pLast = 0;

          if ((offset != null && limit != null)) // With Pagination QueryString
          {
            pOffset = offset.Value;

            if (offset >= 0 && limit >= 1)
            {
              users = users.OrderBy(u => u.OfficerId).Skip(offset.Value).Take(limit.Value).ToList();

              pLimit = limit.Value;

              pNext = pOffset + pLimit >= pTotal ? pOffset : pOffset + pLimit;
              pPrev = pOffset - pLimit < 0 ? 0 : pOffset - pLimit;
              pLast = (((pTotal - 1) - pOffset) % pLimit) == 0 ? pTotal - 1 : (pTotal - 1) - (((pTotal - 1) - pOffset) % pLimit);
            }
          }
          else // Without Pagination QueryString
          {
            pOffset = 0;

            if (pTotal < DEFAULT_LIMIT)
            {
              users = users.OrderBy(u => u.OfficerId).Skip(0).Take(pTotal).ToList();

              pLimit = pTotal;

              pNext = 0;
              pPrev = 0;
              pLast = 0;
            }
            else
            {
              users = users.OrderBy(u => u.OfficerId).Skip(0).Take(DEFAULT_LIMIT).ToList();

              pLimit = DEFAULT_LIMIT;

              pNext = pOffset + pLimit >= pTotal ? pOffset : pOffset + pLimit;
              pPrev = 0;
              pLast = (((pTotal - 1) - pOffset) % pLimit) == 0 ? pTotal - 1 : (pTotal - 1) - (((pTotal - 1) - pOffset) % pLimit);
            }
          }

          var pagination = new PaginationPayload
          {
            offset = pOffset,
            limit = pLimit,
            total = pTotal,
            links = new PaginationPayloadLinksDetail
            {
              next = Request.Scheme + "://" + Request.Host + Request.Path.Value + "?offset=" + pNext + "&limit=" + pLimit,
              prev = Request.Scheme + "://" + Request.Host + Request.Path.Value + "?offset=" + pPrev + "&limit=" + pLimit,
              first = Request.Scheme + "://" + Request.Host + Request.Path.Value + "?offset=" + pFirst + "&limit=" + pLimit,
              last = Request.Scheme + "://" + Request.Host + Request.Path.Value + "?offset=" + pLast + "&limit=" + pLimit
            }
          };

          return Ok(new { users, pagination });
        }
        else
        {
          return CustomHttpResponse.Error(HttpStatusCode.NotFound, "UND002", "No Data Found.", "ไม่พบข้อมูล", "No data found.");
        }
      }
      else
      {
        return CustomHttpResponse.Error(HttpStatusCode.Unauthorized, "UND999", "Unauthorized, Invalid AccessToken.", "แอพฯ ของคุณไม่มีสิทธิ์ใช้งาน เนื่องจาก AccessToken ไม่ถูกต้อง หรือหมดอายุแล้ว, กรุณาติดต่อผู้ดูแลแอพฯ ของคุณ", "The AccessToken is invalid or expired, please contact your Application Administrator.");
      }
    }

    // GET /api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult> Get(string id)
    {
      Guid guid;

      if (Guid.TryParse(id, out guid))
      {
        if (CheckIsValidJwt())
        {
          // Data Payload
          var resp = await _fireObj.GetUserProfilesById(guid);
          var content = resp.Content.ReadAsStringAsync().Result;

          if (resp.StatusCode == HttpStatusCode.OK && (content != "null" && content != null && content != ""))
          {
            var user = JsonConvert.DeserializeObject<UserProfiles>(content);

            return Ok(user);
          }
          else
          {
            return CustomHttpResponse.Error(HttpStatusCode.NotFound, "UND002", "No Data Found.", "ไม่พบข้อมูล", "No data found.");
          }
        }
        else
        {
          return CustomHttpResponse.Error(HttpStatusCode.Unauthorized, "UND999", "Unauthorized, Invalid AccessToken.", "แอพฯ ของคุณไม่มีสิทธิ์ใช้งาน เนื่องจาก AccessToken ไม่ถูกต้อง หรือหมดอายุแล้ว, กรุณาติดต่อผู้ดูแลแอพฯ ของคุณ", "The AccessToken is invalid or expired, please contact your Application Administrator.");
        }
      }
      else
      {
        return CustomHttpResponse.Error(HttpStatusCode.Unauthorized, "UND001", "Invalid Request (Id '" + id + "').", "คุณไม่มีสิทธิ์ใช้งาน เนื่องจากส่งคำร้องขอมาไม่ถูกต้อง", "The request is invalid.");
      }
    }

    // // POST /api/users
    // [HttpPost]
    // public ActionResult Post([FromBody] string value)
    // {
    //   if (CheckIsValidJwt())
    //   {
    //     return Created("ddd", "dd");
    //   }
    //   else
    //   {
    //     return CustomHttpResponse.Error(HttpStatusCode.Unauthorized, "UND999", "Unauthorized, Invalid AccessToken.", "แอพฯ ของคุณไม่มีสิทธิ์ใช้งาน เนื่องจาก AccessToken ไม่ถูกต้อง หรือหมดอายุแล้ว, กรุณาติดต่อผู้ดูแลแอพฯ ของคุณ", "The AccessToken is invalid or expired, please contact your Application Administrator.");
    //   }
    // }


    #region TEMPLATE
    // GET api/customer/5
    // [HttpGet("{id}")]
    // public IActionResult Get(string id)
    // {
    //   // TODO: เช็ค input paramter
    //   if (id.Length == 6)
    //   {
    //     // TODO: เช็คสิทธิ์
    //     if (true)
    //     {
    //       // TODO: อ่านข้อมูลในฐานข้อมูล
    //       if (true)
    //       {
    //         return Ok("data"); // 200 (OK) พร้อมส่งข้อมูลตอบกลับไปใน body payload
    //       }
    //       else
    //       {
    //         return NotFound(); // 404 (Not Found)
    //       }
    //     }
    //     else
    //     {
    //       return Unauthorized(); // 401 (Unauthorized)
    //     }
    //   }
    //   else
    //   {
    //     return BadRequest(); // 400 (Bad Request)
    //   }
    // }


    //  POST api/customer
    // [HttpPost]
    // public IActionResult Post([FromBody] string data)
    // {
    //   // TODO: เช็ค input paramter
    //   if (ModelState.IsValid)
    //   {
    //     // TODO: เช็คสิทธิ์
    //     if (true)
    //     {
    //       // TODO: เพิ่มข้อมูลเข้าไปในฐานข้อมูล
    //       if (true)
    //       {
    //         return Created("api/customer/newid", data); // 201 (Created) พร้อมบอก resource ใหม่ และส่งข้อมูลตอบกลับไปใน body payload
    //       }
    //       else
    //       {
    //         return StatusCode(503); // 503 (Service Unavailable)
    //       }
    //     }
    //     else
    //     {
    //       return Unauthorized(); // 401 (Unauthorized)
    //     }
    //   }
    //   else
    //   {
    //     return BadRequest(); // 400 (Bad Request)
    //   }
    // }
    #endregion



    #region Private methods
    private bool CheckIsValidJwt()
    {
      var result = false;

      // Get Authorization header value
      if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
      {
        return result;
      }
      else
      {
        var token = authorization.First();
        token = token.Replace("Bearer ", "");

        var jwt = new JwtChecker(_config);

        return jwt.Check(token).Result;
      }
    }
    #endregion
  }
}