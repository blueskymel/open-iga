using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Models;

namespace OpenIga.Api.Data;

public class OpenIgaDbContext(DbContextOptions<OpenIgaDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<AccessRequest> AccessRequests => Set<AccessRequest>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Id).HasColumnName("id");
            entity.Property(user => user.Email).HasColumnName("email");
            entity.Property(user => user.Name).HasColumnName("name");
            entity.Property(user => user.Status)
                .HasColumnName("status")
                .HasConversion<string>();
            entity.Property(user => user.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(role => role.Id);
            entity.Property(role => role.Id).HasColumnName("id");
            entity.Property(role => role.Name).HasColumnName("name");
            entity.Property(role => role.Description).HasColumnName("description");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(permission => permission.Id);
            entity.Property(permission => permission.Id).HasColumnName("id");
            entity.Property(permission => permission.Name).HasColumnName("name");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("role_permissions");
            entity.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });
            entity.Property(rolePermission => rolePermission.RoleId).HasColumnName("role_id");
            entity.Property(rolePermission => rolePermission.PermissionId).HasColumnName("permission_id");
            entity.HasOne(rolePermission => rolePermission.Role)
                .WithMany(role => role.RolePermissions)
                .HasForeignKey(rolePermission => rolePermission.RoleId);
            entity.HasOne(rolePermission => rolePermission.Permission)
                .WithMany(permission => permission.RolePermissions)
                .HasForeignKey(rolePermission => rolePermission.PermissionId);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(userRole => new { userRole.UserId, userRole.RoleId });
            entity.Property(userRole => userRole.UserId).HasColumnName("user_id");
            entity.Property(userRole => userRole.RoleId).HasColumnName("role_id");
            entity.HasOne(userRole => userRole.User)
                .WithMany(user => user.UserRoles)
                .HasForeignKey(userRole => userRole.UserId);
            entity.HasOne(userRole => userRole.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(userRole => userRole.RoleId);
        });

        modelBuilder.Entity<AccessRequest>(entity =>
        {
            entity.ToTable("access_requests");
            entity.HasKey(request => request.Id);
            entity.Property(request => request.Id).HasColumnName("id");
            entity.Property(request => request.UserId).HasColumnName("user_id");
            entity.Property(request => request.RoleId).HasColumnName("role_id");
            entity.Property(request => request.Status)
                .HasColumnName("status")
                .HasConversion<string>();
            entity.Property(request => request.RequestedAt)
                .HasColumnName("requested_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(request => request.ApprovedBy).HasColumnName("approved_by");
            entity.Property(request => request.ApprovedAt).HasColumnName("approved_at");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(log => log.Id);
            entity.Property(log => log.Id).HasColumnName("id");
            entity.Property(log => log.Action)
                .HasColumnName("action")
                .HasConversion<string>();
            entity.Property(log => log.PerformedBy).HasColumnName("performed_by");
            entity.Property(log => log.TargetUser).HasColumnName("target_user");
            entity.Property(log => log.Timestamp)
                .HasColumnName("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
