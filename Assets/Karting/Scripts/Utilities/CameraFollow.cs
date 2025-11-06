using System.Collections;
using Mirror;
using Unity.Cinemachine;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    private CinemachineCamera _camera;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        _camera = FindFirstObjectByType<CinemachineCamera>();
        if( _camera != null )
        {
            _camera.Follow = transform;

            StartCoroutine(RefeshCamera());
        }
    }

    IEnumerator RefeshCamera()
    {
        yield return new WaitForEndOfFrame();
        _camera.enabled = false;
        yield return new WaitForEndOfFrame();
        _camera.enabled = true;
    }
    


    //public Transform carTransform;
    //private Vector3 distanceCamera;

    //private float carPositionY;
    //private Transform cameraTransform;
    //private float cameraPositionY;
    //private Vector3 velocity = Vector3.zero;
    //void Start()
    //{
    //    distanceCamera = carTransform.position - transform.position;

    //    carPositionY = carTransform.position.y;
    //    cameraTransform = transform;
    //    cameraPositionY = transform.position.y;
    //}

    //void LateUpdate()
    //{
    //    transform.position = carTransform.position - carTransform.rotation*distanceCamera;
    //    transform.position = Vector3.SmoothDamp(cameraTransform.position, transform.position, ref velocity, 0.3f);

    //    //if (Mathf.Abs(carTransform.position.y - carPositionY) < 0.2)
    //    //    transform.position = new Vector3(transform.position.x, cameraPositionY, transform.position.z);
    //    //else
    //    //{
    //    //    carPositionY = carTransform.position.y;
    //    //    cameraPositionY = cameraTransform.position.y;
    //    //}    

    //    transform.eulerAngles = new Vector3(0, carTransform.eulerAngles.y, 0);
    //}
}
