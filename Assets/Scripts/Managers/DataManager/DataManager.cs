using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {
    public static DataManager Instance { get; private set; }

    [SerializeField] RecordInfo[] recordsInfo;
    [SerializeField] DataInfo[] slotsInfo;
    [SerializeField] PlayerInfo playerInfo;

    [SerializeField] OptionsInfo _defaultInfo;
    [SerializeField] OptionsInfo _currentInfo;

    private string[] _optionsPayload = new string[6];

    public DataInfo[] SlotsInfo { get { return slotsInfo; } }
    public PlayerInfo PlayerInfo { get { return playerInfo; } }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);            
        }
        else { Destroy(gameObject); }

        FillOptionsPayload();
    }

    private void Start() {
        InitializeDefaultOptions();
        InitializeCurrentOptions();

        InitializePlayerInfo();

        CheckData();
        Invoke("RefreshData", .25f);
        Invoke("RefreshRecords", .5f);
    }

    public void RefreshData() {
        StartCoroutine("RetrieveData");
    }

    public void RefreshRecords() {
        StartCoroutine("InitializeRecords");
    }    

    public void CheckData() {
        try {
            DBVault.GetActiveData();
        } catch {
            DBVault.ReBuildDB();
        }
    }    

    public void ResumeData() {
        object[] activeData = DBVault.GetActiveData();
        object[] activeCheckpoint = DBVault.GetActiveCheckpoint();

        playerInfo.SlotID = (int)activeData[(int)SaveData.Slot_ID];
        playerInfo.Name = (string)activeData[(int)SaveData.Name];
        playerInfo.PowerUps = (int)activeData[(int)SaveData.PowerUps];
        playerInfo.Score = (int)activeData[(int)SaveData.Score];
        playerInfo.CurrentHp = (int)activeData[(int)SaveData.CurrentHp]; 
        playerInfo.Runtime = (int)activeData[(int)SaveData.Runtime];
        playerInfo.Checkpoint = new Vector2((int)activeCheckpoint[1], (int)activeCheckpoint[2]);

    }

    public void QuickSaveData() {
        object[] slotData = new object[4];
        object[] checkpointData = new object[2];

        slotData[0] = playerInfo.Name;
        slotData[1] = playerInfo.PowerUps;
        slotData[2] = playerInfo.Score;
        slotData[3] = playerInfo.CurrentHp;

        checkpointData[0] = playerInfo.Checkpoint.x;
        checkpointData[1] = playerInfo.Checkpoint.y;

        DBVault.UpdateActiveSlot(slotData);
        DBVault.SetCheckpoint(checkpointData);

    }

    public void OverwriteData(int slot) {
        int slotID = slotsInfo[slot].SlotID;

        object[] slotData = new object[4];
        object[] checkpointData = new object[2];

        slotData[0] = playerInfo.Name;
        slotData[1] = playerInfo.PowerUps;
        slotData[2] = playerInfo.Score;
        slotData[3] = playerInfo.CurrentHp;        

        checkpointData[0] = playerInfo.Checkpoint.x;
        checkpointData[1] = playerInfo.Checkpoint.y;

        DBVault.SetActiveSlot(slotID);
        DBVault.UpdateSlotByIdx(slotID, slotData);
        DBVault.UpdateCheckpoint(slotID, checkpointData);        
    }

    public void DeleteData(int slotID) {
        DBVault.ResetSlotCPByIdx(slotID);
    }

    public void AssignSlotInfo(int slot) {
        DBVault.SetActiveSlot(slotsInfo[slot].SlotID);

        playerInfo.SlotID = slotsInfo[slot].SlotID;
        playerInfo.Name = slotsInfo[slot].Name;
        playerInfo.PowerUps = slotsInfo[slot].PowerUps;
        playerInfo.Score = slotsInfo[slot].Score;
        playerInfo.CurrentHp = slotsInfo[slot].CurrentHp;
        playerInfo.Runtime = 1;
        playerInfo.Checkpoint = slotsInfo[slot].Checkpoint;
    }

    public DataInfo GetSlotInfo(int slot) {
        return slotsInfo[slot];
    }

    public void SetRecord() {                
        DBVault.SetHighscore(playerInfo.Name, playerInfo.Score);
    }

    public void ResetRecords() {
        DBVault.ResetHighscore();

    }

    private void InitializeCurrentOptions() {
        //Check if there are options in PlayerPrefs and then fill currentInfo

        for (OptionPayload i = 0; (int)i <= _optionsPayload.Length - 1; i++) {
            
            if (!PlayerPrefs.HasKey(i.ToString())) {
                DefaultOption(i);
            } else {
                CurrentOption(i);
            }

        }
    }

    private void FillOptionsPayload() {
        for (OptionPayload i = 0; (int)i <= _optionsPayload.Length - 1; i++) {
            _optionsPayload[(int)i] = i.ToString();
        }
    }

    private void CurrentOption(OptionPayload target) {
        switch (target) {
            case OptionPayload.MasterVolume: {
                    _currentInfo.MasterVolume = PlayerPrefs.GetInt(target.ToString());
                    break;
                }
            case OptionPayload.MusicVolume: {
                    _currentInfo.MusicVolume = PlayerPrefs.GetInt(target.ToString());
                    break;
                }
            case OptionPayload.SfxVolume: {
                    _currentInfo.SfxVolume = PlayerPrefs.GetInt(target.ToString());
                    break;
                }
            case OptionPayload.Mute: {
                    _currentInfo.Mute = PlayerPrefs.GetInt(target.ToString());
                    break;
                }
            case OptionPayload.DisplayResolution: {
                    string value = PlayerPrefs.GetString(target.ToString());
                    Vector2 resolution = new Vector2();
                    string resX = value.Substring(0, 4);
                    string resY = value.Substring(value.Length - 1, 4);
                    resolution.x = int.Parse(resX);
                    resolution.y = int.Parse(resY);

                    _currentInfo.DisplayResolution = resolution;
                    break;
                }
            case OptionPayload.DisplayMode: {
                    _currentInfo.DisplayMode = PlayerPrefs.GetInt(target.ToString());
                    break;
                }
            case OptionPayload.Quality: {
                    _currentInfo.Quality = PlayerPrefs.GetInt(target.ToString());
                    break;
                }
            default: break;
        }
    }

    private void DefaultOption(OptionPayload target) {
        switch (target) {
            case OptionPayload.MasterVolume: {
                _currentInfo.MasterVolume = _defaultInfo.MasterVolume;
                break;
            }
            case OptionPayload.MusicVolume: {
                _currentInfo.MusicVolume = _defaultInfo.MusicVolume;
                break;
            }
            case OptionPayload.SfxVolume: {
                _currentInfo.SfxVolume = _defaultInfo.SfxVolume;
                break;
            }
            case OptionPayload.Mute: {
                _currentInfo.Mute = _defaultInfo.Mute;
                break;
            }
            case OptionPayload.DisplayResolution: {
                _currentInfo.DisplayResolution = _defaultInfo.DisplayResolution;
                break;
            }
            case OptionPayload.DisplayMode: {
                _currentInfo.DisplayMode = _defaultInfo.DisplayMode;
                break;
            }
            case OptionPayload.Quality: {
                _currentInfo.Quality = _defaultInfo.Quality;
                break;
            }
            default: break;
        }
    }

    private void InitializePlayerInfo() {
        playerInfo.SlotID = 0;
        playerInfo.Name = "Default";
        playerInfo.PowerUps = 0;
        playerInfo.Score = 0;
        playerInfo.CurrentHp = 3;
        playerInfo.Runtime = 0;
        playerInfo.Checkpoint = new Vector2(0, 0);
    }

    private void InitializeDefaultOptions() {
        _defaultInfo.MasterVolume = 5;
        _defaultInfo.MusicVolume = 5;
        _defaultInfo.SfxVolume = 5;
        _defaultInfo.Mute = 0;

        _defaultInfo.DisplayResolution = new Vector2(1920f, 1080f);
        _defaultInfo.DisplayMode = (int)OptionDisplayMode.Fullscreen;
        _defaultInfo.Quality = (int)OptionQuality.HIGH;
    }

    private IEnumerator RetrieveData() {
        int i = 0;
        List<object[]> saves = DBVault.GetSlotsData();
        List<object[]> checkpoints = DBVault.GetSlotsCheckpoint();

        foreach (object[] save in saves) {
            slotsInfo[i].SlotID = (int)save[(int)SaveData.Slot_ID];
            slotsInfo[i].Name = (string)save[(int)SaveData.Name];
            slotsInfo[i].PowerUps = (int)save[(int)SaveData.PowerUps];
            slotsInfo[i].Score = (int)save[(int)SaveData.Score];
            slotsInfo[i].CurrentHp = (int)save[(int)SaveData.CurrentHp];
            slotsInfo[i].Runtime = (int)save[(int)SaveData.Runtime];
            slotsInfo[i].Checkpoint = new Vector2((int)checkpoints[i][1], (int)checkpoints[i][2]);
            i++;

            yield return null;
        }

        yield break;
    }

    private IEnumerator InitializeRecords() {
        int i = 0;
        List<object[]> records = DBVault.GetHighscore();       

        if (DBVault.GetHighscoreCount() > 0) {
            foreach (object[] record in records) {            
                recordsInfo[i].Name = (string)record[(int)Record.Name];
                recordsInfo[i].Score = (int)record[(int)Record.Score];

                i++;

                yield return null;
            }

        } else {
            foreach (RecordInfo recordinfo in recordsInfo) {
                recordinfo.Name = "Default";
                recordinfo.Score = 0;

                yield return null;
            }
        } 

        yield break;
    }

}
