using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SkeletonPlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject spawnableSkeleton;         // Yerleþtirilecek prefab
    [SerializeField] private ARRaycastManager raycastManager;      // Raycast yöneticisi
    [SerializeField] private ARPlaneManager planeManager;          // Düzlem yöneticisi

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private bool isModelSpawned = false;

    //  Bu fonksiyon butonla çaðrýlacak
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

            //  Modeli düzleme sabitle
            var plane = planeManager.GetPlane(raycastHits[0].trackableId);
            if (plane != null)
            {
                spawned.transform.SetParent(plane.transform, true);
            }

            isModelSpawned = true;

            //  Düzlemleri görünmez yap (gizleme deðil, sadece mesh kapatýlýyor)
            HideAllPlanes();
        }
        else
        {
            Debug.Log(" Yüzey bulunamadý.");
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
        // PlaneManager açýk kalsýn ki track edilen alan bozulmasýn
    }
}
