using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Manager
{
    public enum VolumeType
    {
        bgm,
        sfx,
        mixer,
    }

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

        // 全局listener挂在audioManager下
        private AudioListener _listener;

        private void Awake()
        {
            // 添加listener
            _listener = GetComponent<AudioListener>();
            if (_listener == null)
                _listener = gameObject.AddComponent<AudioListener>();

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
            if (groups.Length > 0)
            {
                _sfxSource.outputAudioMixerGroup = groups[0];
            }
        }

        // 监听全局设置变更
        private void OnEnable()
        {
            // 不在onEnable这里黏性重放，见官方API
            // https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Audio.AudioMixer.SetFloat.html
            EventBus.Subscribe<ESettingsChanged>(OnSettingsChanged, false);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ESettingsChanged>(OnSettingsChanged);
        }

        private void Start()
        {
            // setFloat方法需要放在Start里
            SetBGMVolume(SettingsMgr.Instance.GetBGMVolume());
            SetSFXVolume(SettingsMgr.Instance.GetSFXVolume());
            SetMixerVolume(SettingsMgr.Instance.GetMixerVolume());
        }

        private void OnSettingsChanged(ESettingsChanged e)
        {
            SetBGMVolume(e.Settings.bgmVolume);
            SetSFXVolume(e.Settings.sfxVolume);
            SetMixerVolume(e.Settings.mixerVolume);
            _audioMixer.GetFloat(bgmVolumeParam, out var db);
        }

        public void PlayBGM(string name, float fadeTime = 1f)
        {
            if (!_clipCache.TryGetValue(name, out var clip))
            {
                clip = Resources.Load<AudioClip>(bgmPath + name);
                _clipCache[name] = clip;
            }
            // 开启携程
            if (_fadeCoroutine != null)
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
            float start = _bgmSource.volume;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                _bgmSource.volume = Mathf.Lerp(start, 0f, t / duration);
                yield return null;
            }

            // 真正停止
            _bgmSource.Stop();
            _bgmSource.volume = 1f;

            _fadeCoroutine = null;
        }

        private IEnumerator FadeToNewBGM(AudioClip newClip, float duration)
        {
            float startVol = _bgmSource.volume;
            // 若当前正在播放旧曲且需要过渡，则先淡出到 0
            if (_bgmSource.isPlaying && duration > 0f)
            {
                float t = 0f;
                while (t < duration)
                {
                    t += Time.unscaledDeltaTime;
                    _bgmSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
                    yield return null;
                }
            }
            else
            {
                // 设置为0f，避免爆音
                _bgmSource.volume = 0f;
            }

            // 切换并开始播放新曲
            _bgmSource.clip = newClip;
            _bgmSource.Play();

            // 从 0 淡入到1
            float target = 1f;
            float t2 = 0f;
            while (t2 < duration)
            {
                t2 += Time.unscaledDeltaTime;
                _bgmSource.volume = Mathf.Lerp(0f, target, t2 / duration);
                yield return null;
            }
            _bgmSource.volume = target;

            _fadeCoroutine = null;
        }
        public void StopSFXImmediate()
        {
            _sfxSource.Stop();
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

        // 设置标量化音量
        public void SetBGMVolume(float db)
        {
            _audioMixer.SetFloat(bgmVolumeParam, db);
        }

        public void SetSFXVolume(float db)
        {
            _audioMixer.SetFloat(sfxVolumeParam, db);
        }

        public void SetMixerVolume(float db)
        {
            _audioMixer.SetFloat(mixerVolumeParam, db);
        }

        // 设置标量化音量
        public void SetBGMVolumeNormalized(float normalized)
        {
            float db;
            if (normalized <= 0f)
            {
                db = -80f;
            }
            else
            {
                db = Mathf.Lerp(-10f, 5f, Mathf.Clamp01(normalized));
            }
            EventBus.Publish(new EVolumeSet(db, VolumeType.bgm));
        }

        public void SetSFXVolumeNormalized(float normalized)
        {
            float db;
            if (normalized <= 0f)
            {
                db = -80f;
            }
            else
            {
                db = Mathf.Lerp(-10f, 5f, Mathf.Clamp01(normalized));
            }
            EventBus.Publish(new EVolumeSet(db, VolumeType.sfx));
        }

        public void SetMixerVolumeNormalized(float normalized)
        {
            float db;
            if (normalized <= 0f)
            {
                db = -80f;
            }
            else
            {
                db = Mathf.Lerp(-10f, 5f, Mathf.Clamp01(normalized));
            }
            EventBus.Publish(new EVolumeSet(db, VolumeType.mixer));
        }

        // 获取标量化音量
        public float GetBGMVolumeNormalized()
        {
            var db = SettingsMgr.Instance.GetBGMVolume();
            if (db <= -80f)
            {
                return 0f;
            }
            return Mathf.InverseLerp(-10f, 5f, db);
        }

        public float GetSFXVolumeNormalized()
        {
            var db = SettingsMgr.Instance.GetSFXVolume();
            if (db <= -80f)
            {
                return 0f;
            }
            return Mathf.InverseLerp(-10f, 5f, db);
        }

        public float GetMixerVolumeNormalized()
        {
            var db = SettingsMgr.Instance.GetMixerVolume();
            if (db <= -80f)
            {
                return 0f;
            }
            return Mathf.InverseLerp(-10f, 5f, db);
        }
    }
}
