public enum PX3SubStates {
    Attacking,
    Dashing,
    Diving,
    Falling,
    Damaged,
    Idle,
    Jumping,
    Walking,
    Running,
    Sprinting,
    Grabbing,
    WallSliding,
    Thumbling,
    Crouching,
    Brake
}

public enum PX3RootStates {
    Dead,
    Grounded,
    Airborne,
    Mixed
}

public enum PX3A_RootSet {
    Ground,
    Mixed,
    Air,
    Death
}

public enum PX3A_GroundSet {
    IRC,
    Thumbling,
    Sprint_S,
    Sprint_Loop,
    Brake
}

public enum PX3A_MixedSet {
    Ledge_Grab,
    WallSlide,
    Dashing,
    Diving,
    Attack1,
    Charged_S,
    Ledge_Jump,
    Charged_E,
    Attack2,
    Attack3
}

public enum PX3A_AirSet {
    Falling,
    Jump1,
    Jump2,
    Jump3
}

public enum PX3SensorType {
    Body,
    Ground,
    Ledge
}

public enum PX3SensorStage {
    Enter, 
    Stay,
    Exit
}
