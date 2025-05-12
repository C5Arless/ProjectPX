using System.Data;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

public static class DBVault {
    //Env vars
    private static string path = "URI=file:" + Application.streamingAssetsPath + "/DB.db";
    private static IDbConnection dbcon;
    private static IDbCommand dbcmd;
    private static IDataReader reader;
    private static string query;

    //Connection Handlers
    private static void OpenConnection() {
        dbcon = new SqliteConnection(path);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();
    }
    private static void CloseConnection() {
        reader.Dispose();
        dbcmd.Dispose();
        dbcon.Close();
    }

    //Getters
    public static int GetActiveSlotIdx() {
        OpenConnection();

        query = "SELECT Slot_ID FROM Slot WHERE Runtime = 1;";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        int retrieve = 0;

        while (reader.Read()) {
            retrieve = reader.GetInt32(reader.GetOrdinal("Slot_ID"));
        }

        CloseConnection();
        return retrieve;
    }
    private static object GetDataByIdx(int idx, string table, string column) {
        OpenConnection();

        query = "SELECT * FROM " + table + " WHERE " + table + "_ID = " + idx + ";";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        object retrieve = null;

        while (reader.Read()) {
            retrieve = reader.GetValue(reader.GetOrdinal(column));          
        }

        CloseConnection();
        return retrieve;
    } //DEPRECATED
    private static object[] GetMinScore() {
        OpenConnection();

        query = "SELECT * FROM Highscore WHERE highscore = (SELECT MIN(highscore) FROM Highscore)";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        object[] minscore = new object[2];
        while (reader.Read()) {
            minscore = new object[] { reader["Highscore_ID"], reader["name"], reader["Highscore"]};
        }

        CloseConnection();
        return minscore;
    }
    private static object[] GetMaxScore() {
        OpenConnection();

        query = "SELECT * FROM Highscore WHERE highscore = (SELECT MAX(highscore) FROM Highscore)";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        object[] maxscore = new object[2];
        while (reader.Read()) {
            maxscore = new object[] { reader["Highscore_ID"], reader["name"], reader["Highscore"] };
        }

        CloseConnection();
        return maxscore;
    }

    ////Public methods
    public static int GetHighscoreCount() {
        OpenConnection();

        query = "SELECT COUNT(Highscore_ID) FROM Highscore";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        int count = new int();

        while (reader.Read()) {
            count = reader.GetInt32(reader.GetOrdinal("COUNT(Highscore_ID)"));
        }

        CloseConnection();
        return count;
    }
    public static object[] GetActiveData() {
        int activeSlot = GetActiveSlotIdx();
        OpenConnection();

        query = "SELECT * FROM Slot WHERE Slot_ID = " + activeSlot + ";";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        object[] retrieve = new object[5];

        while (reader.Read()) {
            retrieve = new object[] {
                reader["Slot_ID"],
                reader["Name"],
                reader["Powerups"],
                reader["Score"],
                reader["CurrentHp"],
                reader["Runtime"]
            };
        }

        CloseConnection();
        return retrieve;
    } 
    public static List<object[]> GetHighscore() {
        OpenConnection();

        query = "SELECT * FROM Highscore ORDER BY highscore ASC";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        List<object[]> highscore = new List<object[]>();
        highscore.Capacity = 5;
        
        while (reader.Read()) {
            object[] array = new object[] { reader["Highscore_ID"], reader["name"], reader["highscore"]};
            highscore.Add(array);
        }

        CloseConnection();
        return highscore;
    }
    public static List<object[]> GetSlotsData() {        
        OpenConnection();
        query = "SELECT * FROM Slot";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        List<object[]> data = new List<object[]>();
        data.Capacity = 3;

        while (reader.Read()) {
            object[] array = new object[] { 
                reader["Slot_ID"],
                reader["Name"],
                reader["Powerups"],
                reader["Score"],
                reader["CurrentHp"],
                reader["Runtime"] 
            };

            data.Add(array);
        }

        CloseConnection();
        return data;        

    }
    public static List<object[]> GetSlotsCheckpoint() {        
        OpenConnection();

        query = "SELECT * FROM Checkpoint;";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        List<object[]> data = new List<object[]>();
        data.Capacity = 3;

        while (reader.Read()) {
            object[] array = new object[] { reader["Slot_ID"], reader["Level_idx"], reader["CP_idx"] };
            data.Add(array);
        }

        CloseConnection();
        return data;

    }
    public static object[] GetActiveCheckpoint() {
        int activeslot = GetActiveSlotIdx();
        OpenConnection();

        query = "SELECT * FROM Checkpoint WHERE Slot_ID = '" + activeslot + "';";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        object[] retrieve = new object[2];

        while (reader.Read()) {
            retrieve = new object[] { reader["Slot_ID"], reader["Level_idx"], reader["CP_idx"]};
        }

        CloseConnection();
        return retrieve;

    }

