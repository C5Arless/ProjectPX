using UnityEngine;
using Cinemachine;

public enum TargetGroupAction {
    Add,
    Remove
}

public static class CinemachineTargetGroupExtension {    
    public delegate void OnTargetModified();
    public static OnTargetModified _onTargetModified;

    public static void ModifyTarget(this CinemachineTargetGroup group, Transform target, float weight, float radius, TargetGroupAction action) {
        if (group == null || target == null)
            return;

        switch (action) {
            case TargetGroupAction.Add:
                group.AddMember(target, weight, radius);
                _onTargetModified();
                break;

            case TargetGroupAction.Remove:
                group.RemoveMember(target);                
                _onTargetModified();
                break;
        }
    }
}
