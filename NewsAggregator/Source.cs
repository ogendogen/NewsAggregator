using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NewsAggregator
{
    [BsonIgnoreExtraElements]
    public class Source
    {
        [BsonId]
        public ObjectId ID { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("url")]
        public string URL { get; set; }
        [BsonElement("category")]
        public string Category { get; set; }
        [BsonElement("lastBuildDate")]
        public string LastBuildDate { get; set; }
    }
}
