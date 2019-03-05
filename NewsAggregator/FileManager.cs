using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography;

namespace NewsAggregator
{
    public class FileManager
    {
        private string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NewsAggregator";

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
                    Directory.CreateDirectory(appPath + "\\" + category);
                }
            }
        }

        private bool isCategoryExists(string category)
        {
            return Directory.Exists(appPath + "\\" + category);
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
            if (!isCategoryExists(category)) Directory.CreateDirectory(appPath + "\\" + category);
        }

        public void addNewArticle(Article article)
        {
            try
            {
                string serialized = JsonConvert.SerializeObject(article);
                File.WriteAllText(appPath + "\\" + article.Category + "\\" + article.ID + ".json", serialized);
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
                    File.WriteAllText(appPath + "\\" + article.Category + "\\" + article.ID + ".json", serialized);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] getAllCategories()
        {
            var output = new List<string>();
            Directory.GetDirectories(appPath)
                .ToList()
                .ForEach(x => output.Add(x.Split(new string[] { "\\" }, StringSplitOptions.None).Last()));
            return output.ToArray();
        }

        public List<Article> getAllArticlesByCategory(string category) // too much memory consuming...
        {
            string[] categories = getAllCategories();
            List<Article> output = new List<Article>();
            foreach (var subcategory in categories)
            {
                string[] files = Directory.GetFiles(appPath + "\\" + category);
                foreach (var file in files)
                {
                    string file_name = file.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
                    string file_content = File.ReadAllText(appPath + "\\" + category + "\\" + file_name);
                    Article article = JsonConvert.DeserializeObject<Article>(file_content);
                    output.Add(article);
                }
            }
            return output;
        }

        public List<string> getAllArticlesTitlesByCategory(string category)
        {
            string[] categories = getAllCategories();
            List<string> output = new List<string>();
            foreach (var subcategory in categories)
            {
                string[] files;
                try
                {
                    files = Directory.GetFiles(appPath + "\\" + category);
                }
                catch (DirectoryNotFoundException)
                {
                    Directory.CreateDirectory(appPath + "\\" + category);
                    continue;
                }
                foreach (var file in files)
                {
                    string file_name = file.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
                    string file_content = File.ReadAllText(appPath + "\\" + category + "\\" + file_name);
                    Article article = JsonConvert.DeserializeObject<Article>(file_content);
                    output.Add(article.Title);
                }
            }
            return output;
        }

        public string getArticleContentByTitleAndCategory(string title, string category)
        {
            string[] files = Directory.GetFiles(appPath + "\\" + category);
            foreach (var file in files)
            {
                string file_content = File.ReadAllText(file);
                Article article = JsonConvert.DeserializeObject<Article>(file_content);
                if (article.Title == title) return article.Content;
                //if (file_content.Contains(title)) return file_content;
            }
            throw new Exception("Wybrany artykuł nie istnieje!");
        }

        public Dictionary<string, DateTime> getAllCategoriesLatestTime()
        {
            Dictionary<string, DateTime> output = new Dictionary<string, DateTime>();
            string[] categories = getAllCategories();
            DateTime maxDt;
            foreach (var category in categories)
            {
                maxDt = new DateTime(2019, 1, 1);
                string[] files = Directory.GetFiles(appPath + "\\" + category);
                foreach (var file in files)
                {
                    string file_name = file.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
                    DateTime fileDt = File.GetCreationTime(appPath + "\\" + category + "\\" + file_name);
                    if (fileDt > maxDt)
                    {
                        maxDt = fileDt;
                    }
                }
                output.Add(category, maxDt);
            }
            return output;
        }

        public string moveFileToTemp(string title, string content) // returns file path
        {
            MD5 md5 = MD5.Create();
            byte[] b_title = Encoding.UTF8.GetBytes(title);
            byte[] hash = md5.ComputeHash(b_title);

            StringBuilder builder = new StringBuilder();
            foreach (var b_byte in hash)
            {
                builder.Append(b_byte.ToString("X2"));
            }

            string final_file = Path.GetTempPath() + "\\" + builder.ToString() + ".html";
            File.WriteAllText(final_file, content);
            return final_file;
        }

        //public List<Article> sortArticles(List<Article> articles, SortMethod sortMethod)
        //{
        //    if (sortMethod == SortMethod.DateAscending) return articles.OrderBy(x => DateTime.ParseExact(x.PubDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
        //    return articles.OrderByDescending(x => DateTime.ParseExact(x.PubDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
        //    //return articles.Sort((x, y) => DateTime.ParseExact(x.PubDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture).CompareTo(DateTime.ParseExact(y.PubDate, "ddd, dd MMM yy HH:mm:ss", CultureInfo.InvariantCulture));
        //}
    }
}
