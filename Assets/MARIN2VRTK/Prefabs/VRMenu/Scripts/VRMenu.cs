using UnityEngine;
using System.Collections.Generic;

public class VRMenu : MonoBehaviour {

    private bool isVisible = false;
    public GameObject menuPanel;

    public List<KeyCode> togglingButtons;

    void Update()
    {
        foreach (KeyCode kc in togglingButtons)
        {
            if (Input.GetKeyDown(kc))
            {
                Toggle();
            }
        }

        if (isVisible && Vector3.Distance(transform.position, Camera.main.transform.position) > 5f)
        {
            Hide();
        }
    }

    public void Toggle()
    {
        if (isVisible) Hide();
        else Show();
    }

    public void Show()
    {
        menuPanel.SetActive(true);
        PlaceInFront(Camera.main);
        isVisible = true;
    }

    public void Hide()
    {
        menuPanel.SetActive(false);
        isVisible = false;
    }

    public void PlaceInFront(Camera camera)
    {
        transform.position = camera.transform.position + camera.transform.forward * 3f;
        transform.LookAt(camera.transform.position + camera.transform.forward * 6f, Vector3.up);
    }
}
