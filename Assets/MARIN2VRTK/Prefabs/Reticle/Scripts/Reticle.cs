using UnityEngine;
using UnityEngine.UI;

// From the Unity VR Samples project, released with Apache License version 2.0.
// https://www.assetstore.unity3d.com/en/#!/content/51519

// The reticle is a small point at the centre of the screen.
// It is used as a visual aid for aiming. The position of the
// reticle is either at a default position in space or on the
// surface of a VRInteractiveItem as determined by the VREyeRaycaster.
public class Reticle : MonoBehaviour
{
    [SerializeField] private float m_DefaultDistance = 5f;      // The default distance away from the camera the reticle is placed.
    private MeshRenderer m_ReticleRenderer;                     // Reference to the image component that represents the reticle.
    private Transform m_Camera;                // The reticle is always placed relative to the camera

    private Vector3 m_OriginalScale;                            // Since the scale of the reticle changes, the original scale needs to be stored.
    private Quaternion m_OriginalRotation;                      // Used to store the original rotation of the reticle.

    void Update()
    {
        // Make sure the object is looking at the camera
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }

    private void Awake()
    {
        m_Camera = Camera.main.transform;

        // Store the original scale and rotation.
        m_OriginalScale = transform.localScale;
        m_OriginalRotation = transform.localRotation;
    }

    public void Hide()
    {
        m_ReticleRenderer.enabled = false;
    }

    public void Show()
    {
        m_ReticleRenderer.enabled = true;
    }

    // This overload of SetPosition is used when the the VREyeRaycaster hasn't hit anything.
    public void SetPosition ()
    {
        // Set the position of the reticle to the default distance in front of the camera.
        transform.position = m_Camera.position + m_Camera.forward * m_DefaultDistance;

        // Set the scale based on the original and the distance from the camera.
        transform.localScale = m_OriginalScale * m_DefaultDistance;

        // The rotation should always be aligned towards the camera
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }

    // This overload of SetPosition is used when the VREyeRaycaster has hit something.
    public void SetPosition (RaycastHit hit)
    {
        transform.position = hit.point;
        transform.localScale = m_OriginalScale * hit.distance;
        
        // The rotation should always be aligned towards the camera
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}