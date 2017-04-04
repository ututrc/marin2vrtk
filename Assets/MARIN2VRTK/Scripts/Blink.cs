using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * A virtual eye lid script for other scripts to use.
 */
public class Blink : MonoBehaviour {

    [Tooltip("How long the blink animation should take. Recommended values are between 0.1 and 0.4.")]
    public float blinkDurationInSeconds = 0.2f;

    public delegate void OnEyesClosed();
    protected OnEyesClosed onEyesClosed;

    public delegate void OnEyesClosedVector3(Vector3 target);
    protected OnEyesClosedVector3 onEyesClosedWithTarget;

    private GameObject blinkCanvas;
    private GameObject upperEyeLid;
    private GameObject lowerEyeLid;

    private Vector2 UPPER_CLOSED;
    private Vector2 LOWER_CLOSED;
    private Vector2 UPPER_OPEN;
    private Vector2 LOWER_OPEN;
    private float canvasHeight;

    void Awake()
    {
        if (Camera.main == null)
        {
            Debug.LogError("No 'Main Camera' found. Please make sure the VR camera is tagged as 'Main Camera'.");
            return;
        }

        GenerateCanvas();

        GenerateEyeLids();
    }

    private void GenerateCanvas()
    {
        blinkCanvas = new GameObject("BlinkCanvas");
        blinkCanvas.AddComponent<RectTransform>();
        Canvas bcc = blinkCanvas.AddComponent<Canvas>();
        bcc.renderMode = RenderMode.ScreenSpaceCamera;
        bcc.worldCamera = Camera.main;
        bcc.planeDistance = Camera.main.nearClipPlane + 0.01f; // 0.01f to make sure there isn't depth fighting
        blinkCanvas.AddComponent<CanvasScaler>();
        
        canvasHeight = blinkCanvas.GetComponent<RectTransform>().sizeDelta.y;
    }

    private void GenerateEyeLids()
    {
        upperEyeLid = new GameObject("UpperEyeLid");
        RectTransform uelrt = upperEyeLid.AddComponent<RectTransform>();
        uelrt.SetParent(blinkCanvas.GetComponent<RectTransform>());
        uelrt.localScale = Vector3.one;
        uelrt.localRotation = Quaternion.identity;
        uelrt.anchorMin = new Vector2(0, 1);
        uelrt.anchorMax = new Vector2(1, 1);
        uelrt.localPosition = Vector3.zero;
        uelrt.sizeDelta = new Vector2(1000, canvasHeight / 2.0f); // 1000 makes sure the "eyelids" are wide enough. No clue why 0 is not enough.
        Image ueli = upperEyeLid.AddComponent<Image>();
        ueli.color = Color.black;

        lowerEyeLid = new GameObject("LowerEyeLid");
        RectTransform lelrt = lowerEyeLid.AddComponent<RectTransform>();
        lelrt.SetParent(blinkCanvas.GetComponent<RectTransform>());
        lelrt.localScale = Vector3.one;
        lelrt.localRotation = Quaternion.identity;
        lelrt.anchorMin = Vector2.zero;
        lelrt.anchorMax = new Vector2(1, 0);
        lelrt.localPosition = Vector3.zero;
        lelrt.sizeDelta = new Vector2(1000, canvasHeight / 2.0f); // 1000 makes sure the "eyelids" are wide enough. No clue why 0 is not enough.
        Image leli = lowerEyeLid.AddComponent<Image>();
        leli.color = Color.black;


        UPPER_OPEN = new Vector2(0, canvasHeight / 4.0f);
        LOWER_OPEN = new Vector2(0, -canvasHeight / 4.0f);

        UPPER_CLOSED = new Vector2(0, -canvasHeight / 4.0f);
        LOWER_CLOSED = new Vector2(0, canvasHeight / 4.0f);

        upperEyeLid.GetComponent<RectTransform>().anchoredPosition = UPPER_OPEN;
        lowerEyeLid.GetComponent<RectTransform>().anchoredPosition = LOWER_OPEN;
    }

    public IEnumerator BlinkDo(OnEyesClosed callback)
    {
        float step = UPPER_OPEN.y - UPPER_CLOSED.y;
        float halfTime = blinkDurationInSeconds / 2f;

        // close eyes
        for (float i = 0; i < halfTime; i += Time.deltaTime)
        {
            upperEyeLid.GetComponent<RectTransform>().anchoredPosition = UPPER_OPEN - new Vector2(0, (i / halfTime) * step);
            lowerEyeLid.GetComponent<RectTransform>().anchoredPosition = LOWER_OPEN + new Vector2(0, (i / halfTime) * step);
            yield return null;
        }

        // do what needed to be done
        onEyesClosed = callback;
        onEyesClosed();

        // open eyes
        for (float i = 0; i < halfTime; i += Time.deltaTime)
        {
            upperEyeLid.GetComponent<RectTransform>().anchoredPosition = UPPER_CLOSED + new Vector2(0, (i / halfTime) * step);
            lowerEyeLid.GetComponent<RectTransform>().anchoredPosition = LOWER_CLOSED - new Vector2(0, (i / halfTime) * step);
            yield return null;
        }

        // ensure that eyes are not squinting
        upperEyeLid.GetComponent<RectTransform>().anchoredPosition = UPPER_OPEN;
        lowerEyeLid.GetComponent<RectTransform>().anchoredPosition = LOWER_OPEN;
        yield return null;
    }

    public IEnumerator BlinkDo(OnEyesClosedVector3 callback, Vector3 target)
    {
        float step = UPPER_OPEN.y - UPPER_CLOSED.y;
        float halfTime = blinkDurationInSeconds / 2f;

        // close eyes
        for (float i = 0; i < halfTime; i += Time.deltaTime)
        {
            upperEyeLid.GetComponent<RectTransform>().anchoredPosition = UPPER_OPEN - new Vector2(0, (i / halfTime) * step);
            lowerEyeLid.GetComponent<RectTransform>().anchoredPosition = LOWER_OPEN + new Vector2(0, (i / halfTime) * step);
            yield return null;
        }

        // do what needed to be done
        onEyesClosedWithTarget = callback;
        onEyesClosedWithTarget(target);

        // open eyes
        for (float i = 0; i < halfTime; i += Time.deltaTime)
        {
            upperEyeLid.GetComponent<RectTransform>().anchoredPosition = UPPER_CLOSED + new Vector2(0, (i / halfTime) * step);
            lowerEyeLid.GetComponent<RectTransform>().anchoredPosition = LOWER_CLOSED - new Vector2(0, (i / halfTime) * step);
            yield return null;
        }

        // ensure that eyes are not squinting
        upperEyeLid.GetComponent<RectTransform>().anchoredPosition = UPPER_OPEN;
        lowerEyeLid.GetComponent<RectTransform>().anchoredPosition = LOWER_OPEN;
        yield return null;
    }
}
