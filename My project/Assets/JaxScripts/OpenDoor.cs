using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private bool playerDectected;
    public Transform doorPos;
    public float width;
    public float height;
    public LayerMask whatIsPlayer;

     SceneSwitch sceneSwitch;

    [SerializeField]
    private string SceneName;

    private void Start() 
    { 
        sceneSwitch = FindObjectOfType<SceneSwitch>();
    }


    private void Update()
    {
        playerDectected = Physics2D.OverlapBox(doorPos.position, new Vector2(width, height), 0, whatIsPlayer);
        if(playerDectected == true ) 
        { 
         if(Input.GetKey(KeyCode.E))
            {
                sceneSwitch.SwitchScene(SceneName);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(doorPos.position, new Vector3(width, height, 1));
    }
}
