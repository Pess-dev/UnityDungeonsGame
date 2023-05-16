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
    private Object playerPrefab;

    //Look
    public Camera cam; 
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    //interaction system
    public float interactionAngle=30;
    private Interactable visibleInteractable;

    //UI
    [SerializeField]
    private TextMeshProUGUI promptText;
    [SerializeField]
    private CircularProgressBar hpBar;
    [SerializeField]
    private CircularProgressBar attackCooldownBar;
    [SerializeField]
    private CircularProgressBar dashCooldownBar;
    [SerializeField]
    private Animator uiAnim;
    [SerializeField]
    private GameObject menuUI;
    [SerializeField]
    private GameObject gameUI;
    [SerializeField]
    private GameObject pauseUI;

    [SerializeField]
    private TextMeshProUGUI playerNameText;

    [SerializeField]
    private Object menuBackground;
    
    private Animator cameraAnim;

    public Architect architect;

    public float fadeDelay = 2f;

    public Object firstCutScene;

    public UnityEvent skipEvent;

    public enum GameplayState
    {
        Game,
        Menu,
        Pause,
        CutScene1,
    }

    [SerializeField]
    private GameplayState gameplayState = GameplayState.Menu;

    public bool fade = false;

    private void Start()
    {
        cameraAnim = GetComponent<Animator>();
        if (unit != null)
        {
            unit.SetControl();
            unit.GetComponent<Health>().hit.AddListener(() => uiAnim.SetTrigger("hit")) ;
        }
        architect.end.AddListener(Win);
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
        UpdateAnimation();
        updateBars();
    }
    private void LateUpdate()
    {
        SyncWithUnit();
    }

    private void UpdateAnimation()
    {
        if (unit == null)
        {
            cameraAnim.SetBool("dash", false);
        }

        cameraAnim.SetBool("dash", unit.GetDashing());

        uiAnim.SetBool("fade", fade);

        uiAnim.SetBool("prompt", promptText.text != "" ? true : false);
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

    private void updateBars()
    {
        if (unit == null)
        {
            hpBar.m_FillAmount = 0;
            attackCooldownBar.m_FillAmount = 0;
            dashCooldownBar.m_FillAmount = 0;
            return;
        }

        hpBar.m_FillAmount = unit.GetHP() / unit.GetMaxHP();

        if (unit.firstItem != null)
            attackCooldownBar.m_FillAmount = 1 - unit.firstItem.GetTimer() / unit.firstItem.cooldown;
        else
            attackCooldownBar.m_FillAmount = 0;

        dashCooldownBar.m_FillAmount = 1 - unit.GetDashTimer() / unit.dashCooldown;
    }

    public void ProcessLook(Vector2 input)
    {
        if (unit == null)
            return; 
        unit.RotateLocal(Vector3.up * (input.x * Time.deltaTime) * xSensitivity + Vector3.right * (-input.y * Time.deltaTime) * ySensitivity);
    }

    public void ProcessMove(Vector2 input) 
    {
        if (unit == null)
            return;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        unit.Move(unit.transform.TransformDirection(moveDirection));
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
        if (promptText != null)
            if (visibleInteractable != null)
                promptText.text = visibleInteractable.promptMessage;
            else
                promptText.text = "";


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
            fade = true;
            yield return new WaitForSeconds(fadeDelay / 2);
        }
        
        gameplayState = gs;
        SyncWithGameplayState();

        if (gs != GameplayState.Pause && gameplayState != GameplayState.Pause)
        {
            yield return new WaitForSeconds(fadeDelay / 2);
            fade = false;
        }


        yield break;
    }

    private void SyncWithGameplayState()
    {
        gameUI.SetActive(false);
        menuUI.SetActive(false);
        pauseUI.SetActive(false);
        cam.enabled = true;
        Time.timeScale = 1;
        switch (gameplayState)
        {
            case GameplayState.Game:
                {
                    ToggleCursor(false);
                    gameUI.SetActive(true);
                    break;
                }
            case GameplayState.Menu:
                {
                    cam.enabled = false;
                    menuUI.SetActive(true);
                    ToggleCursor(true);
                    Instantiate(menuBackground);
                    break;
                }
            case GameplayState.Pause:
                {
                    pauseUI.SetActive(true);
                    Time.timeScale = 0;
                    break;
                }
            case GameplayState.CutScene1:
                {
                    ToggleCursor(false);
                    cam.enabled = false;

                    CutScene cs = ((GameObject)Instantiate(firstCutScene)).GetComponent<CutScene>();
                    cs.end.AddListener(StartNewGame);
                    skipEvent.AddListener(cs.EndInvoke);

                    break;
                }
        }
    }

    public void StartNewGame()
    {
        if (unit != null)
            Destroy(unit);

        playerName = playerNameText.text;

        unit = ((GameObject)Instantiate(playerPrefab)).GetComponent<Unit>();

        unit.GetComponent<Health>().death.AddListener(Defeat);

        unit.SetControl();

        unit.GetComponent<Health>().hit.AddListener(() => uiAnim.SetTrigger("hit"));

        architect.playerUnit = unit;

        architect.NewGame();
    }

    public void StartButton()
    {
        if (firstCutScene != null)
        {
            architect.DestroyNonPlayerObjects();


            coroutine = SetGameplayState(GameplayState.CutScene1);
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
        Application.Quit();
    }

    private void Win()
    {
        if (unit != null)
            Destroy(unit);

        architect.DestroyNonPlayerObjects();

        coroutine = SetGameplayState(GameplayState.Menu);
        StartCoroutine(coroutine);
    }
    private void Defeat()
    {
        if (unit != null)
            Destroy(unit);

        architect.DestroyNonPlayerObjects();

        coroutine = SetGameplayState(GameplayState.Menu);
        StartCoroutine(coroutine);
    }

    
}
