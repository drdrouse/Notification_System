using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Notification_System.EFM;

public partial class NotificationSystemContext : DbContext
{
    public NotificationSystemContext()
    {
    }

    public NotificationSystemContext(DbContextOptions<NotificationSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountStatus> AccountStatuses { get; set; }

    public virtual DbSet<AutoActionJob> AutoActionJobs { get; set; }

    public virtual DbSet<EventCode> EventCodes { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Mail> Mails { get; set; }

    public virtual DbSet<OrganizationUnit> OrganizationUnits { get; set; }

    public virtual DbSet<Password> Passwords { get; set; }

    public virtual DbSet<Phone> Phones { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Subdivision> Subdivisions { get; set; }

    public virtual DbSet<TypePhone> TypePhones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=Notification_System;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.AccountId)
                .ValueGeneratedNever()
                .HasColumnName("Account_Id");
            entity.Property(e => e.AccountLogin).HasColumnName("Account_Login");
            entity.Property(e => e.AccountStatusId).HasColumnName("AccountStatus_Id");
            entity.Property(e => e.ProfileId).HasColumnName("Profile_Id");

            entity.HasOne(d => d.AccountStatus).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.AccountStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Account_Status");

            entity.HasOne(d => d.Profile).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Profile");
        });

        modelBuilder.Entity<AccountStatus>(entity =>
        {
            entity.ToTable("AccountStatus");

            entity.Property(e => e.AccountStatusId)
                .ValueGeneratedNever()
                .HasColumnName("AccountStatus_Id");
            entity.Property(e => e.AccountStatusName).HasColumnName("AccountStatus_Name");
        });

        modelBuilder.Entity<AutoActionJob>(entity =>
        {
            entity.ToTable("AutoActionJob");

            entity.Property(e => e.AutoActionJobId)
                .ValueGeneratedNever()
                .HasColumnName("AutoActionJob_Id");
            entity.Property(e => e.AccountId).HasColumnName("Account_Id");
            entity.Property(e => e.AccountStatusId).HasColumnName("AccountStatus_Id");
            entity.Property(e => e.AutoActionJobStartDate)
                .HasColumnType("datetime")
                .HasColumnName("AutoActionJob_StartDate");

            entity.HasOne(d => d.Account).WithMany(p => p.AutoActionJobs)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_AutoActionJob_Account");

            entity.HasOne(d => d.AccountStatus).WithMany(p => p.AutoActionJobs)
                .HasForeignKey(d => d.AccountStatusId)
                .HasConstraintName("FK_AutoActionJob_AccountStatus");
        });

        modelBuilder.Entity<EventCode>(entity =>
        {
            entity.ToTable("EventCode");

            entity.Property(e => e.EventCodeId)
                .ValueGeneratedNever()
                .HasColumnName("EventCode_Id");
            entity.Property(e => e.EventCodeDescription).HasColumnName("EventCode_Description");
            entity.Property(e => e.EventCodeName).HasColumnName("EventCode_Name");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");

            entity.Property(e => e.LogId)
                .ValueGeneratedNever()
                .HasColumnName("Log_Id");
            entity.Property(e => e.EventCodeId).HasColumnName("EventCode_Id");
            entity.Property(e => e.LogDateTime)
                .HasColumnType("datetime")
                .HasColumnName("Log_DateTime");
            entity.Property(e => e.LogDeccription)
                .IsUnicode(false)
                .HasColumnName("Log_Deccription");
            entity.Property(e => e.ProfileId).HasColumnName("Profile_Id");

            entity.HasOne(d => d.EventCode).WithMany(p => p.Logs)
                .HasForeignKey(d => d.EventCodeId)
                .HasConstraintName("FK_Log_EventCode");

            entity.HasOne(d => d.Profile).WithMany(p => p.Logs)
                .HasForeignKey(d => d.ProfileId)
                .HasConstraintName("FK_Log_Profile");
        });

        modelBuilder.Entity<Mail>(entity =>
        {
            entity.HasKey(e => e.MailsId);

            entity.Property(e => e.MailsId)
                .ValueGeneratedNever()
                .HasColumnName("Mails_Id");
            entity.Property(e => e.MailsName)
                .IsUnicode(false)
                .HasColumnName("Mails_Name");
            entity.Property(e => e.ProfileId).HasColumnName("Profile_Id");

            entity.HasOne(d => d.Profile).WithMany(p => p.Mail)
                .HasForeignKey(d => d.ProfileId)
                .HasConstraintName("FK_Mails_Profile");
        });

