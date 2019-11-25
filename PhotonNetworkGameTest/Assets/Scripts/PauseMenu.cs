using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private int mainMenuIndex = 0;
    [SerializeField]
    private GameObject pauseMainPanel = null;
    [SerializeField]
    private GameObject pauseOptionsPanel = null;
    [SerializeField]
    private GameObject hudPanel = null;

    [SerializeField]
    private Toggle fullScreenToggle = null;
    [SerializeField]
    private Dropdown resolutionDropDown = null;

    private bool isPaused;
    private bool isFullScreen;
    private GameObject[] pauseObjects;
    private PlayerCharacter player;
    private AudioManager audioManager;


    void Start()
    {
        changeResolution(0);
        fullScreenToggle.isOn = Screen.fullScreen;
        audioManager = FindObjectOfType<AudioManager>();
        player = FindObjectOfType<PlayerCharacter>();
        pauseObjects = GameObject.FindGameObjectsWithTag("PauseMenu");
        HidePauseMenu();
        resolutionDropDown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(resolutionDropDown.value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(isFullScreen != Screen.fullScreen)
        {
            isFullScreen = Screen.fullScreen;
        }
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                ShowPauseMenu();
            }
            if (!isPaused)
            {
                HidePauseMenu();
            }
        }
    }

    public void musicSliderChanged(Slider slider)
    {
        audioManager.ChangeAudioVolume(slider.value, Sound.Type.Music);
    }

    public void effectsSliderChanged(Slider slider)
    {
        audioManager.ChangeAudioVolume(slider.value, Sound.Type.Effects);
    }

    public void masterSliderChanged(Slider slider)
    {
        audioManager.ChangeMasterVolume(slider.value);
    }

    void DropdownValueChanged(int change)
    {
        changeResolution(change);
    }

    public void backToMainMenu()
    {
        StartCoroutine(DoSwitchLevel(mainMenuIndex));
    }

    IEnumerator DoSwitchLevel(int level)
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(level);
    }

    private void ShowPauseMenu()
    {
        Cursor.visible = true;
        isPaused = true;
        player.SetControllable(!isPaused);
        pauseMainPanel.SetActive(true);
        pauseOptionsPanel.SetActive(false);
        hudPanel.SetActive(false);
    }

    public void HidePauseMenu()
    {
        Cursor.visible = false;
        isPaused = false;
        player.SetControllable(!isPaused);
        pauseMainPanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    public void ShowOptionsMenu()
    {
        pauseMainPanel.SetActive(false);
        pauseOptionsPanel.SetActive(true);
    }

    public void HideOptionsMenu()
    {
        pauseMainPanel.SetActive(true);
        pauseOptionsPanel.SetActive(false);
    }

    public void changeFullScreen()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, !Screen.fullScreen);
    }

    public void changeResolution(int choiceIndex)
    {
        if(choiceIndex == 0)
        {
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        }
        if (choiceIndex == 1)
        {
            Screen.SetResolution(1280, 720, Screen.fullScreen);
        }
        if (choiceIndex == 2)
        {
            Screen.SetResolution(1024, 576, Screen.fullScreen);
        }
    }

}
