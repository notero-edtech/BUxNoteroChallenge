using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ForieroEngine.Audio.Recording
{
    public static class WavRecorder{

        // audio data is interlaced //
        public static void ConvertAndWrite(this FileStream fileStream, float[] samples){
            Int16[] intData = new Int16[samples.Length];
            // converting in 2 steps : float[] to Int16[] // 
            // then Int16[] to Byte[] //

            Byte[] bytesData = new Byte[samples.Length * 2];
            // bytesData array is twice the size of //
            // dataSource array because a float converted in Int16 is 2 bytes //

            // to convert float to Int16 // 
            int rescaleFactor = 32767; 

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * rescaleFactor);
                Byte[] byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            fileStream.Write(bytesData, 0, bytesData.Length);
        }

        public static void PrepareHeader(this FileStream fileStream, int headerSize = 44){
            byte emptyByte = new byte();

            for (int i = 0; i < headerSize; i++) 
            {
                fileStream.WriteByte(emptyByte);
            }
        }

        // header = 44 default for uncompressed wav //
        public static void WriteHeader(this FileStream fileStream, int outputRate = 44100, int headerSize = 44){
            fileStream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            fileStream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
            fileStream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            fileStream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            fileStream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            fileStream.Write(subChunk1, 0, 4);

            UInt16 two = 2;
            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);
            fileStream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(two);
            fileStream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(outputRate);
            fileStream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(outputRate * 4);

            fileStream.Write(byteRate, 0, 4);

            UInt16 four = 4;
            Byte[] blockAlign = BitConverter.GetBytes(four);
            fileStream.Write(blockAlign, 0, 2);

            UInt16 sixteen = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(sixteen);
            fileStream.Write(bitsPerSample, 0, 2);

            Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
            fileStream.Write(dataString, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(fileStream.Length - headerSize);
            fileStream.Write(subChunk2, 0, 4);

            fileStream.Close();
        }

        public static void WriteHeader(this FileStream fileStream, AudioClip clip)
        {
            var hz = clip.frequency;
            var channels = clip.channels;
            var samples = clip.samples;

            fileStream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            fileStream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
            fileStream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            fileStream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            fileStream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            fileStream.Write(subChunk1, 0, 4);

            //UInt16 two = 2;
            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);
            fileStream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(channels);
            fileStream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(hz);
            fileStream.Write(sampleRate, 0, 4);

            // sampleRate * bytesPerSample*number of channels, here 44100*2*2 //
            Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
            fileStream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(channels * 2);
            fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(bps);
            fileStream.Write(bitsPerSample, 0, 2);

            Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
            fileStream.Write(datastring, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
            fileStream.Write(subChunk2, 0, 4);

            fileStream.Close();
        }

        public static AudioClip TrimSilence(this AudioClip clip, float min)
        {
            var samples = new float[clip.samples];
            clip.GetData(samples, 0);
            return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);

        }

        public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
        {
            return TrimSilence(samples, min, channels, hz, false, false);
        }

        public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream)
        {
            int i;
            for (i = 0; i < samples.Count; i++) if (Mathf.Abs(samples[i]) > min) break;

            samples.RemoveRange(0, i);

            for (i = samples.Count - 1; i > 0; i--) if (Mathf.Abs(samples[i]) > min) break;

            samples.RemoveRange(i, samples.Count - i);

            var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, stream);

            clip.SetData(samples.ToArray(), 0);

            return clip;
        }
    }
}
