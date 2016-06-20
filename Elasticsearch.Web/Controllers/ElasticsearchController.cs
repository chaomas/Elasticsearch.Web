using Elasticsearch.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elasticsearch.Web.Controllers
{
    /// <summary>
    /// 检索控制器
    /// </summary>
    public class ElasticsearchController : BaseApiController
    {
        /// <summary>
        /// 索引数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object Index() 
        {
           
            int length = S.test.Length;
            Random rd = new Random();
            Random rdName = new Random();
            ParallelOptions _po = new ParallelOptions();
            _po.MaxDegreeOfParallelism = 4;
            Parallel.For(0, 100, _po, c =>
            {
                var start = rd.Next(0, S.test.Length - 700);
                var startName = rd.Next(0, S.test.Length - 30);
                person p = new person() { age = DateTime.Now.Millisecond, birthday = DateTime.Now, id = Guid.NewGuid().ToString(), intro = S.test.Substring(start, 629) + c, name = S.test.Substring(startName, 29) + c, sex = true };
                ElasticSearchHelper.Intance.Index("db_test", "person", Guid.NewGuid().ToString(), p);
            });
            return 1;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <returns></returns>
        public object SearchFullFileds()
        {
            //1 搜索数据
            string key = GetStringRequest("Key");
            int? from = GetIntRequest("from");
            int? size = GetIntRequest("size");
            var list = ElasticSearchHelper.Intance.SearchFullFiledss("db_test", "person", key ?? "方鸿渐", from == null ? 0 : from.Value, size == null ? 20 : size.Value);
            return list;
        }

    }
}
