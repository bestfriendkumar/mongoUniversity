using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mdbUniversity_Week2
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
            Console.WriteLine("\nPress Enter");
            Console.ReadLine();
        }

        static async Task MainAsync(string[] args)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("school");

            var col = db.GetCollection<BsonDocument>("students");

            var aggregate = col.Aggregate().Match(new BsonDocument {{"scores.type", "homework"}})
                .Unwind("scores")
                .Match(new BsonDocument { { "scores.type", "homework" } })
                .Sort(Builders<BsonDocument>.Sort.Ascending("_id").Ascending("scores.score"));

            var list = await aggregate.ToListAsync();

            //delete every other document
            var i = true;
            var update = Builders<BsonDocument>.Update.PullFilter("scores",
                Builders<BsonDocument>.Filter("score"));
            foreach (var doc in list)
            {
                if (i)
                {
                    col.UpdateOne()
                    i = false;
                }
                else
                {
                    Console.WriteLine(doc.ToString());
                    i = true;
                }
            }

            //Loop through all the students
            //look at the scores array foreach student
            //delete the lowest homework in the scores array
            /*
             db.students.aggregate(
  // Start with a $match pipeline which can take advantage of an index and limit documents processed
  { $match : {
     "scores.type": "homework"
  }},
  { $unwind : "$scores" },
  { $match : {
     "scores.type": "homework"
  }},
  { $sort : {
      "_id" : 1,
      "scores.score" : 1
  }}
)
             * */


            /* WEEK 2
            
            var client = new MongoClient();
            var db = client.GetDatabase("students");

            var col = db.GetCollection<BsonDocument>("grades");

            var filter = new BsonDocument("type", "homework");
            var list = await col.Find(filter)
                .Sort(Builders<BsonDocument>.Sort.Ascending("student_id").Ascending("score"))
                .ToListAsync();

            var oldId = list[0]["student_id"]; //set to id of first doc in the list
            col.DeleteOne(list[0]); //delete first element of list
            foreach (var doc in list)
            {
                var currId = doc["student_id"]; //whatever id is in the doc we're looking at
                if (currId != oldId)
                {
                    col.DeleteOne(doc);
                    oldId = currId;
                }

            }
            Console.WriteLine("List Count : " + list.Count);
            Console.WriteLine("DB Count : " + col.Find(new BsonDocument()).Count());
            //list.Sort();
             
            var client = new MongoClient();
            var db = client.GetDatabase("movies");

            var col = db.GetCollection<BsonDocument>("movieDetails");

            //var filter = new BsonDocument("countries", "Sweden");

            var list = await col.Find(new BsonDocument())
                .ToListAsync();

            var count = 0;
            foreach (var doc in list)
            {
                if (doc["countries"][1] == "Sweden")
                {
                    count ++;
                    Console.WriteLine(doc);
                }
            }
            Console.WriteLine("List Count : " + list.Count);
            Console.WriteLine("Count : " + count);
             * */

        }
    }
}
