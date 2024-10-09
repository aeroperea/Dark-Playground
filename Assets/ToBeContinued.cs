using UnityEngine;
using System.Collections;

public class ToBeContinued : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitAndQuit());
    }
    
    IEnumerator WaitAndQuit()
    {
        yield return new WaitForSeconds(5);
        Application.Quit();
    }
}
