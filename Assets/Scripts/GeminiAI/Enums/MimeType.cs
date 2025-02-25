using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Enums
{
    public static class MimeType
    {
        public const string TEXT = "text/plain";
        public const string HTML = "text/html";
        public const string JSON = "application/json";
        public const string XML = "application/xml";
        public const string WAV = "audio/wav";
        public const string MP3 = "audio/mp3";
        public const string AIFF = "audio/aiff";
        public const string AAC = "audio/aac";
        public const string OGG = "audio/ogg";
        public const string FLAC = "audio/flac";
        public const string PCM = "audio/pcm";
        public const string PNG = "image/png";
        public const string JPEG = "image/jpeg";
        public const string WEBP = "image/webp";
        public const string HEIC = "image/heic";
        public const string HEIF = "image/heif";

        public static List<string> MimeTypes
        {
            get
            {
                return new List<string>
                {
                    TEXT,
                    HTML,
                    JSON,
                    XML,
                    WAV,
                    MP3,
                    AIFF,
                    AAC,
                    OGG,
                    FLAC,
                    PCM,
                    PNG,
                    JPEG,
                    WEBP,
                    HEIC,
                    HEIF
                };
            }
        }
    }
}
