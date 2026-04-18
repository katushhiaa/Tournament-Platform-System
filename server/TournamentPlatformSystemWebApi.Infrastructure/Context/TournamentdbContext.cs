using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Context;

public partial class TournamentdbContext : DbContext
{
    public TournamentdbContext()
    {
    }

    public TournamentdbContext(DbContextOptions<TournamentdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountStateModel> AccountStates { get; set; }
    public virtual DbSet<MatchModel> Matches { get; set; }
    public virtual DbSet<TeamModel> Teams { get; set; }
    public virtual DbSet<TournamentModel> Tournaments { get; set; }
    public virtual DbSet<TournamentThemeModel> TournamentThemes { get; set; }
    public virtual DbSet<UserModel> Users { get; set; }
    public virtual DbSet<UserDetailModel> UserDetails { get; set; }
    public virtual DbSet<UserPhoneModel> UserPhones { get; set; }
    public virtual DbSet<UserTeamModel> UserTeams { get; set; }
    // OnConfiguring removed. Use dependency-injected DbContextOptions or the design-time factory.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<TournamentStatusType>("tournament_status");
        modelBuilder.Entity<AccountStateModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("account_state_pkey");

            entity.ToTable("account_state");

            entity.HasIndex(e => e.Name, "account_state_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<MatchModel>(entity =>
        {

            entity.HasKey(e => e.Id).HasName("match_pkey");

            entity.ToTable("match");

            entity.HasIndex(e => e.IsValid, "idx_match_is_valid");

            entity.HasIndex(e => e.Level, "idx_match_level");

            entity.HasIndex(e => e.TeamAId, "idx_match_team_a_id");

            entity.HasIndex(e => e.TeamBId, "idx_match_team_b_id");

            entity.HasIndex(e => e.TournamentId, "idx_match_tournament_id");

            entity.HasIndex(e => e.WinnerId, "idx_match_winner_id");

            entity.HasIndex(e => new { e.TournamentId, e.Level, e.OrderNumber }, "unique_match_position").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsValid)
                .HasDefaultValue(true)
                .HasColumnName("is_valid");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.OrderNumber).HasColumnName("order_number");
            entity.Property(e => e.StartDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_date");
            entity.Property(e => e.TeamAId).HasColumnName("team_a_id");
            entity.Property(e => e.TeamAScore)
                .HasDefaultValue(0)
                .HasColumnName("team_a_score");
            entity.Property(e => e.TeamBId).HasColumnName("team_b_id");
            entity.Property(e => e.TeamBScore)
                .HasDefaultValue(0)
                .HasColumnName("team_b_score");
            entity.Property(e => e.TournamentId).HasColumnName("tournament_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.WinnerId).HasColumnName("winner_id");

            entity.HasOne(d => d.TeamA).WithMany(p => p.MatchTeamAs)
                .HasForeignKey(d => d.TeamAId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("match_team_a_id_fkey");

            entity.HasOne(d => d.TeamB).WithMany(p => p.MatchTeamBs)
                .HasForeignKey(d => d.TeamBId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("match_team_b_id_fkey");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Matches)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("match_tournament_id_fkey");

            entity.HasOne(d => d.Winner).WithMany(p => p.MatchWinners)
                .HasForeignKey(d => d.WinnerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("match_winner_id_fkey");
        });

        modelBuilder.Entity<TeamModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("team_pkey");

            entity.ToTable("team");

            entity.HasIndex(e => e.IsDisqualified, "idx_team_is_disqualified");

            entity.HasIndex(e => e.TournamentId, "idx_team_tournament_id");

            entity.HasIndex(e => new { e.Name, e.TournamentId }, "unique_team_name_per_tournament").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDisqualified)
                .HasDefaultValue(false)
                .HasColumnName("is_disqualified");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.TournamentId).HasColumnName("tournament_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Teams)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("team_tournament_id_fkey");
        });

        modelBuilder.Entity<TournamentModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tournament_pkey");

            entity.ToTable("tournament");

            entity.HasIndex(e => e.OrganizerId, "idx_tournament_organizer_id");

            entity.HasIndex(e => e.RegistrationDeadline, "idx_tournament_registration_deadline");

            entity.HasIndex(e => e.StartDate, "idx_tournament_start_date");

            entity.HasIndex(e => e.ThemeId, "idx_tournament_theme_id");

            entity.Property(e => e.Status)
             .HasColumnName("status")
             .HasColumnType("tournament_status")
              .HasConversion(
                  v => v.ToString(),  
                  v => Enum.Parse<TournamentStatusType>(v)
              );
            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BackgroundImg).HasColumnName("background_img");
            entity.Property(e => e.Conditions).HasColumnName("conditions");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_date");
            entity.Property(e => e.MaxTeams).HasColumnName("max_teams");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.RegistrationDeadline)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("registration_deadline");
            entity.Property(e => e.StartDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_date");
            entity.Property(e => e.ThemeId).HasColumnName("theme_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tournament_organizer_id_fkey");

            entity.HasOne(d => d.Theme).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.ThemeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tournament_theme_id_fkey");
        });

        modelBuilder.Entity<TournamentThemeModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tournament_theme_pkey");

            entity.ToTable("tournament_theme");

            entity.HasIndex(e => e.Name, "tournament_theme_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.HasIndex(e => e.AccountStateId, "idx_user_account_state_id");

            entity.HasIndex(e => e.DeletedAt, "idx_user_deleted_at");

            entity.HasIndex(e => e.IsOrganizer, "idx_user_is_organizer");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AccountStateId).HasColumnName("account_state_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsOrganizer)
                .HasDefaultValue(false)
                .HasColumnName("is_organizer");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AccountState).WithMany(p => p.Users)
                .HasForeignKey(d => d.AccountStateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_account_state_id_fkey");
        });

        modelBuilder.Entity<UserDetailModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_details_pkey");

            entity.ToTable("user_details");

            entity.HasIndex(e => e.Email, "idx_user_details_email");

            entity.HasIndex(e => e.UserId, "idx_user_details_user_id");

            entity.HasIndex(e => e.Email, "user_details_email_key").IsUnique();

            entity.HasIndex(e => e.UserId, "user_details_user_id_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.UserDetail)
                .HasForeignKey<UserDetailModel>(d => d.UserId)
                .HasConstraintName("user_details_user_id_fkey");
        });

        modelBuilder.Entity<UserPhoneModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_phone_pkey");

            entity.ToTable("user_phone");

            entity.HasIndex(e => e.PhoneNumber, "idx_user_phone_phone_number");

            entity.HasIndex(e => e.UserId, "idx_user_phone_user_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserPhones)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_phone_user_id_fkey");
        });

        modelBuilder.Entity<UserTeamModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_team_pkey");

            entity.ToTable("user_team");

            entity.HasIndex(e => e.TeamId, "idx_user_team_team_id");

            entity.HasIndex(e => e.UserId, "idx_user_team_user_id");

            entity.HasIndex(e => new { e.UserId, e.TeamId }, "unique_user_team").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Team).WithMany(p => p.UserTeams)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("user_team_team_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserTeams)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("user_team_user_id_fkey");
        });
    



  
    OnModelCreatingPartial(modelBuilder);

}
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

