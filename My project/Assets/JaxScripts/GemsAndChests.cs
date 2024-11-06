using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemsAndChests : MonoBehaviour
{


    [SerializeField]
    Image gem1;
    [SerializeField]
    Image gem2;
    [SerializeField]
    Image gem3;
    [SerializeField]
    Image gem4;
    [SerializeField]
    Canvas win;
    [SerializeField]
    Canvas HUD;

    // Start is called before the first frame update
    void Start()
    {
        gem1.GetComponent<Image>().enabled = false;
        gem2.GetComponent<Image>().enabled = false;
        gem3.GetComponent<Image>().enabled = false;
        gem4.GetComponent<Image>().enabled = false;
        win.GetComponent<Canvas>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        Win(); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Chest1")
        {
            gem1.GetComponent<Image>().enabled = true;
        }
        if (collision.gameObject.tag == "Chest2")
        {
            gem2.GetComponent<Image>().enabled = true;
        }
        if (collision.gameObject.tag == "Chest3")
        {
            gem3.GetComponent<Image>().enabled = true;
        }
        if (collision.gameObject.tag == "Chest4")
        {
            gem4.GetComponent<Image>().enabled = true;
        }
    }
    public void Win()
    {
        if (gem1.GetComponent<Image>().enabled == true && gem2.GetComponent<Image>().enabled == true && gem3.GetComponent<Image>().enabled == true && gem4.GetComponent<Image>().enabled == true)
        {
            HUD.GetComponent<Canvas>().enabled = false;
           
            win.GetComponent<Canvas>().enabled = true;
            Time.timeScale = 0;
        }


    }
}
