namespace Mentora.Persistence;

using Microsoft.EntityFrameworkCore;
using Mentora.Domain.Entities;
using Mentora.Domain.Enums;
using System.Security.Cryptography;
using Mentora.Persistence.Seeding;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Domain> Domains { get; set; }
    public DbSet<SubDomain> SubDomains { get; set; }
    public DbSet<Technology> Technologies { get; set; }
    public DbSet<CareerGoal> CareerGoals { get; set; }
    public DbSet<LearningStyle> LearningStyles { get; set; }
    public DbSet<MenteeProfile> MenteeProfiles { get; set; }
    public DbSet<MentorProfile> MentorProfiles { get; set; }
    public DbSet<MenteeInterest> MenteeInterests { get; set; }
    public DbSet<MentorExpertise> MentorExpertises { get; set; }
    public DbSet<MenteeSubDomain> MenteeSubDomains { get; set; }
    public DbSet<MentorSubDomain> MentorSubDomains { get; set; } 
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).HasColumnName("last_name").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Role).HasColumnName("role").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired(false);
            entity.Property(e => e.LastLogin).HasColumnName("last_login");
            entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
        });
        // MenteeProfile Configuration
        modelBuilder.Entity<MenteeProfile>(entity =>
        {
            entity.ToTable("mentee_profile");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DomainId).HasColumnName("domain_id");
            entity.Property(e => e.CurrentLevel).HasColumnName("current_level");
            entity.Property(e => e.EducationStatus).HasColumnName("education_status");
            entity.Property(e => e.CareerGoalId).HasColumnName("career_goal_id");
            entity.Property(e => e.LearningStyleId).HasColumnName("learning_style_id");
            entity.Property(e => e.CountryCode).HasColumnName("country_code").HasMaxLength(2);
            entity.Property(e => e.ProfilePictureUrl).HasColumnName("profile_picture_url");
            entity.Property(e => e.Bio).HasColumnName("bio").HasMaxLength(1000);
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified");

            entity.HasOne(e => e.User)
                .WithOne(u => u.MenteeProfile)
                .HasForeignKey<MenteeProfile>(e => e.UserId);

            entity.HasOne(e => e.Domain)
                .WithMany(d => d.MenteeProfiles)
                .HasForeignKey(e => e.DomainId);

            entity.HasOne(e => e.CareerGoal)
                .WithMany(c => c.MenteeProfiles)
                .HasForeignKey(e => e.CareerGoalId);

            entity.HasOne(e => e.LearningStyle)
                .WithMany(l => l.MenteeProfiles)
                .HasForeignKey(e => e.LearningStyleId);

            entity.HasOne(e => e.Country)
                .WithMany(c => c.MenteeProfiles)
                .HasForeignKey(e => e.CountryCode);
        });

        // MentorProfile Configuration
        modelBuilder.Entity<MentorProfile>(entity =>
        {
            entity.ToTable("mentor_profile");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DomainId).HasColumnName("domain_id");
            entity.Property(e => e.YearsOfExperience).HasColumnName("years_of_experience");
            entity.Property(e => e.Bio).HasColumnName("bio").HasMaxLength(2000);
            entity.Property(e => e.LinkedInUrl).HasColumnName("linkedin_url");
            entity.Property(e => e.ProfilePictureUrl).HasColumnName("profile_picture_url");
            entity.Property(e => e.PastExperience).HasColumnName("past_experience");
            entity.Property(e => e.IsVerified).HasColumnName("is_verified");
            entity.Property(e => e.AverageRating).HasColumnName("average_rating").HasColumnType("decimal(3,2)");
            entity.Property(e => e.TotalReviews).HasColumnName("total_reviews");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CountryCode).HasColumnName("country_code").HasMaxLength(2);
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified");

            entity.HasOne(e => e.User)
                .WithOne(u => u.MentorProfile)
                .HasForeignKey<MentorProfile>(e => e.UserId);

            entity.HasOne(e => e.Domain)
                .WithMany(d => d.MentorProfiles)
                .HasForeignKey(e => e.DomainId);

            entity.HasOne(e => e.Country)
                .WithMany(c => c.MentorProfiles)
                .HasForeignKey(e => e.CountryCode);
        });

        // Country Configuration
        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("countries");
            entity.HasKey(e => e.CountryCode);
            entity.Property(e => e.CountryCode).HasColumnName("country_code").HasMaxLength(2);
            entity.Property(e => e.CountryName).HasColumnName("country_name").IsRequired().HasMaxLength(100);
        });

        // CareerGoal Configuration
        modelBuilder.Entity<CareerGoal>(entity =>
        {
            entity.ToTable("career_goal");
            entity.HasKey(e => e.CareerGoalId);
            entity.Property(e => e.CareerGoalId).HasColumnName("career_goal_id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
        });

        // LearningStyle Configuration
        modelBuilder.Entity<LearningStyle>(entity =>
        {
            entity.ToTable("learning_style");
            entity.HasKey(e => e.LearningStyleId);
            entity.Property(e => e.LearningStyleId).HasColumnName("learning_style_id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description");
        });

        // Domain Configuration
        modelBuilder.Entity<Domain>(entity =>
        {
            entity.ToTable("domains");
            entity.HasKey(e => e.DomainId);
            entity.Property(e => e.DomainId).HasColumnName("domain_id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description");
        });

        // SubDomain Configuration
        modelBuilder.Entity<SubDomain>(entity =>
        {
            entity.ToTable("subdomain");
            entity.HasKey(e => e.SubDomainId);
            entity.Property(e => e.SubDomainId).HasColumnName("subdomain_id");
            entity.Property(e => e.DomainId).HasColumnName("domain_id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Domain)
                .WithMany(d => d.SubDomains)
                .HasForeignKey(e => e.DomainId);
        });

        // Technology Configuration
        modelBuilder.Entity<Technology>(entity =>
        {
            entity.ToTable("technologies");
            entity.HasKey(e => e.TechnologyId);
            entity.Property(e => e.TechnologyId).HasColumnName("technology_id");
            entity.Property(e => e.SubDomainId).HasColumnName("subdomain_id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.SubDomain)
                .WithMany(s => s.Technologies)
                .HasForeignKey(e => e.SubDomainId);
        });

        // MenteeInterest Configuration
        // MenteeInterest
        modelBuilder.Entity<MenteeInterest>(entity =>
        {
            entity.ToTable("mentee_interests");
            entity.HasKey(e => new { e.UserId, e.TechnologyId });

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TechnologyId).HasColumnName("technology_id");
            entity.Property(e => e.ExperienceLevel).HasColumnName("experience_level");

            // Cascade delete when removing MenteeProfile
            entity.HasOne(e => e.MenteeProfile)
                  .WithMany(m => m.MenteeInterests)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // NO cascade when deleting Technology
            entity.HasOne(e => e.Technology)
                  .WithMany(t => t.MenteeInterests)
                  .HasForeignKey(e => e.TechnologyId)
                  .OnDelete(DeleteBehavior.Restrict);  
        });

        // MentorExpertise – same pattern
        modelBuilder.Entity<MentorExpertise>(entity =>
        {
            entity.ToTable("mentor_expertise");
            entity.HasKey(e => new { e.MentorId, e.TechnologyId });

            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.TechnologyId).HasColumnName("technology_id");

            entity.HasOne(e => e.MentorProfile)
                  .WithMany(m => m.MentorExpertises)
                  .HasForeignKey(e => e.MentorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Technology)
                  .WithMany(t => t.MentorExpertises)
                  .HasForeignKey(e => e.TechnologyId)
                  .OnDelete(DeleteBehavior.Restrict); 
        });

        // EmailVerificationToken Configuration
        modelBuilder.Entity<EmailVerificationToken>(entity =>
        {
            entity.ToTable("email_verification_tokens");
            entity.HasKey(e => e.TokenId);
            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Token).HasColumnName("token").IsRequired();
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UsedAt).HasColumnName("used_at");

            entity.HasOne(e => e.User)
                .WithMany(u => u.EmailVerificationTokens)
                .HasForeignKey(e => e.UserId);
        });


        //// Seed Admin User// Fixed admin user (non-deterministic values removed)
        //var fixedAdminGuid = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"); // ← CHANGE THIS to your own fixed GUID
        //var fixedDate = new DateTime(2025, 6, 1, 10, 0, 0, DateTimeKind.Utc);

        //modelBuilder.Entity<User>().HasData(
        //    new User
        //    {
        //        UserId = fixedAdminGuid,
        //        Email = "admin@mentora.com",
        //        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
        //        FirstName = "System",
        //        LastName = "Administrator",
        //        Role = UserRole.Admin,
        //        CreatedAt = fixedDate,
        //        UpdatedAt = fixedDate,
        //        IsActive = true
        //    }
        //);


        // MenteeSubDomain Configuration (NEW)
        modelBuilder.Entity<MenteeSubDomain>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SubDomainId });

            entity.HasOne(e => e.MenteeProfile)
                .WithMany(m => m.MenteeSubDomains)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SubDomain)
                .WithMany()
                .HasForeignKey(e => e.SubDomainId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MentorSubDomain Configuration (NEW)
        modelBuilder.Entity<MentorSubDomain>(entity =>
        {
            entity.HasKey(e => new { e.MentorId, e.SubDomainId });

            entity.HasOne(e => e.MentorProfile)
                .WithMany(m => m.MentorSubDomains)
                .HasForeignKey(e => e.MentorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SubDomain)
                .WithMany()
                .HasForeignKey(e => e.SubDomainId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.SeedData();
    }


}