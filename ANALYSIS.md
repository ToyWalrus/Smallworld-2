# Smallworld Project Analysis

## Overview

Two sub-projects:

- **`Project/`** — The core domain model, game logic, races, powers, events, and tests. Despite being called the "Unity project," it's largely pure C# — it functions as a standalone game engine library.
- **`ConsoleApp/`** — A .NET console frontend using Spectre.Console that wires UI services into the game engine via dependency injection.

---

## 1. Things Done Well

### Clean Domain Model
The core entities (`Game`, `Player`, `RacePower`, `Region`) are well-scoped and cohesive. `RacePower` is a smart design: rather than having separate race and power logic coordinate externally, the composite object owns the state (tokens, regions) and delegates to race/power components. This mirrors how the board game actually works — you never think of the race and power separately once combined.

**What the architecture doesn't cover:** There's no concept of *turn context* separate from the durable game state. Some things that feel like game-state (tokens in hand, regions conquered this turn for Orcs/Pillaging VP, the "last conquest" flag) are currently tracked ad hoc. A `TurnContext` object passed through the conquering phase would give Orcs, Pillaging, Skeletons, and the last-conquest mechanic a clean shared place to read from, rather than each having their own tracking fields.

### Dependency Injection at the Right Seam
The `IConfirmation`, `IRollDice`, and `ISelection<T>` interfaces are the right boundaries to inject. Powers that need user input (Berserk rolling dice, DragonMaster asking for confirmation) take these via constructor injection, which is why the unit tests can mock them cleanly.

**What the architecture doesn't cover:** The injected interfaces are fine for isolated power behavior, but there's no mechanism for a power to *interrupt* the normal game flow. DragonMaster needs to offer the player a choice *before each conquest*, not just as part of cost calculation. Diplomat needs to run *after* scoring. Right now these are squeezed into the cost pipeline even when they don't belong there. A hook system (e.g., `OnBeforeConquest`, `OnAfterScore`) on the base `Power` class would let each power register for the moment it actually cares about.

### Async Cost Reduction Pipeline
```csharp
// RacePower.GetFinalRegionConquerCost
var raceReduction = await Race.GetRegionConquerCostReduction(region);
var powerReduction = await Power.GetRegionConquerCostReduction(region);
return Math.Max(1, baseCost - raceReduction - powerReduction);
```
Making cost reduction async is the right call — it lets Berserk roll the die mid-calculation and lets DragonMaster ask for confirmation before committing.

