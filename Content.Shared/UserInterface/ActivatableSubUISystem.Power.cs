using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.PowerCell;
using Robust.Shared.Containers;

namespace Content.Shared.UserInterface;

public sealed partial class ActivatableSubUISystem
{
    [Dependency] private readonly ItemToggleSystem _toggle = default!;
    [Dependency] private readonly SharedPowerCellSystem _cell = default!;

    private void InitializePower()
    {
        SubscribeLocalEvent<ActivatableSubUIRequiresPowerCellComponent, ActivatableSubUIOpenAttemptEvent>(OnBatteryOpenAttempt);
        SubscribeLocalEvent<ActivatableSubUIRequiresPowerCellComponent, BoundUIOpenedEvent>(OnBatteryOpened);
        SubscribeLocalEvent<ActivatableSubUIRequiresPowerCellComponent, BoundUIClosedEvent>(OnBatteryClosed);
        SubscribeLocalEvent<ActivatableSubUIRequiresPowerCellComponent, ItemToggledEvent>(OnToggled);
    }

    private void OnToggled(Entity<ActivatableSubUIRequiresPowerCellComponent> ent, ref ItemToggledEvent args)
    {
        // only close ui when losing power
        if (!TryComp<ActivatableSubUIComponent>(ent, out var activatable) || args.Activated)
            return;

        if (activatable.Key == null)
        {
            Log.Error($"Encountered null key in activatable ui on entity {ToPrettyString(ent)}");
            return;
        }

        _uiSystem.CloseUi(ent.Owner, activatable.Key);
    }

    private void OnBatteryOpened(EntityUid uid, ActivatableSubUIRequiresPowerCellComponent component, BoundUIOpenedEvent args)
    {
        var activatable = Comp<ActivatableSubUIComponent>(uid);

        if (!args.UiKey.Equals(activatable.Key))
            return;

        _toggle.TryActivate(uid);
    }

    private void OnBatteryClosed(EntityUid uid, ActivatableSubUIRequiresPowerCellComponent component, BoundUIClosedEvent args)
    {
        var activatable = Comp<ActivatableSubUIComponent>(uid);

        if (!args.UiKey.Equals(activatable.Key))
            return;

        // Stop drawing power if this was the last person with the UI open.
        if (!_uiSystem.IsUiOpen(uid, activatable.Key))
            _toggle.TryDeactivate(uid);
    }

    /// <summary>
    /// Call if you want to check if the UI should close due to a recent battery usage.
    /// </summary>
    public void CheckUsage(EntityUid uid, ActivatableSubUIComponent? active = null, ActivatableSubUIRequiresPowerCellComponent? component = null, PowerCellDrawComponent? draw = null)
    {
        if (!Resolve(uid, ref component, ref draw, ref active, false))
            return;

        if (active.Key == null)
        {
            Log.Error($"Encountered null key in activatable ui on entity {ToPrettyString(uid)}");
            return;
        }

        if (_cell.HasActivatableCharge(uid))
            return;

        _uiSystem.CloseUi(uid, active.Key);
    }

    private void OnBatteryOpenAttempt(EntityUid uid, ActivatableSubUIRequiresPowerCellComponent component, ActivatableSubUIOpenAttemptEvent args)
    {
        if (!TryComp<PowerCellDrawComponent>(uid, out var draw))
            return;

        // Check if we have the appropriate drawrate / userate to even open it.
        if (args.Cancelled ||
            !_cell.HasActivatableCharge(uid, draw, user: args.User) ||
            !_cell.HasDrawCharge(uid, draw, user: args.User))
        {
            args.Cancel();
        }
    }
}
