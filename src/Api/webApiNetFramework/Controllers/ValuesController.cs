using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;
using webApiNetFramework.Filter;

namespace webApiNetFramework.Controllers
{
    [CustomAuthorizationFilter(ClaimType ="Roles", ClaimValue ="App.Role")]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            ClaimsPrincipal p = RequestContext?.Principal as ClaimsPrincipal;

            return new string[] { "value1", "value2", p?.Identity?.Name ?? "<null>" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
