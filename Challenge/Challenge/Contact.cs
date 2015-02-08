using System.Collections.Generic;
using MongoDB.Bson;

namespace Challenge
{
    public class ContactEntity
    {
        public BsonObjectId Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> Phones { get; set; }
    }
}