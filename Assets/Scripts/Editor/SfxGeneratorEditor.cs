#if UNITY_EDITOR
using System.IO;
using BusinessEmpire.Data;
using UnityEditor;
using UnityEngine;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Generates placeholder WAV sound effects and SoundLibrary asset.
    /// Menu: Business Empire → Generate Sound Effects
    /// </summary>
    public static class SfxGeneratorEditor
    {
        private const string SfxFolder = "Assets/Audio/SFX";
        private const string LibraryPath = "Assets/Resources/SoundLibrary.asset";

        [MenuItem("Business Empire/Generate Sound Effects")]
        public static void GenerateAll()
        {
            EnsureFolder("Assets/Audio");
            EnsureFolder(SfxFolder);

            var library = AssetDatabase.LoadAssetAtPath<SoundLibrary>(LibraryPath);
            if (library == null)
            {
                library = ScriptableObject.CreateInstance<SoundLibrary>();
                AssetDatabase.CreateAsset(library, LibraryPath);
            }

            library.buttonClick = SaveClip("button_click", Tone(880, 0.06f, 0.35f));
            library.panelOpen = SaveClip("panel_open", Sweep(400, 700, 0.08f, 0.3f));
            library.panelClose = SaveClip("panel_close", Sweep(700, 350, 0.08f, 0.25f));
            library.error = SaveClip("error", Tone(180, 0.15f, 0.4f));
            library.purchase = SaveClip("purchase", Chord(new float[] { 523f, 659f, 784f }, 0.25f, 0.45f));
            library.upgrade = SaveClip("upgrade", Sweep(500, 1200, 0.2f, 0.4f));
            library.coinCollect = SaveClip("coin", Tone(1318, 0.05f, 0.25f));
            library.cashRegister = SaveClip("cash_register", Chord(new float[] { 880f, 1108f }, 0.12f, 0.35f));
            library.reward = SaveClip("reward", Chord(new float[] { 523f, 659f, 784f, 1046f }, 0.35f, 0.5f));
            library.achievement = SaveClip("achievement", Chord(new float[] { 392f, 523f, 659f, 784f, 1046f }, 0.5f, 0.55f));
            library.levelUp = SaveClip("level_up", Sweep(300, 1500, 0.35f, 0.45f));
            library.dayAdvance = SaveClip("day_advance", Tone(440, 0.1f, 0.3f));
            library.positiveEvent = SaveClip("event_positive", Chord(new float[] { 659f, 784f }, 0.2f, 0.35f));
            library.negativeEvent = SaveClip("event_negative", Tone(220, 0.2f, 0.35f));
            library.mainMenuMusic = SaveClip("music_menu", AmbientLoop(220, 330, 4f, 0.12f));
            library.gameplayMusic = SaveClip("music_game", AmbientLoop(196, 294, 4f, 0.10f));

            EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssets();
            Debug.Log("[Business Empire] Sound effects generated in " + SfxFolder);
        }

        private static float[] Tone(float freq, float duration, float volume)
        {
            int sampleRate = 44100;
            int samples = (int)(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float env = 1f - (t / duration);
                data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * volume * env;
            }
            return data;
        }

        private static float[] Sweep(float from, float to, float duration, float volume)
        {
            int sampleRate = 44100;
            int samples = (int)(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float freq = Mathf.Lerp(from, to, t / duration);
                float env = 1f - (t / duration);
                data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * volume * env;
            }
            return data;
        }

        private static float[] Chord(float[] freqs, float duration, float volume)
        {
            int sampleRate = 44100;
            int samples = (int)(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float env = 1f - (t / duration);
                float sum = 0f;
                foreach (float f in freqs)
                    sum += Mathf.Sin(2 * Mathf.PI * f * t);
                data[i] = sum / freqs.Length * volume * env;
            }
            return data;
        }

        private static float[] AmbientLoop(float f1, float f2, float duration, float volume)
        {
            int sampleRate = 44100;
            int samples = (int)(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float wave = Mathf.Sin(2 * Mathf.PI * f1 * t) * 0.6f + Mathf.Sin(2 * Mathf.PI * f2 * t) * 0.4f;
                data[i] = wave * volume;
            }
            return data;
        }

        private static AudioClip SaveClip(string name, float[] samples)
        {
            string path = $"{SfxFolder}/{name}.wav";
            WriteWav(path, samples, 44100);
            AssetDatabase.ImportAsset(path);

            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            return clip;
        }

        private static void WriteWav(string path, float[] samples, int sampleRate)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            int channels = 1;
            int byteRate = sampleRate * channels * 2;
            short[] intData = new short[samples.Length];
            for (int i = 0; i < samples.Length; i++)
                intData[i] = (short)(Mathf.Clamp(samples[i], -1f, 1f) * 32767f);

            writer.Write(new char[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + intData.Length * 2);
            writer.Write(new char[] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);
            writer.Write(new char[] { 'd', 'a', 't', 'a' });
            writer.Write(intData.Length * 2);
            foreach (short s in intData) writer.Write(s);

            File.WriteAllBytes(path, stream.ToArray());
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path).Replace("\\", "/");
            var name = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
