using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;

namespace NewsAggregator
{
    public class FileManager
    {
        private string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/NewsAggregator";

        public enum SortMethod
        {
            DateAscending = 1,
            DateDescending = 2
        }

        public FileManager(List<string> categories)
        {
            if (!isInstalled())
            {
                Directory.CreateDirectory(appPath);
                foreach (var category in categories)
                {
                    Directory.CreateDirectory(appPath + "/" + category);
                }
            }
        }

        private bool isCategoryExists(string category)
        {
            return Directory.Exists(appPath + "/" + category);
        }

        private bool isArticleExists(Article article)
        {
            string[] directories = Directory.GetDirectories(appPath);
            foreach (var directory in directories)
            {
                string[] files = Directory.GetFiles(appPath + directory);
                if (files.Any(x => x == article.ID + ".json")) return true;
            }
            return false;
        }

        private bool isInstalled()
        {
            return Directory.Exists(appPath);
        }

        public void createNewCategory(string category)
        {
            if (!isCategoryExists(category)) Directory.CreateDirectory(appPath + "/" + category);
        }

        public void addNewArticle(Article article)
        {
            try
            {
                string serialized = JsonConvert.SerializeObject(article);
                File.WriteAllText(appPath + "/" + article.Category + "/" + article.ID + ".json", serialized);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void addNewArticles(List<Article> articles)
        {
            try
            {
                foreach (var article in articles)
                {
                    string serialized = JsonConvert.SerializeObject(article);
                    File.WriteAllText(appPath + "/" + article.Category + "/" + article.ID + ".json", serialized);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] getAllCategories()
        {
            return Directory.GetDirectories(appPath);
        }

        public List<Article> getAllArticlesByCategory(string category)
        {
            string[] categories = getAllCategories();
            List<Article> output = new List<Article>();
            foreach (var subcategory in categories)
            {
                string[] files = Directory.GetFiles(appPath + "/" + category);
                foreach (var file in files)
                {
                    string file_content = File.ReadAllText(appPath + "/" + category + "/" + file);
                    Article article = JsonConvert.DeserializeObject<Article>(file_content);
                    output.Add(article);
                }
            }
            return output;
        }

        public List<Article> sortArticles(List<Article> articles, SortMethod sortMethod)
        {
            if (sortMethod == SortMethod.DateAscending) return articles.OrderBy(x => DateTime.ParseExact(x.PubDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
            return articles.OrderByDescending(x => DateTime.ParseExact(x.PubDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
            //return articles.Sort((x, y) => DateTime.ParseExact(x.PubDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture).CompareTo(DateTime.ParseExact(y.PubDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}
