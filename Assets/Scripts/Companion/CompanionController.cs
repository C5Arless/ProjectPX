using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CompanionController : MonoBehaviour {
    [SerializeField] PlayerInfo _playerInfo;
    [SerializeField] VisualEffect _spark;

    // Update is called once per frame
    void Update() {
        EvaluateHealth();
    }

    private void EvaluateHealth() {
        switch (_playerInfo.CurrentHp) {
            case 1: {
                    _spark.SetInt("HealthDisplayRange", (int)PlayerHealthRange.LOW);
                    break;
                }
            case 2: {
                    _spark.SetInt("HealthDisplayRange", (int)PlayerHealthRange.MID);
                    break;
                }
            case 3: {
                    _spark.SetInt("HealthDisplayRange", (int)PlayerHealthRange.HIGH);
                    break;
                }
            default: {
                    break;
                }
        }
    }
}
