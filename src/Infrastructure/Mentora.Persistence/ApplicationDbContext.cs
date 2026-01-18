namespace Mentora.Persistence;

using Microsoft.EntityFrameworkCore;
using Mentora.Domain.Entities;
using Mentora.Domain.Enums;
using System.Security.Cryptography;

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
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }


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
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();
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
        modelBuilder.Entity<MenteeInterest>(entity =>
        {
            entity.ToTable("mentee_interests");
            entity.HasKey(e => new { e.UserId, e.TechnologyId });
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TechnologyId).HasColumnName("technology_id");
            entity.Property(e => e.ExperienceLevel).HasColumnName("experience_level");

            entity.HasOne(e => e.MenteeProfile)
                .WithMany(m => m.MenteeInterests)
                .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Technology)
                .WithMany(t => t.MenteeInterests)
                .HasForeignKey(e => e.TechnologyId);
        });

        // MentorExpertise Configuration
        modelBuilder.Entity<MentorExpertise>(entity =>
        {
            entity.ToTable("mentor_expertise");
            entity.HasKey(e => new { e.MentorId, e.TechnologyId });
            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.TechnologyId).HasColumnName("technology_id");

            entity.HasOne(e => e.MentorProfile)
                .WithMany(m => m.MentorExpertises)
                .HasForeignKey(e => e.MentorId);

            entity.HasOne(e => e.Technology)
                .WithMany(t => t.MentorExpertises)
                .HasForeignKey(e => e.TechnologyId);
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


        // Seed Admin User
        var adminUserId = Guid.NewGuid();
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = adminUserId,
                Email = "admin@mentora.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FirstName = "System",
                LastName = "Administrator",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );
    }



    /**
     * -------------------------------------------------------------------------
*/
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Countries (Arab Countries)
        modelBuilder.Entity<Country>().HasData(
            new Country { CountryCode = "EG", CountryName = "Egypt" },
            new Country { CountryCode = "SA", CountryName = "Saudi Arabia" },
            new Country { CountryCode = "AE", CountryName = "United Arab Emirates" },
            new Country { CountryCode = "KW", CountryName = "Kuwait" },
            new Country { CountryCode = "QA", CountryName = "Qatar" },
            new Country { CountryCode = "BH", CountryName = "Bahrain" },
            new Country { CountryCode = "OM", CountryName = "Oman" },
            new Country { CountryCode = "JO", CountryName = "Jordan" },
            new Country { CountryCode = "LB", CountryName = "Lebanon" },
            new Country { CountryCode = "IQ", CountryName = "Iraq" },
            new Country { CountryCode = "SY", CountryName = "Syria" },
            new Country { CountryCode = "PS", CountryName = "Palestine" },
            new Country { CountryCode = "YE", CountryName = "Yemen" },
            new Country { CountryCode = "SD", CountryName = "Sudan" },
            new Country { CountryCode = "DZ", CountryName = "Algeria" },
            new Country { CountryCode = "MA", CountryName = "Morocco" },
            new Country { CountryCode = "TN", CountryName = "Tunisia" },
            new Country { CountryCode = "LY", CountryName = "Libya" },
            new Country { CountryCode = "MR", CountryName = "Mauritania" },
            new Country { CountryCode = "DJ", CountryName = "Djibouti" },
            new Country { CountryCode = "SO", CountryName = "Somalia" },
            new Country { CountryCode = "KM", CountryName = "Comoros" }

        );

        // Seed Domain

        modelBuilder.Entity<Domain>().HasData(
            new Domain
            {
                DomainId = 1,
                Name = "Software Engineering",
                Description = "Building software systems and applications"
            },
            new Domain
            {
                DomainId = 2,
                Name = "AI & Data Science",
                Description = "Machine learning and data-driven systems"
            },
            new Domain
            {
                DomainId = 3,
                Name = "Design",
                Description = "User-centered and visual design disciplines"
            },
            new Domain
            {
                DomainId = 4,
                Name = "Product & Business",
                Description = "Product strategy, management, and business skills"
            }
        );

        // Seed Sub-Domain

        modelBuilder.Entity<SubDomain>().HasData(
            // Software Engineering (DomainId = 1)
            new SubDomain { SubDomainId = 1, DomainId = 1, Name = "Backend Development" },
            new SubDomain { SubDomainId = 2, DomainId = 1, Name = "Frontend Development" },
            new SubDomain { SubDomainId = 3, DomainId = 1, Name = "Full Stack Development" },
            new SubDomain { SubDomainId = 4, DomainId = 1, Name = "Mobile Development" },
            new SubDomain { SubDomainId = 5, DomainId = 1, Name = "DevOps & Cloud" },
            new SubDomain { SubDomainId = 6, DomainId = 1, Name = "System Design" },

            // AI & Data Science (DomainId = 2)
            new SubDomain { SubDomainId = 7, DomainId = 2, Name = "Machine Learning" },
            new SubDomain { SubDomainId = 8, DomainId = 2, Name = "Deep Learning" },
            new SubDomain { SubDomainId = 9, DomainId = 2, Name = "Computer Vision" },
            new SubDomain { SubDomainId = 10, DomainId = 2, Name = "Natural Language Processing" },
            new SubDomain { SubDomainId = 11, DomainId = 2, Name = "Data Analysis" },
            new SubDomain { SubDomainId = 12, DomainId = 2, Name = "Data Science" },
            new SubDomain { SubDomainId = 13, DomainId = 2, Name = "Data Engineering" },

            // Design (DomainId = 3)
            new SubDomain { SubDomainId = 14, DomainId = 3, Name = "UI/UX Design" },
            new SubDomain { SubDomainId = 15, DomainId = 3, Name = "Product Design" },
            new SubDomain { SubDomainId = 16, DomainId = 3, Name = "Graphic Design" },
            new SubDomain { SubDomainId = 17, DomainId = 3, Name = "Motion Design" },
            new SubDomain { SubDomainId = 18, DomainId = 3, Name = "Branding" },

            // Product & Business (DomainId = 4)
            new SubDomain { SubDomainId = 19, DomainId = 4, Name = "Product Management" },
            new SubDomain { SubDomainId = 20, DomainId = 4, Name = "Business Analysis" },
            new SubDomain { SubDomainId = 21, DomainId = 4, Name = "Project Management" },
            new SubDomain { SubDomainId = 22, DomainId = 4, Name = "Career Coaching" },
            new SubDomain { SubDomainId = 23, DomainId = 4, Name = "Entrepreneurship" }
        );

        // Seed learning Style
        modelBuilder.Entity<LearningStyle>().HasData(
            new LearningStyle
            {
                LearningStyleId = 1,
                Name = "Visual",
                Description = "Learns best through visuals, diagrams, and examples"
            },
            new LearningStyle
            {
                LearningStyleId = 2,
                Name = "Auditory",
                Description = "Learns best through listening, discussions, and explanations"
            },
            new LearningStyle
            {
                LearningStyleId = 3,
                Name = "Reading/Writing",
                Description = "Prefers text-based materials, notes, and documentation"
            },
            new LearningStyle
            {
                LearningStyleId = 4,
                Name = "Kinesthetic",
                Description = "Learns best through hands-on practice and experimentation"
            },
            new LearningStyle
            {
                LearningStyleId = 5,
                Name = "Project-Based",
                Description = "Learns best by building real projects"
            },
            new LearningStyle
            {
                LearningStyleId = 6,
                Name = "Guided Mentorship",
                Description = "Prefers structured guidance and regular mentor feedback"
            },
            new LearningStyle
            {
                LearningStyleId = 7,
                Name = "Self-Paced",
                Description = "Prefers independent learning at own pace"
            },
            new LearningStyle
            {
                LearningStyleId = 8,
                Name = "Collaborative",
                Description = "Learns best through group discussions and collaboration"
            }
            );


        // Sead Career goals
        modelBuilder.Entity<CareerGoal>().HasData(
            new CareerGoal
            {
                CareerGoalId = 1,
                Name = "Grow and Advance in My Current Field"
            },
            new CareerGoal
            {
                CareerGoalId = 2,
                Name = "Explore a New Career Path"
            },
            new CareerGoal
            {
                CareerGoalId = 3,
                Name = "Start My Own Business or Project"
            },
            new CareerGoal
            {
                CareerGoalId = 4,
                Name = "Get Guidance on My Career Journey"
            },
            new CareerGoal
            {
                CareerGoalId = 5,
                Name = "Prepare for Leadership or Management Roles"
            },
            new CareerGoal
            {
                CareerGoalId = 6,
                Name = "Something Else"
            }
        );


        // Seed Technologies
        modelBuilder.Entity<Technology>().HasData(
            // Backend Development (SubDomainId = 1)
            new Technology { TechnologyId = 1, SubDomainId = 1, Name = "Node.js" },
            new Technology { TechnologyId = 2, SubDomainId = 1, Name = ".NET / ASP.NET Core" },
            new Technology { TechnologyId = 3, SubDomainId = 1, Name = "Spring Boot" },
            new Technology { TechnologyId = 4, SubDomainId = 1, Name = "Django" },
            new Technology { TechnologyId = 5, SubDomainId = 1, Name = "Flask" },
            new Technology { TechnologyId = 6, SubDomainId = 1, Name = "Laravel" },
            new Technology { TechnologyId = 7, SubDomainId = 1, Name = "Express.js" },
            new Technology { TechnologyId = 8, SubDomainId = 1, Name = "FastAPI" },
            new Technology { TechnologyId = 9, SubDomainId = 1, Name = "NestJS" },

            // Frontend Development (SubDomainId = 2)
            new Technology { TechnologyId = 10, SubDomainId = 2, Name = "HTML" },
            new Technology { TechnologyId = 11, SubDomainId = 2, Name = "CSS" },
            new Technology { TechnologyId = 12, SubDomainId = 2, Name = "JavaScript" },
            new Technology { TechnologyId = 13, SubDomainId = 2, Name = "TypeScript" },
            new Technology { TechnologyId = 14, SubDomainId = 2, Name = "React" },
            new Technology { TechnologyId = 15, SubDomainId = 2, Name = "Angular" },
            new Technology { TechnologyId = 16, SubDomainId = 2, Name = "Vue.js" },
            new Technology { TechnologyId = 17, SubDomainId = 2, Name = "Next.js" },
            new Technology { TechnologyId = 18, SubDomainId = 2, Name = "Nuxt.js" },
            new Technology { TechnologyId = 19, SubDomainId = 2, Name = "SASS / SCSS" },
            new Technology { TechnologyId = 20, SubDomainId = 2, Name = "Tailwind CSS" },
            new Technology { TechnologyId = 21, SubDomainId = 2, Name = "Bootstrap" },

            // Full Stack Development (SubDomainId = 3)
            new Technology { TechnologyId = 22, SubDomainId = 3, Name = "MERN Stack" },
            new Technology { TechnologyId = 23, SubDomainId = 3, Name = "MEAN Stack" },
            new Technology { TechnologyId = 24, SubDomainId = 3, Name = "LAMP Stack" },
            new Technology { TechnologyId = 25, SubDomainId = 3, Name = "Django + React" },
            new Technology { TechnologyId = 26, SubDomainId = 3, Name = "Next.js Full Stack" },
            new Technology { TechnologyId = 27, SubDomainId = 3, Name = "REST APIs" },
            new Technology { TechnologyId = 28, SubDomainId = 3, Name = "GraphQL" },

            // Mobile Development (SubDomainId = 4)
            new Technology { TechnologyId = 29, SubDomainId = 4, Name = "Flutter" },
            new Technology { TechnologyId = 30, SubDomainId = 4, Name = "React Native" },
            new Technology { TechnologyId = 31, SubDomainId = 4, Name = "Android (Kotlin)" },
            new Technology { TechnologyId = 32, SubDomainId = 4, Name = "Android (Java)" },
            new Technology { TechnologyId = 33, SubDomainId = 4, Name = "iOS (Swift)" },
            new Technology { TechnologyId = 34, SubDomainId = 4, Name = "Xamarin" },
            new Technology { TechnologyId = 35, SubDomainId = 4, Name = "Ionic" },

            // DevOps & Cloud (SubDomainId = 5)
            new Technology { TechnologyId = 36, SubDomainId = 5, Name = "Docker" },
            new Technology { TechnologyId = 37, SubDomainId = 5, Name = "Kubernetes" },
            new Technology { TechnologyId = 38, SubDomainId = 5, Name = "AWS" },
            new Technology { TechnologyId = 39, SubDomainId = 5, Name = "Azure" },
            new Technology { TechnologyId = 40, SubDomainId = 5, Name = "Google Cloud Platform" },
            new Technology { TechnologyId = 41, SubDomainId = 5, Name = "CI/CD Pipelines" },
            new Technology { TechnologyId = 42, SubDomainId = 5, Name = "Jenkins" },
            new Technology { TechnologyId = 43, SubDomainId = 5, Name = "GitHub Actions" },
            new Technology { TechnologyId = 44, SubDomainId = 5, Name = "GitLab CI" },
            new Technology { TechnologyId = 45, SubDomainId = 5, Name = "Terraform" },
            new Technology { TechnologyId = 46, SubDomainId = 5, Name = "Ansible" },
            new Technology { TechnologyId = 47, SubDomainId = 5, Name = "Nginx" },

            // System Design (SubDomainId = 6)
            new Technology { TechnologyId = 48, SubDomainId = 6, Name = "High Level Design (HLD)" },
            new Technology { TechnologyId = 49, SubDomainId = 6, Name = "Low Level Design (LLD)" },
            new Technology { TechnologyId = 50, SubDomainId = 6, Name = "Microservices Architecture" },
            new Technology { TechnologyId = 51, SubDomainId = 6, Name = "REST Architecture" },
            new Technology { TechnologyId = 52, SubDomainId = 6, Name = "Event-Driven Architecture" },
            new Technology { TechnologyId = 53, SubDomainId = 6, Name = "Design Patterns" },
            new Technology { TechnologyId = 54, SubDomainId = 6, Name = "Scalability & Load Balancing" },
            new Technology { TechnologyId = 55, SubDomainId = 6, Name = "Caching (Redis, Memcached)" },
            new Technology { TechnologyId = 56, SubDomainId = 6, Name = "Message Queues (Kafka, RabbitMQ)" },

            // Machine Learning (SubDomainId = 7)
            new Technology { TechnologyId = 57, SubDomainId = 7, Name = "Python" },
            new Technology { TechnologyId = 58, SubDomainId = 7, Name = "Scikit-learn" },
            new Technology { TechnologyId = 59, SubDomainId = 7, Name = "TensorFlow" },
            new Technology { TechnologyId = 60, SubDomainId = 7, Name = "PyTorch" },
            new Technology { TechnologyId = 61, SubDomainId = 7, Name = "Keras" },
            new Technology { TechnologyId = 62, SubDomainId = 7, Name = "XGBoost" },
            new Technology { TechnologyId = 63, SubDomainId = 7, Name = "LightGBM" },
            new Technology { TechnologyId = 64, SubDomainId = 7, Name = "CatBoost" },

            // Deep Learning (SubDomainId = 8)
            new Technology { TechnologyId = 65, SubDomainId = 8, Name = "TensorFlow" },
            new Technology { TechnologyId = 66, SubDomainId = 8, Name = "PyTorch" },
            new Technology { TechnologyId = 67, SubDomainId = 8, Name = "Keras" },
            new Technology { TechnologyId = 68, SubDomainId = 8, Name = "CNNs" },
            new Technology { TechnologyId = 69, SubDomainId = 8, Name = "RNNs" },
            new Technology { TechnologyId = 70, SubDomainId = 8, Name = "Transformers" },

            // Computer Vision (SubDomainId = 9)
            new Technology { TechnologyId = 71, SubDomainId = 9, Name = "OpenCV" },
            new Technology { TechnologyId = 72, SubDomainId = 9, Name = "TensorFlow" },
            new Technology { TechnologyId = 73, SubDomainId = 9, Name = "PyTorch" },
            new Technology { TechnologyId = 74, SubDomainId = 9, Name = "YOLO" },
            new Technology { TechnologyId = 75, SubDomainId = 9, Name = "MediaPipe" },

            // NLP (SubDomainId = 10)
            new Technology { TechnologyId = 76, SubDomainId = 10, Name = "NLTK" },
            new Technology { TechnologyId = 77, SubDomainId = 10, Name = "SpaCy" },
            new Technology { TechnologyId = 78, SubDomainId = 10, Name = "Hugging Face Transformers" },
            new Technology { TechnologyId = 79, SubDomainId = 10, Name = "BERT" },
            new Technology { TechnologyId = 80, SubDomainId = 10, Name = "GPT Models" },

            // Data Analysis (SubDomainId = 11)
            new Technology { TechnologyId = 81, SubDomainId = 11, Name = "Python" },
            new Technology { TechnologyId = 82, SubDomainId = 11, Name = "Pandas" },
            new Technology { TechnologyId = 83, SubDomainId = 11, Name = "NumPy" },
            new Technology { TechnologyId = 84, SubDomainId = 11, Name = "SQL" },
            new Technology { TechnologyId = 85, SubDomainId = 11, Name = "Excel" },
            new Technology { TechnologyId = 86, SubDomainId = 11, Name = "Power BI" },
            new Technology { TechnologyId = 87, SubDomainId = 11, Name = "Tableau" },
            new Technology { TechnologyId = 88, SubDomainId = 11, Name = "Google Sheets" },

            // Data Science (SubDomainId = 12)
            new Technology { TechnologyId = 89, SubDomainId = 12, Name = "Python" },
            new Technology { TechnologyId = 90, SubDomainId = 12, Name = "R" },
            new Technology { TechnologyId = 91, SubDomainId = 12, Name = "Jupyter Notebook" },
            new Technology { TechnologyId = 92, SubDomainId = 12, Name = "Matplotlib" },
            new Technology { TechnologyId = 93, SubDomainId = 12, Name = "Seaborn" },
            new Technology { TechnologyId = 94, SubDomainId = 12, Name = "SQL" },
            new Technology { TechnologyId = 95, SubDomainId = 12, Name = "Machine Learning Libraries" },

            // Data Engineering (SubDomainId = 13)
            new Technology { TechnologyId = 96, SubDomainId = 13, Name = "Apache Spark" },
            new Technology { TechnologyId = 97, SubDomainId = 13, Name = "Apache Airflow" },
            new Technology { TechnologyId = 98, SubDomainId = 13, Name = "Hadoop" },
            new Technology { TechnologyId = 99, SubDomainId = 13, Name = "BigQuery" },
            new Technology { TechnologyId = 100, SubDomainId = 13, Name = "Snowflake" },
            new Technology { TechnologyId = 101, SubDomainId = 13, Name = "Redshift" },
            new Technology { TechnologyId = 102, SubDomainId = 13, Name = "Kafka" },
            new Technology { TechnologyId = 103, SubDomainId = 13, Name = "ETL Pipelines" },

            // UI/UX Design (SubDomainId = 14)
            new Technology { TechnologyId = 104, SubDomainId = 14, Name = "Figma" },
            new Technology { TechnologyId = 105, SubDomainId = 14, Name = "Adobe XD" },
            new Technology { TechnologyId = 106, SubDomainId = 14, Name = "Sketch" },
            new Technology { TechnologyId = 107, SubDomainId = 14, Name = "InVision" },
            new Technology { TechnologyId = 108, SubDomainId = 14, Name = "Zeplin" },
            new Technology { TechnologyId = 109, SubDomainId = 14, Name = "FigJam" },

            // Product Design (SubDomainId = 15)
            new Technology { TechnologyId = 110, SubDomainId = 15, Name = "Figma" },
            new Technology { TechnologyId = 111, SubDomainId = 15, Name = "Adobe XD" },
            new Technology { TechnologyId = 112, SubDomainId = 15, Name = "Sketch" },
            new Technology { TechnologyId = 113, SubDomainId = 15, Name = "User Journey Mapping" },
            new Technology { TechnologyId = 114, SubDomainId = 15, Name = "Wireframing" },
            new Technology { TechnologyId = 115, SubDomainId = 15, Name = "Prototyping" },

            // Graphic Design (SubDomainId = 16)
            new Technology { TechnologyId = 116, SubDomainId = 16, Name = "Photoshop" },
            new Technology { TechnologyId = 117, SubDomainId = 16, Name = "Illustrator" },
            new Technology { TechnologyId = 118, SubDomainId = 16, Name = "InDesign" },
            new Technology { TechnologyId = 119, SubDomainId = 16, Name = "Canva" },

            // Motion Design (SubDomainId = 17)
            new Technology { TechnologyId = 120, SubDomainId = 17, Name = "After Effects" },
            new Technology { TechnologyId = 121, SubDomainId = 17, Name = "Premiere Pro" },
            new Technology { TechnologyId = 122, SubDomainId = 17, Name = "Blender" },
            new Technology { TechnologyId = 123, SubDomainId = 17, Name = "Cinema 4D" },

            // Branding (SubDomainId = 18)
            new Technology { TechnologyId = 124, SubDomainId = 18, Name = "Illustrator" },
            new Technology { TechnologyId = 125, SubDomainId = 18, Name = "Photoshop" },
            new Technology { TechnologyId = 126, SubDomainId = 18, Name = "Brand Identity Systems" },
            new Technology { TechnologyId = 127, SubDomainId = 18, Name = "Logo Design Tools" },

            // Product Management (SubDomainId = 19)
            new Technology { TechnologyId = 128, SubDomainId = 19, Name = "Jira" },
            new Technology { TechnologyId = 129, SubDomainId = 19, Name = "Confluence" },
            new Technology { TechnologyId = 130, SubDomainId = 19, Name = "Notion" },
            new Technology { TechnologyId = 131, SubDomainId = 19, Name = "Trello" },
            new Technology { TechnologyId = 132, SubDomainId = 19, Name = "Miro" },
            new Technology { TechnologyId = 133, SubDomainId = 19, Name = "Product Roadmaps" },
            new Technology { TechnologyId = 134, SubDomainId = 19, Name = "OKRs" },

            // Business Analysis (SubDomainId = 20)
            new Technology { TechnologyId = 135, SubDomainId = 20, Name = "Excel" },
            new Technology { TechnologyId = 136, SubDomainId = 20, Name = "Power BI" },
            new Technology { TechnologyId = 137, SubDomainId = 20, Name = "Tableau" },
            new Technology { TechnologyId = 138, SubDomainId = 20, Name = "SQL" },
            new Technology { TechnologyId = 139, SubDomainId = 20, Name = "BPMN" },
            new Technology { TechnologyId = 140, SubDomainId = 20, Name = "SWOT Analysis" },

            // Project Management (SubDomainId = 21)
            new Technology { TechnologyId = 141, SubDomainId = 21, Name = "Jira" },
            new Technology { TechnologyId = 142, SubDomainId = 21, Name = "Trello" },
            new Technology { TechnologyId = 143, SubDomainId = 21, Name = "Asana" },
            new Technology { TechnologyId = 144, SubDomainId = 21, Name = "Monday.com" },
            new Technology { TechnologyId = 145, SubDomainId = 21, Name = "ClickUp" },

            // Career Coaching (SubDomainId = 22)
            new Technology { TechnologyId = 146, SubDomainId = 22, Name = "Resume Review Tools" },
            new Technology { TechnologyId = 147, SubDomainId = 22, Name = "Interview Preparation Frameworks" },
            new Technology { TechnologyId = 148, SubDomainId = 22, Name = "LinkedIn Optimization" },
            new Technology { TechnologyId = 149, SubDomainId = 22, Name = "Career Planning Models" },

            // Entrepreneurship (SubDomainId = 23)
            new Technology { TechnologyId = 150, SubDomainId = 23, Name = "Lean Canvas" },
            new Technology { TechnologyId = 151, SubDomainId = 23, Name = "Business Model Canvas" },
            new Technology { TechnologyId = 152, SubDomainId = 23, Name = "Pitch Deck Tools" },
            new Technology { TechnologyId = 153, SubDomainId = 23, Name = "Market Research Tools" },
            new Technology { TechnologyId = 154, SubDomainId = 23, Name = "Financial Modeling" }
        );

    }
}