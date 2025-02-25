using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioRecorder : MonoBehaviour
    {
        [Header("Audio Recording")]
        [SerializeField] private int recordingFrequency = 16000; // 16kHz recommended for VAD
        [SerializeField] private int bufferSizeInSeconds = 1;

        private bool isProcessingActive = false;
        private List<float> audioBuffer = new List<float>();
        private int bufferSize;
        private int currentBufferIndex = 0;
        private AudioClip microphoneClip;


        public Action<float[]> onGetAudioFlux;

        public int RecordingFrequency => recordingFrequency;
        public int BufferSizeInSeconds => bufferSizeInSeconds;


        private void Start()
        {
            bufferSize = recordingFrequency * bufferSizeInSeconds;
        }

        public void ToggleProcessing()
        {
            if (!isProcessingActive)
                StartProcessing();
            else
                StopProcessing();
        }

        private void StartProcessing()
        {
            if (Microphone.devices.Length <= 0)
            {
                return;
            }

            // Start recording
            microphoneClip = Microphone.Start(Microphone.devices[0], true, 10, recordingFrequency);

            isProcessingActive = true;

            // Start processing audio
            StartCoroutine(ProcessAudioRealtime());
        }

        private void StopProcessing()
        {
            if (!isProcessingActive)
                return;

            Microphone.End(Microphone.devices[0]);
            isProcessingActive = false;

            // Clear the buffer
            audioBuffer.Clear();
            currentBufferIndex = 0;
        }

        private IEnumerator ProcessAudioRealtime()
        {
            // Last position in microphone clip
            int prevPos = 0;

            while (isProcessingActive)
            {
                // Get current position in microphone clip
                int currPos = Microphone.GetPosition(Microphone.devices[0]);

                if (currPos != prevPos)
                {
                    // Calculate number of samples to read
                    int samplesToRead = 0;

                    if (currPos > prevPos)
                    {
                        samplesToRead = currPos - prevPos;
                    }
                    else
                    {
                        // Wrap-around (buffer has looped)
                        samplesToRead = (microphoneClip.samples - prevPos) + currPos;
                    }

                    // Read samples from microphone clip
                    float[] samples = new float[samplesToRead];

                    if (currPos > prevPos)
                    {
                        microphoneClip.GetData(samples, prevPos);
                    }
                    else
                    {
                        // Read in two parts because of wrap-around
                        float[] firstPart = new float[microphoneClip.samples - prevPos];
                        float[] secondPart = new float[currPos];

                        microphoneClip.GetData(firstPart, prevPos);
                        microphoneClip.GetData(secondPart, 0);

                        // Combine both parts
                        Array.Copy(firstPart, 0, samples, 0, firstPart.Length);
                        Array.Copy(secondPart, 0, samples, firstPart.Length, secondPart.Length);
                    }

                    // Add samples to buffer
                    foreach (float sample in samples)
                    {
                        audioBuffer.Add(sample);
                        currentBufferIndex++;

                        // When buffer is full, process it
                        if (currentBufferIndex >= bufferSize)
                        {
                            float[] bufferToProcess = audioBuffer.ToArray();
                            audioBuffer.Clear();
                            currentBufferIndex = 0;

                            onGetAudioFlux?.Invoke(bufferToProcess);
                        }
                    }

                    prevPos = currPos;
                }

                yield return null;
            }
        }

    }
}
