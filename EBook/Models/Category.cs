using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EBook.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        [DisplayName("Display order")]
        public int DisplayOrder { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

    }
}
