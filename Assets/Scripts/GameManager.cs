using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Text CurrentLevelText, NextLevelText;
    private Image fill;
    private int level;
    private float startDistance, currentDistance;
    private GameObject player, finish;
    public GameObject swipeHand;

    private TextMesh startText;

    public static GameManager instance;
    [SerializeField] private Canvas gameOverCanvas;

    [SerializeField] private Text gameOverText;




    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }

        CurrentLevelText = GameObject.Find("CurrentLevelText").GetComponent<Text>();
        NextLevelText = GameObject.Find("NextLevelText").GetComponent<Text>();

        fill = GameObject.Find("Fill").GetComponent<Image>();

        player = GameObject.Find("Player");
        finish = GameObject.Find("Finish");
        swipeHand = GameObject.Find("Swipe");

        startText = GameObject.Find("StartText").GetComponent<TextMesh>();



       

    }

    void Start()
    {

        Scene scene = SceneManager.GetActiveScene();
       // print(scene.buildIndex); // checking to see which value will return. It returns 0 
        int currentlevel = scene.buildIndex + 1;

        startText.text = "Level " + currentlevel;

        NextLevelText.text =  currentlevel + 1 + "";
        CurrentLevelText.text = currentlevel.ToString();

        startDistance = Vector3.Distance(player.transform.position, finish.transform.position);
       


    }

    void Update()
    {

        currentDistance = Vector3.Distance(player.transform.position, finish.transform.position);
        if(player.transform.position.z < finish.transform.position.z)
            fill.fillAmount = 1 - (currentDistance / startDistance);
    }


    public void RemoveUI()
    {
        swipeHand.SetActive(false);
    }

    public void GameOver()
    {
        gameOverCanvas.enabled = true;
    }
    public void GameOverText(string gameOverInfo)
    {
        gameOverText.text = gameOverInfo;
       
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }








}
