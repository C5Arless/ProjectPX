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

public enum MusicTracks {
    MainMenu_Intro,
    MainMenu_Loop,
    Lab_Intro,
    Lab_Loop,
    Map1_Intro,
    Map1_Loop,
    Warp1_Intro,
    Warp1_Loop,
    Warp2_Intro,
    Warp2_Loop,
    Warp3_Intro,
    Warp3_Loop1,
    Warp3_Loop2,
    Warp3_Loop3,
    Warp3_End
}

public enum SFXTracks {
    Keyboard_1,
    Keyboard_2,
    Keyboard_3,
    Keyboard_4,
    AirSweep,
    Scatter,
    Overdrive,
    Bump,
    Boop,
    Whoop,
    Crash,
    Pow,
    Tap,
    Spotlight
}

public enum OptionDisplayMode {
    Fullscreen,
    Windowed,
    Borderless
}

public enum OptionQuality {
    LOW,
    MEDIUM,
    HIGH
}

public enum OptionPayload {
    MasterVolume,
    MusicVolume,
    SfxVolume,
    Mute,
    DisplayResolution,
    DisplayMode,
    Quality
}