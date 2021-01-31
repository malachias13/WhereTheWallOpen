using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamSensitivity : MonoBehaviour {
    CinemachineFreeLook freeLook;
    float defX, defY;

    private void Awake() {
        freeLook = GetComponent<CinemachineFreeLook>();
        defX = freeLook.m_XAxis.m_MaxSpeed;
        defY = freeLook.m_YAxis.m_MaxSpeed;
    }
    public void SetSensitivityX(float value) {
        freeLook.m_XAxis.m_MaxSpeed = defX * value;
    }
    public void SetSensitivityY(float value) {
        freeLook.m_YAxis.m_MaxSpeed = defY * value;
    }
}