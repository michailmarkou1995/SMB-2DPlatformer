using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] 
    private float time;
    [SerializeField]
    private Text timerText;
    // Start is called before the first frame update
    void Start()
    {
        timerText.text = string.Format("{0:000}", time);
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            timerText.text = string.Format("{0:000}", time);
        }
    }
}
