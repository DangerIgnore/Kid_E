/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using UnityEngine.Events;
using Leap;

[AddComponentMenu("")]
public class SimpleFacingCameraCallbacks : MonoBehaviour
{

    public Transform toFaceCamera;

    private bool _initialized = false;
    private bool _isFacingCamera = false;

    public UnityEvent OnBeginFacingCamera;
    public UnityEvent OnEndFacingCamera;
    private GestureTest gesture;
    private HandModelBase leftHandModel;
    private Transform uiPanel;

    void Start()
    {

        if (toFaceCamera != null) initialize();
        if (GameObject.Find("Interaction Manager"))
        {
            gesture = GameObject.Find("Interaction Manager").GetComponent<GestureTest>();
        }
        leftHandModel = gesture.leftHandModel;
        uiPanel = this.transform.GetChild(0);
    }

    private void initialize()
    {
        // Set "_isFacingCamera" to be whatever the current state ISN'T, so that we are
        // guaranteed to fire a UnityEvent on the first initialized Update().
        _isFacingCamera = !GetIsFacingCamera(toFaceCamera, Camera.main);
        _initialized = true;
    }

    void Update()
    {
        if (toFaceCamera != null && !_initialized)
        {
            initialize();
        }
        if (!_initialized) return;

        if (GetIsFacingCamera(toFaceCamera, Camera.main, _isFacingCamera ? 0.77F : 0.82F) != _isFacingCamera)
        {
            //print("IsLeftHandOpenFullHand:" + gesture.IsLeftHandOpenFullHand());
            //print("IsLeftHandOpenFullIT:" + gesture.IsLeftHandOpenFullIT());

            _isFacingCamera = !_isFacingCamera;

            if (_isFacingCamera&& gesture.IsLeftHandOpenFullIT())
            {
                OnBeginFacingCamera.Invoke();
            }
            else
            {
                OnEndFacingCamera.Invoke();
            }
        }
    }
    private void FixedUpdate()
    {
        if (!leftHandModel.IsTracked)
        {
            uiPanel.gameObject.SetActive(false);
        }
        else
        {
            uiPanel.gameObject.SetActive(true);
        }

    }
    public static bool GetIsFacingCamera(Transform facingTransform, Camera camera, float minAllowedDotProduct = 0.8F)
    {
        float result = Vector3.Dot((camera.transform.position - facingTransform.position).normalized, facingTransform.forward);
        //print(result);
        return result > minAllowedDotProduct;
    }

}