using System;
//using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Attributes;

namespace SocialTrender
{
    public class SearchDataModel
    {
        //[BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; } = "";
        public SearchData Data { get; set; }

        public SearchDataModel(SearchData data)
        {
            Data = data;
        }
    }

    public struct SearchData
    {
        public string Keyword { get; set; }
        public PostData[] Posts { get; set; }

        public SearchData(string keyword, PostData[] posts)
        {
            Keyword = keyword;
            Posts = posts;
        }
    }

    public struct PostData
    {
        public string Link { get; set; }
        public string[] Tags { get; set; }

        public PostData(string link, string[] tags)
        {
            Link = link;
            Tags = tags;
        }
    }
}
