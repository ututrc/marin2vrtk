using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

// From the Unity VR Samples project, released with Apache License version 2.0.
// https://www.assetstore.unity3d.com/en/#!/content/51519

// A script that turns any object (at the moment "any" = any 3D-object or any
// rectangular GUI-component, i.e. a button) into an interactable object.
// The script contains functions to handle different events.
[RequireComponent (typeof (VRInteractiveItem))]
public class VRButton : MonoBehaviour
{
    private VRInteractiveItem m_InteractiveItem;
    private Renderer m_Renderer;

    private void Awake ()
    {
        m_InteractiveItem = GetComponent<VRInteractiveItem>();
        if (m_InteractiveItem == null) gameObject.AddComponent<VRInteractiveItem>();

        if (gameObject.GetComponent<RectTransform>() != null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = GetComponent<RectTransform>().sizeDelta;
        }
        else if (gameObject.GetComponent<MeshFilter>() != null)
        {
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = GetComponent<MeshFilter>().mesh;
        }
    }


    private void OnEnable()
    {
        m_InteractiveItem.OnOver += HandleOver;
        m_InteractiveItem.OnOut += HandleOut;
        m_InteractiveItem.OnClick += HandleClick;
        m_InteractiveItem.OnDoubleClick += HandleDoubleClick;
    }


    private void OnDisable()
    {
        m_InteractiveItem.OnOver -= HandleOver;
        m_InteractiveItem.OnOut -= HandleOut;
        m_InteractiveItem.OnClick -= HandleClick;
        m_InteractiveItem.OnDoubleClick -= HandleDoubleClick;
    }


    //Handle the Over event
    private void HandleOver()
    {
        Debug.Log("Show over state");
        
    }


    //Handle the Out event
    private void HandleOut()
    {
        Debug.Log("Show out state");

    }


    //Handle the Click event
    private void HandleClick()
    {
        Debug.Log("Show click state");
    }


    //Handle the DoubleClick event
    private void HandleDoubleClick()
    {
        Debug.Log("Show double click");
    }
}
