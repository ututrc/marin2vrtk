using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * An automatically scaling shaft for a teleport marker. Attach the prefab
 * that includes this script as a child object to the teleport marker.
 */
public class TeleportMarkerShaft : MonoBehaviour {

    private RaycastHit hit;
    private LineRenderer lr;
    private Renderer rend;

    public float width = 0.1f;
    public float lenghtThreshold = 0.2f;
    public GameObject lineEndMarker;
    public Material lineMaterial;

    void Awake()
    {
        if (GetComponent<LineRenderer>() == null)
        {
            gameObject.AddComponent<LineRenderer>();
        }
        lr = GetComponent<LineRenderer>();
        lr.material = lineMaterial;
        
        rend = GetComponent<Renderer>();
        lr.useWorldSpace = true;
        lr.SetWidth(width, width);
    }

	void Update () {
	    if (Physics.Raycast(transform.parent.position, Vector3.down, out hit))
        {
            if (hit.distance < lenghtThreshold)
            {
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, Vector3.zero);
                return;
            }

            // from: transform.parent.position, to: hit.point
            lr.SetPosition(0, transform.parent.position);
            lr.SetPosition(1, hit.point);

            rend.material.mainTextureScale = new Vector3(hit.distance / width, 1, 1);

            if (lineEndMarker != null)
            {
                lineEndMarker.transform.parent.position = hit.point;
                lineEndMarker.SetActive(true);
            }
        }
        else
        {
            // from: transform.parent.position, to: downwards "infinity" (1000 units)
            lr.SetPosition(0, transform.parent.position);
            lr.SetPosition(1, transform.parent.position + Vector3.down * 1000);

            rend.material.mainTextureScale = new Vector3(1000 / width, 1, 1);

            if (lineEndMarker != null)
            {
                lineEndMarker.SetActive(false);
            }
        }
	}
}
