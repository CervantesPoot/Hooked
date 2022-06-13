using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool spawned = true;
    public bool wallHit = false;
    public bool grappling = false;

    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float horizontalSpeed = 1f;
    [SerializeField]
    private float swingSpeed = 1f;
    [SerializeField]
    private float releaseSpeed = 1f;
    [SerializeField]
    private float offsetLine = 1.15f;

    private float time = 1f;
    private int line = 2;
    private Vector3 lanePosition = Vector3.zero;

    private Rigidbody physics;

    //private float timeRotation = 0f;
    private int rotation = 0;
    private Vector3 rotationDirection = Vector3.forward;

    private GameObject actualHook;

    private GrapplingHook grapplingMechanics;

    private Vector3 positionCheckPoint;
    private int lineCheckPoint;
    private int rotationCheckPoint;

    public LevelManager level;

    private Animator animatorController;

    private void Awake()
    {
        physics = GetComponent<Rigidbody>();
        grapplingMechanics = GetComponent<GrapplingHook>();
        animatorController = GetComponentInChildren<Animator>();
    }

    #region Movement

    private void Update()
    {
        if (wallHit == false)
        {
            if (spawned == true)
            {
                if (grapplingMechanics.triggered == false)
                {
                    this.transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
                }
                else
                {
                    physics.velocity = transform.forward * swingSpeed;
                }
                if (time < 1)
                {
                    /// switch lane position
                    time += Time.deltaTime * horizontalSpeed;
                    //float offset = lanePosition.x + lanePosition.y + lanePosition.z;
                    //Vector3 newPos = this.transform.position + (this.transform.right * offset * Time.deltaTime);
                    this.transform.Translate(lanePosition * Time.deltaTime * horizontalSpeed, Space.Self);
                    //this.transform.Translate((lanePosition * horizontalSpeed * Time.deltaTime), Space.Self);

                    //float delta = time / (1f);
                    //float offset = lanePosition.x + lanePosition.y + lanePosition.z;
                    //Vector3 newPos = this.transform.position + (this.transform.right * offset);
                    //this.transform.position = Vector3.Lerp(this.transform.position, newPos, delta);
                }
            }
        }
        /// rotate player
        // Determine which direction to rotate towards
        Vector3 targetDirection = rotationDirection;

        // The step size is equal to speed times frame time.
        float singleStep = horizontalSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);

    }

    public void MoveFordward()
    {
        if (grappling == true || grapplingMechanics.triggered == true)
        {
            /// perform grappling to the front
            GrapplingEvent();
        }
        //else
        //{
        //    /// run
        //}
    }

    public void MoveLeft()
    {
        LineChange(-1);
        if (grappling == true || grapplingMechanics.triggered == true)
        {
            /// perform grappling to the left lane
            GrapplingEvent();
        }
    }

    public void MoveRight()
    {
        LineChange(1);
        if (grappling == true || grapplingMechanics.triggered == true)
        {
            /// perform grappling to the right lane
            GrapplingEvent();
        }
    }

    private void LineChange(int value)
    {
        int oldValue = line;
        line += value;
        if (line <= 0)
        {
            line = 1;
            oldValue = line;
            return;
        }
        if (line > 3)
        {
            line = 3;
            oldValue = line;
            return;
        }
        if (line != oldValue)
        {
            if (line < oldValue)
            {
                //lanePosition = offsetLine * Vector3.left;
                lanePosition = this.transform.right * offsetLine * -1;
            }
            else
            {
                //lanePosition = (offsetLine * Vector3.right);
                lanePosition = this.transform.right * offsetLine;
            }
            time = 0f;
        }

    }

    private void RotationChange(int value)
    {
        rotation += value;
        //timeRotation = 0f;
        if (rotation < 0)
        {
            rotation = 7;
            value++;
            rotation += value;
        }
        if (rotation > 7)
        {
            rotation = 0;
            value--;
            rotation += value;
        }
        switch (rotation)
        {
            case 0:
                rotationDirection = new Vector3(0, 0, 1);
                break;
            case 1:
                rotationDirection = new Vector3(1, 0, 1);
                break;
            case 2:
                rotationDirection = new Vector3(1, 0, 0);
                break;
            case 3:
                rotationDirection = new Vector3(1, 0, -1);
                break;
            case 4:
                rotationDirection = new Vector3(0, 0, -1);
                break;
            case 5:
                rotationDirection = new Vector3(-1, 0, -1);
                break;
            case 6:
                rotationDirection = new Vector3(-1, 0, 0);
                break;
            case 7:
                rotationDirection = new Vector3(-1, 0, 1);
                break;
        }
    }

    private void GrapplingEvent()
    {
        if (grapplingMechanics.triggered == false)
        {
            Vector3 point = actualHook.transform.GetChild(0).GetChild(line - 1).transform.position;
            grapplingMechanics.StartGrapple(point, 2);
            animatorController.Play("StartSwing");
            //physics.AddForce((transform.forward) * speed * 30);
        }
        else
        {
            grapplingMechanics.StopGrapple();
            animatorController.Play("EndSwing");

            physics.AddForce((transform.forward + (transform.up * 2)) * releaseSpeed);

            if (actualHook != null && grappling == true)
            {
                GrapplingEvent();
            }
        }
    }

    public void PauseCharacter()
    {
        physics.isKinematic = true;
        spawned = false;
    }

    public void ResumeCharacter()
    {
        animatorController.Play("Run");
        physics.isKinematic = false;
        spawned = true;
        if (actualHook != null && grappling == true)
        {
            MoveFordward();
            physics.AddForce((transform.forward + (transform.up * 2)) * releaseSpeed);
        }
    }

    #endregion Movement

    public void OverrideData(int newLine, int newRotation)
    {
        line += newLine;
        RotationChange(newRotation);
    }

    public void OverrideHardData()
    {
        line = 2;
        rotation = 0;
        RotationChange(0);
        lineCheckPoint = 2;
        rotationCheckPoint = 0;
        wallHit = false;
        grappling = false;
        actualHook = null;
        SetCharacter();
        animatorController.Play("Stand");
    }

    #region RespawnLogic
    IEnumerator Respawn()
    {
        /// pending perform animation hit
        yield return new WaitForSeconds(2f);
        /// pending perform animation or camera movement to respawn point
        SetCharacter();
        wallHit = false;
        //spawned = true;
        grappling = false;
        StartCoroutine(level.Respawn());
        yield break;
    }

    private void SetCharacter()
    {
        this.transform.position = positionCheckPoint;
        line = lineCheckPoint;
        int rotationChange = (rotation - rotationCheckPoint) * -1;
        RotationChange(rotationChange);
    }

    #endregion RespawnLogic

    #region Inputs
    public void Forward(InputAction.CallbackContext action)
    {
        if (spawned == false)
        {
            return;
        }
        if (action.phase == InputActionPhase.Started)
        {
            MoveFordward();
        }
    }

    public void Left(InputAction.CallbackContext action)
    {
        if (spawned == false)
        {
            return;
        }
        if (action.phase == InputActionPhase.Started)
        {
            MoveLeft();
            //RotationChange(-1);
        }
    }

    public void Right(InputAction.CallbackContext action)
    {
        if (spawned == false)
        {
            return;
        }
        if (action.phase == InputActionPhase.Started)
        {
            MoveRight();
            //RotationChange(1);
        }
    }

    #endregion Inputs

    #region collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Modifier"))
        {
            if (other.GetComponent<Modifier>().activated == true)
            {
                return;
            }
            other.GetComponent<Modifier>().activated = true;
            int lineChange = other.GetComponent<Modifier>().lineModifier;
            int rotationChange = other.GetComponent<Modifier>().rotationModifier;
            OverrideData(lineChange, rotationChange);
            physics.velocity = Vector3.zero;
        }
        else if (other.tag.Equals("Grappleable"))
        {
            actualHook = other.transform.parent.gameObject;
            grappling = true;
        }
        else if (other.tag.Equals("CheckPoint"))
        {
            positionCheckPoint = this.transform.position;
            lineCheckPoint = line;
            rotationCheckPoint = rotation;
        }
        else if (other.tag.Equals("DeadZone"))
        {
            StartCoroutine(Respawn());
        }
        else if (other.tag.Equals("Collectables"))
        {
            /// pending logic collectables
            Destroy(other.gameObject);
        }
        else if (other.tag.Equals("EndPoint"))
        {
            spawned = false;
            physics.velocity = Vector3.zero;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Grappleable"))
        {
            actualHook = null;
            grappling = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Block"))
        {
            wallHit = true;
            spawned = false;
            grappling = false;
            physics.velocity = Vector3.zero;
            StartCoroutine(Respawn());
            animatorController.Play("Hit");
        }
    }
    #endregion collisions

}
