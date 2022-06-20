using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscenes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    public void Prologue()  // load Level 1 after prologue is done
    {
        SceneManager.LoadScene(2);
    }

    public void Epilogue()  // load Main Menu after epilogue is done 
    {
        SceneManager.LoadScene(0);
    }
}
