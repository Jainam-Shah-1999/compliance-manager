using System.ComponentModel;

namespace KpaFinAdvisors.Common.Models
{
    public class Blog
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [DisplayName("Blog description")]
        public string Description { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        
        public string UpdatedBy { get; set; } = String.Empty;

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Updated { get; set; } = DateTime.Now;
    }
}
