using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    public Unit unit;

    public string playerName = "Noname";

    [SerializeField]
    private Object playerUnitPrefab;

    public Camera cam;
    public float defaultSensitivity = 30f;
    public float minSensitivity = 10f;
    public float maxSensitivity = 50f;
    public float sensitivity = 30f;
    public float rotateInertion = 30f;

    public float volume = 1f;

    public float interactionAngle=30;
    private Interactable visibleInteractable;

    [SerializeField]
    private Object menuBackground;
 
    public Architect architect;

    public float fadeDelay = 2f;

    public float deadscreenDelay = 3f;

    public Object firstCutScene;
    public Object secondCutScene;

    public UnityEvent skipEvent;

    public UnityEvent unitSet;

    public UnityEvent gameplayStateChanged;

    public UnityEvent newGame;

    public string leaderBoardData;

    public int reachedLevel = 0;

    private float unitLifetime = 0;

    bool writeProgress = true;

    public UnityEvent<float> volumeChanged;
    public UnityEvent<float> sensetivityChanged;

    public enum GameplayState
    {
        Game,
        Menu,
        Pause,
        CutScene,
        EndCutScene
    }

    [SerializeField]
    private GameplayState gameplayState = GameplayState.Menu;

    public bool gameplayStateChanging = false;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        LoadData();
        UpdateLeaderboard();
    }

    private void Start()
    {   
        architect.end.AddListener(Victory);
        architect.changeDelay = fadeDelay;
        architect.DestroyNonPlayerObjects();

        architect.LevelChangeStart.AddListener(() => {
            coroutine = SetGameplayState(GameplayState.Game);
            StartCoroutine(coroutine);
        });

        coroutine = SetGameplayState(GameplayState.Menu);
        StartCoroutine(coroutine); 
    }

    private void Update()
    { 
        CheckInteractable();

        if (unit != null)
            unitLifetime = unit.GetLifeTime();
    }
    private void LateUpdate()
    {
        SyncWithUnit();
    }

    private void ToggleCursor(bool visible)
    {
        Cursor.visible = visible;
        if (visible)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void SyncWithUnit() 
    {
        if (unit == null)
        { 
            return; 
        }

        Vector3 eye;

        if (unit.cameraPlace == null)
            eye = unit.transform.position;
        else
            eye = unit.cameraPlace.position;

        cam.transform.position = eye;
        Quaternion newRotation = unit.transform.rotation;
        newRotation = Quaternion.Euler(newRotation.eulerAngles + Vector3.right * (-unit.xRotation - newRotation.eulerAngles.x));
        cam.transform.rotation = newRotation;
    }

    private Vector3 deltaRotation = Vector3.zero;

    public void ProcessLook(Vector2 input)
    {
        if (unit == null)
            return;

        Vector3 newForward = Vector3.up * (input.x * Time.fixedDeltaTime) * sensitivity + Vector3.right * (input.y * Time.fixedDeltaTime) * sensitivity;

        deltaRotation = Vector3.Lerp(deltaRotation, newForward, rotateInertion * Time.fixedDeltaTime);

        unit.RotateLocal(deltaRotation);
    }

    public void ProcessMove(Vector2 input) 
    {
        if (unit == null)
            return;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        unit.SetMoveDirection(unit.transform.TransformDirection(moveDirection));
    }

    public void Dash(Vector2 input) 
    {
        if (unit == null)
            return;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        unit.Dash(unit.transform.TransformDirection(moveDirection));
    }

    public void Jump()
    {
        if (unit == null)
            return;

        unit.Jump();
    }

    public void Fire()
    {
        if (unit == null)
            return;

        unit.UseItem();
    }

    public void Interact()
    {
        if (unit == null || visibleInteractable == null)
            return;

        visibleInteractable.Interact(unit);
    }

    public void AltInteractPushed()
    {
        if (unit == null)
            return;

        if (unit.CheckGrabbed()) 
            unit.DiscardItem();
    }

    public void Switch()
    {
        if (unit == null)
            return;

        unit.SwitchItems();
    }

    public void Skip()
    {
        skipEvent.Invoke();
    }

    void CheckInteractable()
    {
        if (unit == null)
            return;
         
        Collider[] collisions = Physics.OverlapSphere(unit.cameraPlace.position, unit.interactDistance);

        if (visibleInteractable != null)
            visibleInteractable.setOutline(false);

        visibleInteractable = null;

        foreach (Collider collision in collisions)
        {
            Interactable interactable = collision.transform.GetComponentInParent<Interactable>();

            if (interactable == null)
                continue;

            if (!interactable.getActive())
                continue;

            if (Vector3.Angle(collision.transform.position - cam.transform.position, cam.transform.forward) >= interactionAngle)
                continue;

            if (visibleInteractable == null)
            {
                visibleInteractable = interactable;
                continue;
            }

            Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 lastItem = cam.WorldToScreenPoint(visibleInteractable.transform.position);
            Vector3 currentItem = cam.WorldToScreenPoint(interactable.transform.position);

            if ((currentItem - center).magnitude < (lastItem - center).magnitude)
                visibleInteractable = interactable;
        }

        if (visibleInteractable != null)
        {
            visibleInteractable.setOutline(true);
            visibleInteractable.Viewed();
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (unit != null)
        { 
            Gizmos.DrawWireSphere(unit.cameraPlace.position, unit.interactDistance);
        }
    }


    private IEnumerator coroutine;
    public IEnumerator SetGameplayState(GameplayState gs)
    {

        if (gs != GameplayState.Pause && gameplayState != GameplayState.Pause)
        {
            gameplayStateChanging = true;
            yield return new WaitForSeconds(fadeDelay / 2);
        }
        
        gameplayState = gs;
        SyncWithGameplayState();
        gameplayStateChanged.Invoke();

        if (gs != GameplayState.Pause && gameplayState != GameplayState.Pause)
        {
            yield return new WaitForSeconds(fadeDelay / 2);
            gameplayStateChanging = false;
        }

        yield break;
    }

    private void SyncWithGameplayState()
    {
        if (architect.currentLevel > reachedLevel)
        {
            reachedLevel = architect.currentLevel;
        }
        
        cam.enabled = true;
        Time.timeScale = 1;
        switch (gameplayState)
        {
            case GameplayState.Game:
                {
                    ToggleCursor(false);
                    break;
                }
            case GameplayState.Menu:
                {
                    cam.enabled = false;
                    ToggleCursor(true);
                    Instantiate(menuBackground);
                    break;
                }
            case GameplayState.Pause:
                {
                    Time.timeScale = 0;
                    break;
                }
            case GameplayState.CutScene:
                {
                    ToggleCursor(false);
                    cam.enabled = false;

                    CutScene cs = ((GameObject)Instantiate(firstCutScene)).GetComponent<CutScene>();
                    cs.end.AddListener(StartNewGame);
                    skipEvent.AddListener(cs.EndInvoke);

                    break;
                }
            case GameplayState.EndCutScene:
                {
                    ToggleCursor(false);
                    cam.enabled = false;

                    CutScene cs = ((GameObject)Instantiate(secondCutScene)).GetComponent<CutScene>();
                    cs.end.AddListener(EndGame);
                    skipEvent.AddListener(cs.EndInvoke);

                    break;
                }
        }
    }

    public void StartNewGame()
    {
        Enemy.enemyKilled = 0;

        if (unit != null)
            Destroy(unit);

        writeProgress = true;

        newGame.Invoke();

        unit = ((GameObject)Instantiate(playerUnitPrefab)).GetComponent<Unit>();

        unit.GetComponent<Health>().death.AddListener(Defeat);

        unit.SetControl();

        architect.playerUnit = unit;

        architect.NewGame();

        unitSet.Invoke();
    }


    public void StartLevel(int level)
    {
        if (unit != null)
            Destroy(unit);

        writeProgress = false;

        newGame.Invoke();

        unit = ((GameObject)Instantiate(playerUnitPrefab)).GetComponent<Unit>();

        unit.GetComponent<Health>().death.AddListener(Defeat);

        unit.SetControl();

        architect.playerUnit = unit;

        architect.StartLevel(level);

        unitSet.Invoke();
    }

    public void EndGame()
    {
        architect.DestroyNonPlayerObjects();
        coroutine = SetGameplayState(GameplayState.Menu);
        StartCoroutine(coroutine);
    }

    public void StartButton()
    {
        if (firstCutScene != null)
        {
            architect.DestroyNonPlayerObjects();
            coroutine = SetGameplayState(GameplayState.CutScene);
            StartCoroutine(coroutine);
        }
        else
        {
            StartNewGame();
        }
    }

    public void Pause()
    {
        if (gameplayState == GameplayState.Pause)
        {
            UnPause();
            return;
        }
        if (gameplayState != GameplayState.Game)
            return;
        ToggleCursor(true); 
        
        coroutine = SetGameplayState(GameplayState.Pause);
        StartCoroutine(coroutine);
    }

    public void UnPause()
    {
        if (gameplayState != GameplayState.Pause)
            return;
        ToggleCursor(false);

        coroutine = SetGameplayState(GameplayState.Game);
        StartCoroutine(coroutine);
    }
    public void Suicide()
    {
        UnPause();
        if (unit != null)
            unit.GetComponent<Health>().Kill();
    }

    public void Exit()
    {
        SaveData();
        Application.Quit();
    }

    private void Victory()
    {
        if (unit != null)
            Destroy(unit);

        architect.DestroyNonPlayerObjects();

        if (writeProgress)
            AddToLeaderBoard(true, unitLifetime);
        
      //  if (writeProgress)
        coroutine = SetGameplayState(GameplayState.EndCutScene);
      //  else
      //      coroutine = SetGameplayState(GameplayState.Menu);
        StartCoroutine(coroutine);
    }
    private void Defeat()
    {
        if (writeProgress && Enemy.enemyKilled > 0)
            AddToLeaderBoard(false, unitLifetime);

        coroutine = DelayDefeat();
        StartCoroutine(coroutine);
    }

    public IEnumerator DelayDefeat()
    {
        yield return new WaitForSeconds(deadscreenDelay);

        if (unit != null)
            Destroy(unit);

        architect.DestroyNonPlayerObjects();

        coroutine = SetGameplayState(GameplayState.Menu);
        StartCoroutine(coroutine);
    }

    public Interactable GetVisibleInteractable()
    {
        return visibleInteractable;
    }

    public GameplayState GetGameplayState()
    {
        return gameplayState;
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        AudioListener.volume = volume;
    }

    public void SetSensitivity(float newValue)
    {
        sensitivity = minSensitivity + (maxSensitivity - minSensitivity) * newValue;
    }

    public float GetVolume()
    {
        return volume;
    }

    public float GetSensitivity()
    {
        return sensitivity;
    }

    private void UpdateLeaderboard()
    {
        List<string> lines = new List<string>(leaderBoardData.Split("\n"));
        for (int i = 0; i < lines.Count; ++i)
        {
            if (lines[i] == "")
            {
                lines.RemoveAt(i);
                i--;
            }
        }
        lines.Sort(compareLeaderboard);
        lines.Reverse();
        leaderBoardData = "";
        foreach (string data in lines)
        {
            leaderBoardData += data + "\n";
        }
    }

    private void AddToLeaderBoard(bool victory, float lifetime)
    {
        if (playerName.Length == 0)
        {
            playerName = "Noname";
        }
        leaderBoardData += "\n";
        leaderBoardData += playerName + " - " + (victory ? "victory" : "defeat")+" - ";
        int min = (int)Mathf.Floor(lifetime/60);
        int sec = (int)lifetime - min * 60; 
        System.DateTime dt = System.DateTime.Now;
        leaderBoardData += min.ToString() + " m " + sec.ToString()+" s, "+Enemy.enemyKilled + " killed goblins, " + dt.ToString("yyyy-MM-dd");

        leaderBoardData += "\n";
        UpdateLeaderboard();
    }

    int compareLeaderboard(string x, string y)
    {
        if (x == "")
            return 0;
        if (y == "")
            return 0;
        string[] x_ = x.Split(' ');
        string[] y_ = y.Split(' ');
        if (x_[2] == "victory" && y_[2] != "victory")
            return 2;
        if (x_[2] == "defeat" && y_[2] != "defeat")
        {
            if (int.Parse(x_[8]) > int.Parse(y_[8]))
                return 1;
            else 
                return -1;
        }
        
        
        float Xseconds = float.Parse(x_[4]) * 60 + float.Parse(x_[6]);
        float Yseconds = float.Parse(y_[4]) * 60 + float.Parse(y_[6]);

        if (Xseconds < Yseconds) 
            return x_[2] == "victory" ? 1 : -1;
        else
            return x_[2] == "victory" ? -1 : 1;
    }

    private void LoadData()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Noname");
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", defaultSensitivity);
        volume = PlayerPrefs.GetFloat("Volume", 1);
        reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 0);
        leaderBoardData = PlayerPrefs.GetString("Leaderboard", "");
        SetVolume(volume);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("ReachedLevel", reachedLevel);
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.SetString("Leaderboard", leaderBoardData);
    }

    private void OnDestroy()
    {
        SaveData();
    }
}