using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Vector3 dragOrigin;
    public Vector3 minBounds;
    public Vector3 maxBounds;

    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 direction = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
        Vector3 move = new Vector3(direction.x * moveSpeed, 0, direction.y * moveSpeed);

        Vector3 targetPosition = transform.position + move;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.z, maxBounds.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        dragOrigin = Input.mousePosition;
    }
}
