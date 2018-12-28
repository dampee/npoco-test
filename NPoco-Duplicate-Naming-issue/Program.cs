﻿using NPoco;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NPoco_Duplicate_Naming_issue
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IDatabase db = new Database("SqLiteDb");

            InitializeDb.CreateTables(db.ConnectionString);

            db.Execute("delete  from [user] where userid > 1");
            db.Execute("delete  from contact");

            var userEd = new UserDto() { Name = "Ed", Email = "ed@test.test" };
            db.Insert(userEd);
            userEd.Email = "ed@newprovider.com";
            db.Update(userEd); // all good user Ed is now on timestamp 2

            var mariaUserId = db.Insert(new UserDto()
            {
                Name = "Maria",
                Email = "maria@test.test"
            });

            List<UserDto> users = db.Fetch<UserDto>();
            if (users.Count != 3) { throw new InvalidOperationException("we should have 3 users by now"); }
            
            db.Insert(new ContactDto() { UserId = Convert.ToInt32(mariaUserId), Description = "test at " + DateTime.Now.ToString()});
            db.Insert(new ContactDto() { UserId = Convert.ToInt32(mariaUserId), Description = "test at " + DateTime.Now.ToString()});

            var contacts = db.Fetch<ContactDto>("WHERE Userid = @0", mariaUserId);
            if (contacts.Count != 2) { throw new InvalidOperationException("we should have 2 contacts by now"); }

            var sql = "select a.*, b.* from [User] a inner join Contact b on a.UserId = b.UserId ";
            var userWithContacts = db.FetchOneToMany<UserDto>(a => a.Contacts, sql);

            foreach (var userWithContact in userWithContacts)
            {
                Console.WriteLine("User : " + userWithContact.Name);
                foreach (var contactDto in userWithContact.Contacts)
                {
                    Console.WriteLine("Contact : " + contactDto.Description);
                }
            }
            
            if (Debugger.IsAttached) { Debugger.Break(); } else { Console.ReadKey(); }
        }

        [TableName("User")]
        [PrimaryKey("UserId", AutoIncrement = true)]
        public class UserDto
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            [NPoco.Reference(NPoco.ReferenceType.Many, ColumnName = "UserId", ReferenceMemberName = "UserId")]
            //[Column]
            public List<ContactDto> Contacts { get; set; }
            [VersionColumn("Timestamp", VersionColumnType.Number)]
            public int TimeStamp { get; set; }
        }

        [TableName("Contact")]
        [PrimaryKey("ContactId", AutoIncrement = true)]
        public class ContactDto
        {
            public int ContactId { get; set; }
            
            [Column("UserId")]
            public int UserId { get; set; }
            public string Description { get; set; }
            [VersionColumn("Timestamp", VersionColumnType.Number)]
            public int TimeStamp { get; set; }
        }
    }
}
