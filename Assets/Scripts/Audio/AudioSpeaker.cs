using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

namespace Assets.Scripts.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSpeaker : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

#if PLATFORM_ANDROID
            Permission.RequestUserPermission(Permission.Microphone);
#endif
        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            _audioSource.clip = audioClip;
            _audioSource.Play();
        }
    }
}
