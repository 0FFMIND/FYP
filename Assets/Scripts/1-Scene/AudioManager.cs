using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utils.SingletonPattern;

namespace AudioSystem
{
    public class AudioManager : SingletonMB<AudioManager>
    {
        private string bgmVolumeParam = "BGMVolume";
        private string mixerVolumeParam = "MixerVolume";
        private string sfxVolumeParam = "SFXVolume";
        // 在resource下
        private string mixerPath = "Audio/Mixer";
        private string bgmPath = "Audio/bgm/";
        private string sfxPath = "Audio/sfx/";
        //
        private AudioMixer _audioMixer;
        private AudioSource _bgmSource;
        private AudioSource _sfxSource;
        private Coroutine _fadeCoroutine;
        // 缓存，如果不是第一次加载则查找缓存
        private Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();
        private void Awake()
        {
            _audioMixer = Resources.Load<AudioMixer>(mixerPath);
            // 动态创建两个source，一个播放bgm，一个播放sfx
            if (_bgmSource == null)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
                // isloop
                _bgmSource.loop = true;
            }
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
            }
            // 绑定mixer的输出到audioSource
            var groups = _audioMixer.FindMatchingGroups("bgm");
            if (groups.Length > 0)
            {
                _bgmSource.outputAudioMixerGroup = groups[0];
            }
            groups = _audioMixer.FindMatchingGroups("sfx");
            if(groups.Length > 0)
            {
                _sfxSource.outputAudioMixerGroup = groups[0];
            }
        }
        public void PlayBGM(string name, float fadeTime = 1f)
        {
            if(!_clipCache.TryGetValue(name, out var clip))
            {
                clip = Resources.Load<AudioClip>(bgmPath + name);
                _clipCache[name] = clip;
            }
            // 开启携程
            if(_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeToNewBGM(clip, fadeTime));
        }
        public void StopBGM(float fadeTime = 1f)
        {
            // 如果正有 FadeToNewBGM 在跑，先停了它
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            // 启动一个新的淡出协程
            _fadeCoroutine = StartCoroutine(FadeOutAndStop(fadeTime));
        }

        private IEnumerator FadeOutAndStop(float duration)
        {
            // 读当前音量（分贝）并换成线性
            _audioMixer.GetFloat(bgmVolumeParam, out var startDb);
            float startVol = Mathf.Pow(10f, startDb / 20f);

            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float v = Mathf.Lerp(startVol, 0f, t / duration);
                _audioMixer.SetFloat(bgmVolumeParam, Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f);
                yield return null;
            }

            // 真正停止
            _bgmSource.Stop();
            _fadeCoroutine = null;
        }
        private IEnumerator FadeToNewBGM(AudioClip newClip, float duration)
        {
            // 先读当前音量（分贝）并换成线性
            _audioMixer.GetFloat(bgmVolumeParam, out var startDb);
            float startVol = Mathf.Pow(10f, startDb / 20f);

            float t = 0f;
            // 淡出
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float v = Mathf.Lerp(startVol, 0f, t / duration);
                _audioMixer.SetFloat(bgmVolumeParam, Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f);
                yield return null;
            }

            // 切换并播放新曲
            _bgmSource.clip = newClip;
            _bgmSource.Play();

            // 淡入
            t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float v = Mathf.Lerp(0f, 1f, t / duration);
                _audioMixer.SetFloat(bgmVolumeParam, Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f);
                yield return null;
            }

            _fadeCoroutine = null;
        }
        public void PlaySFX(string name, float volumeScale = 1f)
        {
            if (!_clipCache.TryGetValue(name, out var clip))
            {
                clip = Resources.Load<AudioClip>(sfxPath + name);
                _clipCache[name] = clip;
            }
            _sfxSource.PlayOneShot(clip, volumeScale);
        }
        // 设置音量
        // 输入0-1的数值
        public void SetBGMVolume(float normalized)
        {
            float db = Mathf.Log10(Mathf.Clamp01(normalized)) * 20f;
            _audioMixer.SetFloat(bgmVolumeParam, db);
        }
        public void SetSFXVolume(float normalized)
        {
            float db = Mathf.Log10(Mathf.Clamp01(normalized)) * 20f;
            _audioMixer.SetFloat(sfxVolumeParam, db);
        }
        public void SetMixerVolume(float normalized)
        {
            float db = Mathf.Log10(Mathf.Clamp01(normalized)) * 20f;
            _audioMixer.SetFloat(mixerVolumeParam, db);
        }
    }
}

