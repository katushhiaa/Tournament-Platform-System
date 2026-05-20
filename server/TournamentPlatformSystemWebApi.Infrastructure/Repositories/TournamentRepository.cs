using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Repositories;

public class TournamentRepository : BaseRepository<Tournament, TournamentModel>, ITournamentRepository
{
    public TournamentRepository(TournamentdbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public override async Task<Tournament?> GetByIdAsync(Guid id)
    {
        var dbModel = await _context.Set<TournamentModel>()
            .Include(x => x.Organizer)
            .Include(x => x.Theme)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return _mapper.Map<Tournament>(dbModel);
    }

    public override async Task<ICollection<Tournament>> GetAllAsync()
    {
        return await _context.Set<TournamentModel>()
            .Include(x => x.Theme)
            .AsNoTracking()
            .ProjectTo<Tournament>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> IsTitleUniqueAsync(string title, Guid organizerId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return true;

        var normalized = title.Trim().ToLowerInvariant();

        var exists = await _context.Set<TournamentModel>()
            .AsNoTracking()
            .AnyAsync(t => t.OrganizerId == organizerId
                           && t.Name.ToLower() == normalized);

        return !exists;
    }

    public async Task<bool> UpdateStatus(Guid id, TournamentStatus status)
    {
        var dbModel = await _context.Set<TournamentModel>().FindAsync(id);

        if (dbModel == null)
            return false;

        dbModel.Status = (TournamentStatusType)status;
        dbModel.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        _context.Set<TournamentModel>().Update(dbModel);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetParticipantsCountAsync(Guid tournamentId)
    {
        return await _context.Set<TeamModel>()
            .AsNoTracking()
            .CountAsync(t => t.TournamentId == tournamentId && (t.IsDisqualified == null || t.IsDisqualified == false));
    }

    public async Task<IReadOnlyList<Team>> GetTeamsAsync(Guid tournamentId)
    {
        return await _context.Set<TeamModel>()
            .AsNoTracking()
            .Where(t => t.TournamentId == tournamentId && (t.IsDisqualified == null || t.IsDisqualified == false))
            .ProjectTo<Team>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> IsUserInTournamentAsync(Guid tournamentId, Guid userId)
    {
        return await _context.Set<UserTeamModel>()
            .AsNoTracking()
            .Include(ut => ut.Team)
            .AnyAsync(ut => ut.UserId == userId && ut.Team.TournamentId == tournamentId);
    }

    public async Task<Team> AddParticipantAsync(Guid tournamentId, Guid userId, string teamName)
    {
        // create new team and user-team link
        var teamModel = new TeamModel
        {
            Id = Guid.NewGuid(),
            Name = teamName ?? string.Empty,
            TournamentId = tournamentId,
            IsDisqualified = false,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _context.Set<TeamModel>().AddAsync(teamModel);

        var userTeam = new UserTeamModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TeamId = teamModel.Id,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _context.Set<UserTeamModel>().AddAsync(userTeam);

        await _context.SaveChangesAsync();

        return _mapper.Map<Team>(teamModel);
    }

    public async Task<bool> DisqualifyParticipantAsync(Guid tournamentId, Guid userId)
    {
        // find user-team relation
        var userTeam = await _context.Set<UserTeamModel>()
            .Include(ut => ut.Team)
            .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.Team.TournamentId == tournamentId);

        if (userTeam == null)
            return false;

        var team = userTeam.Team;
        if (team == null)
            return false;

        team.IsDisqualified = true;
        team.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        _context.Set<TeamModel>().Update(team);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task AddMatchesAsync(IEnumerable<TournamentPlatformSystemWebApi.Core.Entities.Match> matches)
    {
        if (matches == null) return;

        var models = matches.Select(m =>
        {
            var mm = _mapper.Map<MatchModel>(m);
            mm.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            mm.UpdatedAt = null;
            return mm;
        }).ToList();

        await _context.Set<MatchModel>().AddRangeAsync(models);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<TournamentPlatformSystemWebApi.Core.Entities.Match>> GetMatchesAsync(Guid tournamentId)
    {
        return await _context.Set<MatchModel>()
            .AsNoTracking()
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.Level)
            .ThenBy(m => m.OrderNumber)
            .ProjectTo<TournamentPlatformSystemWebApi.Core.Entities.Match>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TournamentPreviewDto>> GetForUserAsync(Guid userId, int page, int pageSize, IReadOnlyList<TournamentStatus>? statuses)
    {
        var skip = (page - 1) * pageSize;

        var statusFilters = statuses != null && statuses.Count > 0
            ? statuses.Select(s => (TournamentStatusType)s).ToList()
            : null;
        var query = _context.Set<TournamentModel>()
            .AsNoTracking();

        if (statusFilters != null)
            query = query.Where(t => statusFilters.Contains(t.Status));

        var rows = await query
            .Where(t => t.OrganizerId == userId || t.Teams.Any(team => team.UserTeams.Any(ut => ut.UserId == userId)))
            .OrderByDescending(t => t.StartDate)
            .ThenBy(t => t.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(t => new
            {
                t.Id,
                Title = t.Name,
                t.Status,
                t.BackgroundImg,
                SportName = t.Theme != null ? t.Theme.Name : null,
                t.StartDate,
                t.MaxTeams,
                ParticipantsCount = t.Teams.Count(team => team.IsDisqualified == null || team.IsDisqualified == false)
            })
            .ToListAsync();

        return rows.Select(r => new TournamentPreviewDto
        {
            Id = r.Id,
            Title = r.Title,
            Status = r.Status.ToString().ToLowerInvariant(),
            BackgroundImg = r.BackgroundImg,
            SportName = r.SportName,
            StartDate = r.StartDate,
            ParticipantsCount = r.ParticipantsCount,
            MaxParticipants = r.MaxTeams
        }).ToList();
    }

    public async Task<IReadOnlyList<TournamentPreviewDto>> GetAllPreviewAsync(int page, int pageSize, bool randomize, IReadOnlyList<TournamentStatus>? statuses)
    {
        var skip = (page - 1) * pageSize;

        var statusFilters = statuses != null && statuses.Count > 0
            ? statuses.Select(s => (TournamentStatusType)s).ToList()
            : null;

        var query = _context.Set<TournamentModel>()
            .AsNoTracking();

        if (statusFilters != null)
            query = query.Where(t => statusFilters.Contains(t.Status));

        query = randomize
            ? query.OrderBy(_ => EF.Functions.Random())
            : query.OrderByDescending(t => t.StartDate).ThenBy(t => t.Id);

        var rows = await query
            .Skip(skip)
            .Take(pageSize)
            .Select(t => new
            {
                t.Id,
                Title = t.Name,
                t.Status,
                t.BackgroundImg,
                SportName = t.Theme != null ? t.Theme.Name : null,
                t.StartDate,
                t.MaxTeams,
                ParticipantsCount = t.Teams.Count(team => team.IsDisqualified == null || team.IsDisqualified == false)
            })
            .ToListAsync();

        return rows.Select(r => new TournamentPreviewDto
        {
            Id = r.Id,
            Title = r.Title,
            Status = r.Status.ToString().ToLowerInvariant(),
            BackgroundImg = r.BackgroundImg,
            SportName = r.SportName,
            StartDate = r.StartDate,
            ParticipantsCount = r.ParticipantsCount,
            MaxParticipants = r.MaxTeams
        }).ToList();
    }

}

