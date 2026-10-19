using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Management_API.Entities
{
    [Table("tasks")]
    public class TaskItem
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("IsCompleted")]
        public bool? IsCompleted { get; set; }

        [Column("description")]
        public string? Description { get; set; }
    }
}
