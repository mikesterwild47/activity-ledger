using Ledger.Data;

namespace Ledger.Logic.Templates;

public static class EventTemplates
{
    private static readonly Dictionary<string, string> Templates = new(StringComparer.OrdinalIgnoreCase)
    {
        ["user.signed_in"] = "{actor} signed in",
        ["user.signed_out"] = "{actor} signed out",
        ["record.created"] = "{actor} created {targetType} '{target}'",
        ["record.updated"] = "{actor} updated {targetType} '{target}'",
        ["record.deleted"] = "{actor} deleted {targetType} '{target}'",
        ["role.granted"] = "{actor} granted the '{target}' role",
    };

    public static bool IsKnown(string eventType) => Templates.ContainsKey(eventType);

    public static string Render(ActivityEvent activityEvent) =>
        Templates.TryGetValue(activityEvent.EventType, out var template)
            ? template
                .Replace("{actor}", activityEvent.Actor)
                .Replace("{targetType}", activityEvent.TargetType)
                .Replace("{target}", activityEvent.Target)
            : activityEvent.EventType;
}
