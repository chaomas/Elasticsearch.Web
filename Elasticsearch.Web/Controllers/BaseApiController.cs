using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Elasticsearch.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseApiController : ApiController
    {
     
        /// <summary>
        /// 
        /// </summary>
        public BaseApiController() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        public BaseApiController(string collectionName)
        {
          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramter"></param>
        /// <returns></returns>
        [NonAction]
        public string GetStringRequest(string paramter)
        {
            return HttpContext.Current.Request.QueryString[paramter] ?? "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramter"></param>
        /// <returns></returns>
         [NonAction]
        public int? GetIntRequest(string paramter)
        {
            string tmp = HttpContext.Current.Request.QueryString[paramter] ?? "";
            int tag = 0;
            if (!int.TryParse(tmp, out tag))
            {
                return null;
            }
            return tag;
        }
    }
}
