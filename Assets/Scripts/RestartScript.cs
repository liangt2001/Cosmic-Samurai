using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScript : MonoBehaviour
{


    Transform playertransform;

    public void RestartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void Start()
    {
        playertransform = GameObject.Find("Player").transform;
    }

    private void FixedUpdate()
    {
        if (playertransform.position.y < -10) RestartGame();
    }

}
