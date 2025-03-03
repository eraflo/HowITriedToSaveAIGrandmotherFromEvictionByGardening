using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    [CreateAssetMenu(fileName = "PCMData", menuName = "Audio/PCM Data")]
    public class PCMData : ScriptableObject
    {
        public byte[] pcmBytes;
        public int sampleRate = 44100;
        public int channels = 1;
    }
}
