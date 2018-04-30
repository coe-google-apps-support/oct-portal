using System.ComponentModel.DataAnnotations.Schema;

namespace CoE.Ideas.Shared.WordPress
{
    [Table("wp_usermeta")]
    internal class UserMetadata
    {
        public UserMetadata()
        { }

        public UserMetadata(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [Column("umeta_id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("meta_key")]
        public string Key { get; set; }

        [Column("meta_value")]
        public string Value { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
