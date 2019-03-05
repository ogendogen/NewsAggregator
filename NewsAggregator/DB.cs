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

        public DB(string connectionString= "mongodb+srv://aggregator:aggregator@cluster0-9mhht.mongodb.net/test?retryWrites=true", string dbName="aggregator")
        {
            var mongoUrl = new MongoUrl(connectionString);
            client = new MongoClient(mongoUrl);
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

        public List<Source> getAllSources()
        {
            try
            {
                var bson_sources = db.GetCollection<Source>("sources");
                var sources_list = bson_sources.Find(new BsonDocument()).ToList();
                List<Source> output = new List<Source>();
                sources_list.ForEach(x => output.Add(x));
                return output;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void addNewSource(Source source)
        {
            try
            {
                sources.InsertOne(source);
                addNewCategory(source.Category);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void removeSource(Source source)
        {
            try
            {
                sources.DeleteOne(source.ToBsonDocument());
                removeCategory(source.Category);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void addNewCategory(string category)
        {
            try
            {
                var bson_sources = db.GetCollection<BsonDocument>("sources");
                BsonDocument metadata = bson_sources.Find(new BsonDocument("metadata", "1")).ToBsonDocument();

                BsonElement buffer;
                if (metadata.TryGetElement(category, out buffer)) return; // taka kategoria już istnieje

                metadata.Add(new BsonElement(category, DateTime.Now.ToString("ddd, dd MMM yy HH:mm:ss", new CultureInfo("en-US"))));
                bson_sources.ReplaceOne(new BsonDocument("_id", metadata["_id"]), metadata);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void removeCategory(string category, bool removeArticles = false)
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
    }
}
