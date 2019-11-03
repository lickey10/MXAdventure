using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeController : MonoBehaviour
{
    //var force : Vector3;
    Transform car;
    int power;
    int MaxSpeed = 150;
    float currentSpeed;
    //private var trigfunction : Vector3;
    Camera ccamera;
    //private var max : Vector3;
    Transform fwheel;
    Transform bwheel;
    WheelCollider fWheelCollider;
    WheelCollider bWheelCollider;
    int[] gearRatio;

    private float distance;
    private float startTime = 0;
    private Vector3 point;
    private GameObject objectObj;
    private float duration;
    private float beginningBikeSoundPitch;
    //var raycast : Transform;
    //var isgrounded : boolean;
    Vector3 CenterOfMass;
    private Vector3 trickStartPosition;
    private bool crashed = false;
    private bool buttonPressed = false;

    public GUIStyle customGuiStyle;

    // Start is called before the first frame update
    void Start()
    {
        //max.Set(8,8,0);
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
        crashed = false;

        //ccamera.transparencySortMode = TransparencySortMode.Perspective;
        beginningBikeSoundPitch = GetComponent<AudioSource>().pitch;

        ApplyBrake();
    }

    // Update is called once per frame
    void Update()
    {
        EngineSound();

        if (Input.GetMouseButtonDown(0) || buttonPressed)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            buttonPressed = true;

            if (Physics.Raycast(ray,out hit))
            {
                startTime = Time.time; point = hit.point;
                objectObj = hit.transform.gameObject;
                string name;
                name = objectObj.name;

                if (name == "greenGrip")
                    ApplyGas();
                else if (name == "brakeRotor")
                    ApplyBrake();
            }
        }
        else if (Input.GetMouseButtonUp(0) || !buttonPressed)
        {
            buttonPressed = false;
            duration = Time.time - startTime;

            if (currentSpeed > 0)
                ApplyGas(-20);
        }
    }

    void FixedUpdate()
    {
        //rotate wheel image
        fwheel.Rotate(0, 0, fWheelCollider.rpm / 60 * 360 * Time.deltaTime * -1);
        bwheel.Rotate(0, 0, bWheelCollider.rpm / 60 * 360 * Time.deltaTime * -1);

        currentSpeed = 2 * 22 / 7 * bWheelCollider.radius * bWheelCollider.rpm * 60 / 1000;
        currentSpeed = Mathf.Round(currentSpeed);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "MeshPiece")
        {
            if (!crashed)
                gamestate.Instance.SetLives(gamestate.Instance.getLives() - 1);

            if (gamestate.Instance.getLives() <= 0)
            {
                Application.LoadLevel("gameover");
            }
            else
            {
                crashed = true;
            }
        }
        else if (col.name == "Coin")
        {
            if (!crashed)
                gamestate.Instance.SetScore(gamestate.Instance.GetScore() + 1);
        }
    }

    void OnGUI()
    {
        if (crashed)
        {
            GUI.depth = 10;

            GUI.Box(new Rect((Screen.width - 150) / 2, (Screen.height - 75) / 2, 150, 75), "", new GUIStyle(GUI.skin.box));

            //			var logoX : int = (Screen.width - 300 ) / 2;
            //			var logoY : int = (Screen.height - 450) / 2;

            //			customGuiStyle.font = (Font)Resources.Load("Fonts/advlit");
            customGuiStyle = new GUIStyle(GUI.skin.box);
            customGuiStyle.fontSize = 30;
            customGuiStyle.normal.textColor = Color.white;
            customGuiStyle.alignment = TextAnchor.MiddleCenter;
            customGuiStyle.normal.background = null;

            GUI.skin = null;

            if (GUI.Button(new Rect((Screen.width - 150) / 2, (Screen.height - 75) / 2, 150, 75), "Try Again", customGuiStyle))
            {
                Application.LoadLevel(Application.loadedLevel);
            }

            GUI.depth = 0;
        }
    }

    void ApplyGas()
    {
        ApplyGas(power);
    }

    void ApplyGas(int torque)
    {
        if (IsGrounded())
        {
            if (currentSpeed < MaxSpeed && !crashed)
            {
                bWheelCollider.brakeTorque = 0;

                if (bWheelCollider.motorTorque > 0 || torque > 0)
                    bWheelCollider.motorTorque += torque;
            }
            else
                bWheelCollider.motorTorque = 0;

            if (trickStartPosition != new Vector3(0, 0, 0))
                checkForTrick(car.transform.position);
        }
        else//rotate since they are in the air
        {
            if (trickStartPosition == new Vector3(0, 0, 0))
                trickStartPosition = car.transform.position;

            //bwheel.Rotate(0,0,bWheelCollider.rpm/60*360*Time.deltaTime * -1);
            car.transform.Rotate(0, 0, 20 * Time.deltaTime);
        }
    }

    void ApplyBrake()
    {
        if (IsGrounded())
        {
            if (!crashed)
            {
                bWheelCollider.brakeTorque += 30;
                bWheelCollider.motorTorque = 0;
            }
        }
        else//rotate since they are in the air
        {
            car.transform.Rotate(0, 0, 20 * Time.deltaTime * -1);
        }
    }

    bool IsGrounded()
    {
        bool grounded;

        //check if back wheel is on the ground
        grounded = Physics.Raycast(bWheelCollider.transform.position, -Vector3.up, (bWheelCollider.radius + bWheelCollider.suspensionDistance + 0.1f) * 6);

        if (grounded)//check if front wheel is on the ground
            grounded = Physics.Raycast(fWheelCollider.transform.position, -Vector3.up, (fWheelCollider.radius + fWheelCollider.suspensionDistance + 0.1f) * 6);

        return grounded;
    }

    void LateUpdate()
    {
        //After we move, adjust the camera to follow the player
        ccamera.transform.position = new Vector3(transform.position.x, transform.position.y, ccamera.transform.position.z);
    }

    private void EngineSound()
    {
        int i = 0;

        for (i = 0; i < gearRatio.Length; i++)
        {
            if (gearRatio[i] > currentSpeed)
            {
                break;
            }
        }

        float gearMinValue = 0.00f;
        float gearMaxValue = 0.00f;

        if (i == 0)
        {
            gearMinValue = 0;
        }
        else
        {
            gearMinValue = gearRatio[i - 1];
        }

        if (i > gearRatio.Length - 1)
            i = gearRatio.Length - 1;

        gearMaxValue = gearRatio[i];
        float enginePitch = ((currentSpeed - gearMinValue) / (gearMaxValue - gearMinValue)) + 1;

        if (enginePitch >= beginningBikeSoundPitch)//keep it from going lower than starting value
            GetComponent<AudioSource>().pitch = enginePitch;
    }

    //check the position of the vehicle to see if he did a trick while in the air
    private void checkForTrick(Vector3 trickEndPosition)
    {
        float xDifference = 0;

        xDifference = trickEndPosition.x - trickStartPosition.x;

        if (xDifference < -180 || xDifference > 180)//they did a flip
        {
            string didATrick = "hi";
            xDifference = 2;
        }

        trickStartPosition = new Vector3(0, 0, 0);
    }
}
