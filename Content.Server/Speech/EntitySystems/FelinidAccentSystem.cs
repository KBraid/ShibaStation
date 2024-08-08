using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random;
using System.Linq;

namespace Content.Server.Speech.EntitySystems;

public sealed partial class FelinidAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    [GeneratedRegex(@"na", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex NaRegex();

    [GeneratedRegex(@"ne", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex NeRegex();

    [GeneratedRegex(@"ni", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex NiRegex();

    [GeneratedRegex(@"no", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex NoRegex();

    [GeneratedRegex(@"nu", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex NuRegex();

    [GeneratedRegex(@"new", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex NewRegex();

    private static readonly Dictionary<string, string> DirectReplacements = new()
    {
        { "right now", "right meow" },
        { "okay", "meowkay" },
        { "friend", "furriend" },
        { "good", "paw-sitive" },
        { "bad", "clawful" },
        { "please", "purrlease" },
        { "you", "mew" },
        { "food", "noms" },
        { "drink", "sips" },
        { "sleep", "catnap" },
        { "catastrophe", "cat-astrophe" },
        { "angry", "hissy" },
        { "scared", "scaredy-cat" },
        { "howdy", "meowdy" },
        { "awesome", "purrsome" },
        { "amazing", "ameowzing" },
        { "cute", "paw-dorable" },
        { "excuse me", "pawdon me" },
        { "pardon me", "pawdon me" },
        { "morning", "meowning" },
        { "party", "pawty" },
        { "very", "purry" }
    };

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FelinidAccentComponent, AccentGetEvent>(OnAccentGet);
    }

    public string Accentuate(string message, FelinidAccentComponent component)
    {
        // Step 1: Direct word/phrase replacements
        foreach (var (first, replace) in DirectReplacements)
        {
            var regex = new Regex($@"(?<!\w){first}(?!\w)", RegexOptions.IgnoreCase);
            message = regex.Replace(message, match => PreserveCase(match.Value, replace));
        }

        // Step 2: Character manipulations
        // Replace 'na' with 'nya'
        message = NaRegex().Replace(message, match => PreserveCase(match.Value, "nya"));

        // Replace 'new' with 'mew' when it's part of a word
        message = NewRegex().Replace(message, match => PreserveCase(match.Value, "mew"));

        // Replace 'ne' with 'nye'
        message = NeRegex().Replace(message, match => PreserveCase(match.Value, "nye"));

        // Replace 'ni' with 'nyi'
        message = NiRegex().Replace(message, match => PreserveCase(match.Value, "nyi"));

        // Replace 'no' with 'nyo'
        message = NoRegex().Replace(message, match => PreserveCase(match.Value, "nyo"));

        // Replace 'nu' with 'nyu'
        message = NuRegex().Replace(message, match => PreserveCase(match.Value, "nyu"));

        return message;
    }

    private static string PreserveCase(string original, string replacement)
    {
        if (original.All(char.IsUpper))
        {
            return replacement.ToUpper();
        }
        return char.IsUpper(original[0]) ? char.ToUpper(replacement[0]) + replacement.Substring(1) : replacement;
    }

    private void OnAccentGet(EntityUid uid, FelinidAccentComponent component, AccentGetEvent args)
    {
        args.Message = Accentuate(args.Message, component);
    }
}
