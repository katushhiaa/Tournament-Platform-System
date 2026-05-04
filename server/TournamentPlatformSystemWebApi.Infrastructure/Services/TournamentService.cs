using System;
using System.Threading.Tasks;
using AutoMapper;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace TournamentPlatformSystemWebApi.Infrastructure.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IMapper _mapper;
    private readonly TournamentPlatformSystemWebApi.Application.Interfaces.IStorageService _storageService;

    public TournamentService(ITournamentRepository tournamentRepository, IMapper mapper, TournamentPlatformSystemWebApi.Application.Interfaces.IStorageService storageService)
    {
        _tournamentRepository = tournamentRepository;
        _mapper = mapper;
        _storageService = storageService;
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
            throw new ValidationException("Tournament title must be unique for the organizer");

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

        var createdId = await _tournamentRepository.CreateAsync(entity);

        var result = new TournamentDto
        {
            Id = createdId,
            Title = entity.Name,
            Description = entity.Description,
            Conditions = entity.Conditions,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            RegistrationCloseDate = entity.RegistrationDeadline,
            Sport = null,
            MaxParticipants = entity.MaxTeams,
            Status = "draft",
            OrganizerId = organizerId
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
}
