using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<VideoStream> VideoStreams => Set<VideoStream>();

    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Choice> Choices => Set<Choice>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();

    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();

    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<DiscussionThread> DiscussionThreads => Set<DiscussionThread>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AnalyticsEvent> AnalyticsEvents => Set<AnalyticsEvent>();
    public DbSet<UserRelationship> UserRelationships => Set<UserRelationship>();

    public DbSet<CourseReview> CourseReviews => Set<CourseReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map PostgreSQL enum types from schema.sql
        modelBuilder.HasPostgresEnum("user_role", new[] { "student", "instructor", "admin" });
        modelBuilder.HasPostgresEnum("user_status", new[] { "active", "suspended", "pending" });
        modelBuilder.HasPostgresEnum("course_level", new[] { "beginner", "intermediate", "advanced" });
        modelBuilder.HasPostgresEnum("enrollment_status", new[] { "active", "completed", "cancelled" });
        modelBuilder.HasPostgresEnum("material_kind", new[] { "video", "pdf", "slide", "html", "audio", "other" });
        modelBuilder.HasPostgresEnum("question_type", new[] { "mcq", "multi_select", "short_text", "long_text", "numeric" });
        modelBuilder.HasPostgresEnum("quiz_attempt_status", new[] { "in_progress", "submitted", "graded", "abandoned" });
        modelBuilder.HasPostgresEnum("submission_status", new[] { "submitted", "graded", "late", "resubmitted" });
        modelBuilder.HasPostgresEnum("certificate_status", new[] { "issued", "revoked" });
        modelBuilder.HasPostgresEnum("notification_type", new[] { "system", "course", "assignment", "message" });
        modelBuilder.HasPostgresEnum("payment_status", new[] { "pending", "completed", "failed", "refunded" });
        modelBuilder.HasPostgresEnum("relationship_type", new[] { "follower", "following", "blocked", "colleague" });

        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Role).HasColumnName("role").HasColumnType("user_role").IsRequired();
            b.Property(x => x.Name).HasColumnName("name").HasMaxLength(255);
            b.Property(x => x.Email).HasColumnName("email").HasMaxLength(255);
            b.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
            b.Property(x => x.Status).HasColumnName("status").HasColumnType("user_status").IsRequired();

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.CreatedBy).HasColumnName("created_by");
            b.Property(x => x.UpdatedBy).HasColumnName("updated_by");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasIndex(x => x.Email).IsUnique();
            b.HasIndex(x => x.Email).HasDatabaseName("idx_users_email");
        });

        modelBuilder.Entity<Course>(b =>
        {
            b.ToTable("courses");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.InstructorId).HasColumnName("instructor_id").IsRequired();

            b.Property(x => x.Title).HasColumnName("title").HasMaxLength(255);
            b.Property(x => x.Description).HasColumnName("description");
            b.Property(x => x.Language).HasColumnName("language").HasMaxLength(50);
            b.Property(x => x.Level).HasColumnName("level").HasColumnType("course_level");
            b.Property(x => x.IsPublished).HasColumnName("is_published");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.CreatedBy).HasColumnName("created_by");
            b.Property(x => x.UpdatedBy).HasColumnName("updated_by");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Instructor)
                .WithMany(u => u.CoursesTaught)
                .HasForeignKey(x => x.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.InstructorId).HasDatabaseName("idx_courses_instructor");
        });

        modelBuilder.Entity<Enrollment>(b =>
        {
            b.ToTable("enrollments", tb => tb.HasCheckConstraint("enrollments_progress_check", "progress >= 0 AND progress <= 100"));
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            b.Property(x => x.CourseId).HasColumnName("course_id").IsRequired();

            b.Property(x => x.Status).HasColumnName("status").HasColumnType("enrollment_status").IsRequired();
            b.Property(x => x.EnrolledOn).HasColumnName("enrolled_on");
            b.Property(x => x.CompletedOn).HasColumnName("completed_on");
            b.Property(x => x.Progress).HasColumnName("progress").HasPrecision(5, 2);

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique(); // UNIQUE(user_id, course_id)
            b.HasIndex(x => x.CourseId);
        });

        modelBuilder.Entity<Module>(b =>
        {
            b.ToTable("modules");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.CourseId).HasColumnName("course_id").IsRequired();
            b.Property(x => x.Title).HasColumnName("title").HasMaxLength(255);
            b.Property(x => x.Position).HasColumnName("position");
            b.Property(x => x.Description).HasColumnName("description");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.CreatedBy).HasColumnName("created_by");
            b.Property(x => x.UpdatedBy).HasColumnName("updated_by");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.CourseId, x.Position }).IsUnique();
            b.HasIndex(x => new { x.CourseId, x.Position }).HasDatabaseName("idx_modules_course_position");
        });

        modelBuilder.Entity<Material>(b =>
        {
            b.ToTable("materials");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.ModuleId).HasColumnName("module_id").IsRequired();
            b.Property(x => x.Kind).HasColumnName("kind").HasColumnType("material_kind").IsRequired();
            b.Property(x => x.Title).HasColumnName("title").HasMaxLength(255);
            b.Property(x => x.Description).HasColumnName("description");
            b.Property(x => x.StorageUrl).HasColumnName("storage_url").HasMaxLength(1024);
            b.Property(x => x.MimeType).HasColumnName("mime_type").HasMaxLength(255);
            b.Property(x => x.SizeBytes).HasColumnName("size_bytes");
            b.Property(x => x.RequiresAuth).HasColumnName("requires_auth");
            b.Property(x => x.Position).HasColumnName("position");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.CreatedBy).HasColumnName("created_by");
            b.Property(x => x.UpdatedBy).HasColumnName("updated_by");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Module)
                .WithMany(m => m.Materials)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.ModuleId, x.Position }).HasDatabaseName("idx_materials_module_position");
        });

        modelBuilder.Entity<VideoStream>(b =>
        {
            b.ToTable("video_streams");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.MaterialId).HasColumnName("material_id").IsRequired();
            b.Property(x => x.Provider).HasColumnName("provider").HasMaxLength(100);
            b.Property(x => x.StreamUrl).HasColumnName("stream_url").HasMaxLength(1024);
            b.Property(x => x.PlaybackPolicy).HasColumnName("playback_policy").HasMaxLength(50);
            b.Property(x => x.DurationSeconds).HasColumnName("duration_seconds");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Material)
                .WithOne(m => m.VideoStream)
                .HasForeignKey<VideoStream>(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Quiz>(b =>
        {
            b.ToTable("quizzes");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.ModuleId).HasColumnName("module_id").IsRequired();
            b.Property(x => x.Title).HasColumnName("title").HasMaxLength(255);
            b.Property(x => x.TimeLimitSeconds).HasColumnName("time_limit_seconds");
            b.Property(x => x.AttemptsAllowed).HasColumnName("attempts_allowed");
            b.Property(x => x.ShuffleQuestions).HasColumnName("shuffle_questions");
            b.Property(x => x.PassScore).HasColumnName("pass_score").HasPrecision(5, 2);

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.CreatedBy).HasColumnName("created_by");
            b.Property(x => x.UpdatedBy).HasColumnName("updated_by");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Module)
                .WithMany(m => m.Quizzes)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Question>(b =>
        {
            b.ToTable("questions");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.QuizId).HasColumnName("quiz_id").IsRequired();
            b.Property(x => x.Type).HasColumnName("type").HasColumnType("question_type").IsRequired();
            b.Property(x => x.Prompt).HasColumnName("prompt");
            b.Property(x => x.Points).HasColumnName("points").HasPrecision(8, 2);
            b.Property(x => x.Position).HasColumnName("position");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Choice>(b =>
        {
            b.ToTable("choices");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.QuestionId).HasColumnName("question_id").IsRequired();
            b.Property(x => x.Text).HasColumnName("text");
            b.Property(x => x.IsCorrect).HasColumnName("is_correct");
            b.Property(x => x.Position).HasColumnName("position");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuizAttempt>(b =>
        {
            b.ToTable("quiz_attempts");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.QuizId).HasColumnName("quiz_id").IsRequired();
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            b.Property(x => x.StartedAt).HasColumnName("started_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.SubmittedAt).HasColumnName("submitted_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.Score).HasColumnName("score").HasPrecision(8, 2);
            b.Property(x => x.Status).HasColumnName("status").HasColumnType("quiz_attempt_status");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.User)
                .WithMany(u => u.QuizAttempts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.QuizId, x.UserId }).HasDatabaseName("idx_quiz_attempts_quiz_user");
        });

        modelBuilder.Entity<AttemptAnswer>(b =>
        {
            b.ToTable("attempt_answers", tb => tb.HasCheckConstraint("chk_answer_presence", "(answer_text IS NOT NULL) OR (selected_choice_id IS NOT NULL)"));
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.AttemptId).HasColumnName("attempt_id").IsRequired();
            b.Property(x => x.QuestionId).HasColumnName("question_id").IsRequired();
            b.Property(x => x.AnswerText).HasColumnName("answer_text");
            b.Property(x => x.SelectedChoiceId).HasColumnName("selected_choice_id");
            b.Property(x => x.IsCorrect).HasColumnName("is_correct");
            b.Property(x => x.AwardedPoints).HasColumnName("awarded_points").HasPrecision(8, 2);

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Attempt)
                .WithMany(a => a.Answers)
                .HasForeignKey(x => x.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Question)
                .WithMany(q => q.AttemptAnswers)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.SelectedChoice)
                .WithMany()
                .HasForeignKey(x => x.SelectedChoiceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Assignment>(b =>
        {
            b.ToTable("assignments");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.ModuleId).HasColumnName("module_id").IsRequired();
            b.Property(x => x.Title).HasColumnName("title").HasMaxLength(255);
            b.Property(x => x.Instructions).HasColumnName("instructions");
            b.Property(x => x.DueAt).HasColumnName("due_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.MaxPoints).HasColumnName("max_points").HasPrecision(8, 2);

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.CreatedBy).HasColumnName("created_by");
            b.Property(x => x.UpdatedBy).HasColumnName("updated_by");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Module)
                .WithMany(m => m.Assignments)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Submission>(b =>
        {
            b.ToTable("submissions");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.AssignmentId).HasColumnName("assignment_id").IsRequired();
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            b.Property(x => x.SubmittedAt).HasColumnName("submitted_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.StorageUrl).HasColumnName("storage_url").HasMaxLength(1024);
            b.Property(x => x.Grade).HasColumnName("grade").HasPrecision(8, 2);
            b.Property(x => x.Feedback).HasColumnName("feedback");
            b.Property(x => x.Status).HasColumnName("status").HasColumnType("submission_status");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.User)
                .WithMany(u => u.Submissions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // schema.sql has this constraint commented out; keep it disabled here too.
        });

        modelBuilder.Entity<Certificate>(b =>
        {
            b.ToTable("certificates");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            b.Property(x => x.CourseId).HasColumnName("course_id").IsRequired();
            b.Property(x => x.Serial).HasColumnName("serial").HasMaxLength(255);
            b.Property(x => x.IssuedAt).HasColumnName("issued_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.Status).HasColumnName("status").HasColumnType("certificate_status");
            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.User)
                .WithMany(u => u.Certificates)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Course)
                .WithMany(c => c.Certificates)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.Serial).IsUnique();
            b.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique().HasDatabaseName("uq_certificates_user_course");
        });

        modelBuilder.Entity<DiscussionThread>(b =>
        {
            b.ToTable("discussion_threads");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.CourseId).HasColumnName("course_id").IsRequired();
            b.Property(x => x.ModuleId).HasColumnName("module_id");
            b.Property(x => x.Title).HasColumnName("title").HasMaxLength(255);
            b.Property(x => x.IsLocked).HasColumnName("is_locked");
            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.CreatedBy).HasColumnName("created_by");

            b.HasOne(x => x.Course)
                .WithMany(c => c.DiscussionThreads)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Module)
                .WithMany(m => m.DiscussionThreads)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Post>(b =>
        {
            b.ToTable("posts");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.ThreadId).HasColumnName("thread_id").IsRequired();
            b.Property(x => x.AuthorId).HasColumnName("author_id").IsRequired();
            b.Property(x => x.Body).HasColumnName("body");

            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Thread)
                .WithMany(t => t.Posts)
                .HasForeignKey(x => x.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.ThreadId, x.CreatedAt }).HasDatabaseName("idx_posts_thread_created_at");
        });

        modelBuilder.Entity<Notification>(b =>
        {
            b.ToTable("notifications");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            b.Property(x => x.Type).HasColumnName("type").HasColumnType("notification_type").IsRequired();
            b.Property(x => x.Message).HasColumnName("message");
            b.Property(x => x.IsRead).HasColumnName("is_read");
            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Payment>(b =>
        {
            b.ToTable("payments");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            b.Property(x => x.CourseId).HasColumnName("course_id");
            b.Property(x => x.Amount).HasColumnName("amount").HasPrecision(12, 2);
            b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(10);
            b.Property(x => x.Status).HasColumnName("status").HasColumnType("payment_status");
            b.Property(x => x.PaymentDate).HasColumnName("payment_date").HasColumnType("timestamp with time zone");
            b.Property(x => x.Provider).HasColumnName("provider").HasMaxLength(255);
            b.Property(x => x.TransactionId).HasColumnName("transaction_id").HasMaxLength(255);
            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Course)
                .WithMany(c => c.Payments)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AnalyticsEvent>(b =>
        {
            b.ToTable("analytics_events");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.UserId).HasColumnName("user_id");
            b.Property(x => x.EventType).HasColumnName("event_type").HasMaxLength(255);
            b.Property(x => x.EventData).HasColumnName("event_data").HasColumnType("jsonb");
            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");

            b.HasOne(x => x.User)
                .WithMany(u => u.AnalyticsEvents)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(x => new { x.EventType, x.CreatedAt }).HasDatabaseName("idx_analytics_event_type_created_at");
        });

        modelBuilder.Entity<UserRelationship>(b =>
        {
            b.ToTable("user_relationships");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            b.Property(x => x.RelatedUserId).HasColumnName("related_user_id").IsRequired();
            b.Property(x => x.RelationshipType).HasColumnName("relationship_type").HasColumnType("relationship_type").IsRequired();
            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");

            b.HasOne(x => x.User)
                .WithMany(u => u.Relationships)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.RelatedUser)
                .WithMany(u => u.RelatedRelationships)
                .HasForeignKey(x => x.RelatedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.UserId, x.RelatedUserId }).IsUnique();
        });

        modelBuilder.Entity<CourseReview>(b =>
        {
            b.ToTable("course_reviews", tb => tb.HasCheckConstraint("course_reviews_rating_check", "rating >= 1 AND rating <= 5"));
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.CourseId).HasColumnName("course_id").IsRequired();
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            b.Property(x => x.Rating).HasColumnName("rating");
            b.Property(x => x.ReviewText).HasColumnName("review_text");
            b.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            b.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");

            b.HasOne(x => x.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.User)
                .WithMany(u => u.CourseReviews)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique().HasDatabaseName("uq_course_reviews_user_course");
            b.HasIndex(x => new { x.CourseId, x.CreatedAt }).HasDatabaseName("idx_course_reviews_course_created_at");
        });
    }
}