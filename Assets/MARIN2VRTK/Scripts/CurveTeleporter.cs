using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRStandardAssets.Utils;

public class CurveTeleporter : MonoBehaviour
{

    // external
    [Tooltip("Which button is used for teleporting")]
    public KeyCode teleportButton;
    [Tooltip("How steep the angle of the curve should be")]
    [Range(0.0f, 0.5f)]
    public float curveAngle = 0.1f;
    [Tooltip("How far away the points of the curve are (smaller values give more accurate curves)")]
    [Range(0.2f, 2.0f)]
    public float curveNodeDistance = 1f;
    [Tooltip("What is the maximum lenght of the curve")]
    [Range(.1f, 100.0f)]
    public float curveMaxLenght = 15f;
    [Tooltip("Prefab that is repeated to create a visible curve")]
    public GameObject curveGraphicTemplate;
    [Tooltip("Prefab that is used to indicate where the raycast hit (if it hit)")]
    public GameObject hitGraphicTemplate;
    public bool blinking = true;
    public GameObject cameraRig;
    public bool teleportingEnabled = true;

    // internal
    private RaycastHit hitInfo;
    private int levels;
    private List<GameObject> nodeObjects;
    private GameObject raycastCurve;
    private GameObject hitGraphic;
    private Blink blinkScript;

    // Use this for SteamVR controllers
    /*
    private Valve.VR.EVRButtonId touchButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private SteamVR_Controller.Device controller
    {
        get
        {
            return SteamVR_Controller.Input((int)trackeObj.index);
        }
    }
    private SteamVR_TrackedObject trackeObj;
    
    if (touchButton.GetDown())
    */

#if UNITY_EDITOR
    private float previousCurveAngle;
    private float previousCurveNodeLenght;
    private float previousCurveMaxLenght;
#endif

    void Update ()
    {
        if (teleportingEnabled)
        {
            if (Input.GetKey(teleportButton))
            {
                if (CurvedRaycast(transform.position, transform.forward, out hitInfo))
                {
                    hitGraphic.SetActive(true);
                    hitGraphic.transform.position = hitInfo.point;
                    if (hitInfo.collider.gameObject.tag == "TeleportArea")
                    {
                        ColorCurve(Color.green);
                    }
                    else
                    {
                        ColorCurve(Color.red);
                    }
                }
                else
                {
                    hitGraphic.SetActive(false);
                    ColorCurve(Color.red);
                }
            }
            else if (Input.GetKeyUp(teleportButton))
            {
                if (hitInfo.collider != null && hitInfo.collider.gameObject.tag == "TeleportArea")
                {
                    if (blinking) StartCoroutine(blinkScript.BlinkDo(MoveTo, hitInfo.point));
                    else MoveTo(hitInfo.point);
                }
                hitGraphic.SetActive(false);
                ClearNodes();
            }
        }        
	}

    private void ColorCurve(Color c)
    {
        hitGraphic.GetComponent<TeleportMarker>().SetColor(c);
        nodeObjects.ForEach(t => t.GetComponent<CurveNode>().SetColor(c));
    }

    private void MoveTo(Vector3 target)
    {
        cameraRig.transform.position = GetCameraRigPosition(target);
    }

    private Vector3 GetCameraRigPosition(Vector3 playerFeetPosition)
    {
        float x = playerFeetPosition.x - Camera.main.transform.localPosition.x;
        float z = playerFeetPosition.z - Camera.main.transform.localPosition.z;
        return new Vector3( x, playerFeetPosition.y, z);
    }

    void Awake()
    {
        if (blinking)
        {
            // instantiate blinking
            blinkScript = GameObject.FindObjectOfType<Blink>();
            if (blinkScript == null)
            {
                blinkScript = this.gameObject.AddComponent<Blink>();
            }
        }

        // Instantiate teleport marker
        hitGraphic = (GameObject)GameObject.Instantiate(hitGraphicTemplate);
        hitGraphic.name = "TeleportMarker";

        // Instantiate nodes
        nodeObjects = new List<GameObject>();
        raycastCurve = new GameObject("RaycastCurve");
        int maxNumberOfNodes = (int)Mathf.Ceil(curveMaxLenght / curveNodeDistance);
        for (int i = 1; i <= maxNumberOfNodes; i++)
        {
            GameObject newNode = (GameObject)GameObject.Instantiate(curveGraphicTemplate);
            newNode.name = "Node" + i.ToString();
            newNode.transform.SetParent(raycastCurve.transform);
            nodeObjects.Add(newNode);
        }
    }

    public bool CurvedRaycast(Vector3 from, Vector3 direction, out RaycastHit hitInfo)
    {
#if UNITY_EDITOR
        CheckForEditorChanges();
#endif
        Ray ray = new Ray(from, direction);
        if (Physics.Raycast(ray, out hitInfo, curveNodeDistance + 0.1f)) // + 0.1f to account for floating point precision
        {
            // We hit something! Unroll the recursion.
            for (int i = levels; i < nodeObjects.Count; i++)
            {
                nodeObjects[i].SetActive(false);
            }
            levels = 0; // TODO: this should probably be stored inside the function instead of a global variable
            return true;
        }
        else
        {
            nodeObjects[levels].SetActive(true); // TODO: should these be here since this is just a raycast method?
            nodeObjects[levels].transform.position = from + Vector3.Normalize(direction) * curveNodeDistance; // TODO: should these be here since this is just a raycast method?

            if ((levels + 1) * curveNodeDistance >= curveMaxLenght)
            {
                // Didn't hit anything, and reached the lenght limit of the curve. Unroll the recursion.
                levels = 0;
                return false;
            }
            else
            {
                // Didn't hit anything, WE HAVE TO GO DEEPER.
                levels += 1;
                Vector3 newDirection = direction + Vector3.down * curveAngle * curveNodeDistance; // TODO: these should probably be passed as function arguments instead of global variables
                return CurvedRaycast(from + Vector3.Normalize(direction) * curveNodeDistance, newDirection, out hitInfo);
            }
        }
    }

    private void ClearNodes()
    {
        foreach (GameObject nodeObject in nodeObjects)
        {
            nodeObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    // This is to make sure that the values can be tweaked in the editor while playing
    private void CheckForEditorChanges()
    {
        if (blinking)
        {
            // instantiate blinking
            blinkScript = GameObject.FindObjectOfType<Blink>();
            if (blinkScript == null)
            {
                blinkScript = this.gameObject.AddComponent<Blink>();
            }
        }

        if (previousCurveAngle != curveAngle || previousCurveMaxLenght != curveMaxLenght || previousCurveNodeLenght != curveNodeDistance)
        {
            Debug.Log("CurveTeleporter: Values changed, reinstantiated variables");

            levels = 0;

            foreach (GameObject node in nodeObjects)
            {
                GameObject.Destroy(node);
            }
            nodeObjects.Clear();
            int maxNumberOfNodes = (int)Mathf.Ceil(curveMaxLenght / curveNodeDistance);
            for (int i = 1; i <= maxNumberOfNodes; i++)
            {
                GameObject newNode = (GameObject)GameObject.Instantiate(curveGraphicTemplate);
                newNode.name = "Node" + i.ToString();
                newNode.transform.SetParent(raycastCurve.transform);
                nodeObjects.Add(newNode);
            }
        }

        previousCurveAngle = curveAngle;
        previousCurveMaxLenght = curveMaxLenght;
        previousCurveNodeLenght = curveNodeDistance;
    }
#endif
}
