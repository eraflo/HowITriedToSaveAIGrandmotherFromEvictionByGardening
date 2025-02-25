using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioConverter
    {
        public string ConvertAudioClipToBase64String(AudioClip audioClip)
        {
            byte[] bytes = ConvertAudioClipToWav(audioClip);
            return Convert.ToBase64String(bytes);
        }

        public byte[] ConvertAudioClipToWav(AudioClip clip)
        {
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            // Convert float samples to PCM 16 bits
            Int16[] intData = new Int16[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (Int16)(samples[i] * 32767);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // WAV Header
                    writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                    writer.Write(36 + intData.Length * 2);
                    writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
                    writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                    writer.Write(16);
                    writer.Write((ushort)1); // PCM Format
                    writer.Write((ushort)clip.channels);
                    writer.Write(clip.frequency);
                    writer.Write(clip.frequency * clip.channels * 2); // Byte rate
                    writer.Write((ushort)(clip.channels * 2)); // Block align
                    writer.Write((ushort)16); // Bits per sample

                    // Audio data
                    writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                    writer.Write(intData.Length * 2);

                    foreach (Int16 sample in intData)
                    {
                        writer.Write(sample);
                    }
                }

                return stream.ToArray();
            }
        }

        public string ConvertAudioSampleToBase64String(byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }

        public byte[] ConvertSamplesToWav(float[] samples, int frequency, int channels)
        {
            // Convert float samples to PCM 16 bits
            Int16[] intData = new Int16[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (Int16)(samples[i] * 32767);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // Wav header
                    writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                    writer.Write(36 + intData.Length * 2);
                    writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
                    writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                    writer.Write(16);
                    writer.Write((ushort)1); // PCM Format
                    writer.Write((ushort)channels);
                    writer.Write(frequency);
                    writer.Write(frequency * channels * 2); // Byte rate
                    writer.Write((ushort)(channels * 2)); // Block align
                    writer.Write((ushort)16); // Bits per sample

                    // Audio data
                    writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                    writer.Write(intData.Length * 2);

                    foreach (Int16 sample in intData)
                    {
                        writer.Write(sample);
                    }
                }

                return stream.ToArray();
            }
        }
    }
}
