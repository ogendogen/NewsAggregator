using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggregator
{
    public static class Synchronizer
    {
        public static void synchronize(DB db, FileManager fileManager)
        {
            Dictionary<string, DateTime> filesLatestTimes = fileManager.getAllCategoriesLatestTime();
            foreach (var file in filesLatestTimes)
            {
                List<Article> articles = db.getAllArticlesSinceDateByCategory(file.Value, file.Key);
                fileManager.addNewArticles(articles);
            }
        }
    }
}
