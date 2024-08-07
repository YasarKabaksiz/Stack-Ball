using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image levelSlider;
    public Image currentLevelImg;
    public Image nextlevelImg;
    
    public GameObject settingBTN;
    public GameObject allBTN;

    public GameObject soundONBTN;
    public GameObject soundOFFBTN;
    public bool soundOnOffBo;

    public TextMeshProUGUI currentLevelText, nextLevelText;

    public bool buttonSettingBo;
    
    public Material playerMat;
    private PlayerController player;
    
    [SerializeField] private GameObject homeUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject finishUI;
    [SerializeField] private GameObject gameoverUI;

    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverBestText;
    


    private void Awake()
    {
        playerMat = FindObjectOfType<PlayerController>().transform.GetChild(0).GetComponent<MeshRenderer>().material;
        player = FindObjectOfType<PlayerController>();
        
        levelSlider.transform.GetComponent<Image>().color = playerMat.color + Color.gray;
        levelSlider.color = playerMat.color;
        currentLevelImg.color = playerMat.color;
        nextlevelImg.color = playerMat.color;
        
        soundONBTN.GetComponent<Button>().onClick.AddListener((() => SoundManager.instance.SoundOnOff()));
        soundOFFBTN.GetComponent<Button>().onClick.AddListener((() => SoundManager.instance.SoundOnOff()));
        
        //Tüm lokal data prefs sill
        //PlayerPrefs.DeleteAll();
        
    }

    private void Start()
    {
        currentLevelText.text = FindObjectOfType<LevelSpawner>().level.ToString();
        nextLevelText.text= FindObjectOfType<LevelSpawner>().level + 1 + "";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !ignoreUI() && player.playerstate == PlayerController.PlayerState.Prepare)
        {
            player.playerstate = PlayerController.PlayerState.Playing;
            homeUI.SetActive(false);
            gameUI.SetActive(true);
            finishUI.SetActive(false);
            gameoverUI.SetActive(false);
        }

        if (player.playerstate == PlayerController.PlayerState.Finish)
        {
            homeUI.SetActive(false);
            gameUI.SetActive(false);
            finishUI.SetActive(true);
            gameoverUI.SetActive(false);
            
        }
        
        if (player.playerstate == PlayerController.PlayerState.Died)
        {
            homeUI.SetActive(false);
            gameUI.SetActive(false);
            finishUI.SetActive(false);
            gameoverUI.SetActive(true);

            gameOverScoreText.text = ScoreManager.instance.score.ToString();
            gameOverBestText.text = PlayerPrefs.GetInt("HighScore").ToString();

            if (Input.GetMouseButtonDown(0))
            {
                ScoreManager.instance.ResetScore();
                SceneManager.LoadScene(0);
            }
        }
        
        if (SoundManager.instance.sound)
        {
            soundONBTN.SetActive(true);
            soundOFFBTN.SetActive(false);
        }
        else
        {
            soundONBTN.SetActive(false);
            soundOFFBTN.SetActive(true);
        }
    }
    
    private bool ignoreUI()
    {
        
        PointerEventData pointerEventData=new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        
        List<RaycastResult> raycastResults=new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData,raycastResults);

        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject.GetComponent<IgnoreGameUI>() != null)
            {
                raycastResults.RemoveAt(i);
                i--;
            }
        }
        

        return raycastResults.Count > 0;
    }
    
    public void LevelSliderFill(float fillAmount)
    {
        levelSlider.fillAmount = fillAmount;
    }

    public void settingShow()
    {
        buttonSettingBo = !buttonSettingBo;
        allBTN.SetActive(buttonSettingBo);
    }
    
}
