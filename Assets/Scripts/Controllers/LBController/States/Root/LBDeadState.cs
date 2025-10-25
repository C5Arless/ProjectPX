using UnityEngine;
using System.Collections;

public class LBDeadState : LBBaseState, IContextInit {
    public LBDeadState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true;
        
        InitializeContext();
    }

    public override void EnterState() {
        //Enter logic
        
        Ctx.RigidBody.isKinematic = true;
        Ctx.StartCoroutine(EnterDeadRoutine());
    }

    public override void UpdateState() {
        //Update logic

        //CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic        
    }

    public override void CheckSwitchStates() {
        //Switch logic        
    }

    public void InitializeContext() {
        //
    }

    private IEnumerator EnterDeadRoutine() {
        float targetScale = 0f;
        float currentScale = 1f;        
        
        Ctx.SetScale(currentScale);
        Ctx.SetMask(currentScale);

        while (currentScale > targetScale) {
            currentScale -= .01f;
            
            Ctx.SetScale(currentScale);
            Ctx.SetMask(currentScale);

            yield return null;
        }

        currentScale = targetScale;

        Ctx.SetMask(currentScale);
        Ctx.SetScale(currentScale);

        yield return new WaitForSeconds(5f);
        
        Ctx.InitializeFSM();
        yield break;
    }
}
