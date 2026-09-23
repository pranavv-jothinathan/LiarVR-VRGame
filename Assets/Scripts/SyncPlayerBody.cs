using UnityEngine;
using Photon.Pun;

public class SyncPlayerBody : MonoBehaviourPun
{
    private Transform _mainCamTransform;

    void Start()
    {
        if (photonView.IsMine)
        {
            
            if (Camera.main != null)
            {
                _mainCamTransform = Camera.main.transform;
            }

            // only local to thide
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) r.enabled = true;
        }
    }

    void LateUpdate()
    {
        if (!photonView.IsMine || _mainCamTransform == null) return;

        //position same
        transform.position = _mainCamTransform.position;

        // rotation
        Vector3 targetRot = _mainCamTransform.eulerAngles;
        transform.rotation = Quaternion.Euler(0, targetRot.y, 0);
    }
}