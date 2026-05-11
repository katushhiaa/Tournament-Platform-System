using System;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface ITournamentService
{
    Task<TournamentDto> CreateTournamentAsync(TournamentCreateDto dto, Guid organizerId);
    Task<StorageUploadResult> UploadImageAsync(Guid tournamentId, Guid organizerId, System.IO.Stream stream, string fileName, string contentType, long length);
    Task<TournamentPlatformSystemWebApi.Application.DTOs.TournamentStartResponse> StartTournament(Guid tournamentId, Guid organizerId);
    Task<IReadOnlyList<TournamentPlatformSystemWebApi.Application.DTOs.MatchesRoundDto>> GetTournamentMatchesAsync(Guid tournamentId);
}