        modelBuilder.Entity<OrganizationUnit>(entity =>
        {
            entity.ToTable("OrganizationUnit");

            entity.Property(e => e.OrganizationUnitId)
                .ValueGeneratedNever()
                .HasColumnName("OrganizationUnit_Id");
            entity.Property(e => e.PostId).HasColumnName("Post_Id");
            entity.Property(e => e.ProfileId).HasColumnName("Profile_Id");
            entity.Property(e => e.SubdivisionId).HasColumnName("Subdivision_Id");

            entity.HasOne(d => d.Post).WithMany(p => p.OrganizationUnits)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_OrganizationUnit_Post");

            entity.HasOne(d => d.Profile).WithMany(p => p.OrganizationUnits)
                .HasForeignKey(d => d.ProfileId)
                .HasConstraintName("FK_OrganizationUnit_Profile");

            entity.HasOne(d => d.Subdivision).WithMany(p => p.OrganizationUnits)
                .HasForeignKey(d => d.SubdivisionId)
                .HasConstraintName("FK_OrganizationUnit_Subdivision");
        });

        modelBuilder.Entity<Password>(entity =>
        {
            entity.ToTable("Password");

            entity.Property(e => e.PasswordId)
                .ValueGeneratedNever()
                .HasColumnName("Password_Id");
            entity.Property(e => e.AccountId).HasColumnName("Account_Id");
            entity.Property(e => e.PasswordStartDate).HasColumnName("Password_StartDate");
            entity.Property(e => e.PasswordStopDate).HasColumnName("Password_StopDate");
            entity.Property(e => e.PasswordValue).HasColumnName("Password_Value");

            entity.HasOne(d => d.Account).WithMany(p => p.Passwords)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Password_Account");
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.PhonesId);

            entity.Property(e => e.PhonesId)
                .ValueGeneratedNever()
                .HasColumnName("Phones_Id");
            entity.Property(e => e.PhonesIsPriority).HasColumnName("Phones_IsPriority");
            entity.Property(e => e.PhonesNum)
                .IsUnicode(false)
                .HasColumnName("Phones_Num");
            entity.Property(e => e.ProfileId).HasColumnName("Profile_Id");
            entity.Property(e => e.TypePhoneId).HasColumnName("TypePhone_Id");

            entity.HasOne(d => d.Profile).WithMany(p => p.Phones)
                .HasForeignKey(d => d.ProfileId)
                .HasConstraintName("FK_Phones_Profile");

            entity.HasOne(d => d.TypePhone).WithMany(p => p.Phones)
                .HasForeignKey(d => d.TypePhoneId)
                .HasConstraintName("FK_Phones_TypePhone");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Post");

            entity.Property(e => e.PostId)
                .ValueGeneratedNever()
                .HasColumnName("Post_Id");
            entity.Property(e => e.PostName)
                .IsUnicode(false)
                .HasColumnName("Post_Name");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("Profile");

            entity.Property(e => e.ProfileId)
                .ValueGeneratedNever()
                .HasColumnName("Profile_Id");
            entity.Property(e => e.ProfileName).HasColumnName("Profile_Name");
            entity.Property(e => e.ProfilePatronymic).HasColumnName("Profile_Patronymic");
            entity.Property(e => e.ProfilePhoto)
                .HasColumnType("image")
                .HasColumnName("Profile_Photo");
            entity.Property(e => e.ProfileSurname).HasColumnName("Profile_Surname");
            entity.Property(e => e.ProfileTabNum).HasColumnName("Profile_TabNum");
        });

        modelBuilder.Entity<Subdivision>(entity =>
        {
            entity.ToTable("Subdivision");

            entity.Property(e => e.SubdivisionId)
                .ValueGeneratedNever()
                .HasColumnName("Subdivision_Id");
            entity.Property(e => e.SubdivisionName)
                .IsUnicode(false)
                .HasColumnName("Subdivision_Name");
            entity.Property(e => e.SubdivisionPath)
                .IsUnicode(false)
                .HasColumnName("Subdivision_Path");
            entity.Property(e => e.SubdivisionPid).HasColumnName("Subdivision_PId");
        });

        modelBuilder.Entity<TypePhone>(entity =>
        {
            entity.ToTable("TypePhone");

            entity.Property(e => e.TypePhoneId)
                .ValueGeneratedNever()
                .HasColumnName("TypePhone_Id");
            entity.Property(e => e.TypePhoneName)
                .IsUnicode(false)
                .HasColumnName("TypePhone_Name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
