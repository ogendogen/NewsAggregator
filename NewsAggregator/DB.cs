using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace NewsAggregator
{
    public class DB
    {
        private MongoClient client;
        private IMongoDatabase db;
        private IMongoCollection<Article> articles;

        public DB(string connectionString="mongodb://localhost", string dbName="aggregator")
        {
            client = new MongoClient(connectionString);
            db = client.GetDatabase(dbName);
            articles = db.GetCollection<Article>("articles");
        }

        public List<Article> getAllArticlesByCategory(string category)
        {
            return articles.Find(new BsonDocument()).ToList().Where(x => x.Category == category).Cast<Article>().ToList();
        }

        //public List<Article> getAllArticlesSinceDate(DateTime dt)
        //{
        //    Int32 unixTimestamp = (Int32)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //    return articles.Find(new BsonDocument()).ToList().Where(x => x.PubDate > unixTimestamp).Cast<Article>().ToList();
        //}

        public List<Article> getAllArticles()
        {
            return articles.Find(new BsonDocument()).ToList().Cast<Article>().ToList();
        }
    }
}
