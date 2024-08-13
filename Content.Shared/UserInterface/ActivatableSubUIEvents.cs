using Robust.Shared.Player;

namespace Content.Shared.UserInterface;
public sealed class ActivatableSubUIOpenAttemptEvent : CancellableEntityEventArgs
{
    public EntityUid User { get; }
    public ActivatableSubUIOpenAttemptEvent(EntityUid who)
    {
        User = who;
    }
}

public sealed class UserOpenActivatableSubUIAttemptEvent : CancellableEntityEventArgs //have to one-up the already stroke-inducing name
{
    public EntityUid User { get; }
    public EntityUid Target { get; }
    public UserOpenActivatableSubUIAttemptEvent(EntityUid who, EntityUid target)
    {
        User = who;
        Target = target;
    }
}

public sealed class AfterActivatableSubUIOpenEvent : EntityEventArgs
{
    public EntityUid User { get; }
    public readonly EntityUid Actor;

    public AfterActivatableSubUIOpenEvent(EntityUid who, EntityUid actor)
    {
        User = who;
        Actor = actor;
    }
}

/// <summary>
/// This is after it's decided the user can open the UI,
/// but before the UI actually opens.
/// Use this if you need to prepare the UI itself
/// </summary>
public sealed class BeforeActivatableSubUIOpenEvent : EntityEventArgs
{
    public EntityUid User { get; }
    public BeforeActivatableSubUIOpenEvent(EntityUid who)
    {
        User = who;
    }
}

public sealed class ActivatableSubUIPlayerChangedEvent : EntityEventArgs
{
}