    //Setters
    private static void UpdateValueByIdx(int idx, string table, string column, object value) {
        OpenConnection();

        query = "UPDATE " + table + " SET " + column + " = '" + value + "' WHERE " + table + "_ID = " + idx + ";";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();
    } 
    private static void InsertHighscore(int idx, string name, int score) {
        OpenConnection();

        query = "INSERT INTO Highscore VALUES('" + idx + "', '" + name + "', '" + score + "');";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();
    }
    private static void UpdateHighscore(string name, int score) {
        object[] minscore = GetMinScore();

        if (score > (int)minscore[2]) {
            UpdateValueByIdx((int)minscore[0], "Highscore", "name", name);
            UpdateValueByIdx((int)minscore[0], "Highscore", "Highscore", score);
        } else {
            return;
        }
    }
    private static void UpdateTimescore(string name, int score) {
        object[] maxscore = GetMaxScore();

        if (score < (int)maxscore[2]) {
            UpdateValueByIdx((int)maxscore[0], "Highscore", "name", name);
            UpdateValueByIdx((int)maxscore[0], "Highscore", "Highscore", score);
        }
        else {
            return;
        }
    }
    private static void InitDB() {
        OpenConnection();

        query = "CREATE TABLE Slot (Slot_ID int PRIMARY Key, Name varchar(10), Powerups int, Score int, CurrentHp int, Runtime int); \n";
        query += "CREATE TABLE Checkpoint (Slot_ID int PRIMARY KEY, Level_idx int, CP_idx int, foreign key(Slot_ID) references Slot(Slot_ID)); \n";
        query += "CREATE TABLE Highscore (Highscore_ID int PRIMARY Key, Name varchar(10), Highscore int); \n";

        query += "INSERT INTO Slot VALUES('1', 'Slot 1', '0', '0', '3', '0'); \n";
        query += "INSERT INTO Slot VALUES('2', 'Slot 2', '0', '0', '3', '0'); \n";
        query += "INSERT INTO Slot VALUES('3', 'Slot 3', '0', '0', '3', '0'); \n";

        query += "INSERT INTO Checkpoint VALUES('1', '0', '0'); \n";
        query += "INSERT INTO Checkpoint VALUES('2', '0', '0'); \n";
        query += "INSERT INTO Checkpoint VALUES('3', '0', '0'); \n";

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();
    }
    private static void UpdateActiveSlotOld(string column, object value) {
        int activeslot = GetActiveSlotIdx();

        UpdateValueByIdx(activeslot, "Slot", column, value);

    }  //DEPRECATED
    public static void DisposeActiveSlot() { 
        int activeslot = GetActiveSlotIdx();
        UpdateValueByIdx(activeslot, "Slot", "Runtime", 0);
    }  

    ////Public methods
    public static void UpdateCheckpoint(int activeslot, object[] cpinfo) {
        OpenConnection();

        query = "UPDATE Checkpoint SET Level_idx = '" + cpinfo[0] + 
            "', CP_idx = '" + cpinfo[1] + 
            "' WHERE Slot_ID = '" + activeslot + "';";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();
    }
    public static void UpdateSlotByIdx(int idx, object[] value) {
        OpenConnection();

        query = "UPDATE Slot SET Name = '" + value[0] + 
            "', Powerups = '" + value[1] +
            "', Score = '" + value[2] +
            "', CurrentHp = '" + value[3] +
            "' WHERE Slot_ID = " + idx + ";";  //La capienza dell'array non considera id e runtime come campi del nuovo array

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();
    }  
    public static void SetActiveSlot(int idx) {
        int activeslot = GetActiveSlotIdx();
        if (activeslot == 0) {
            UpdateValueByIdx(idx, "Slot", "Runtime", 1);
        } else {
            UpdateValueByIdx(activeslot, "Slot", "Runtime", 0);
            UpdateValueByIdx(idx, "Slot", "Runtime", 1);
        }
    }
    public static void UpdateActiveSlot(object[] value) {
        int activeslot = GetActiveSlotIdx();

        UpdateSlotByIdx(activeslot, value);
    } //MULTI //OVERLOAD
    public static void SetHighscore(string name, int score) {
        int count = GetHighscoreCount();

        if (count <= 4) {
            InsertHighscore(count + 1, name, score);
        } else {
            UpdateTimescore(name, score);
        }
    }
    public static void SetCheckpoint(object[] checkpoint) {
        int activeslot = GetActiveSlotIdx();
        UpdateCheckpoint(activeslot, checkpoint);
    }

    //DROP//DELETE
    private static void DropTable(string table) {
        OpenConnection();

        query = "DROP TABLE " + table + ";";

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();
    }
    private static void DeleteFromTableByIdx(string table, int idx) {
        OpenConnection();

        query = "DELETE FROM " + table + " WHERE " + table + "_ID = " + idx + ";";

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();

    } //DEPRECATED
    private static void DeleteFromTable(string table) {
        OpenConnection();

        query = "DELETE FROM " + table + ";";

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();

    }

    //DEVTOOLS
    public static void ResetDB() {
        ResetSlotCPByIdx(1);
        ResetSlotCPByIdx(2);
        ResetSlotCPByIdx(3);
        DeleteFromTable("Highscore");
    }
    public static void ReBuildDB() {
        DropTable("Slot");
        DropTable("Checkpoint");
        DropTable("Highscore");

        InitDB();
    }
    public static void ResetHighscore() {
        DeleteFromTable("Highscore");
    }
    public static void ResetSlotCPByIdx(int idx) {
        OpenConnection();

        query = "UPDATE Slot SET Name = 'Slot " + 
            idx + "', " +
            "Powerups = '0', " +
            "Score = '0', " +
            "CurrentHp = '3', " +
            "Runtime = '0' " +
            "WHERE Slot_ID = '" + idx + "'; \n";

        query += "UPDATE Checkpoint SET Level_idx = '0', " +
            "CP_idx = '0' " +
            "WHERE Slot_ID = '" + idx + "'; \n";

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();
    }
    private static void InitHS() {
        OpenConnection();

        query = "CREATE TABLE Highscore (Highscore_ID int PRIMARY Key, Name varchar(10), Highscore int); \n";

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        CloseConnection();
    } //DEPRECATED
}
