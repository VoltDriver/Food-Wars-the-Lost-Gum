using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class BindCameraConfiner : MonoBehaviour
{
    private CinemachineConfiner confiner;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        confiner = GetComponent<CinemachineConfiner>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name);
        GameObject confinerObject = GameObject.Find("CameraConfiner");
        Debug.Log(confinerObject.name);
        // add/replace current confiner with the newly found one. 
        var collider = confinerObject.GetComponent<PolygonCollider2D>();
        confiner.m_BoundingShape2D = collider;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
