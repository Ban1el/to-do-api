using System;

namespace API.Models;

public class BaseEntity
{
    public DateTime DateCreated { get; set; }
    public DateTime? DateModified { get; set; }
    public int CreatedBy { get; set; }
    public int ModifiedBy { get; set; }
    public bool IsActive { get; set; }
}
