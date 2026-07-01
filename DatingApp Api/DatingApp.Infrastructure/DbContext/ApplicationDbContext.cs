using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<MemberLike>()
                .HasKey(x => new { x.SourceMemberID, x.TargetMemberID });

            modelBuilder.Entity<MemberLike>()
                .HasOne(s => s.SourceMember)
                .WithMany(t => t.LikedMembers)
                .HasForeignKey(x => x.SourceMemberID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberLike>()
              .HasOne(s => s.TargetMember)
              .WithMany(t => t.LikedByMembers)
              .HasForeignKey(x => x.TargetMemberID)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
              .HasOne(m => m.Sender)
              .WithMany(m => m.MessagesSent)
              .HasForeignKey(m => m.SenderId)
              .OnDelete(DeleteBehavior.Restrict); // أو NoAction

            modelBuilder.Entity<Message>()
              .HasOne(m => m.Recipient)
              .WithMany(m => m.MessagesRecieved)
              .HasForeignKey(m => m.RecipientId)
              .OnDelete(DeleteBehavior.Restrict);


        }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }

        public virtual DbSet<MemberLike> Likes { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Connection> Connections { get; set; }
    }
}
