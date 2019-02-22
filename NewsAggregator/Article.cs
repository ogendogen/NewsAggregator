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
using XZ.NET;
using System.Diagnostics;

namespace NewsAggregator
{
    public class Article
    {
        [BsonId]
        public ObjectId ID { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("source")]
        public string Source { get; set; }
        [BsonElement("category")]
        public string Category { get; set; }
        [BsonElement("content")]
        public string Content
        {
            get
            {
                return Content;
            }
            set
            {
                try
                {
                    string input = value;
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    var xzStream = new XZInputStream(inputBytes);
                    byte[] buffer = new byte[2048];
                    StringBuilder output = new StringBuilder();
                    while (true)
                    {
                        int count = xzStream.Read(buffer, 0, buffer.Length);
                        output.Append(buffer);
                        if (count == 0) break;
                        int a;
                    }
                    Content = output.ToString();
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                }
            }
        }
        [BsonElement("pubDate")]
        public string PubDate { get; set; }

        private DateTime dtPubDate;
        public override string ToString()
        {
            return "ID: " + ID.ToString() + "\nTitle: " + Title + "\nSource:" + Source + "\nCategory: " + Category + "\nContent: " + Content + "\nPubDate: " + PubDate.ToString();
        }
    }
}
