using UnityEngine;

public interface IContextInit 
{
    void InitializeContext();
}

public interface IPhysics {
    void HandleGravity(Rigidbody rb);

}

public interface IVFXInit {
    void InitializeParticles();
}

public interface IWalk {
    void HandleWalk();

}
