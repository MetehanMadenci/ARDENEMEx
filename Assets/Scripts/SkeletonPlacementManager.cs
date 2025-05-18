using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SkeletonPlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject spawnableSkeleton;         // Yerle�tirilecek prefab
    [SerializeField] private ARRaycastManager raycastManager;      // Raycast y�neticisi
    [SerializeField] private ARPlaneManager planeManager;          // D�zlem y�neticisi

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private bool isModelSpawned = false;

    //  Bu fonksiyon butonla �a�r�lacak
    public void PlaceModel()
    {
        if (isModelSpawned) return;

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        if (raycastManager.Raycast(screenCenter, raycastHits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = raycastHits[0].pose;

            // Modeli instantiate et
            GameObject spawned = Instantiate(spawnableSkeleton, hitPose.position, hitPose.rotation);
            spawned.transform.localScale = Vector3.one * 0.1f; // Gerekirse ayarla

            //  Modeli d�zleme sabitle
            var plane = planeManager.GetPlane(raycastHits[0].trackableId);
            if (plane != null)
            {
                spawned.transform.SetParent(plane.transform, true);
            }

            isModelSpawned = true;

            //  D�zlemleri g�r�nmez yap (gizleme de�il, sadece mesh kapat�l�yor)
            HideAllPlanes();
        }
        else
        {
            Debug.Log(" Y�zey bulunamad�.");
        }
    }

    private void HideAllPlanes()
    {
        foreach (var plane in planeManager.trackables)
        {
            var renderer = plane.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.enabled = false;
        }
        // PlaneManager a��k kals�n ki track edilen alan bozulmas�n
    }
}
