using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic.MusicEditor
{
    public static class MusicMap
    {
        public static Texture2D PaintWaveformSpectrum(this AudioClip audio, int width, int height, Color col)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float[] samples = new float[audio.channels * audio.samples];
            float[] waveform = new float[width];
            audio.GetData(samples, 0);
            int packSize = (samples.Length / width) + 1;
            int waveIndex = 0;

            for (int i = 0; i < samples.Length; i += packSize)
            {
                waveform[waveIndex] = Mathf.Abs(samples[i]);
                waveIndex++;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tex.SetPixel(x, y, Color.black);
                }
            }

            for (int x = 0; x < waveform.Length; x++)
            {
                for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
                {
                    tex.SetPixel(x, (height / 2) + y, col);
                    tex.SetPixel(x, (height / 2) - y, col);
                }
            }
            tex.Apply();

            return tex;
        }
    }
}