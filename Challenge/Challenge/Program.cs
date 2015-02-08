using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Challenge
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to chalenge.");
            Console.WriteLine("If do you want upload a new contact list please write (1) and press enter.");
            Console.WriteLine("If do you want search contacts from database please write (2) and press enter.");
            Console.WriteLine("If do you want close program please write (exit) and press enter.");

            string command = string.Empty;

            while (!command.Equals("exit"))
            {

                Console.WriteLine("----------------------------");
                Console.WriteLine("Please write feature command.");

                command = Console.ReadLine();

                switch (command)
                {
                    case "1": UploadContactXml();
                        break;
                    case "2": SearchContactsFromDb();
                        break;
                    case "exit": break;
                    default: Console.WriteLine("{0} is not recognized as an command!!!!", command);
                        break;
                }
            }
        }



        private static void UploadContactXml()
        {
            while (true)
            {
                Console.WriteLine("Please Write Contact Xml File Path...");

                string path = Console.ReadLine();

                if (File.Exists(path))
                {
                    List<XmlContact> xmlContacts = GetXmlContacts(path);
                    IEnumerable<ContactEntity> contacts = MergeContacts(xmlContacts);

                    InsertOrUpdateToDb(contacts);

                    Console.WriteLine("{0} contact is uploaded to database successfully...{1}", contacts.Count(), Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("File Is Not Exists!!!!{0}", Environment.NewLine);
                    continue;
                }
                break;
            }
        }

        private static void InsertOrUpdateToDb(IEnumerable<ContactEntity> contacts)
        {
            MongoCollection<ContactEntity> dbContacts = Db.ContactEntityCollection;

            //Stopwatch mainSW = new Stopwatch();
            //Stopwatch selectSW = new Stopwatch();
            //Stopwatch updateSW = new Stopwatch();
            //Stopwatch insertSW = new Stopwatch();

            //mainSW.Start();
            foreach (ContactEntity item in contacts)
            {
                //selectSW.Start();

                //ToDo: Find a way for bulk select!!
                ContactEntity dbContact = dbContacts.AsQueryable().FirstOrDefault(p => p.Name.Equals(item.Name) && p.LastName.Equals(item.LastName));

                //selectSW.Stop();
                if (dbContact == null)
                {
                    dbContact = item;

                    //insertSW.Start();
                    //dbContacts.Save(dbContact);
                    //insertSW.Stop();
                }
                else
                {
                    string[] currentPhones = item.Phones.ToArray();
                    string[] newPhones = currentPhones.Except(dbContact.Phones.ToArray()).ToArray();

                    if (!newPhones.Any())
                    {
                        continue;
                    }

                    dbContact.Phones = dbContact.Phones.ToArray().Union(newPhones);

                    //updateSW.Start();
                    //dbContacts.Save(dbContact);
                    //updateSW.Stop();
                }

                dbContacts.Save(dbContact);
            }
            //mainSW.Stop();

            //long mainElapsed = mainSW.ElapsedMilliseconds;
            //long selectElapsed = selectSW.ElapsedMilliseconds;
            //long updateElapsed = updateSW.ElapsedMilliseconds;
            //long insertElapsed = insertSW.ElapsedMilliseconds;
            //long otherElapsed = mainElapsed - selectElapsed - updateElapsed - insertElapsed;
        }


        private static IEnumerable<ContactEntity> MergeContacts(IEnumerable<XmlContact> xmlContacts)
        {
            IEnumerable<ContactEntity> contacts = xmlContacts.GroupBy(p => new { p.Name, p.LastName }, r => r.Phone)
                .Select(p =>
                    new ContactEntity
                    {
                        Name = p.Key.Name,
                        LastName = p.Key.LastName,
                        Phones = new List<string>(p.Distinct().ToArray())
                    })
                .ToArray();

            return contacts;
        }

        private static List<XmlContact> GetXmlContacts(string path)
        {

            XmlRootAttribute root = new XmlRootAttribute { ElementName = "contacts" };
            XmlSerializer serializer = new XmlSerializer(typeof(List<XmlContact>), root);

            StreamReader reader = new StreamReader(path);
            List<XmlContact> contacts = (List<XmlContact>)serializer.Deserialize(reader);

            return contacts;
        }

        private static void SearchContactsFromDb()
        {
            Console.WriteLine("Please write name...");
            string name = Console.ReadLine();

            var query = Query<ContactEntity>.Where(p => p.Name.ToLower().StartsWith(name.ToLower()));
            MongoCursor<ContactEntity> contactEntities = Db.ContactEntityCollection.Find(query);
            ContactEntity[] entities = contactEntities.ToArray();

            foreach (ContactEntity item in entities)
            {
                Console.WriteLine("{0} {1} : {2}", item.Name, item.LastName, string.Join(" | ", item.Phones));
            }
        }
    }
}
