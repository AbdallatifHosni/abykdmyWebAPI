
using AbyKhedma.Entities;
using Core.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Xml;


namespace AbyKhedma.Persistance
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<RequestFlow> RequestFlows { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Requirement> Requirements { get; set; }
        public virtual DbSet<Statement> Statements { get; set; }
        public virtual DbSet<Reel> Reels { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<Attachment> Files { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<SubAnswerRequirement> SubAnswerRequirements { get; set; }
        public virtual DbSet<StatusLookup> StatusLookup { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }
        public virtual DbSet<RequestAudit> RequestAudits { get; set; }
        public virtual DbSet<Message>  Messages { get; set; }
   

        /*Stats*/
        public virtual DbSet<RequestStatusStats> RequestStatusStats { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// Configure the one-to-many relationship between User and Request for CustomerId
            //modelBuilder.Entity<User>().ToTable("User");
            //modelBuilder.Entity<Request>().ToTable("Requests");
            modelBuilder.Entity<RequestStatusStats>(e => e.HasNoKey());

            modelBuilder.Entity<Request>()
             .HasOne(s => s.Requester)
             .WithMany()
             .HasForeignKey(e => e.RequesterId);

            modelBuilder.Entity<Request>()
          .HasOne(s => s.Employee)
          .WithMany()
          .HasForeignKey(e => e.AssignedEmployeeId);


            modelBuilder.Entity<SubAnswerRequirement>()
           .HasOne(s => s.Answer)
           .WithOne(s => s.SubAnswerRequirement);
            //.HasForeignKey<Answer>(e => e.Id)
            //.IsRequired();

            modelBuilder.Entity<Message>()
            .HasOne(e => e.ChatMessage)
            .WithMany(e => e.Messages)
            .HasForeignKey(e => e.ChatMessageId)
            .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<User>()
            //    .HasIndex(e => e.SearchableFullName)
            //      .IsClustered(false);
                 
            

        }

    }
}
