using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Diagnostics;

namespace NewsAggregator
{
    public class Article
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("source")]
        public string Source { get; set; }
        [BsonElement("category")]
        public string Category { get; set; }
        [BsonElement("content")]
        public string Content { get; set; }
        [BsonElement("pubDate")]
        public DateTime PubDate { get; set; }

        public override string ToString()
        {
            return Title;
            //return "ID: " + ID.ToString() + "\nTitle: " + Title + "\nSource:" + Source + "\nCategory: " + Category + "\nContent: " + Content + "\nPubDate: " + PubDate.ToString();
        }
    }
}
