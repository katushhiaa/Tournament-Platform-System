using System;
using System.Linq.Expressions;
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
            .Include(t => t.UserTeams)
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

    public async Task<bool> IsTeamNameUsedAsync(Guid tournamentId, string teamName)
    {
        if (string.IsNullOrWhiteSpace(teamName))
            return false;

        var normalized = teamName.Trim().ToLowerInvariant();
        return await _context.Set<TeamModel>()
            .AsNoTracking()
            .Where(t => t.TournamentId == tournamentId)
            .AnyAsync(t => t.Name != null && t.Name.ToLower() == normalized);
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

    public async Task<bool> RemoveParticipantAsync(Guid tournamentId, Guid userId)
    {
        var userTeam = await _context.Set<UserTeamModel>()
            .Include(ut => ut.Team)
            .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.Team.TournamentId == tournamentId);

        if (userTeam == null)
            return false;

        var team = userTeam.Team;
        _context.Set<UserTeamModel>().Remove(userTeam);

        if (team != null)
        {
            var remaining = await _context.Set<UserTeamModel>().AnyAsync(ut => ut.TeamId == team.Id && ut.Id != userTeam.Id);
            if (!remaining)
            {
                _context.Set<TeamModel>().Remove(team);
            }
        }

        await _context.SaveChangesAsync();
        return true;
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

        // Convert Guid.Empty placeholders to nulls so DB foreign keys are not violated
        foreach (var mm in models)
        {
            if (mm.TeamAId == Guid.Empty)
                mm.TeamAId = null;
            if (mm.TeamBId == Guid.Empty)
                mm.TeamBId = null;
            if (mm.WinnerId == Guid.Empty)
                mm.WinnerId = null;
        }

        await _context.Set<MatchModel>().AddRangeAsync(models);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<TournamentPlatformSystemWebApi.Core.Entities.Match>> GetMatchesAsync(Guid tournamentId)
    {
        return await _context.Set<MatchModel>()
            .AsNoTracking()
            .Where(m => m.TournamentId == tournamentId)
            .Include(m => m.TeamA)
            .Include(m => m.TeamB)
            .OrderBy(m => m.Level)
            .ThenBy(m => m.OrderNumber)
            .ProjectTo<TournamentPlatformSystemWebApi.Core.Entities.Match>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TournamentPlatformSystemWebApi.Core.Entities.Match> UpdateMatchAsync(TournamentPlatformSystemWebApi.Core.Entities.Match match)
    {
        if (match == null) throw new ArgumentNullException(nameof(match));

        var dbModel = await _context.Set<MatchModel>().FindAsync(match.Id);
        if (dbModel == null)
            throw new KeyNotFoundException("Match not found");

        _mapper.Map(match, dbModel);

        if (dbModel.TeamAId == Guid.Empty)
            dbModel.TeamAId = null;
        if (dbModel.TeamBId == Guid.Empty)
            dbModel.TeamBId = null;
        if (dbModel.WinnerId == Guid.Empty)
            dbModel.WinnerId = null;

        dbModel.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        _context.Set<MatchModel>().Update(dbModel);
        await _context.SaveChangesAsync();

        return _mapper.Map<TournamentPlatformSystemWebApi.Core.Entities.Match>(dbModel);
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

    public async Task<IReadOnlyList<TournamentPreviewDto>> GetAllPreviewAsync(int page, int pageSize, bool randomize, IReadOnlyList<TournamentStatus>? statuses, string? searchQuery = null)
    {
        var skip = (page - 1) * pageSize;

        var statusFilters = statuses != null && statuses.Count > 0
            ? statuses.Select(s => (TournamentStatusType)s).ToList()
            : null;

        var query = _context.Set<TournamentModel>()
            .AsNoTracking();

        if (statusFilters != null)
            query = query.Where(t => statusFilters.Contains(t.Status));

        query = ApplySearchFilter(query, searchQuery);
        query = ApplySearchRanking(query, searchQuery, randomize);

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

    private static IQueryable<TournamentModel> ApplySearchFilter(IQueryable<TournamentModel> query, string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
            return query;

        var normalized = searchQuery.Trim().ToLowerInvariant();
        return query.Where(t =>
            t.Name.ToLower().Contains(normalized) ||
            (t.Description != null && t.Description.ToLower().Contains(normalized)));
    }

    private static IOrderedQueryable<TournamentModel> ApplySearchRanking(IQueryable<TournamentModel> query, string searchQuery, bool randomize)
    {

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var normalized = searchQuery.Trim().ToLowerInvariant();
            var tokens = searchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var ordered = query
                .OrderByDescending(t => t.Name.ToLower().Contains(normalized))
                .ThenByDescending(t => t.Description != null && t.Description.ToLower().Contains(normalized))
                .ThenByDescending(BuildContainsAnyExpression(tokens, t => t.Name))
                .ThenByDescending(BuildContainsAnyExpression(tokens, t => t.Description));

            if (randomize)
            {
                ordered = ordered.ThenBy(_ => EF.Functions.Random());
                return ordered;
            }
            return ordered;
        }
        else if (randomize)
        {
            var ordered = query.OrderBy(_ => EF.Functions.Random());
            return ordered;
        }
        else
        {
            var ordered = query.OrderBy(t => t.StartDate).ThenBy(t => t.Id);
            return ordered;
        }


    }

    private static Expression<Func<TournamentModel, bool>> BuildContainsAnyExpression(string[] tokens, Expression<Func<TournamentModel, string?>> propertySelector)
    {
        var parameter = propertySelector.Parameters[0];
        var property = propertySelector.Body;
        var stringType = typeof(string);
        var toLowerMethod = stringType.GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
        var containsMethod = stringType.GetMethod(nameof(string.Contains), new[] { stringType })!;

        Expression body = Expression.Constant(false);
        var notNull = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));

        foreach (var token in tokens)
        {
            var lowered = Expression.Call(property, toLowerMethod);
            var containsCall = Expression.Call(lowered, containsMethod, Expression.Constant(token));
            var condition = Expression.AndAlso(notNull, containsCall);
            body = Expression.OrElse(body, condition);
        }

        return Expression.Lambda<Func<TournamentModel, bool>>(body, parameter);
    }
}

