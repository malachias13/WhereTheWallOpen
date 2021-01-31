using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//using Cinemachine.Editor;
//using Cinemachine.Utility;

namespace ManaFlux
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get;private set; }

        private CinemachineFreeLook cinemachineFreeLookCamera;

        [Header("Camera Stats")]
        public float amplitudeGain;
        public float frequemcyGain;
        public float ShakeDuration;



     //   private CinemachineVirtualCamera camera;
        private float shakeTimer;

        private void Start()
        {
            Instance = this;
            cinemachineFreeLookCamera = GetComponent<CinemachineFreeLook>();
        //    camera = GetComponent<CinemachineVirtualCamera>();

        }




        public void CameraShake(float intensity,float time)
        {

            // Virtual camera
            /*  CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                  camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

              cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
              shakeTimer = time;*/

            // Shake each free look camera rig. 

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin_0 =
            cinemachineFreeLookCamera.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin_0.m_AmplitudeGain = intensity;

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin_1 =
            cinemachineFreeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin_1.m_AmplitudeGain = intensity;

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin_2 =
            cinemachineFreeLookCamera.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin_2.m_AmplitudeGain = intensity;



            shakeTimer = time;
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
            }
            else if (shakeTimer <= 0f)
            {
                StopCameraShake();
            }
        }
        public void StopCameraShake()
        {
            // stop the free look camera rigs.

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin_0 =
            cinemachineFreeLookCamera.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin_0.m_AmplitudeGain = 0f;

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin_1 =
            cinemachineFreeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin_1.m_AmplitudeGain = 0f;

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin_2 =
            cinemachineFreeLookCamera.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin_2.m_AmplitudeGain = 0f;
        }
        
    }
}