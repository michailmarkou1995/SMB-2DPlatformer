using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rightMovementButton : MonoBehaviour
{

    public Player playerClass;
    public Button rightButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //patima kai kratima koumpiou?
        if (Input.GetButtonDown("rightButton"))
        {
            playerClass.PlayerMovement(1);
        }
        


    }
    private void Test()
    {
        playerClass.PlayerMovement(1);
    }
}
