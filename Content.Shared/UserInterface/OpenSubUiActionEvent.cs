using Content.Shared.Actions;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Shared.UserInterface;

public sealed partial class OpenSubUiActionEvent : InstantActionEvent
{
    [DataField(required: true, customTypeSerializer: typeof(EnumSerializer))]
    public Enum? Key { get; private set; }
}
