using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class GamepadVRMovement : MonoBehaviour
{
    // internal
    private bool rotating;
    private bool immediateTurn = true;
    private float rotationCounter;
    private bool readyToMove = true;
    private bool incrementalTurning = true;
    private Blink blinkScript;
    private bool turboMode = false;
    private GameObject currentTarget;
    private RaycastHit hit;
    private GameObject teleportGroundMarker;
    private GameObject teleportAirMarker;
    private GameObject player;

    // external
    [Tooltip("Should virtual eye lids be used in teleport transitions and turning?")]
    public bool blinking = true;
    [Tooltip("Is joystick movement allowed in the first place?")]
    public bool allowJoystickMovement = true;
    [Tooltip("Is flying joystick movement allowed?")]
    public bool allowJoystickFlying = true;
    [Tooltip("Flying speed")]
    public float freeMoveSpeed = 3f;
    [Tooltip("Walking speed.")]
    public float crawlMoveSpeed = 5f;
    [Tooltip("Should teleportation to ground be limited by distance?")]
    public bool limitedRangeGroundTeleport = false;
    [Tooltip("How far the user can teleport in one jump?")]
    public float teleportRange = 15f;
    [Tooltip("How much a joystick must be pushed vertically to initiate a turn.")]
    [Range(0f, 1f)]
    public float turnSensitivity = 0.5f;
    [Tooltip("How much to turn per each turn.")]
    public float turnAmountInDegrees = 45f;
    [Tooltip("If joystick is held, how often the player should turn.")]
    public float rotationFrequencyInSeconds = 1;
    [Tooltip("The image that is toggled to blur peripheral vision.")]
    public GameObject blindImage;
    [Tooltip("The prefab of the teleportation marker that is shown on ground (initialized at start).")]
    public GameObject teleportGroundMarkerTemplate;
    [Tooltip("The prefab of the teleportation marker that is shown in the air ground (initialized at start).")]
    public GameObject teleportAirMarkerTemplate;
    [Tooltip("The gaze pointer (should be already present in the scene).")]
    public GameObject gazePointer;

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

        teleportGroundMarker = GameObject.Instantiate(teleportGroundMarkerTemplate);
        teleportAirMarker = GameObject.Instantiate(teleportAirMarkerTemplate);

        player = this.gameObject;
        player.GetComponent<CharacterController>().detectCollisions = false;
    }



    void Update()
    {
        bool blindsOn = false;
        if (Input.GetAxis("LT") > 0.1f || Input.GetAxis("RT") > 0.1f)
        {
            turboMode = true;
        }
        else
        {
            turboMode = false;
        }

        if (turboMode || !incrementalTurning)
        {
            if (Mathf.Abs(Input.GetAxis("RightHorizontal")) > 0f)
            {
                if (turboMode)
                {
                    blindsOn = true;
                    player.transform.localEulerAngles += Input.GetAxis("RightHorizontal") * Vector3.up;
                }
                else
                {
                    player.transform.localEulerAngles += Input.GetAxis("RightHorizontal") * Vector3.up * 0.5f;
                }
            }
            else
            {
                rotating = false;
                rotationCounter = 0;
                immediateTurn = true;
            }
        }
        else
        {
            if (Mathf.Abs(Input.GetAxis("RightHorizontal")) > turnSensitivity)
            {
                rotating = true;
            }
            else
            {
                rotating = false;
                rotationCounter = 0;
                immediateTurn = true;
            }
        }

        if (rotating)
        {
            if (immediateTurn)
            {
                if (Input.GetAxis("RightHorizontal") > turnSensitivity) StartCoroutine(blinkScript.BlinkDo(TurnRight));
                else StartCoroutine(blinkScript.BlinkDo(TurnLeft));

                immediateTurn = false;
            }

            rotationCounter += Time.deltaTime;
            if (rotationCounter > rotationFrequencyInSeconds)
            {
                if (Input.GetAxis("RightHorizontal") > turnSensitivity) StartCoroutine(blinkScript.BlinkDo(TurnRight));
                else StartCoroutine(blinkScript.BlinkDo(TurnLeft));

                rotationCounter = 0;
            }
        }
        else
        {
            immediateTurn = true;
        }

        /*
         * 
         * TELEPORT BUTTON HELD DOWN
         * 
         */
        if (Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.JoystickButton5) || Input.GetKey(KeyCode.Space))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                currentTarget = hit.collider.gameObject;
            }
            else
            {
                currentTarget = null;
            }

            gazePointer.GetComponent<Renderer>().enabled = false;
            if (allowJoystickFlying)
            {

                if (currentTarget != null)
                {
                    if (limitedRangeGroundTeleport && Vector3.Distance(hit.point, Camera.main.transform.position) > teleportRange)
                    {
                        teleportGroundMarker.SetActive(false);
                        teleportAirMarker.SetActive(true);
                        teleportAirMarker.transform.position = Camera.main.transform.position + Camera.main.transform.forward * teleportRange;
                    }
                    else
                    {
                        teleportGroundMarker.SetActive(true);
                        teleportAirMarker.SetActive(false);
                        teleportGroundMarker.transform.position = hit.point;
                    }
                }
                else
                {
                    teleportGroundMarker.SetActive(false);
                    teleportAirMarker.SetActive(true);
                    teleportAirMarker.transform.position = Camera.main.transform.position + Camera.main.transform.forward * teleportRange;
                }
            }
            else
            {
                teleportAirMarker.SetActive(false);
                if (currentTarget != null)
                {
                    teleportGroundMarker.SetActive(true);
                    teleportGroundMarker.transform.position = hit.point;
                }
                else
                {
                    teleportGroundMarker.SetActive(false);
                }
            }
        }
        /*
        * 
        * TELEPORT BUTTON RELEASED
        * 
        */
        else if (Input.GetKeyUp(KeyCode.JoystickButton4) || Input.GetKeyUp(KeyCode.JoystickButton5) || Input.GetKeyUp(KeyCode.Space))
        {
            gazePointer.GetComponent<Renderer>().enabled = true;
            if (teleportGroundMarker.activeSelf)
            {
                if (readyToMove)
                {
                    StartCoroutine(blinkScript.BlinkDo(MoveTo, teleportGroundMarker.transform.position + Vector3.up));
                }
            }
            else if (teleportAirMarker.activeSelf)
            {
                if (readyToMove)
                {
                    StartCoroutine(blinkScript.BlinkDo(MoveTo, teleportAirMarker.transform.position + Vector3.up));
                }
            }

        }
        else
        {
            teleportAirMarker.SetActive(false);
            teleportGroundMarker.SetActive(false);
        }

        /*
         * 
         * JOYSTICK MOVEMENT
         * 
         */
        if (allowJoystickMovement)
        {
            float moveSpeed = 0.7f;
            if (turboMode) moveSpeed = 8f;
            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0f)
            {

                if (allowJoystickFlying)
                {
                    if (turboMode) blindsOn = true;
                    player.GetComponent<CharacterController>().Move(Camera.main.transform.forward * 0.01f * Input.GetAxis("Vertical") * freeMoveSpeed * moveSpeed);
                }
                else
                {
                    player.GetComponent<CharacterController>().SimpleMove(Camera.main.transform.forward * Input.GetAxis("Vertical") * crawlMoveSpeed * moveSpeed);
                }
            }
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0f)
            {
                if (allowJoystickFlying)
                {
                    if (turboMode) blindsOn = true;
                    player.GetComponent<CharacterController>().Move(Camera.main.transform.right * 0.01f * Input.GetAxis("Horizontal") * freeMoveSpeed * moveSpeed);
                }
                else
                {
                    player.GetComponent<CharacterController>().SimpleMove(Camera.main.transform.right * Input.GetAxis("Horizontal") * crawlMoveSpeed * moveSpeed);
                }
            }
            if (Mathf.Abs(Input.GetAxis("RightVertical")) > 0f)
            {
                if (allowJoystickFlying)
                {
                    if (turboMode) blindsOn = true;
                    player.GetComponent<CharacterController>().Move(-Vector3.up * 0.005f * Input.GetAxis("RightVertical") * freeMoveSpeed * moveSpeed);
                }
            }
        }

        if (blindsOn)
        {
            blindImage.SetActive(true);
        }
        else
        {
            blindImage.SetActive(false);
        }
    }

    private void MoveTo(Vector3 location)
    {
        // move
        player.transform.localPosition = location;
        teleportGroundMarker.SetActive(false);
        teleportAirMarker.SetActive(false);
    }

    private void TurnRight()
    {
        player.transform.localEulerAngles += new Vector3(0, turnAmountInDegrees, 0);
    }

    private void TurnLeft()
    {
        player.transform.localEulerAngles -= new Vector3(0, turnAmountInDegrees, 0);
    }

    public void SetSmoothTurning(bool state)
    {
        if (state)
        {
            incrementalTurning = false;
        }
        else
        {
            incrementalTurning = true;
        }
    }

    public void AllowFreeMove(bool allow)
    {
        allowJoystickFlying = allow;
        if (allow)
        {
            player.layer = 8;
        }
        else
        {

            player.layer = 2;
        }
    }

    public void MoveToModel()
    {
        if (GameObject.Find("Models").transform.childCount > 0)
        {
            Vector3 target = GetModelCenter(GameObject.Find("Models"));
            if (!float.IsNaN(target.x))
            {
                Vector3 modelCenter = GetModelCenter(GameObject.Find("Models"));
                StartCoroutine(blinkScript.BlinkDo(MoveTo, modelCenter));
            }
        }
    }

    private Vector3 GetModelCenter(GameObject go)
    {
        Vector3 sum = Vector3.zero;
        MeshRenderer[] modelPieces = go.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mr in modelPieces)
        {
            sum += mr.bounds.center;
        }

        Vector3 average = new Vector3(sum.x / (float)modelPieces.Length, sum.y / (float)modelPieces.Length, sum.z / (float)modelPieces.Length);

        return average;
    }
}
