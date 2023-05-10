using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public GameObject[] birds;
    public Transform obj;
    public const int NUM_BIRDS = 300; //Number of birds spawned. Affects behavior
    public Vector3 avgPos;
    public Vector3 avgOrientation;
    public
    void Start()
    {
        obj = this.gameObject.GetComponent<Transform>();
        GameObject bird = obj.transform.GetChild(0).gameObject;
        birds = new GameObject[NUM_BIRDS];
        birds[0] = bird;
        for (int i = 1; i < NUM_BIRDS; i++) {
            birds[i] = Instantiate(bird, obj);
            
            birds[i].transform.position = new Vector3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-20f, 20f),0 );
            birds[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(0,0,UnityEngine.Random.Range(-180f, 180f)));
        }
    }

    // Update is called once per frame
    void Update()
    {

        
        for (int i = 0; i < NUM_BIRDS; i++) {
            avgPos += birds[i].transform.position;
            avgOrientation += birds[i].transform.eulerAngles;
            
        }
        avgPos /= NUM_BIRDS + 1;
        avgPos.z /= 10;
        avgOrientation /= NUM_BIRDS + 1;

        Debug.DrawLine(new Vector3(), avgPos, Color.blue);


    }

    public void OnBlinkCalled(GameObject caller) {

    }

}
