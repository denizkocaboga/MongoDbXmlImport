using MongoDB.Driver;

namespace Challenge
{
    public static class Db
    {

        private static MongoDatabase _db;
        public static MongoDatabase CurrentDb
        {
            get
            {
                if (_db != null) return _db;

                const string connectionString = "mongodb://localhost";
                MongoClient client = new MongoClient(connectionString);

                MongoServer server = client.GetServer();

                _db = server.GetDatabase("Challenge");

                return _db;
            }
        }

        public static MongoCollection<ContactEntity> ContactEntityCollection
        {
            get
            {
                return CurrentDb.GetCollection<ContactEntity>("ContactEntities");
            }
        }
    }
}