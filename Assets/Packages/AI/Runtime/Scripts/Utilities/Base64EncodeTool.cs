using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Services.AI
{
    public static class Base64EncodeTool
    {
        public static string AudioClipToBase64(AudioClip audioClip)
        {
            try
            {
                float[] data = new float[audioClip.samples * audioClip.channels];
                audioClip.GetData(data, 0);

                byte[] bytes = new byte[data.Length * 2];
                int index = 0;
                foreach (float sample in data)
                {
                    short convertedSample = (short)(sample * short.MaxValue);
                    BitConverter.GetBytes(convertedSample).CopyTo(bytes, index);
                    index += 2;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                        writer.Write(36 + bytes.Length);
                        writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
                        writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                        writer.Write(16);
                        writer.Write((ushort)1);
                        writer.Write((ushort)audioClip.channels);
                        writer.Write(audioClip.frequency);
                        writer.Write(audioClip.frequency * audioClip.channels * 2);
                        writer.Write((ushort)(audioClip.channels * 2));
                        writer.Write((ushort)16);
                        writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                        writer.Write(bytes.Length);
                        writer.Write(bytes);
                    }
                    byte[] wavBytes = stream.ToArray();
                    return Convert.ToBase64String(wavBytes);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error converting AudioClip to Base64: {ex.Message}");
                return string.Empty;
            }

        }

        public static string AudioClipToBase64(float[] data)
        {
            byte[] bytes = new byte[data.Length * 2];
            int index = 0;

            foreach (float sample in data)
            {
                short convertedSample = (short)(Mathf.Clamp(sample, -1.0f, 1.0f) * short.MaxValue);

                bytes[index] = (byte)(convertedSample & 0xFF);
                bytes[index + 1] = (byte)((convertedSample >> 8) & 0xFF);

                index += 2;
            }

            return Convert.ToBase64String(bytes);
        }

        public static AudioClip Base64ToAudioClip(string base64Audio, int sampleRate = 24000, int channels = 1)
        {
            try
            {
                byte[] audioBytes = Convert.FromBase64String(base64Audio);

                float[] audioData = new float[audioBytes.Length / 2];
                for (int i = 0; i < audioData.Length; i++)
                {
                    short sample = BitConverter.ToInt16(audioBytes, i * 2);
                    audioData[i] = sample / 32768.0f;
                }
                AudioClip audioClip = AudioClip.Create("Decoded Audio", audioData.Length, 1, sampleRate, false);
                audioClip.SetData(audioData, 0);

                return audioClip;
            }
            catch (Exception ex)
            {
                Debug.LogError("Error converting base64 to AudioClip: " + ex.Message);
                return null;
            }
        }


        public static bool IsFormatSupported(string base64Format)
        {
            if (string.IsNullOrEmpty(base64Format))
                return false;

            if (base64Format.Length % 4 != 0)
                return false;

            var base64Regex = new Regex(@"^[a-zA-Z0-9\+/]*={0,2}$", RegexOptions.None);
            if (!base64Regex.IsMatch(base64Format))
                return false;

            try
            {
                Convert.FromBase64String(base64Format);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
