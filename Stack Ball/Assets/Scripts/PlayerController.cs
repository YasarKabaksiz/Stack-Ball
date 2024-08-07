using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;

    private bool Carpma;
    
    float currentTime;
    bool invincible;
    
    public GameObject fireBreath, winEffect, splashEffect ;

    
    [SerializeField]
    AudioClip win, death, idestory, destory, bounce;
    
    public int currentObstacleNumber;
    public int totalObstacleNumber;
    
    [SerializeField] private float FallingSpeed = 100f;
    [SerializeField] private float JumpingSpeed = 50f;
    
    public Image InvictableSlider;
    public GameObject InvictableOBJ;
    public GameObject gameOverUI;
    public GameObject finishUI;
    
    public enum PlayerState
    {
        Prepare,
        Playing,
        Died,
        Finish
    }

    [HideInInspector]
    public PlayerState playerstate = PlayerState.Prepare;
    private void Start()
    {
        totalObstacleNumber = FindObjectsOfType<ObstacleController>().Length;
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentObstacleNumber = 0;
       
    }

    private void Update()
    {
        if (playerstate == PlayerState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Carpma = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Carpma = false;
            }

            if (invincible)
            {
                currentTime -= Time.deltaTime * .5f;
                if (!fireBreath.activeInHierarchy)
                {
                    fireBreath.SetActive(true);
                }
            }
        
            else
            {
                if (fireBreath.activeInHierarchy)
                {
                    fireBreath.SetActive(false);
                }
            
                if (Carpma)
                {
                    currentTime += Time.deltaTime * 0.5f;
                }
                else
                {
                    currentTime -= Time.deltaTime * 0.5f;
                }
            }
            
            if (currentTime >= 0.15f || InvictableSlider.color == Color.red)
            {
                InvictableOBJ.SetActive(true);
            }
            else
            {
                InvictableOBJ.SetActive(false);
            }
        
            if (currentTime >= 1)
            {
                currentTime = 1;
                invincible = true;
                InvictableSlider.color=Color.red;
            }
            else if (currentTime <= 0)
            {
                currentTime = 0;
                invincible = false;
                InvictableSlider.color=Color.white;
            }
            
            if ( InvictableOBJ.activeInHierarchy)
            {
                InvictableSlider.fillAmount = currentTime / 1;
            }
        }

        if (playerstate == PlayerState.Finish)
        {
            if (Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<LevelSpawner>().NextLevel();
            }
        }
    }

    public void shatterObstacles()
    {
        
        
        if (invincible)
        {
            
            ScoreManager.instance.addScore(2);
        }
        else
        {
            ScoreManager.instance.addScore(1);
        }
        
    }
    private void FixedUpdate()
    {
        if (playerstate == PlayerState.Playing)
        {
            if (Carpma)
            {
                rb.velocity = new Vector3(0, -FallingSpeed * Time.fixedDeltaTime * 7, 0);
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Carpma)
        {
            rb.velocity = new Vector3(0, JumpingSpeed * Time.deltaTime * 5, 0);

            if (collision.gameObject.tag != "Finish")
            {
                GameObject splash = Instantiate(splashEffect);
                splash.transform.SetParent(collision.transform);
                splash.transform.localEulerAngles = new Vector3(90, Random.Range(0, 359), 0);
                float randomScale = Random.Range(0.18f, 0.25f);
                splash.transform.localScale = new Vector3(randomScale, randomScale, 1);
                splash.transform.position = new Vector3(transform.position.x, transform.position.y - 0.22f,
                    transform.position.z);
                splash.GetComponent<SpriteRenderer>().color =
                    transform.GetChild(0).GetComponent<MeshRenderer>().material.color;

            }
        }

        else
        {
            if (invincible)
            {
                if (collision.gameObject.tag == "enemy" || collision.gameObject.tag == "plane")
                {
                    
                    collision.transform.parent.GetComponent<ObstacleController>().ShatterAllObstacles();
                    SoundManager.instance.playSoundFX(idestory, 0.5f);
                    currentObstacleNumber++;

                    shatterObstacles();
                }
            }
            else
            {
                if (collision.gameObject.tag == "enemy")
                {
                    
                    collision.transform.parent.GetComponent<ObstacleController>().ShatterAllObstacles();
                    SoundManager.instance.playSoundFX(destory, 0.5f);
                    currentObstacleNumber++;

                    shatterObstacles();
                }
            
                else if (collision.gameObject.tag == "plane")
                {
                    gameOverUI.SetActive(true);
                    playerstate = PlayerState.Died;
                    transform.GetChild(0).gameObject.SetActive(false);
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    SoundManager.instance.playSoundFX(death, 0.5f);
                    //ScoreManager.instance.ResetScore();


                }
                
            }
        }
        
        FindObjectOfType<GameUI>().LevelSliderFill(currentObstacleNumber /(float)totalObstacleNumber);
        
        
        if(collision.gameObject.tag=="Finish" && playerstate == PlayerState.Playing)
        {
            playerstate = PlayerState.Finish;
            SoundManager.instance.playSoundFX(this.win, 0.5f);
            finishUI.SetActive(true);
            finishUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level"+ PlayerPrefs.GetInt("Level",1);
            GameObject win = Instantiate(winEffect);
            win.transform.SetParent(Camera.main.transform);
            win.transform.localPosition = Vector3.up * 1.5f;
            win.transform.eulerAngles = Vector3.zero;
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!Carpma || collision.gameObject.tag == "Finish")
        {
            rb.velocity = new Vector3(0, JumpingSpeed * Time.deltaTime * 5, 0);
            SoundManager.instance.playSoundFX(bounce, 0.5f);
        }
    }
}
