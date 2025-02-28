using Content.Shared.Actions;
using Content.Shared.Movement.Systems;
using Robust.Shared.Map;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Xenonids.QueenEye;

/// <summary>
/// This handles...
/// </summary>
public sealed class XenoQueenEyeSystem : EntitySystem
{
    [Dependency] private readonly   INetManager _net = default!;
    [Dependency] private readonly   SharedActionsSystem _actions = default!;
    [Dependency] private readonly   SharedEyeSystem _eye = default!;
    [Dependency] private readonly   SharedMoverController _mover = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<XenoQueenEyeComponent, XenoQueenEyeActionEvent>(OnXenoQueenEyeAction);
        SubscribeLocalEvent<XenoQueenEyeComponent, ComponentRemove>(OnEyeRemove);
    }

    private void OnXenoQueenEyeAction(Entity<XenoQueenEyeComponent> ent, ref XenoQueenEyeActionEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        if (ent.Comp.Active)
        {
            ClearEye(ent);
        }
        else
        {
            SetupEye(ent);
            AttachEye(ent);
        }

        ent.Comp.Active = !ent.Comp.Active;

        foreach (var (actionId, action) in _actions.GetActions(ent))
        {
            if (action.BaseEvent is XenoQueenEyeActionEvent)
                _actions.SetToggled(actionId, ent.Comp.Active);
        }
    }

    private void OnEyeRemove(Entity<XenoQueenEyeComponent> ent, ref ComponentRemove args)
    {
        ClearEye(ent);
    }

    private void SetupEye(Entity<XenoQueenEyeComponent> ent)
    {
        if (_net.IsClient)
            return;

        if (ent.Comp.RemoteEntity != null)
            return;

        var proto = ent.Comp.RemoteEntityProto;

        EntityCoordinates? coords = Transform(ent.Owner).Coordinates;
        ent.Comp.RemoteEntity = SpawnAtPosition(proto, coords.Value);
        Dirty(ent);
    }

    private void ClearEye(Entity<XenoQueenEyeComponent> ent)
    {
        if (_net.IsClient)
            return;

        var user = ent.Owner;

        if (TryComp(user, out EyeComponent? eyeComp))
        {
            _eye.SetDrawFov(user, true, eyeComp);
        }

        QueueDel(ent.Comp.RemoteEntity);
        ent.Comp.RemoteEntity = null;
        Dirty(ent);
    }

    private void AttachEye(Entity<XenoQueenEyeComponent> ent)
    {
        if (ent.Comp.RemoteEntity == null)
            return;

        // Attach them to the portable eye that can move around.
        var user = ent.Owner;

        if (TryComp(user, out EyeComponent? eyeComp))
        {
            _eye.SetDrawFov(user, false, eyeComp);
            _eye.SetTarget(user, ent.Comp.RemoteEntity.Value, eyeComp);
        }

        _mover.SetRelay(user, ent.Comp.RemoteEntity.Value);
    }
}
