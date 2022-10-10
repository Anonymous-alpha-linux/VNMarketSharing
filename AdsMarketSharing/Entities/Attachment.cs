using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string PublicPath { get; set; }
        public string FileType { get; set; }
        public string FileTag { get; set; }
        public float FileSize { get; set; }
        public User User { get; set; }
    }
}
