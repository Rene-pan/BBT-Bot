using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CamController_v2 : MonoBehaviour
{
    [SerializeField] float horizontalSpeed = 1.0f;
    [SerializeField] float verticalSpeed = 1.0f;
    [SerializeField] float orbitingDamping = 10f;
    [SerializeField] Vector2 Limit;
    Vector3 localRot;
    private void Update()
    {
        ThirdPersonCam();
    }
    void ThirdPersonCam()
    {
        float h = horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");
        localRot.x += h;
        localRot.y -= v;
        localRot.y = Mathf.Clamp(localRot.y, Limit.x, Limit.y);
        Quaternion QT = Quaternion.Euler(localRot.y, localRot.x, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, QT, Time.deltaTime * orbitingDamping);
    }

}
