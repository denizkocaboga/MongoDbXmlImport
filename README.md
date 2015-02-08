There are two features:
1) Bulk import
2) Get contact info by name

For bulk import you are given a xml file which has names, lastnames and phone numbers of various people.
The list is contain more than one phone number for each person and there can be duplicate records.
You are asked to find and merge those people and save the resulting contacts list into a mongodb collection.
There can be other xml files to import so checking for existing contact info on mongo.

For retrieving contact info you is search only in names and get all the matching contacts.

The structure for the mongo collection is like this,

{
	id : objectid,
	name : "Ahmet",
	lastName : "Mehmet",
	phones : ["+90 555 222 33 44", "+90 555 666 77 88"]
}

