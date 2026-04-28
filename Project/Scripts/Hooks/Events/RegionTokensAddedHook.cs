using Smallworld.Models;

namespace Smallworld.Hooks;

class RegionTokensAddedHook : IHook
{
    public string Name => "RegionTokensAdded";

    public Region Region { get; set; }
    public Token Token { get; set; }
    public int AddedCount { get; set; }
}