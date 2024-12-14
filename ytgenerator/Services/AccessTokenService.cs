using Microsoft.EntityFrameworkCore;
using ytgenerator.Data;
using ytgenerator.Data.Entities;

namespace ytgenerator.Services;
public interface IAccessTokenService
{
    Task AddAccessToken(string AccessToken, Guid userId);
    Task<bool> IsAccessTokenBlacklisted(string AccessToken);
    Task DisableAllAccessTokensForUser(Guid userId);
}

public class AccessTokenService : IAccessTokenService
{
    private readonly ApplicationDbContext _context;

    public AccessTokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAccessToken(string AccessToken, Guid userId)
    {
        var AccessTokens = new AccessToken
        {
            Value = AccessToken,
            UserId = userId
        };

        _context.AccessTokens.Add(AccessTokens);
        await _context.SaveChangesAsync();
    }

    public async Task DisableAllAccessTokensForUser(Guid userId)
    {
        var AccessTokens = await _context.AccessTokens.Where(t => t.UserId == userId && t.IsValid).ToListAsync();
        foreach (var AccessToken in AccessTokens)
        {
            AccessToken.IsValid = false;
        }
        _context.AccessTokens.UpdateRange(AccessTokens);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsAccessTokenBlacklisted(string AccessToken)
    {
        return await _context.AccessTokens.AnyAsync(t => t.Value == AccessToken && !t.IsValid);
    }
}

