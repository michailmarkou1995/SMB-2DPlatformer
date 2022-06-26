using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //orizmos taxititas kai simeas
    public float playerSpeed = 5.0f;
    private float Flag = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //dimiourgia function 
    public void PlayerMovement(float buttonFlag)
    {
        Flag = buttonFlag;
    }

    // Update is called once per frame
    void Update()
    {

        //elenxos simeas gia metakinisi embros kai piso
        if (Flag == 1)
        {
            transform.Translate(Vector2.right * playerSpeed * Time.deltaTime);
        }


        if(Flag == -1)
        {
            transform.Translate(Vector2.left * playerSpeed * Time.deltaTime);
        }
    }
}
