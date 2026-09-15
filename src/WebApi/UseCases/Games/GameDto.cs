using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Games;

/// <summary>游戏板块的展示 DTO（camelCase 序列化）。</summary>
public sealed class GameDto
{
    /// <summary>游戏稳定标识（如 civ6）。</summary>
    [Required]
    public string Key { get; }

    /// <summary>游戏显示名（如「文明 6」）。</summary>
    [Required]
    public string DisplayName { get; }

    public GameDto(string key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }
}
