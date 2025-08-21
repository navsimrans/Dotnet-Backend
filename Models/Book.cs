namespace MyApi.Models;

using System.ComponentModel.DataAnnotations;


public class Book
{
    public int Id { get; set; }     // Primary Key

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title can't exceed 100 characters")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Author is required")]
    [StringLength(50, ErrorMessage = "Author can't exceed 50 characters")]
    public string Author { get; set; }
}

