using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UI : MonoBehaviour
{

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
    private GameObject playerNameText;

    [SerializeField]
    private PlayerControl player;

    [SerializeField]
    private TextMeshProUGUI leaderboardText;

    [SerializeField]
    private Button secondLevel;
    [SerializeField]
    private Button thirdLevel;

    [SerializeField]
    private Image aim;

    public UnityEvent sliderUpdated;

    private void Start()
    {
        gameUI.SetActive(false);
        menuUI.SetActive(false);
        pauseUI.SetActive(false);
        player.newGame.AddListener(() => player.playerName = playerNameText.GetComponent<TMP_InputField>().text);
        player.unitSet.AddListener(SetUnit);
        player.gameplayStateChanged.AddListener(SyncWithGameplayState);
        SyncUI();

    }

    private void Update()
    {
        UpdateAnimation();
        updateBars();
        UpdatePromptText();
    }

    private void UpdatePromptText()
    {
        if (player.GetVisibleInteractable() != null)
            promptText.text = player.GetVisibleInteractable().promptMessage;
        else
            promptText.text = "";
    }

    private void UpdateAnimation()
    {
        uiAnim.SetBool("fade", player.gameplayStateChanging);
        uiAnim.SetBool("prompt", promptText.text != "" ? true : false);

        if (player.unit == null && player.GetGameplayState() == PlayerControl.GameplayState.Game){
            uiAnim.SetBool("dead", true);
            gameUI.SetActive(false);
        }
        else
            uiAnim.SetBool("dead", false);
    }

    private void updateBars()
    {
        if (player.unit == null)
        {
            hpBar.m_FillAmount = 0;
            attackCooldownBar.m_FillAmount = 0;
            dashCooldownBar.m_FillAmount = 0;
            return;
        }

        hpBar.m_FillAmount = Mathf.Clamp01(player.unit.health.GetHP() / player.unit.health.GetMaxHP());

        if (player.unit.firstItem != null)
            attackCooldownBar.m_FillAmount = Mathf.Clamp01(1 - player.unit.firstItem.GetTimer() / player.unit.firstItem.cooldown);
        else
            attackCooldownBar.m_FillAmount = 0;

        dashCooldownBar.m_FillAmount = Mathf.Clamp01(1 - player.unit.GetDashTimer() / player.unit.dashCooldown);

        Ray ray = new Ray(player.unit.cameraPlace.position, player.unit.cameraPlace.forward);
        RaycastHit hit;
        aim.gameObject.SetActive(false);
        if (Physics.Raycast(ray, out hit))
        {
            Health health = hit.transform.GetComponentInParent<Health>();
            Item item = hit.transform.GetComponentInParent<Item>();

            if (health != null)
                if (health.mortal)
                {
                    aim.gameObject.SetActive(true);
                    float newColor = Mathf.Clamp01(health.GetHP() / health.GetMaxHP());
                    aim.color = new Color(1, newColor, newColor, aim.color.a);
                }

            if (item != null)
                if (item.GetGrabbed())
                    aim.gameObject.SetActive(false);

        }
    }

    private void SetUnit()
    {
        if (player.unit != null)
        {
            player.unit.GetComponent<Health>().hit.AddListener(() => uiAnim.SetTrigger("hit"));
        }
    }

    private void SyncWithGameplayState()
    {
        gameUI.SetActive(false);
        menuUI.SetActive(false);
        pauseUI.SetActive(false);
        switch (player.GetGameplayState())
        {
            case PlayerControl.GameplayState.Game:
                {
                    gameUI.SetActive(true);
                    break;
                }
            case PlayerControl.GameplayState.Menu:
                {
                    menuUI.SetActive(true);
                    SyncProgress();
                    
                    break;
                }
            case PlayerControl.GameplayState.Pause:
                {
                    pauseUI.SetActive(true);
                    break;
                }
            case PlayerControl.GameplayState.CutScene:
                {
                    break;
                }
        }
    }

    public void SyncUI()
    {
        player.volumeChanged.Invoke(player.volume);
        player.sensetivityChanged.Invoke((player.sensitivity - player.minSensitivity) / player.maxSensitivity);

        playerNameText.GetComponent<TMP_InputField>().text = player.playerName;

        SyncProgress();
    }

    public void SyncProgress()
    {
        if (player.reachedLevel == 2)
            thirdLevel.interactable = true;
        else 
            thirdLevel.interactable = false;

        if (player.reachedLevel == 1 || player.reachedLevel == 2)
            secondLevel.interactable = true;
        else
            secondLevel.interactable = false;

        leaderboardText.text = player.leaderBoardData;
    }

    public void ClearProgress()
    {
        player.leaderBoardData = "";
        player.reachedLevel = 0;
        SyncProgress();
    }
}
