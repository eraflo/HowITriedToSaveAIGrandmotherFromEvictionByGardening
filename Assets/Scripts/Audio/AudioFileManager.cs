using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Audio
{
    public class AudioFileManager : IDisposable
    {
        private FileStream fileStream;
        private BinaryWriter writer;
        private bool disposed = false;

        public AudioFileManager(string filename, int channels = 1, int rate = 24000, int sampleWidth = 2)
        {
            fileStream = new FileStream(filename, FileMode.Create);
            writer = new BinaryWriter(fileStream);

            int sampleRate = rate;
            int bitsPerSample = sampleWidth * 8;
            int byteRate = sampleRate * channels * bitsPerSample / 8;
            int blockAlign = channels * bitsPerSample / 8;

            // Write the WAV file header
            writer.Write(new char[] { 'R', 'I', 'F', 'F' });
            writer.Write((int)0); // Placeholder for file size
            writer.Write(new char[] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[] { 'f', 'm', 't', ' ' });
            writer.Write((int)16); // Size of the 'fmt ' chunk
            writer.Write((short)1); // Audio format (1 for PCM)
            writer.Write((short)channels); // Number of channels
            writer.Write(sampleRate); // Sample rate
            writer.Write(byteRate); // Byte rate
            writer.Write((short)blockAlign); // Block align
            writer.Write((short)bitsPerSample); // Bits per sample
            writer.Write(new char[] { 'd', 'a', 't', 'a' });
            writer.Write((int)0); // Placeholder for data size
        }

        public void WriteSamples(float[] samples)
        {
            foreach (float sample in samples)
            {
                short sampleValue = (short)(sample * short.MaxValue);
                writer.Write(sampleValue);
            }
        }

        public void WriteSamples(byte[] samples)
        {
            writer.Write(samples);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (writer != null)
                    {
                        long dataSize = fileStream.Position - 44;
                        long fileSize = dataSize + 36;

                        // Update the file size and data size in the header
                        fileStream.Seek(4, SeekOrigin.Begin);
                        writer.Write((int)fileSize);
                        fileStream.Seek(40, SeekOrigin.Begin);
                        writer.Write((int)dataSize);

                        writer.Close();
                        fileStream.Close();
                        writer = null;
                        fileStream = null;
                    }
                }
                disposed = true;
            }
        }
    }
}
