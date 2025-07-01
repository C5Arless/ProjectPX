using UnityEngine;

public interface IParentTrigger {
    void TriggerEnter(Collider other);
    void TriggerStay(Collider other);
    void TriggerExit(Collider other);

}