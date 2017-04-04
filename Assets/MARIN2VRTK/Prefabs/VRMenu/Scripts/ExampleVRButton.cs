using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

// A script that turns any object (at the moment "any" = any 3D-object or any
// rectangular GUI-component, i.e. a button) into an interactable object.
// The script contains functions to handle different events.
public class ExampleVRButton : MonoBehaviour
{
    private VRInteractiveItem m_InteractiveItem;
    private Renderer m_Renderer;

    private Image m_Image;
    private Color normalColor = new Color(1, 1, 1, 1);
    private Color normalOver = new Color(0.5f, 0.9f, 0.5f, 1);

    private void Awake()
    {
        m_InteractiveItem = gameObject.AddComponent<VRInteractiveItem>();

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

        m_Image = GetComponent<Image>();
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
        m_Image.color = normalOver;
    }


    //Handle the Out event
    private void HandleOut()
    {
        m_Image.color = normalColor;
    }


    //Handle the Click event
    private void HandleClick()
    {
        Debug.Log("Clicked the button! Great Job!");
        HandleOver();
    }


    //Handle the DoubleClick event
    private void HandleDoubleClick()
    {

    }

}
