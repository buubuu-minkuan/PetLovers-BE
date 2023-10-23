using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Data.Entities
{
    public partial class PetLoversDbContext : DbContext
    {
        public PetLoversDbContext()
        {
        }

        public PetLoversDbContext(DbContextOptions<PetLoversDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblNotification> TblNotifications { get; set; } = null!;
        public virtual DbSet<TblOtpverify> TblOtpverifies { get; set; } = null!;
        public virtual DbSet<TblPetTradingPost> TblPetTradingPosts { get; set; } = null!;
        public virtual DbSet<TblPost> TblPosts { get; set; } = null!;
        public virtual DbSet<TblPostAttachment> TblPostAttachments { get; set; } = null!;
        public virtual DbSet<TblPostHashtag> TblPostHashtags { get; set; } = null!;
        public virtual DbSet<TblPostReaction> TblPostReactions { get; set; } = null!;
        public virtual DbSet<TblPostStored> TblPostStoreds { get; set; } = null!;
        public virtual DbSet<TblReport> TblReports { get; set; } = null!;
        public virtual DbSet<TblReward> TblRewards { get; set; } = null!;
        public virtual DbSet<TblRole> TblRoles { get; set; } = null!;
        public virtual DbSet<TblTradeRequest> TblTradeRequests { get; set; } = null!;
        public virtual DbSet<TblUser> TblUsers { get; set; } = null!;
        public virtual DbSet<TblUserFollowing> TblUserFollowings { get; set; } = null!;
        public virtual DbSet<TblUserReward> TblUserRewards { get; set; } = null!;
        public virtual DbSet<TblUserTimeout> TblUserTimeouts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblNotification>(entity =>
            {
                entity.ToTable("tblNotification");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Content)
                    .HasMaxLength(100)
                    .HasColumnName("content");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.TradeId).HasColumnName("tradeId");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updateAt");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblNotifications)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK__tblNotifi__postI__02084FDA");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblNotifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblNotifi__userI__0C85DE4D");
            });

            modelBuilder.Entity<TblOtpverify>(entity =>
            {
                entity.ToTable("tblOTPVerify");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.ExpiredAt)
                    .HasColumnType("datetime")
                    .HasColumnName("expiredAt");

                entity.Property(e => e.OtpCode)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("otpCode");
            });

            modelBuilder.Entity<TblPetTradingPost>(entity =>
            {
                entity.ToTable("tblPetTradingPost");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Age)
                    .HasMaxLength(100)
                    .HasColumnName("age");

                entity.Property(e => e.Breed)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("breed");

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("gender");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.Type)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.Weight)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("weight");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblPetTradingPosts)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblPetTra__postI__634EBE90");
            });

            modelBuilder.Entity<TblPost>(entity =>
            {
                entity.ToTable("tblPost");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(200)
                    .HasColumnName("address");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createAt");

                entity.Property(e => e.IsFree).HasColumnName("isFree");

                entity.Property(e => e.IsProcessed).HasColumnName("isProcessed");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updateAt");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblPosts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblPost__userId__03F0984C");
            });

            modelBuilder.Entity<TblPostAttachment>(entity =>
            {
                entity.ToTable("tblPostAttachment");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Attachment)
                    .IsUnicode(false)
                    .HasColumnName("attachment");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblPostAttachments)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblPostAt__postI__4F47C5E3");
            });

            modelBuilder.Entity<TblPostHashtag>(entity =>
            {
                entity.ToTable("tblPostHashtag");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Hashtag)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("hashtag");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblPostHashtags)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblPostHa__postI__607251E5");
            });

            modelBuilder.Entity<TblPostReaction>(entity =>
            {
                entity.ToTable("tblPostReaction");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Attachment)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("attachment");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createAt");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.TypeReact)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("typeReact");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updateAt");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblPostReactions)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblPostRe__postI__02FC7413");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblPostReactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblPostRe__userI__0A9D95DB");
            });

            modelBuilder.Entity<TblPostStored>(entity =>
            {
                entity.ToTable("tblPostStored");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createAt");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblPostStoreds)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblPostSt__postI__06CD04F7");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblPostStoreds)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblPostSt__userI__04E4BC85");
            });

            modelBuilder.Entity<TblReport>(entity =>
            {
                entity.ToTable("tblReport");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CommentId).HasColumnName("commentId");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createAt");

                entity.Property(e => e.IsProcessed).HasColumnName("isProcessed");

                entity.Property(e => e.ModeratorId).HasColumnName("moderatorId");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.Reason)
                    .HasMaxLength(200)
                    .HasColumnName("reason");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .HasColumnName("type");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updatedAt");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.TblReports)
                    .HasForeignKey(d => d.CommentId)
                    .HasConstraintName("FK__tblReport__comme__7F2BE32F");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblReports)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK__tblReport__postI__7E37BEF6");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblReports)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblReport__userI__7D439ABD");
            });

            modelBuilder.Entity<TblReward>(entity =>
            {
                entity.ToTable("tblReward");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.TotalComment).HasColumnName("totalComment");

                entity.Property(e => e.TotalPost).HasColumnName("totalPost");
            });

            modelBuilder.Entity<TblRole>(entity =>
            {
                entity.ToTable("tblRole");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<TblTradeRequest>(entity =>
            {
                entity.ToTable("tblTradeRequest");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createAt");

                entity.Property(e => e.IsProcessed).HasColumnName("isProcessed");

                entity.Property(e => e.PostId).HasColumnName("postId");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updateAt");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblTradeRequests)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblTradeR__postI__0E6E26BF");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblTradeRequests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblTradeR__userI__00200768");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.ToTable("tblUser");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createAt");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Image)
                    .IsUnicode(false)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updateAt");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TblUsers)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblUser__roleId__05D8E0BE");
            });

            modelBuilder.Entity<TblUserFollowing>(entity =>
            {
                entity.ToTable("tblUserFollowing");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.FollowerId).HasColumnName("followerId");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.TblUserFollowingFollowers)
                    .HasForeignKey(d => d.FollowerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblUserFo__follo__07C12930");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserFollowingUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblUserFo__userI__09A971A2");
            });

            modelBuilder.Entity<TblUserReward>(entity =>
            {
                entity.ToTable("tblUserReward");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateAt).HasColumnName("createAt");

                entity.Property(e => e.RewardId).HasColumnName("rewardId");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Reward)
                    .WithMany(p => p.TblUserRewards)
                    .HasForeignKey(d => d.RewardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblUserRe__rewar__08B54D69");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserRewards)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblUserRe__userI__0D7A0286");
            });

            modelBuilder.Entity<TblUserTimeout>(entity =>
            {
                entity.ToTable("tblUserTimeout");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createAt");

                entity.Property(e => e.ExpiredAt)
                    .HasColumnType("datetime")
                    .HasColumnName("expiredAt");

                entity.Property(e => e.ModeratorId).HasColumnName("moderatorId");

                entity.Property(e => e.Reason).HasColumnName("reason");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Moderator)
                    .WithMany(p => p.TblUserTimeoutModerators)
                    .HasForeignKey(d => d.ModeratorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblUserTi__moder__01142BA1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserTimeoutUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblUserTi__userI__0B91BA14");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
