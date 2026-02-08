using UnityEngine;

public class UiFollowWorldObject : MonoBehaviour
{
    public Transform target;
    public Vector3 worldOffset;
    public RectTransform uiElement;
    public Camera worldCamera;

    private void LateUpdate()
    {
        if (target == null || uiElement == null) return;

        if (worldCamera == null) worldCamera = Camera.main;

        Vector3 screenPos = worldCamera.WorldToScreenPoint(target.position + worldOffset);

        uiElement.position = screenPos;

        bool isBehind = screenPos.z < 0f;
        uiElement.gameObject.SetActive(!isBehind);
    }
}
