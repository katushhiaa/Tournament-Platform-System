using System;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface ITournamentService
{
    Task<TournamentDto> CreateTournamentAsync(TournamentCreateDto dto, Guid organizerId);
    Task<StorageUploadResult> UploadImageAsync(Guid tournamentId, Guid organizerId, System.IO.Stream stream, string fileName, string contentType, long length);
}
