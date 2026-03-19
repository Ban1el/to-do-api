using System;

namespace API.Models;

public class ToDo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public DateTime? DateModified { get; set; }
    public bool IsActive { get; set; }
}