**What the architecture doesn't cover:** The two reductions are calculated independently and then summed. There's no case in the current rules where this matters, but consider: a future race or power might need to know what the *other* reduction will be before deciding its own. More importantly, there's currently nothing that prevents both race and power from reducing cost below 1 before the `Math.Max(1, ...)` clamp — the clamp at the end is correct, but it means a race can't introspect "is this already at minimum cost?" before deciding whether to spend a special resource (like Sorcerer's token stash).

### Thorough Race/Power Coverage and Tests
All 14 races are implemented. 20+ powers are implemented. The test files cover the edge cases that matter — Elf token recovery, Skeleton redeployment token formula, Ghoul refusing to enter decline. This is the highest-value testing you can do on a rule-heavy game; rules bugs are invisible until someone plays and notices.

**What the architecture doesn't cover:** The tests validate individual race/power behavior in isolation, but there are no integration-level tests for *interactions* between a race and a power in the same combo. Some combinations have meaningful interactions (Ghoul+Berserk, Amazon+Commando) and some are explicitly called out as edge cases in the rules. A small suite of combo interaction tests would catch the cases where two correct implementations combine incorrectly.

### Token Type System on Regions
Using a `List<Token>` on `Region` rather than individual boolean flags per token type is a good forward-looking decision. It handles "multiple things can occupy a region" cleanly.

**What the architecture doesn't cover:** `Token` is currently a flat enum. Some tokens carry a count (there are multiple Encampments, multiple Race tokens), but the list stores one entry per token rather than `(Token type, int count)`. This means "how many race tokens are in this region" requires a `Count()` call rather than a direct lookup, and adding/removing tokens is more verbose than it needs to be. A small `TokenStack` or `Dictionary<Token, int>` would make token arithmetic cleaner, which matters for the redeployment phase.

### Event Aggregator for Cross-Cutting Concerns
The pub/sub system via `IEventAggregator` decouples the state machine from the renderer. The console renderer can subscribe to `RegionConqueredEvent` and update the display without the game logic knowing the renderer exists.

**What the architecture doesn't cover:** Events are fire-and-forget with no return value, which is correct for rendering but insufficient for interactions that need to *block* until the player responds. The current workaround is calling UI service interfaces directly from within race/power logic. This creates two different communication patterns (events for notifications, interface calls for interactions), which can be confusing to follow. Deciding on a consistent split — events for "this happened," interfaces for "I need an answer" — and documenting it would prevent the pattern from drifting.

### `GamePlayer` Wrapper
`GamePlayer` gives the game flow layer a place to track turn-specific player state (`DidEnterDeclineLastTurn`) without polluting `Player`.

**What the architecture doesn't cover:** `DidEnterDeclineLastTurn` is the only field on `GamePlayer` beyond what `Player` already provides, which makes the wrapper feel thin. As more turn-state accumulates (tokens in hand during redeployment, whether the player has taken their last conquest this turn), `GamePlayer` is exactly the right place for it — but right now the seam isn't obvious enough that future additions will go there instinctively. Making the distinction explicit in a comment or naming it `TurnPlayer` / `PlayerTurnState` would signal the intent more clearly.

---

## 2. Architectural Improvements

### The Game Loop Should Be Async, Not a State Machine

The FSM approach — `TurnStartState`, `TurnPlayState`, etc. — is a common instinct for turn-based games, but it produces the wrong result: flow control becomes data instead of code, and following "what happens next" requires mentally traversing state transitions rather than reading top-to-bottom.

The real problem the FSM is solving is simpler: **the game needs to pause and wait for player input.** That's exactly what `async/await` is for.

The game loop should be written as sequential code that reads like the rulebook, with `await` at every point where the player makes a choice:

```csharp
async Task RunGame(Game game)
{
    while (!game.IsOver)
    {
        foreach (var player in game.Players)
            await RunTurn(game, player);
        game.AdvanceRound();
    }
    ShowWinner(game);
}

async Task RunTurn(Game game, Player player)
{
    if (player.NeedsToPickRacePower)
    {
        var combo = await ui.PickRacePower(game.AvailableCombos);
        player.TakeRacePower(combo);
    }

    var action = await ui.PickAction(TurnAction.Conquer, TurnAction.EnterDecline);

    if (action == TurnAction.Conquer)
        await RunConquerPhase(game, player);
    else
        player.EnterDecline();

    await RunRedeployPhase(game, player);
    player.TallyVP(game.Regions);
}
```

This is maintainable because the code *is* the rules. Adding a new phase means adding a new `await` call in the right place.

The existing IO interfaces (`IConfirmation`, `ISelection<T>`, `IRollDice`) are already async — they are the `await` points. They just aren't wired into a sequential game loop yet. The state machine can be removed; the interfaces stay.

**Where events still belong:** The `IEventAggregator` is correct for notifying the renderer that something changed — `RegionConqueredEvent` triggers a board redraw, `ChangeTurnEvent` updates the player panel. Events are for *observers reacting to what happened*. `await` is for *the logic deciding what happens next*. These are different jobs and should stay separate.

### Triggered Abilities Need a Hook System

Some races and powers need to fire *at specific moments* in the game loop regardless of where the loop currently is — Orcs score mid-turn, Diplomat acts after scoring, DragonMaster offers a choice before each conquest. Right now these are squeezed into the cost pipeline even when they don't belong there.

The pattern that scales: register hooks at named moments in the game loop.

```csharp
async Task<ConquerResult> Conquer(Player player, Region region)
{
    await hooks.Run(GameEvent.BeforeConquer, player, region);
    var result = region.ExecuteConquer(player);
    await hooks.Run(GameEvent.AfterConquer, player, region, result);
    return result;
}
```

Each race/power registers for the events it cares about. Berserk registers `BeforeConquer` to roll the die. Orcs register `AfterConquer` to increment their VP counter. Diplomat registers `AfterScore` to pick a player to protect. The game loop doesn't need to know any specific race or power exists — it just runs the hooks at each defined moment. This is the same model used by every major card/strategy game engine (MtG's stack, Hearthstone's triggers).

Critically, this means **the game loop never contains `if` statements about specific races or powers.** A hook point is the signal that something extensible happens here; an `if` statement in the loop is the signal that you forgot to add a hook point. Ghoul's "conquers in decline before the active race" is not a special case hardcoded into `RunConquerPhase` — it's Ghoul registering into a `BeforeConquerPhase` hook:

```csharp
async Task RunConquerPhase(Game game, Player player)
{
    await hooks.Run(GameEvent.BeforeConquerPhase, player); // Ghoul fires here
    while (player.HasTokensToConquer) { ... }
}

// In Ghoul.RegisterEffects:
registry.Register(GameEvent.BeforeConquerPhase, async player =>
{
    if (!player.HasInDecline<Ghoul>()) return;
    await RunGhoulConquerPhase(player);
});
```

Any future race that also needs to act before the conquer phase registers the same hook. The loop never changes.

The base `Race` and `Power` classes would expose virtual `RegisterHooks(IHookRegistry hooks)` methods called once at setup, replacing the current pattern of overriding `OnTurnStart`, `OnTurnEnd`, etc. with scattered callbacks.

### `ISelection<T>` Is Too Generic
```csharp
public interface ISelection<T>
{
    Task<T> SelectAsync(List<T> options);
}
```

`RegionSelection` needs to show conquest cost per region, and `RacePowerSelection` needs to show the VP cost for passing over a combo. Neither of these is in the `T` itself, so the implementations recalculate them independently — duplicating logic that already lives in the domain model.

**Suggested change:** Use typed selection interfaces that carry the context the UI actually needs:

```csharp
public interface IRegionSelection
{
    Task<Region> SelectAsync(IReadOnlyList<(Region region, int cost)> options);
}

public interface IRacePowerSelection
{
    Task<RacePower> SelectAsync(IReadOnlyList<(RacePower combo, int vpCost)> options);
}
```

The caller computes cost once and passes it in, the renderer just displays what it receives.

---

## 3. Unity MVP Focus

The domain model in `Project/` is Unity-compatible as-is — it's pure C#. The Unity work is almost entirely the presentation layer: replace `IConfirmation`, `IRollDice`, and `ISelection<T>` with Unity UI implementations. The DI setup will need to swap `Microsoft.Extensions.DependencyInjection` for a Unity-compatible container or manual instantiation, but the interfaces themselves don't change.

### Must Have

**Map rendering** — The board is the game. Players need to see regions, adjacency, occupancy, and token counts at a glance. A grid with colored regions by type and overlaid token icons is the minimum.

**Race/power selection UI** — The 6-combo selection where passing over a combo costs 1 VP. VP cost shown clearly per option.

**Conquest interaction** — Click-to-select-region with validation highlighting (conquerable vs. not). Cost shown before confirmation.

**Redeployment UI** — After conquering, assign remaining tokens back to owned regions. Required for correct gameplay.

**Turn/phase indicator** — Current player, current phase, remaining tokens in hand.

**Score tracking** — VP per player, updated each round.

**Game end screen** — Final scores and winner after N rounds.

### Nice to Have (Post-MVP)

- Per-region tooltips showing all active tokens/modifiers
- Undo last conquest
- AI opponent (even random-move makes solo testing possible)
- Animated token movement

---

## Still Left to Implement

- Redeployment phase (UI + state)
- Game end condition (round counter, winner determination)
- Last-conquest reinforcement die roll flow
- Sorcerer power (token placement on conquered region)
- Diplomat power (enforcement of "cannot attack chosen opponent next turn")
- Ghoul conquering-in-decline phase (before active race's conquer phase)
- Abandoned region handling in the conquer flow
- Other players' troop redeployment after being conquered
