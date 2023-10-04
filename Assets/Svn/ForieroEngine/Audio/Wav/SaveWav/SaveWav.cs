using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace ForieroEngine.Audio.Recording
{
    public static class SaveWav
    {
        const int HEADER_SIZE = 44;

        public static byte[] GetWavBytes(AudioClip clip)
        {
            using (var memStream = CreateEmpty())
            {
                ConvertAndWrite(memStream, clip);
                WriteHeader(memStream, clip);

                return memStream.ToArray();
            }
        }

        public static AudioClip TrimSilence(AudioClip clip, float min)
        {
            var samples = new float[clip.samples];

            clip.GetData(samples, 0);

            return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
        }

        public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
        {
            return TrimSilence(samples, min, channels, hz, false);
        }

        public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool stream)
        {
            int i;

            for (i = 0; i < samples.Count; i++)
            {
                if (Mathf.Abs(samples[i]) > min)
                {
                    break;
                }
            }

            samples.RemoveRange(0, i);

            for (i = samples.Count - 1; i > 0; i--)
            {
                if (Mathf.Abs(samples[i]) > min)
                {
                    break;
                }
            }

            samples.RemoveRange(i, samples.Count - i);

            var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, stream);

            clip.SetData(samples.ToArray(), 0);

            return clip;
        }

        static MemoryStream CreateEmpty()
        {
            var memStream = new MemoryStream();
            byte emptyByte = new byte();

            for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
            {
                memStream.WriteByte(emptyByte);
            }

            return memStream;
        }

        static void ConvertAndWrite(MemoryStream memStream, AudioClip clip)
        {
            if (!clip) return;
            var samples = new float[clip.samples * clip.channels];

            if (clip.GetData(samples, 0))
            {
                const float rescaleFactor = 32767f; //to convert float to Int16

                for (int i = 0; i < samples.Length; i++)
                {
                    short intData = (short)(samples[i] * rescaleFactor);
                    byte lo = (byte)((ushort)intData & 0xff);
                    byte hi = (byte)(((ushort)intData & 0xff00) >> 8);
                    memStream.WriteByte(lo);
                    memStream.WriteByte(hi);
                }
            }
            else
            {
                Debug.LogError("clip.GetData false");
            }
        }

        static void WriteHeader(MemoryStream memStream, AudioClip clip)
        {
            if (!clip) return;            
            var hz = clip.frequency;
            var channels = clip.channels;
            var samples = clip.samples;

            memStream.Seek(0, SeekOrigin.Begin);

            using (var writer = new BinaryWriter(memStream))
            {
                Byte[] riff = new byte[4] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' };
                writer.Write(riff, 0, 4);

                int chunkSize = (int)(memStream.Length - 8);
                writer.Write(chunkSize);

                Byte[] wave = new byte[4] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' };
                writer.Write(wave, 0, 4);

                Byte[] fmt = new byte[4] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' };
                writer.Write(fmt, 0, 4);

                int subChunk1 = 16;
                writer.Write(subChunk1);

                UInt16 audioFormat = 1;
                writer.Write(audioFormat);

                UInt16 numChannels = (UInt16)channels;
                writer.Write(numChannels);

                int sampleRate = hz;
                writer.Write(sampleRate);

                int byteRate = hz * channels * 2; // sampleRate * bytesPerSample*number of channels, here 44100*2*2
                writer.Write(byteRate);

                UInt16 blockAlign = (ushort)(channels * 2);
                writer.Write(blockAlign);

                UInt16 bitsPerSample = 16;
                writer.Write(bitsPerSample);

                Byte[] datastring = new byte[4] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' };
                writer.Write(datastring, 0, 4);

                int subChunk2 = samples * channels * 2;
                writer.Write(subChunk2);
            }

        }
    }
}