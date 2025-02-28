using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.QueenEye;

/// <summary>
/// Indicates this entity can interact with the hive via floating Queen Eye.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class XenoQueenEyeComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Active;

    /// <summary>
    /// The invisible eye entity being used to look around.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? RemoteEntity;

    /// <summary>
    /// Prototype that represents the 'eye' of the Queen
    /// </summary>
    [DataField(readOnly: true)]
    public EntProtoId? RemoteEntityProto = "RMCXenoQueenEyeHolo";
}
