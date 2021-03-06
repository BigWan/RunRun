﻿#if UNITY_EDITOR

using UnityEngine;

public class UIDebug : MonoBehaviour {

    public bool isDebug;
    public bool onlayRayCastTarget;

    Vector3[] corners = new Vector3[4];
    RectTransform[] rts;

    void OnDrawGizmos() {
        if (!isDebug) return;        
        
        rts = GetComponentsInChildren<RectTransform>();

        foreach (var rt in rts) {

            rt.GetWorldCorners(corners);
            Gizmos.color = Color.blue;
            for (int i = 0; i < 4; i++) {                
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }

        }
    }


}

#endif
