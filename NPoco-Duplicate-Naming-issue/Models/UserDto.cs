using NPoco;
using System.Collections.Generic;

namespace NPoco_Duplicate_Naming_issue.Models
{
    [TableName("User")]
    [PrimaryKey("UserId", AutoIncrement = true)]
    public class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [NPoco.Reference(NPoco.ReferenceType.Many, ColumnName = "UserId", ReferenceMemberName = "UserId")]
        //[Column]
        public List<Models.ContactDto> Contacts { get; set; }
        [VersionColumn("Timestamp", VersionColumnType.Number)]
        public int TimeStamp { get; set; }
    }
}

