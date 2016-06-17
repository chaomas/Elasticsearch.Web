using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elasticsearch.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class ik
    {
        /// <summary>
        /// 
        /// </summary>
        public List<tokens> tokens { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class tokens
    {
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int start_offset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int end_offset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int position { get; set; }
    }

    /// <summary>
    /// 测试数据对象
    /// </summary>
    public class personList
    {
        /// <summary>
        /// 
        /// </summary>
        public personList()
        {
            this.list = new List<person>();
        }
        /// <summary>
        /// 
        /// </summary>
        public int hits { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int took { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<person> list { get; set; }
    }
    /// <summary>
    /// 人名
    /// </summary>
    public class person
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int age { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool sex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime birthday { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string intro { get; set; }
    }
}