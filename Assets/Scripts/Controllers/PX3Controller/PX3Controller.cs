using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class PX3Controller : MonoBehaviour {
    [SerializeField] Rigidbody rb;
    
    PX3StateHandler _stateHandler;
    PX3AnimHandler _animHandler;
    PX3InputHandler _inputHandler;
    PX3SensorHandler _sensorHandler;
 }
