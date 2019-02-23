#cd C:\Users\Marcin\Documents\Visual Studio 2015\Projects\NewsAggregator\NewsAggregator
import pymongo
import urllib.request
import xml.etree.ElementTree as ET
import lzma
from datetime import datetime, timedelta

# Flag to turn on/off compressing
isCompressing = False

# MongoDB connection and selecting collections
client = pymongo.MongoClient("mongodb://localhost:27017/")
db = client["aggregator"]
articles = db["articles"]
sources = db["sources"]

# Mapping categories last visit time from MongoDB to Python dictionary
categoriesTimes = dict()
metadata = sources.find_one({"metadata": True})
for x in metadata:
    if x == "_id" or x == "metadata":
        continue
    categoriesTimes[x] = datetime.strptime(metadata[x], "%a, %d %b %y %H:%M:%S %z")

# Iterating over all sources
latestPubDate = None
for x in sources.find():
    try:
        feed = urllib.request.urlopen(x["url"]).read()
    except KeyError:
        continue
    tree = ET.fromstring(feed)
    latestPubDate = None

    lastBuildDateFromChannel = tree.find("channel/lastBuildDate")
    lastBuildDateFromDB = x["lastBuildDate"]
    # build dates are equal - no changes on rss channel
    if lastBuildDateFromDB == lastBuildDateFromChannel:
        continue

    for item in tree.findall("channel/item"):
        title = item.find("title")
        url = item.find("link")
        pubDate = item.find("pubDate")

        # get data about article
        title = title.text
        url = url.text
        strPubDate = pubDate.text

        # in case any node is empty or doesn't exist omit article
        if url is None or title is None or pubDate is None:
            continue

        dtPubDate = datetime.strptime(strPubDate, "%a, %d %b %y %H:%M:%S %z")

        category = x["category"]
        # if category == "Biznes":
        #     print(dtPubDate)
        #     print(categoriesTimes[category])
        #     print("\n")
        source = x["name"]
        # if article is new than download it and insert to MongoDB
        # print(str(dtPubDate) + "vs" + str(categoriesTimes[category]))

        if dtPubDate > categoriesTimes[category]:
            content = str(urllib.request.urlopen(url).read())
            if isCompressing:
                compressed = str(lzma.compress(str.encode(content)))

            if latestPubDate is None:
                latestPubDate = dtPubDate
            elif dtPubDate > latestPubDate:
                latestPubDate = dtPubDate
            try:
                articles.insert_one({"title": title, "content": compressed if isCompressing else content, "category": category, "source": source, "pubDate": strPubDate})
                print("Dodane: " + title)
            except Exception:
                print("Duplikat!!! " + title)
            strLatestPubDate = latestPubDate.strftime("%a, %d %b %y %H:%M:%S %z")
            sources.update_one({"metadata": True}, {"$set": {category: strLatestPubDate}})

print("OK")