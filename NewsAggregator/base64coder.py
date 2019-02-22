import lzma
import pymongo

client = pymongo.MongoClient("mongodb://localhost:27017/")
db = client["aggregator"]
articles = db["articles"]
found = articles.find_one()["content"]
#compressed = lzmaCompressString(found)
for x in articles.find():
    compressed = str(lzma.compress(str.encode(x["content"])))
    articles.update_one({"_id": x["_id"]}, {"$set": {"content": compressed}})

print("OK")
# compressed = lzma.compress(str.encode(found))
# print("Uncompressed: " + str(len(str(found))))
# print("Compressed: " + str(len(compressed)))