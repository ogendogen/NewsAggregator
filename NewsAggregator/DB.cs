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
            var filterBuilder = Builders<Article>.Filter;
            var filter = filterBuilder.Gte(x => x.PubDate, dt);
            return articles.Find(filter).ToList();
        }

        public List<Article> getAllArticlesSinceDateByCategory(DateTime dt, string category)
        {
            var filterBuilder = Builders<Article>.Filter;
            var filter = filterBuilder.Gte(x => x.PubDate, dt) & filterBuilder.Eq(x => x.Category, category);
            return articles.Find(filter).ToList();
        }

        public List<Article> getAllArticles()
        {
            return articles.Find(new BsonDocument()).ToList().Cast<Article>().ToList();
        }

        public List<Article> getArticlesBySource(string source)
        {
            var filterBuilder = Builders<Article>.Filter;
            var filter = filterBuilder.Eq(x => x.Source, source);
            return articles.Find(filter).ToList();
            //return getAllArticles().Where(x => x.Source == source).ToList();
        }

        public void addNewCategory(string category)
        {
            try
            {
                var bson_sources = db.GetCollection<BsonDocument>("sources");
                BsonDocument metadata = bson_sources.Find(new BsonDocument("metadata", "1")).ToBsonDocument();
                metadata.Add(new BsonElement(category, DateTime.Now.ToString("ddd, dd MMM yy HH:mm:ss")));
                bson_sources.ReplaceOne(new BsonDocument("_id", metadata["_id"]), metadata);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void removeCategory(string category, bool removeArticles=true)
        {
            var bson_sources = db.GetCollection<BsonDocument>("sources");
            BsonDocument metadata = bson_sources.Find(new BsonDocument("metadata", "1")).ToBsonDocument();
            metadata.Remove(category);
            bson_sources.ReplaceOne(new BsonDocument("_id", metadata["_id"]), metadata);

            if (removeArticles)
            {
                articles.DeleteMany(new BsonDocument("category", category));
            }
        }

        public void addNewSource(Source source)
        {
            sources.InsertOne(source);
        }

        public void removeSource(Source source)
        {
            sources.DeleteOne(source.ToBsonDocument());
        }
    }
}
