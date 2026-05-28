using System;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface ITournamentService
{
    Task<TournamentDto> CreateTournamentAsync(TournamentCreateDto dto, Guid organizerId);
    Task<TournamentDto> SaveTournamentDraftAsync(TournamentCreateDto dto, Guid organizerId);
    Task<StorageUploadResult> UploadImageAsync(Guid tournamentId, Guid organizerId, System.IO.Stream stream, string fileName, string contentType, long length);
    Task<TournamentPlatformSystemWebApi.Application.DTOs.TournamentStartResponse> StartTournament(Guid tournamentId, Guid organizerId);
    Task<IReadOnlyList<TournamentPlatformSystemWebApi.Application.DTOs.MatchesRoundDto>> GetTournamentMatchesAsync(Guid tournamentId);
    Task<TournamentPlatformSystemWebApi.Application.DTOs.MatchDto> SaveMatchResultAsync(Guid tournamentId, Guid matchId, MatchUpdateDto dto, Guid organizerId);
    Task<TournamentPlatformSystemWebApi.Application.DTOs.TournamentDetailsDto> GetTournamentDetailsAsync(Guid tournamentId);
    Task<TournamentPlatformSystemWebApi.Application.DTOs.TournamentDto> UpdateTournamentAsync(Guid tournamentId, TournamentCreateDto dto, Guid organizerId);
    Task<TournamentPlatformSystemWebApi.Application.DTOs.TeamDto> AddParticipantAsync(Guid tournamentId, Guid userId, Guid actorId);
    Task RemoveParticipantAsync(Guid tournamentId, Guid userId);
    Task DisqualifyParticipantAsync(Guid tournamentId, Guid userId, Guid actorId);
    Task<IReadOnlyList<TournamentPlatformSystemWebApi.Application.DTOs.TeamDto>> GetTournamentParticipantsAsync(Guid tournamentId);
    Task<IReadOnlyList<TournamentPlatformSystemWebApi.Application.DTOs.EventDto>> GetTournamentEventsAsync(Guid tournamentId);
    Task<IReadOnlyList<TournamentPreviewDto>> GetTournamentsForUserAsync(Guid userId, int page, int pageSize, IReadOnlyList<TournamentStatus>? statuses);
    Task<IReadOnlyList<TournamentPreviewDto>> GetAllTournamentsAsync(int page, int pageSize, bool randomize, IReadOnlyList<TournamentStatus>? statuses);
}
