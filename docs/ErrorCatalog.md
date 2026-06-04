# Error Catalog and Transactional Code Snippets

## 1. Error Response Schema

All API errors return `ErrorResponseDto`:

```json
{
  "error": {
    "code": 400,
    "type": "ValidationError",
    "message": "Validation failed",
    "fieldErrors": [
      { "field": "title", "message": "Title is required" }
    ],
    "path": "/api/tournaments",
    "timestamp": "2026-06-03T00:00:00Z",
    "traceId": "..."
  }
}
```

Fields:
- `code` — HTTP status code.
- `type` — error category.
- `message` — human-readable description.
- `fieldErrors` — field-level validation issues.
- `path`, `timestamp`, `traceId` — diagnostics.

## 2. Error Catalog

### 2.1 Authentication / Refresh / Login

| HTTP Code | Type | Meaning | Where used |
|---|---|---|---|
| 400 | ValidationError | Missing required fields, invalid payload, invalid token format | `AuthenticationService.RefreshAsync`, `RegisterAsync`, `LoginAsync` |
| 401 | Unauthorized | Invalid credentials, invalid refresh token, expired token | `AuthenticationService.RefreshAsync`, `LoginAsync` |
| 403 | Forbidden | Account inactive | `LoginAsync` |
| 409 | Conflict | Duplicate email during registration | `AuthenticationService.RegisterAsync` |
| 429 | TooManyRequests | Too many failed login attempts | `AuthenticationService.LoginAsync` |
| 500 | InternalServerError | Unexpected error inside auth flow | `AuthController` catch-all |

### 2.2 Tournament endpoints

| HTTP Code | Type | Meaning | Where used |
|---|---|---|---|
| 400 | ValidationError | Invalid payload, business validation failure | `TournamentService` methods: `CreateTournamentAsync`, `SaveTournamentDraftAsync`, `UploadImageAsync`, `StartTournament`, `SaveMatchResultAsync`, `UpdateTournamentAsync` |
| 401 | Unauthorized | User not authorized to perform action | `TournamentService` and controllers |
| 403 | Forbidden | Tournament closed for changes or not organizer | `TournamentService` |
| 404 | NotFound | Tournament / match / user not found | `TournamentService` and controllers |
| 409 | Conflict | Tournament title duplicate, invalid state transition | `TournamentService` / controllers |
| 500 | InternalServerError | Unexpected server-side failure | `TournamentsController` catch-all |

### 2.3 User preferences

| HTTP Code | Type | Meaning | Where used |
|---|---|---|---|
| 400 | ValidationError | Invalid theme ids | `UserPreferencesService.UpdateUserThemePreferencesAsync` |
| 401 | Unauthorized | Not authenticated | `UserPreferencesController` |
| 404 | NotFound | User not found | `UserPreferencesService` / controller |

### 2.4 Health

| HTTP Code | Type | Meaning | Where used |
|---|---|---|---|
| 200 | OK | Service healthy | `HealthHandler` |
| 503 | ServiceUnavailable | Service not ready / DB unavailable | `HealthHandler` |

## 3. Transactional code highlights

### 3.1 `TournamentService.StartTournament`

This business flow now wraps match creation and tournament state update in a single DB transaction.

```csharp
var provider = _context.Database.ProviderName ?? string.Empty;
var useTransaction = _context.Database.CurrentTransaction == null
    && !provider.Contains("InMemory", StringComparison.OrdinalIgnoreCase);

await using var txn = useTransaction ? await _context.Database.BeginTransactionAsync() : null;
try
{
    await _tournamentRepository.AddMatchesAsync(matches);
    await _tournamentRepository.UpdateStatus(tournamentId, TournamentStatus.IN_PROGRESS);
    if (useTransaction && txn != null)
    {
        await txn.CommitAsync();
    }
}
catch
{
    if (useTransaction && txn != null)
    {
        await txn.RollbackAsync();
    }
    throw;
}
```

### 3.2 `TournamentService.SaveMatchResultAsync`

Scoring a match now updates both the finished match and the next-round match (when present), then commits in one atomic transaction.

```csharp
await using var txn = useTransaction ? await _context.Database.BeginTransactionAsync() : null;
try
{
    var updatedMatch = await _tournamentRepository.UpdateMatchAsync(match);
    if (nextMatch != null)
        await _tournamentRepository.UpdateMatchAsync(nextMatch);

    if (match.Level == maxLevel)
    {
        await _tournamentRepository.UpdateStatus(tournamentId, TournamentStatus.COMPLETED);
    }

    if (useTransaction && txn != null)
    {
        await txn.CommitAsync();
    }
    return dto;
}
catch
{
    if (useTransaction && txn != null)
    {
        await txn.RollbackAsync();
    }
    throw;
}
```

### 3.3 `UserRepository.CreateAsync`

User creation persists the account, optional detail, and phone records in one transaction when supported.

```csharp
var useTransaction = ShouldUseTransaction();
await using var txn = useTransaction ? await _context.Database.BeginTransactionAsync() : null;
try
{
    await _context.Set<UserModel>().AddAsync(dbModel);
    await _context.SaveChangesAsync();
    if (entity.UserDetail != null)
    {
        await _context.Set<UserDetailModel>().AddAsync(userDetail);
        await _context.SaveChangesAsync();
    }
    if (useTransaction && txn != null)
    {
        await txn.CommitAsync();
    }
    return dbModel.Id;
}
catch
{
    if (useTransaction && txn != null)
    {
        await txn.RollbackAsync();
    }
    throw;
}
```

### 3.4 `UserRepository.SetUserThemePreferencesAsync`

Setting theme preferences now cleans existing rows, inserts new preferences, and then marks setup as complete in one transaction.

```csharp
var useTransaction = ShouldUseTransaction();
await using var txn = useTransaction ? await _context.Database.BeginTransactionAsync() : null;
try
{
    _context.Set<UserTournamentThemePreferenceModel>().RemoveRange(existing);
    await _context.Set<UserTournamentThemePreferenceModel>().AddRangeAsync(items);
    await _context.SaveChangesAsync();
    await SetPreferencesSetupCompletedAsync(userId, true);
    await txn.CommitAsync();
}
catch
{
    await txn.RollbackAsync();
    throw;
}
```

### 3.5 `AuthenticationService.RegisterAsync`

Registration uses a transaction boundary around user creation plus refresh token persistence to ensure rollback if refresh token storage fails.

```csharp
var useTransaction = _context != null
    && _context.Database.CurrentTransaction == null
    && !(_context.Database.ProviderName?.Contains("InMemory", StringComparison.OrdinalIgnoreCase) ?? false);
await using var txn = useTransaction ? await _context.Database.BeginTransactionAsync() : null;
try
{
    var createdUserId = await _userRepository.CreateAsync(userEntity);
    await _userRepository.SetRefreshTokenForUser(createdUserId, refreshToken, jwtId, refreshExpires);
    if (useTransaction && txn != null)
    {
        await txn.CommitAsync();
    }
}
catch
{
    if (useTransaction && txn != null)
    {
        await txn.RollbackAsync();
    }
    throw;
}
```
