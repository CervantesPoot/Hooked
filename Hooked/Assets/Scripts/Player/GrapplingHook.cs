using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public LineRenderer lr;

    public Transform player;
    public Transform firePoint;

    public bool triggered = false;

    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;

    private SpringJoint joint;






    [Header("General Settings:")]
    [SerializeField] private int precision = 40;
    [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)] [SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;

    private float moveTime = 0;

    private bool strightLine = true;

    private int pos;

    private float limitTime = 3f;

    //Called after Update
    void LateUpdate()
    {
        moveTime += Time.deltaTime;
        DrawRopev2();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    public void StartGrapple(Vector3 hitPoint, int newPos)
    {
        limitTime = 2f;
        lr.enabled = true;
        pos = newPos;
        if (pos == 2)
        {
            float direction = Random.Range(0f, 1f);
            if (direction >= 0 && direction <= 0.5f)
            {
                pos = 1;
            }
            else
            {
                pos = 3;
            }
        }
        //lr.positionCount = 2;
        lr.positionCount = precision;

        triggered = true;

        strightLine = false;

        grapplePoint = hitPoint;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

        //The distance grapple will try to keep from grapple point. 
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        //Adjust these values to fit your game.
        //joint.spring = 4.5f;
        //joint.damper = 1f;
        //joint.massScale = 4.5f;
        joint.spring = 50f;
        joint.damper = 1f;
        joint.massScale = 4.5f;

        currentGrapplePosition = firePoint.position;

        waveSize = StartWaveSize;

    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    public void StopGrapple()
    {
        triggered = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    void DrawRopev2()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        if (!strightLine)
        {
            //if (lr.GetPosition(precision - 1).normalized == grapplePoint.normalized)
            if (Vector3.Distance(lr.GetPosition(precision - 1).normalized, grapplePoint.normalized) <= 0.3f)
            {
                strightLine = true;
            }
            else
            {
                DrawRopeWaves();
            }
        }
        else
        {
            if (waveSize > 0)
            {
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                waveSize = 0;

                if (lr.positionCount != 2) { lr.positionCount = 2; }

                DrawRopeNoWaves();
            }
        }
        limitTime -= Time.deltaTime;
        if (limitTime <= 0)
        {
            StopGrapple();
        }
    }

    void DrawRopeWaves()
    {
        for (int i = 0; i < precision; i++)
        {
            float delta = (float)i / ((float)precision - 1f);
            Vector3 offset = Vector3.zero;
            if (pos == 1)
            {
                offset = new Vector3(ropeAnimationCurve.Evaluate(delta) * waveSize * -1, 0, 0); //Vector3.left;
            }
            else if (pos == 3)
            {
                offset = new Vector3(ropeAnimationCurve.Evaluate(delta) * waveSize, 0, 0); //Vector3.right;
            }

            Vector3 targetPosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, delta) + offset;
            Vector3 currentPosition = Vector3.Lerp(currentGrapplePosition, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            lr.SetPosition(i, currentPosition);
        }
    }

    void DrawRopeNoWaves()
    {
        lr.SetPosition(0, firePoint.position);
        lr.SetPosition(1, grapplePoint);
    }
}
