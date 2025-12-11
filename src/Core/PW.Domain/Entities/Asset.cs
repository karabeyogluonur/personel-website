

using PW.Domain.Common;

namespace PW.Domain.Entities
{
    public class Asset : BaseEntity
    {
        public string FileName { get; set; }
        public string Folder { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public long SizeBytes { get; set; }
        public string AltText { get; set; }
    }
}
