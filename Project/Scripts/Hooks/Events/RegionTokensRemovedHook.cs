using Smallworld.Models;

namespace Smallworld.Hooks;

public class RegionTokensRemovedHook : IHook
{
    public string Name => "RegionTokensRemoved";

    public Region Region { get; set; }
    public Token Token { get; set; }
    public int RemovedCount { get; set; }
}