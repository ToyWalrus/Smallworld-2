using System;
using Smallworld.Hooks;

namespace Smallworld.Utils;

public static class HooksService
{
    private static IHooks _instance;

    /// <summary>
    /// Initializes the hooks service with the provided hooks instance.
    /// Should be called once during application startup.
    /// </summary>
    public static void Initialize(IHooks hooks)
    {
        _instance = hooks ?? throw new ArgumentNullException(nameof(hooks));
    }

    /// <summary>
    /// Gets the current hooks instance.
    /// </summary>
    public static IHooks Instance
    {
        get => _instance ?? throw new InvalidOperationException("HooksService not initialized. Call Initialize() first.");
    }
}
