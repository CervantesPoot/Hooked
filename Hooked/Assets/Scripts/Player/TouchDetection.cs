using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TouchDetection : MonoBehaviour
{
    [SerializeField]
    private float minDistance = 20f;
    [SerializeField]
    private float maxTime = 1f;

    [SerializeField]
    [Range(0, 1)]
    private float directionOffset = 0.5f;

    #region Events
    public UnityEvent OnForwardSwipe;
    public UnityEvent OnLeftSwipe;
    public UnityEvent OnRightSwipe;
    #endregion Events

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float startTime;
    private float endTime;

    private InputActionPhase phase;

    private bool swiped = false;

    public void Touch(InputAction.CallbackContext action)
    {
        if (action.phase == InputActionPhase.Started)
        {
            swiped = false;
            phase = action.phase;
            startTime = (float)action.startTime;
        }
        else if (action.phase == InputActionPhase.Canceled)
        {
            phase = action.phase;
            endTime = (float)action.time;
            ValidateDirection();
        }
        //Debug.Log("touch " + action.phase);
    }

    public void SwipeDetection(InputAction.CallbackContext action)
    {
        if (phase == InputActionPhase.Started)
        {
            phase = InputActionPhase.Disabled;
            startPosition = action.ReadValue<Vector2>();
            endPosition = action.ReadValue<Vector2>();
        }
        //else if (phase == InputActionPhase.Canceled)
        //{
        //    //phase = InputActionPhase.Disabled;
        //    //endTime = (float)action.time;
        //    //if (swiped == false)
        //    //{
        //    //    ValidateDirection();
        //    //}

        //    endPosition = action.ReadValue<Vector2>();
        //    phase = InputActionPhase.Disabled;
        //    ValidateDirection();
        //}
        else
        {

            endPosition = action.ReadValue<Vector2>();
        }
        //Debug.Log("swipe " + action.phase);
    }

    private void ValidateDirection()
    {
        /// the Touch swipe was to short 
        if (Vector3.Distance(startPosition, endPosition) < minDistance)
        {
            /// forward input
            OnForwardSwipe.Invoke();
            return;
        }
        /// the swipe take to long
        if (endTime - startTime > maxTime)
        {
            return;
        }
        Vector3 direction = (startPosition - endPosition).normalized;
        /// left input
        if (direction.x >= directionOffset)
        {
            OnLeftSwipe.Invoke();
        }
        /// right input
        else if (direction.x <= (directionOffset * -1))
        {
            OnRightSwipe.Invoke();
        }
        /// forward input
        else
        {
            OnForwardSwipe.Invoke();
        }
    }
}
