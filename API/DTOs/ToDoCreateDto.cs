using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ToDoCreateDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
