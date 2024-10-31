// credit: https://gist.github.com/R-WebsterNoble/70614880b0d3940d3b2b741fbbb311a2

using System;
using System.IO;
using System.Text;
using ExtraInformation.Constants;
using UnityEngine;

namespace ExtraInformation.AssetRippers
{
	public static class AudioClipToWavFile
	{
		private const uint headerSize = 44;
		private const float rescaleFactor = 32767; //to convert float to Int16

		public static void Save(string filename, AudioClip clip, bool trim = false)
		{
			if (!filename.ToLower().EndsWith(".wav"))
			{
				filename += ".wav";
			}

			string filepath = Path.Combine(IOPaths.PATH, filename);

			// Make sure directory exists if user is saving to sub dir.
			Directory.CreateDirectory(Path.GetDirectoryName(filepath));

			using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
			{
				using (BinaryWriter writer = new BinaryWriter(fileStream))
				{
					byte[] wav = GetWav(clip, out uint length, trim);
					writer.Write(wav, 0, (int)length);
				}
			}
		}

		public static byte[] GetWav(AudioClip clip, out uint length, bool trim = false)
		{
			byte[] data = ConvertAndWrite(clip, out length, out uint samples, trim);

			WriteHeader(data, clip, length, samples);

			return data;
		}

		private static byte[] ConvertAndWrite(AudioClip clip, out uint length, out uint samplesAfterTrimming, bool trim)
		{
			float[] samples = new float[clip.samples * clip.channels];

			clip.GetData(samples, 0);

			int sampleCount = samples.Length;

			int start = 0;
			int end = sampleCount - 1;

			if (trim)
			{
				for (int i = 0; i < sampleCount; i++)
				{
					if ((short)(samples[i] * rescaleFactor) == 0)
						continue;

					start = i;
					break;
				}

				for (int i = sampleCount - 1; i >= 0; i--)
				{
					if ((short)(samples[i] * rescaleFactor) == 0)
						continue;

					end = i;
					break;
				}
			}

			byte[] buffer = new byte[(sampleCount * 2) + headerSize];

			uint p = headerSize;

			for (int i = start; i <= end; i++)
			{
				short value = (short)(samples[i] * rescaleFactor);
				buffer[p++] = (byte)(value >> 0);
				buffer[p++] = (byte)(value >> 8);
			}

			length               = p;
			samplesAfterTrimming = (uint)(end - start + 1);
			return buffer;
		}

		private static void AddDataToBuffer(byte[] buffer, ref uint offset, byte[] addBytes)
		{
			foreach (byte b in addBytes)
			{
				buffer[offset++] = b;
			}
		}

		private static void WriteHeader(byte[] stream, AudioClip clip, uint length, uint samples)
		{
			uint hz = (uint)clip.frequency;
			ushort channels = (ushort)clip.channels;

			uint offset = 0u;

			byte[] riff = Encoding.UTF8.GetBytes("RIFF");
			AddDataToBuffer(stream, ref offset, riff);

			byte[] chunkSize = BitConverter.GetBytes(length - 8);
			AddDataToBuffer(stream, ref offset, chunkSize);

			byte[] wave = Encoding.UTF8.GetBytes("WAVE");
			AddDataToBuffer(stream, ref offset, wave);

			byte[] fmt = Encoding.UTF8.GetBytes("fmt ");
			AddDataToBuffer(stream, ref offset, fmt);

			byte[] subChunk1 = BitConverter.GetBytes(16u);
			AddDataToBuffer(stream, ref offset, subChunk1);

			//const ushort two = 2;
			const ushort one = 1;

			byte[] audioFormat = BitConverter.GetBytes(one);
			AddDataToBuffer(stream, ref offset, audioFormat);

			byte[] numChannels = BitConverter.GetBytes(channels);
			AddDataToBuffer(stream, ref offset, numChannels);

			byte[] sampleRate = BitConverter.GetBytes(hz);
			AddDataToBuffer(stream, ref offset, sampleRate);

			byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
			AddDataToBuffer(stream, ref offset, byteRate);

			ushort blockAlign = (ushort)(channels * 2);
			AddDataToBuffer(stream, ref offset, BitConverter.GetBytes(blockAlign));

			ushort bps = 16;
			byte[] bitsPerSample = BitConverter.GetBytes(bps);
			AddDataToBuffer(stream, ref offset, bitsPerSample);

			byte[] dataString = Encoding.UTF8.GetBytes("data");
			AddDataToBuffer(stream, ref offset, dataString);

			byte[] subChunk2 = BitConverter.GetBytes(samples * 2);
			AddDataToBuffer(stream, ref offset, subChunk2);
		}
	}
}