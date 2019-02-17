using NPoco;

namespace NPoco_Duplicate_Naming_issue.Models
{

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
