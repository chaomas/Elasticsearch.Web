using PlainElastic.Net;
using PlainElastic.Net.Mappings;
using PlainElastic.Net.Queries;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elasticsearch.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class ElasticSearchHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly ElasticSearchHelper Intance = new ElasticSearchHelper();
        private ElasticConnection Client;
        private ElasticSearchHelper()
        {
            var node = new Uri("http://localhost:9200");
            Client = new ElasticConnection("localhost", 9200);
        }
        /// <summary>
        /// 数据索引
        /// </summary>       
        /// <param name="indexName">索引名称</param>
        /// <param name="indexType">索引类型</param>
        /// <param name="id">索引文档id，不能重复,如果重复则覆盖原先的</param>
        /// <param name="jsonDocument">要索引的文档,json格式</param>
        /// <returns></returns>
        public IndexResult Index(string indexName, string indexType, string id, string jsonDocument)
        {

            var serializer = new JsonNetSerializer();
            string cmd = new IndexCommand(indexName, indexType, id);
            OperationResult result = Client.Put(cmd, jsonDocument);

            var indexResult = serializer.ToIndexResult(result.Result);
            return indexResult;
        }
        /// <summary>
        /// 
        /// </summary>
        public IndexResult Index(string indexName, string indexType, string id, object document)
        {
            var serializer = new JsonNetSerializer();
            var jsonDocument = serializer.Serialize(document);
            return Index(indexName, indexType, id, jsonDocument);
        }

        
       
         /// <summary>
        /// //全文检索，单个字段或者多字段 或关系  //字段intro 包含词组key中的任意一个单词
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="indexType"></param>
        /// <param name="key"></param>
        /// <param name="from"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public personList Search(string indexName, string indexType, string key, int from, int size) 
        {
            string cmd = new SearchCommand(indexName, indexType);
            string query = new QueryBuilder<person>()
                //1 查询
                .Query(b =>
                            b.Bool(m =>
                                //并且关系
                                m.Must(t =>

                                    //分词的最小单位或关系查询
                                   t.QueryString(t1 => t1.DefaultField("intro").Query(key))
                                    //.QueryString(t1 => t1.DefaultField("name").Query(key))
                                    // t .Terms(t2=>t2.Field("intro").Values("研究","方鸿渐"))
                                    //范围查询
                                    // .Range(r =>  r.Field("age").From("100").To("200") )  
                                     )
                                  )
                                )
                //分页
                 .From(from)
                 .Size(size)
                //排序
                // .Sort(c => c.Field("age", SortDirection.desc))
                //添加高亮
                  .Highlight(h => h
                      .PreTags("<b>")
                      .PostTags("</b>")
                      .Fields(
                             f => f.FieldName("intro").Order(HighlightOrder.score),
                             f => f.FieldName("_all")
                             )
                     )
                    .Build();


            string result = Client.Post(cmd, query);
            var serializer = new JsonNetSerializer();

            var list = serializer.ToSearchResult<person>(result);
            personList datalist = new personList();
            datalist.hits = list.hits.total;
            datalist.took = list.took;
            var personList = list.hits.hits.Select(c => c._source).ToList();
            datalist.list.AddRange(personList);
            return datalist;

           
        }
   
        /// <summary>
        ///   //全文检索，多字段 并关系  //字段intro 或者name 包含词组key    
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="indexType"></param>
        /// <param name="key"></param>
        /// <param name="from"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public personList SearchFullFileds(string indexName, string indexType, string key, int from, int size)
        {
            MustQuery<person> mustNameQueryKeys = new MustQuery<person>();
            MustQuery<person> mustIntroQueryKeys = new MustQuery<person>();
            var arrKeys = GetIKTokenFromStr(key);
            foreach (var item in arrKeys)
            {
                mustNameQueryKeys = mustNameQueryKeys.Term(t3 => t3.Field("name").Value(item)) as MustQuery<person>;
                mustIntroQueryKeys = mustIntroQueryKeys.Term(t3 => t3.Field("intro").Value(item)) as MustQuery<person>;
            }

            string cmd = new SearchCommand(indexName, indexType);
            string query = new QueryBuilder<person>()
                //1 查询
                .Query(b =>
                            b.Bool(m =>
                                m.Should(t =>
                                         t.Bool(m1 =>
                                                     m1.Must(
                                                             t2 =>
                                                                 //t2.Term(t3=>t3.Field("name").Value("研究"))
                                                                 //   .Term(t3=>t3.Field("name").Value("方鸿渐"))  
                                                                 mustNameQueryKeys
                                                             )
                                                )
                                       )
                               .Should(t =>
                                         t.Bool(m1 =>
                                                     m1.Must(t2 =>
                                                         //t2.Term(t3 => t3.Field("intro").Value("研究"))
                                                         //.Term(t3 => t3.Field("intro").Value("方鸿渐"))  
                                                                     mustIntroQueryKeys
                                                            )
                                                )
                                      )
                                  )
                        )
                //分页
                 .From(from)
                 .Size(size)
                //排序
                // .Sort(c => c.Field("age", SortDirection.desc))
                //添加高亮
                  .Highlight(h => h
                      .PreTags("<b>")
                      .PostTags("</b>")
                      .Fields(
                             f => f.FieldName("intro").Order(HighlightOrder.score),
                              f => f.FieldName("name").Order(HighlightOrder.score)
                             )
                     )
                    .Build();


            string result = Client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            var list = serializer.ToSearchResult<person>(result);
            personList datalist = new personList();
            var personList = list.hits.hits.Select(c => c._source).ToList();
            datalist.hits = list.hits.total;
            datalist.took = list.took;
            datalist.list.AddRange(personList);
            return datalist;


        }

        //全文检索，多字段 并关系
        //搜索age在100到200之间，并且字段intro 或者name 包含词组key
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="indexType"></param>
        /// <param name="key"></param>
        /// <param name="from"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public personList SearchFullFiledss(string indexName, string indexType, string key, int from, int size)
        {
            MustQuery<person> mustNameQueryKeys = new MustQuery<person>();
            MustQuery<person> mustIntroQueryKeys = new MustQuery<person>();
            var arrKeys = GetIKTokenFromStr(key);
            foreach (var item in arrKeys)
            {
                mustNameQueryKeys = mustNameQueryKeys.Term(t3 => t3.Field("name").Value(item)) as MustQuery<person>;
                mustIntroQueryKeys = mustIntroQueryKeys.Term(t3 => t3.Field("intro").Value(item)) as MustQuery<person>;
            }

            string cmd = new SearchCommand(indexName, indexType);
            string query = new QueryBuilder<person>()
                //1 查询
                .Query(b =>
                            b.Bool(m =>
                                m.Must(t =>
                                          t.Range(r => r.Field("age").From("1").To("500"))
                                          .Bool(ms =>
                                                    ms.Should(ts =>
                                                             ts.Bool(m1 =>
                                                                         m1.Must(
                                                                                 t2 =>
                                                                                     //t2.Term(t3=>t3.Field("name").Value("研究"))
                                                                                     //   .Term(t3=>t3.Field("name").Value("方鸿渐"))  
                                                                                     //
                                                                                      mustNameQueryKeys
                                                                                 )
                                                                    )
                                                           )
                                                   .Should(ts =>
                                                             ts.Bool(m1 =>
                                                                         m1.Must(t2 =>
                                                                             //t2.Term(t3 => t3.Field("intro").Value("研究"))
                                                                             //.Term(t3 => t3.Field("intro").Value("方鸿渐"))  

                                                                                        //
                                                                                        mustIntroQueryKeys
                                                                                )
                                                                    )
                                                          )
                                                      )
                                                        )
                                                      )
                       )
                //分页
                 .From(from)
                 .Size(size)
                //排序
                // .Sort(c => c.Field("age", SortDirection.desc))
                //添加高亮
                  .Highlight(h => h
                      .PreTags("<b>")
                      .PostTags("</b>")
                      .Fields(
                             f => f.FieldName("intro").Order(HighlightOrder.score),
                              f => f.FieldName("name").Order(HighlightOrder.score)
                             )
                     )
                    .Build();


            string result = Client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            var list = serializer.ToSearchResult<person>(result);
            var personList = list.hits.hits.Select(c => c._source).ToList();
            personList datalist = new personList();
            datalist.hits = list.hits.total;
            datalist.took = list.took;
            datalist.list.AddRange(personList);
            return datalist;


        }

        //分词映射
        private static string BuildCompanyMapping()
        {
            return new MapBuilder<person>()
                .RootObject(typeName: "Person",
                            map: r => r
                    .All(a => a.Enabled(false))
                    .Dynamic(false)
                    .Properties(pr => pr
                        .String(Person => Person.name, f => f.Analyzer(DefaultAnalyzers.standard).Boost(2))
                        .String(Person => Person.intro, f => f.Analyzer("ik"))
                        )
              )
              .BuildBeautified();
        }

        //将语句用ik分词，返回分词结果的集合
        private List<string> GetIKTokenFromStr(string key)
        {
            string s = "/db_test/_analyze?analyzer=ik";
            var result = Client.Post(s, "{" + key + "}");
            var serializer = new JsonNetSerializer();
            var list = serializer.Deserialize(result, typeof(ik)) as ik;
            return list.tokens.Select(c => c.token).ToList();
        }


    }

}