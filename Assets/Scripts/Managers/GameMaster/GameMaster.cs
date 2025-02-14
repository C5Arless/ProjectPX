using System.Collections;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    public static GameMaster Instance;

    private void Awake() {

        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Game Master Start");        

        ScenesManager.Instance.MainMenu();

        //AudioManager Call
    }
    
}
