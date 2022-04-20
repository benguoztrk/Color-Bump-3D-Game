using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody myRigidBody;

    private Vector3 lastMousePos;

    [SerializeField] private float sensitivity = .5f,
                                    clampDelta = 50f,
                                        bounds = 5f;

    [HideInInspector]
    public bool canMove , gameOver, finish;
    [HideInInspector]
    public int m_playerPrefScene;

    [SerializeField]
    private GameObject breakablePlayer;


    void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
        
    }



    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -bounds, bounds), transform.position.y, transform.position.z);
        if (canMove)
        {
            transform.position += FindObjectOfType<CameraMovement>().camVelocity;
        }
       

        if (!canMove && gameOver)
        {
            StartCoroutine(GameoverCoroutine());
            //GameManager.instance.GameOver();

            if (Input.GetMouseButtonDown(0))
            {

                GameManager.instance.RestartGame();
               // GameManager.instance.GameOverText("Tap to Restart");
                Time.timeScale = 1;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
            }
        }
        else if (!canMove && !finish)
        {
            if (Input.GetMouseButtonDown(0))
            { 
                FindObjectOfType<GameManager>().RemoveUI();
                canMove = true;
            }
            
        }
           
    }



    void FixedUpdate()
    {

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            
        }

        if (canMove)
        {
            if (Input.GetMouseButton(0)) //left mouse button or mobile device touch
            {
                Vector3 vector = lastMousePos - Input.mousePosition;
                lastMousePos = Input.mousePosition;
                vector = new Vector3(vector.x, 0, vector.y);

                Vector3 moveForce = Vector3.ClampMagnitude(vector, clampDelta);

                myRigidBody.AddForce(Vector3.forward * 2 + (-moveForce * sensitivity - myRigidBody.velocity), ForceMode.VelocityChange);


            }
        }

        myRigidBody.velocity.Normalize();
        
    }

    private void GameOver()
    {
        GameObject shatterSphere = Instantiate(breakablePlayer, transform.position, Quaternion.identity);

        foreach (Transform o in shatterSphere.transform)
        {
            o.GetComponent<Rigidbody>().AddForce(Vector3.forward * 4, ForceMode.Impulse);
            //Vector3.forward * myRigidBody.velocity.magnitude, ForceMode.Impulse
        }


        canMove = false;
        gameOver = true;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Time.timeScale = .3f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }


    IEnumerator NextLevel()
    {
        finish = true;
        canMove = false;
        m_playerPrefScene = PlayerPrefs.GetInt("Gameplay", 1);  
       
       PlayerPrefs.SetInt("Gameplay", m_playerPrefScene + 1);
      // print("current playerref scene: " + m_playerPrefScene); //Testing to see what is the current saved playerRef scene

       Scene scene = SceneManager.GetActiveScene();
       // print("curent active scene : "+ scene.name); /testing to see if the current active scene matches to saved playerRef scene

        string playerRefSceneName = string.Concat("Gameplay", m_playerPrefScene);
        //print(playerRefSceneName);


        if (playerRefSceneName != scene.name)  //if they are not matched delete all the key and values. Assign again
        {
            PlayerPrefs.DeleteAll();
            m_playerPrefScene = PlayerPrefs.GetInt("Gameplay", 1);
            PlayerPrefs.SetInt("Gameplay", m_playerPrefScene + 1);
        }


       yield return new WaitForSeconds(1);
       SceneManager.LoadScene("Gameplay" + PlayerPrefs.GetInt("Gameplay"));

        
    }

    IEnumerator GameoverCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.GameOver();
    }
    void OnCollisionEnter(Collision target)
    {
        if (target.gameObject.tag == "Enemy")
        {
            if(!gameOver)
                GameOver();
        }
    }


    void OnTriggerEnter(Collider target)
    {
        if (target.gameObject.name == "Finish")
        {
            StartCoroutine(NextLevel());
        }
    }

















}
