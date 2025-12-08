namespace RestFullAPI.DTOs
{
    public class PermissionDto
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Module { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreatePermissionDto
    {
        public string PermissionName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Module { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdatePermissionDto
    {
        public string? PermissionName { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Module { get; set; }
        public bool? IsActive { get; set; }
    }
}


