using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {
    public static DataManager Instance { get; private set; }

    [SerializeField] RecordInfo[] recordsInfo;
    [SerializeField] DataInfo[] slotsInfo;
    [SerializeField] PlayerInfo playerInfo;

    public DataInfo[] SlotsInfo { get { return slotsInfo; } }
    public PlayerInfo PlayerInfo { get { return playerInfo; } }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);            
        }
        else { Destroy(gameObject); }
        
    }

    private void Start() {
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

    public void InitializePlayerInfo() {
        playerInfo.SlotID = 0;
        playerInfo.Name = "Default";
        playerInfo.PowerUps = 0;
        playerInfo.Score = 0;
        playerInfo.CurrentHp = 3;
        playerInfo.Runtime = 0;
        playerInfo.Checkpoint = new Vector2(0, 0);
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
