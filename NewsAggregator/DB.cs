using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
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
        private IMongoCollection<Source> sources;

        public DB(string connectionString="mongodb://localhost", string dbName="aggregator")
        {
            client = new MongoClient(connectionString);
            db = client.GetDatabase(dbName);
            articles = db.GetCollection<Article>("articles");
            sources = db.GetCollection<Source>("sources");
        }

        public List<Article> getAllArticlesByCategory(string category)
        {
            return articles.Find(new BsonDocument()).ToList().Where(x => x.Category == category).Cast<Article>().ToList();
        }

        public List<string> getAllCategories()
        {
            var s_sources = sources.Find(new BsonDocument()).ToList();
            HashSet<string> categories = new HashSet<string>();
            s_sources.ForEach(x => categories.Add(x.Category));
            return categories.ToList();
        }

        public List<Article> getAllArticlesSinceDate(DateTime dt)
        {
            //Int32 unixTimestamp = (Int32)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //return articles.Find(new BsonDocument()).ToList().Where(x => x.PubDate > unixTimestamp).Cast<Article>().ToList();
            var articles = getAllArticles();
            List<Article> output = new List<Article>();
            foreach (var article in articles)
            {
                //Fri, 22 Feb 19 14:12:00 +0100
                string s_articleDate = article.PubDate; // ddd, dd MMM yy HH:MM:ss GMT
                s_articleDate = s_articleDate.Remove(s_articleDate.Length - 5, 5).Trim();
                DateTime dt_articleDate = DateTime.ParseExact(s_articleDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture);
                if (dt_articleDate >= dt) output.Add(article);
            }
            return output;
        }

        public List<Article> getAllArticlesSinceDateByCategory(DateTime dt, string category)
        {
            var articles = getAllArticlesByCategory(category);
            List<Article> output = new List<Article>();
            foreach (var article in articles)
            {
                //Fri, 22 Feb 19 14:12:00 +0100
                string s_articleDate = article.PubDate; // ddd, dd MMM yy HH:MM:ss GMT
                s_articleDate = s_articleDate.Remove(s_articleDate.Length - 5, 5).Trim();
                DateTime dt_articleDate = DateTime.ParseExact(s_articleDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture);
                if (dt_articleDate >= dt) output.Add(article);
            }
            return output;
        }

        public List<Article> getAllArticles()
        {
            return articles.Find(new BsonDocument()).ToList().Cast<Article>().ToList();
        }

        public List<Article> getArticlesBySource(string source)
        {
            return getAllArticles().Where(x => x.Source == source).ToList();
        }
    }
}
