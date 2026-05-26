using System;
using System.Threading.Tasks;
using AutoMapper;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Core.Entities;
using System.ComponentModel.DataAnnotations;
using TournamentPlatformSystemWebApi.Common.Exceptions;

namespace TournamentPlatformSystemWebApi.Infrastructure.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IThemeRepository _themeRepository;
    private readonly IMapper _mapper;
    private readonly TournamentPlatformSystemWebApi.Application.Interfaces.IStorageService _storageService;
    private readonly TournamentPlatformSystemWebApi.Application.Interfaces.IUserRepository _userRepository;

    public TournamentService(ITournamentRepository tournamentRepository, IThemeRepository themeRepository, IMapper mapper, TournamentPlatformSystemWebApi.Application.Interfaces.IStorageService storageService, TournamentPlatformSystemWebApi.Application.Interfaces.IUserRepository userRepository)
    {
        _tournamentRepository = tournamentRepository;
        _mapper = mapper;
        _storageService = storageService;
        _themeRepository = themeRepository;
        _userRepository = userRepository;
    }

    public async Task<TournamentDto> CreateTournamentAsync(TournamentCreateDto dto, Guid organizerId)
    {
        if (dto == null)
            throw new ValidationException("Tournament data is required");

        var title = dto.Title?.Trim();
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Title is required");

        // Ensure unique title per organizer
        var unique = await _tournamentRepository.IsTitleUniqueAsync(title, organizerId);
        if (!unique)
            throw new DuplicateTournamentTitleException("Tournament title must be unique for the organizer");

        // DEV-342 [bug] Do not allow creating a tournament with a start date in the past
        var startUtc = dto.StartDate.ToUniversalTime().Date;
        if (startUtc < DateTime.UtcNow.Date)
            throw new ValidationException("Start date cannot be in the past");

        var entity = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = title,
            Description = dto.Description,
            Conditions = dto.Conditions,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            RegistrationDeadline = dto.RegistrationCloseDate,
            MaxTeams = dto.MaxParticipants,
            Status = TournamentStatus.REGISTRATION_OPEN,
            OrganizerId = organizerId,
            ThemeId = dto.Sport
        };

        if (!dto.Sport.HasValue || !await _themeRepository.IsSportWithId(dto.Sport.Value))
        {
            throw new ArgumentException("Sport with this Id not found");
        }

        var createdId = await _tournamentRepository.CreateAsync(entity);

        var newTournamentWithDetails = await _tournamentRepository.GetByIdAsync(createdId);

        var result = new TournamentDto
        {
            Id = createdId,
            Title = entity.Name,
            Description = entity.Description,
            Conditions = entity.Conditions,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            RegistrationCloseDate = entity.RegistrationDeadline,
            SportId = newTournamentWithDetails.ThemeId,
            SportName = newTournamentWithDetails.ThemeName,
            MaxParticipants = entity.MaxTeams,
            Status = "registration_open",
            OrganizerId = organizerId,
            OrganizerName = newTournamentWithDetails.OrganizerName
        };

        return result;
    }

    public async Task<TournamentDto> SaveTournamentDraftAsync(TournamentCreateDto dto, Guid organizerId)
    {
        if (dto == null)
            throw new ValidationException("Tournament data is required");

        var title = dto.Title?.Trim();
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Title is required");


        var entity = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = title,
            Description = dto.Description,
            Conditions = dto.Conditions,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            RegistrationDeadline = dto.RegistrationCloseDate,
            MaxTeams = dto.MaxParticipants,
            Status = TournamentStatus.DRAFT,
            OrganizerId = organizerId,
            ThemeId = dto.Sport
        };

        var createdId = await _tournamentRepository.CreateAsync(entity);

        var newTournamentWithDetails = await _tournamentRepository.GetByIdAsync(createdId);

        var result = new TournamentDto
        {
            Id = createdId,
            Title = entity.Name,
            Description = entity.Description,
            Conditions = entity.Conditions,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            RegistrationCloseDate = entity.RegistrationDeadline,
            SportId = newTournamentWithDetails.ThemeId,
            SportName = newTournamentWithDetails.ThemeName,
            MaxParticipants = entity.MaxTeams,
            Status = "draft",
            OrganizerId = organizerId,
            OrganizerName = newTournamentWithDetails.OrganizerName
        };

        return result;
    }

    public async Task<StorageUploadResult> UploadImageAsync(Guid tournamentId, Guid organizerId, System.IO.Stream stream, string fileName, string contentType, long length)
    {
        // validate size and content type
        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (length <= 0 || length > 5 * 1024 * 1024) throw new ValidationException("File is empty or too large (max 5MB)");
        if (Array.IndexOf(allowed, contentType) < 0) throw new ValidationException("Unsupported file type");

        var existing = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (existing == null) throw new KeyNotFoundException("Tournament not found");
        if (existing.OrganizerId != organizerId) throw new UnauthorizedAccessException("Not the organizer");

        var ext = System.IO.Path.GetExtension(fileName) ?? string.Empty;
        var safeName = $"tournament_{tournamentId}_{Guid.NewGuid()}{ext}";

        var upload = await _storageService.UploadAsync(stream, safeName, contentType);

        existing.BackgroundImg = upload.Url;
        await _tournamentRepository.UpdateAsync(existing);

        return upload;
    }

    public async Task<TournamentPlatformSystemWebApi.Application.DTOs.TournamentStartResponse> StartTournament(Guid tournamentId, Guid organizerId)
    {
        var existing = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (existing == null) throw new KeyNotFoundException("Tournament not found");
        if (existing.OrganizerId != organizerId) throw new UnauthorizedAccessException("Not the organizer");
        if (existing.Status == TournamentStatus.IN_PROGRESS)
            throw new TournamentPlatformSystemWebApi.Common.Exceptions.TournamentAlreadyStartedException("Tournament is already started");
        if (existing.Status != TournamentStatus.REGISTRATION_CLOSED)
            throw new TournamentPlatformSystemWebApi.Common.Exceptions.TournamentClosedForChangesException("Tournament is not in registration_closed status");

        var participantsCount = await _tournamentRepository.GetParticipantsCountAsync(tournamentId);
        if (participantsCount < 2)
            throw new TournamentPlatformSystemWebApi.Common.Exceptions.InsufficientParticipantsException("At least 2 participants are required to start the tournament");

        // load teams and build bracket participants
        var teams = await _tournamentRepository.GetTeamsAsync(tournamentId);

        var participants = teams.Select(t => new TournamentPlatformSystemWebApi.Core.Logic.BracketParticipant
        {
            Id = t.Id,
            Name = t.Name
        }).ToList().AsReadOnly();

        // Use single-elimination generator with random seeding by default
        var generator = new TournamentPlatformSystemWebApi.Core.Logic.SingleEliminationBracketGenerator();
        var seeding = new TournamentPlatformSystemWebApi.Core.Logic.RandomSeedingStrategy();

        var matches = generator.Generate(tournamentId, participants, seeding);

        // assign statuses and bye flags (important before persisting)
        TournamentPlatformSystemWebApi.Core.Logic.MatchStatusHelper.AssignStatuses(matches);

        // persist generated matches
        await _tournamentRepository.AddMatchesAsync(matches);

        // update tournament status to IN_PROGRESS
        await _tournamentRepository.UpdateStatus(tournamentId, TournamentStatus.IN_PROGRESS);

        // return minimal start response
        var response = new TournamentPlatformSystemWebApi.Application.DTOs.TournamentStartResponse
        {
            TournamentId = tournamentId,
            Status = TournamentStatus.IN_PROGRESS.ToString().ToLowerInvariant(),
            MatchesCreated = matches?.Count ?? 0
        };

        return response;
    }

    public async Task<IReadOnlyList<TournamentPlatformSystemWebApi.Application.DTOs.MatchesRoundDto>> GetTournamentMatchesAsync(Guid tournamentId)
    {
        var matches = await _tournamentRepository.GetMatchesAsync(tournamentId);

        var totalRounds = matches.Any() ? matches.Max(m => m.Level) : 0;

        var grouped = matches
            .GroupBy(m => m.Level)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var round = g.Key;
                var remaining = totalRounds - round + 1; // 1 => final

                string display;
                if (remaining == 1) display = "final";
                else if (remaining == 2) display = "semifinal";
                else if (remaining == 3) display = "quarterfinal";
                else display = $"1/{(1 << remaining)}";

                return new TournamentPlatformSystemWebApi.Application.DTOs.MatchesRoundDto
                {
                    Round = round,
                    RoundDisplayName = display,
                    Matches = g.Select(m => new TournamentPlatformSystemWebApi.Application.DTOs.MatchDto
                    {
                        MatchId = m.Id,
                        TournamentId = m.TournamentId,
                        Round = m.Level,
                        OrderNumber = m.OrderNumber,
                        Player1Id = m.TeamAId,
                        Player2Id = m.TeamBId,
                        Player1Name = m.TeamAName,
                        Player2Name = m.TeamBName,
                        Status = m.Status,
                        IsBye = m.IsBye,
                        ScorePlayer1 = m.TeamAScore,
                        ScorePlayer2 = m.TeamBScore,
                        WinnerId = m.WinnerId
                    }).ToList().AsReadOnly(),
                    MatchesCount = g.Count(),
                    NotByeMatchesCount = g.Count(x => x.TeamBId != null && x.TeamBId != Guid.Empty)
                };
            }).ToList().AsReadOnly();

        return grouped;
    }

    public async Task<TournamentPlatformSystemWebApi.Application.DTOs.TournamentDetailsDto> GetTournamentDetailsAsync(Guid tournamentId)
    {
        var existing = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (existing == null) throw new KeyNotFoundException("Tournament not found");

        var participantsCount = await _tournamentRepository.GetParticipantsCountAsync(tournamentId);


        var result = new TournamentPlatformSystemWebApi.Application.DTOs.TournamentDetailsDto
        {
            Id = existing.Id,
            Title = existing.Name,
            Description = existing.Description,
            Conditions = existing.Conditions,
            StartDate = existing.StartDate,
            EndDate = existing.EndDate,
            RegistrationCloseDate = existing.RegistrationDeadline,
            SportId = existing.ThemeId,
            SportName = existing.ThemeName,
            MaxParticipants = existing.MaxTeams,
            Status = existing.Status.ToString().ToLowerInvariant(),
            OrganizerId = existing.OrganizerId ?? Guid.Empty,
            OrganizerName = existing.OrganizerName ?? string.Empty,
            BackgroundImg = existing.BackgroundImg,
            ParticipantsCount = participantsCount
        };

        return result;
    }

    public async Task<TournamentPlatformSystemWebApi.Application.DTOs.TournamentDto> UpdateTournamentAsync(Guid tournamentId, TournamentCreateDto dto, Guid organizerId)
    {
        if (dto == null)
            throw new ValidationException("Tournament data is required");

        var existing = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (existing == null) throw new KeyNotFoundException("Tournament not found");
        if (existing.OrganizerId != organizerId) throw new UnauthorizedAccessException("Not the organizer");

        // Do not allow editing when tournament already in progress or completed
        if (existing.Status == TournamentStatus.IN_PROGRESS || existing.Status == TournamentStatus.COMPLETED)
            throw new TournamentPlatformSystemWebApi.Common.Exceptions.TournamentAlreadyStartedException("Tournament cannot be edited in its current status");

        var title = dto.Title?.Trim();
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Title is required");

        // If title changed, ensure uniqueness for organizer
        if (!string.Equals(existing.Name?.Trim(), title, StringComparison.InvariantCultureIgnoreCase))
        {
            var unique = await _tournamentRepository.IsTitleUniqueAsync(title, organizerId);
            if (!unique)
                throw new TournamentPlatformSystemWebApi.Common.Exceptions.DuplicateTournamentTitleException("Tournament title must be unique for the organizer");
        }

        // Validate sport exists
        if (!dto.Sport.HasValue || !await _themeRepository.IsSportWithId(dto.Sport.Value))
        {
            throw new ArgumentException("Sport with this Id not found");
        }

        // Check participants count vs new limit
        var participantsCount = await _tournamentRepository.GetParticipantsCountAsync(tournamentId);
        if (dto.MaxParticipants < participantsCount)
            throw new TournamentPlatformSystemWebApi.Common.Exceptions.InsufficientParticipantsException("New max participants is less than current participants count");

        // map updated fields
        existing.Name = title;
        existing.Description = dto.Description;
        existing.Conditions = dto.Conditions;
        existing.StartDate = dto.StartDate;
        existing.EndDate = dto.EndDate;
        existing.RegistrationDeadline = dto.RegistrationCloseDate;
        existing.MaxTeams = dto.MaxParticipants;
        existing.ThemeId = dto.Sport;

        var updated = await _tournamentRepository.UpdateAsync(existing);

        var newTournamentWithDetails = await _tournamentRepository.GetByIdAsync(updated.Id);

        var result = new TournamentPlatformSystemWebApi.Application.DTOs.TournamentDto
        {
            Id = updated.Id,
            Title = updated.Name,
            Description = updated.Description,
            Conditions = updated.Conditions,
            StartDate = updated.StartDate,
            EndDate = updated.EndDate,
            RegistrationCloseDate = updated.RegistrationDeadline,
            SportId = newTournamentWithDetails.ThemeId,
            SportName = newTournamentWithDetails.ThemeName,
            MaxParticipants = updated.MaxTeams,
            Status = updated.Status.ToString().ToLowerInvariant(),
            OrganizerId = organizerId,
            OrganizerName = newTournamentWithDetails.OrganizerName
        };

        return result;
    }

    public async Task<TournamentPlatformSystemWebApi.Application.DTOs.TeamDto> AddParticipantAsync(Guid tournamentId, Guid userId, Guid actorId)
    {
        var existing = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (existing == null) throw new KeyNotFoundException("Tournament not found");

        // Only allow adding participants while registration is open
        if (existing.Status != TournamentPlatformSystemWebApi.Core.Entities.TournamentStatus.REGISTRATION_OPEN)
            throw new TournamentPlatformSystemWebApi.Common.Exceptions.TournamentClosedForChangesException("Tournament is closed for registration");
        // check max participants
        var participantsCount = await _tournamentRepository.GetParticipantsCountAsync(tournamentId);
        if (existing.MaxTeams > 0 && participantsCount >= existing.MaxTeams)
            throw new TournamentPlatformSystemWebApi.Common.Exceptions.MaxParticipantsReachedException("Maximum participants reached");

        // check user exists
        var user = await _userRepository.GetUserWithDetails(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        // Only the organizer can add other users; users may add themselves
        if (actorId != userId && existing.OrganizerId != actorId)
            throw new UnauthorizedAccessException("Only the tournament organizer can add other participants");
        // user must be a player (not organizer)
        if (user.IsOrganizer == true) throw new UnauthorizedAccessException("User does not have player role");

        // check already added
        var already = await _tournamentRepository.IsUserInTournamentAsync(tournamentId, userId);
        if (already) throw new TournamentPlatformSystemWebApi.Common.Exceptions.ParticipantAlreadyAddedException("Player already added to tournament");

        // derive team name from user details
        var teamName = user.FullName ?? user.UserDetail?.Email ?? $"player_{userId.ToString().Substring(0, 8)}";

        var team = await _tournamentRepository.AddParticipantAsync(tournamentId, userId, teamName);

        return new TournamentPlatformSystemWebApi.Application.DTOs.TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            UserId = userId,
            TournamentId = team.TournamentId,
            IsDisqualified = team.IsDisqualified,
            CreatedAt = team.CreatedAt,
            UpdatedAt = team.UpdatedAt
        };
    }

    public async Task DisqualifyParticipantAsync(Guid tournamentId, Guid userId, Guid actorId)
    {
        var existing = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (existing == null) throw new KeyNotFoundException("Tournament not found");

        // disallow if registration closed, in progress, or completed
        if (existing.Status == TournamentPlatformSystemWebApi.Core.Entities.TournamentStatus.REGISTRATION_CLOSED
            || existing.Status == TournamentPlatformSystemWebApi.Core.Entities.TournamentStatus.IN_PROGRESS
            || existing.Status == TournamentPlatformSystemWebApi.Core.Entities.TournamentStatus.COMPLETED)
        {
            throw new TournamentPlatformSystemWebApi.Common.Exceptions.TournamentClosedForChangesException("Tournament is closed for changes");
        }

        // check participant exists
        var isIn = await _tournamentRepository.IsUserInTournamentAsync(tournamentId, userId);
        if (!isIn) throw new KeyNotFoundException("Participant not found in tournament");

        // Only the organizer can disqualify participants
        if (existing.OrganizerId != actorId)
            throw new UnauthorizedAccessException("Only the tournament organizer can disqualify participants");

        var success = await _tournamentRepository.DisqualifyParticipantAsync(tournamentId, userId);
        if (!success)
            throw new Exception("Failed to disqualify participant");
    }

    public async Task<IReadOnlyList<TournamentPlatformSystemWebApi.Application.DTOs.TeamDto>> GetTournamentParticipantsAsync(Guid tournamentId)
    {
        var existing = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (existing == null) throw new KeyNotFoundException("Tournament not found");

        var teams = await _tournamentRepository.GetTeamsAsync(tournamentId);

        var dtos = teams.Select(t => new TournamentPlatformSystemWebApi.Application.DTOs.TeamDto
        {
            Id = t.Id,
            Name = t.Name,
            UserId = t.Participants.FirstOrDefault()?.UserId ?? Guid.Empty,
            TournamentId = t.TournamentId,
            IsDisqualified = t.IsDisqualified,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList().AsReadOnly();

        return dtos;
    }

    public async Task<IReadOnlyList<TournamentPlatformSystemWebApi.Application.DTOs.EventDto>> GetTournamentEventsAsync(Guid tournamentId)
    {
        var existing = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (existing == null) throw new KeyNotFoundException("Tournament not found");

        var events = new List<TournamentPlatformSystemWebApi.Application.DTOs.EventDto>();

        // Created event
        events.Add(new TournamentPlatformSystemWebApi.Application.DTOs.EventDto
        {
            Id = Guid.NewGuid(),
            Type = "tournament_created",
            Message = "Tournament created",
            CreatedAt = existing.CreatedAt ?? DateTime.UtcNow
        });

        // Registration closed event if registration deadline passed or status indicates closed
        if (existing.RegistrationDeadline != default && (existing.RegistrationDeadline <= DateTime.UtcNow || existing.Status == TournamentPlatformSystemWebApi.Core.Entities.TournamentStatus.REGISTRATION_CLOSED))
        {
            events.Add(new TournamentPlatformSystemWebApi.Application.DTOs.EventDto
            {
                Id = Guid.NewGuid(),
                Type = "registration_closed",
                Message = "Registration closed",
                CreatedAt = existing.RegistrationDeadline
            });
        }

        // Tournament started event when status is in progress
        if (existing.Status == TournamentPlatformSystemWebApi.Core.Entities.TournamentStatus.IN_PROGRESS)
        {
            events.Add(new TournamentPlatformSystemWebApi.Application.DTOs.EventDto
            {
                Id = Guid.NewGuid(),
                Type = "tournament_started",
                Message = "Tournament started",
                CreatedAt = existing.StartDate
            });
        }

        // Tournament completed event when status is completed
        if (existing.Status == TournamentPlatformSystemWebApi.Core.Entities.TournamentStatus.COMPLETED)
        {
            events.Add(new TournamentPlatformSystemWebApi.Application.DTOs.EventDto
            {
                Id = Guid.NewGuid(),
                Type = "tournament_completed",
                Message = "Tournament completed; winners determined",
                CreatedAt = existing.EndDate != default ? existing.EndDate : DateTime.UtcNow
            });
        }

        return events.AsReadOnly();
    }
    public async Task<IReadOnlyList<TournamentPreviewDto>> GetTournamentsForUserAsync(Guid userId, int page, int pageSize, IReadOnlyList<TournamentStatus>? statuses)
    {
        if (page < 1)
            throw new ValidationException("Page must be greater than or equal to 1");

        if (pageSize < 1)
            throw new ValidationException("PageSize must be greater than or equal to 1");

        return await _tournamentRepository.GetForUserAsync(userId, page, pageSize, statuses);
    }

    public async Task<IReadOnlyList<TournamentPreviewDto>> GetAllTournamentsAsync(int page, int pageSize, bool randomize, IReadOnlyList<TournamentStatus>? statuses)
    {
        if (page < 1)
            throw new ValidationException("Page must be greater than or equal to 1");

        if (pageSize < 1)
            throw new ValidationException("PageSize must be greater than or equal to 1");

        return await _tournamentRepository.GetAllPreviewAsync(page, pageSize, randomize, statuses);
    }
}
