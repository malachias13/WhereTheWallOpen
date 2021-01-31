using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaFlux
{
    public class SoundHandler : MonoBehaviour
    {
        public AudioSource deadSound;
        public AudioSource footSteps;
        public AudioSource endSound;

        public List<DialogPlayer.DialogLine> narrations = new List<DialogPlayer.DialogLine>();

        bool hasPlayedOnce = false;
        bool hasPlayedNarration = false;

        void Start()
        {
            deadSound = GetComponent<AudioSource>();
        }

        public void PlayDeadSound(bool isdead)
        {
            if (isdead)
            {
                if (!deadSound.isPlaying && hasPlayedOnce == false)
                {
                    deadSound.Play();

                    // Shakes the camera when the player dead
                    CameraController.Instance.CameraShake(4f, 1f);
                    hasPlayedOnce = true;
                }
            } else
            {
                hasPlayedOnce = false;
            }
        }
        public void PlayFootsteps()
        {
            if(!footSteps.isPlaying)
            footSteps.Play();
        }
        public void PlayEndSound()
        {
            if (!endSound.isPlaying)
                endSound.Play();
        }

        public void PlayNarration(int index)
        {
            var narration = narrations[index - 1];
            DialogPlayer.Instance.PlayLine(narration);
            // if (!narration.isPlaying && hasPlayedNarration == false)
            // {
            //     narration.Play();
            //     hasPlayedNarration = true;
            // } else
            // {
            //     hasPlayedNarration = false;
            // }

        }
    }
}