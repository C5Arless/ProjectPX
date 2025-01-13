public enum PlayerVFX {
    AirRing,
    DashTrail,
    AttackBurst,
    JumpTrail,
    JumpBump
}
public enum EnemyVFX {
    Spawn,
    Death,
    Trail
}
public enum EnvVFX {
    Shock,
    Smoke,
    Splash,
    Boom
}
public enum Scenes {
    MainMenu,
    Lab,
    Map1,
    Warp1,
    Warp2,
    Warp3
}
public enum Cp {
    CP_0,
    CP_1,
    CP_2,
    CP_3,
    CP_4
}
public enum SaveData {
    Slot_ID,
    Name,
    PowerUps,
    Score,
    CurrentHp,
    Runtime
}

public enum SaveSlot {
    One,
    Two,
    Three
}

public enum Record {
    Name = 1,
    Score = 2
}

public enum UIMode {
    MainScreen,
    MainMenu,
    Slots,
    Pause,
    Records
}

public enum VCameraMode {
    MenuVCameras,
    GameVCameras
}

public enum MenuVCameras {
    MainScreen,
    PauseStart,
    PauseEnd,
    Menu,
    Slots,
    Record
}

public enum AnimatorSignal {
    attackSig = 1,
    jumpSig = 2,
    dashSig = 3,
    k_attackSig = 4,
    s_dashSig = 5,
}

public enum MainCanvasButtons {
    ContinueButton,
    NewGameButton,
    OptionsButton,
    HighScoreButton,
    ExitButton
}

public enum SlotsCanvasButtons {
    Slot1Button,
    Slot2Button,
    Slot3Button
}

public enum PauseCanvasButtons {
    ResumeGameButton,
    SaveGameButton,
    OptionsButton,
    MainMenuButton,
    ExitButton
}
