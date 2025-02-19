using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 类：AudioMgr
/// 描述：管理游戏中的音效和背景音乐，提供音量控制、播放功能以及从Resources文件夹加载音频资源的功能。
/// </summary>
public class AudioMgr : UnitySingleton<AudioMgr>
{
    #region 常量与字段

    private Dictionary<string, AudioSource> audioSources;

    public const string BGM_TYPE = "BGM";
    public const string SFX_TYPE = "SFX";
    public const string GUIDE_TYPE = "Guide";

    private float bgmVolume = 1.0f; // 背景音乐音量
    private float sfxVolume = 1.0f; // 音效音量
    private float guideVolume = 1.0f; // 引导音效音量

    private const string basePath = "HQG/";
    // 缓存已加载的 AudioClip
    private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();
    #endregion

    #region 初始化


    private new void Awake()
    {
        base.Awake();
        // 为每种音频类型创建一个音频源并存入字典
        audioSources = new Dictionary<string, AudioSource>
        {
            { BGM_TYPE, CreateAudioSource("BGMSource", loop: true) }, // 背景音乐
            { SFX_TYPE, CreateAudioSource("SFXSource", loop: false) }, // 特效音乐
            { GUIDE_TYPE, CreateAudioSource("GUIDESource", loop: false) } // 引导音乐
        };
    }
    public override void Initialize(params object[] args)
    {
        // 设置初始音量
        SetVolume(BGM_TYPE, bgmVolume);
        SetVolume(SFX_TYPE, sfxVolume);
        SetVolume(GUIDE_TYPE, guideVolume);
    }

    /// <summary>
    /// 创建并配置 AudioSource
    /// </summary>
    /// <param name="name">AudioSource 名称</param>
    /// <param name="loop">是否循环播放</param>
    /// <returns>配置好的 AudioSource</returns>
    private AudioSource CreateAudioSource(string name, bool loop)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = this.transform;
        AudioSource source = obj.AddComponent<AudioSource>();
        source.loop = loop;
        return source;
    }

    #endregion

    #region 音量管理

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="audioType">音频类型</param>
    /// <param name="volume">音量大小</param>
    public void SetVolume(string audioType, float volume)
    {
        if (audioSources.TryGetValue(audioType, out var source))
        {
            source.volume = volume;
            switch (audioType)
            {
                case BGM_TYPE: bgmVolume = volume; break;
                case SFX_TYPE: sfxVolume = volume; break;
                case GUIDE_TYPE: guideVolume = volume; break;
            }
        }
        else
        {
            Debug.LogError($"音频源类型未找到：{audioType}");
        }
    }

    /// <summary>
    /// 获取音量
    /// </summary>
    /// <param name="audioType">音频类型</param>
    /// <returns>音量大小</returns>
    public float GetVolume(string audioType)
    {
        return audioSources.TryGetValue(audioType, out var source) ? source.volume : 0f;
    }

    #endregion

    #region 音频播放方法

    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="clip">音频剪辑</param>
    /// <param name="onAudioFinished">音频播放完成时的回调</param>
    /// <param name="token">取消令牌</param>
    public async UniTask PlayAudio(AudioClip clip, Action onAudioFinished = null, CancellationToken token = default)
    {
        if (clip == null)
        {
            Log("尝试播放的 AudioClip 为 null。", LogColor.Red);
            return;
        }

        // 获取适当的 AudioSource
        AudioSource audioSource = GetPooledAudioSource(SFX_TYPE);
        if (audioSource == null)
        {
            Log("无法获取空闲的 AudioSource。", LogColor.Red);
            return;
        }

        // 设置并播放音频剪辑
        audioSource.clip = clip;
        audioSource.Play();

        // 如果有回调，等待音频播放完成后触发
        if (onAudioFinished != null)
        {
            if (clip.length <= 0)
            {
                Log("音频时长为0，无法播放音频！", LogColor.Red);
                return;
            }

            if (token.IsCancellationRequested)
            {
                Log("任务已被取消.");
                return; // 直接返回，避免后续操作
            }

            try
            {
                // 等待音频播放完成或取消
                await UniTask.Delay(TimeSpan.FromSeconds(clip.length), cancellationToken: token);
                onAudioFinished?.Invoke(); // 触发音频播放完毕回调
            }
            catch (OperationCanceledException)
            {
                Log("音频播放被取消.");
            }
        }
    }

    /// <summary>
    /// 获取可用 AudioSource
    /// </summary>
    /// <param name="type">音频类型</param>
    /// <param name="pool">对应类型的对象池</param>
    /// <returns>可用的 AudioSource</returns>
    private AudioSource GetPooledAudioSource(string type)
    {
        if (audioSources.TryGetValue(type, out var bgmSource))
        {
            return bgmSource;
        }
        return null;
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="clip">AudioClip</param>
    /// <param name="loop">是否循环播放</param>
    /// <param name="token">取消令牌</param>
    /// <returns>异步任务</returns>
    private void PlayBGMAsync(AudioClip clip, bool loop, CancellationToken token)
    {
        if (audioSources.TryGetValue(BGM_TYPE, out var bgmSource))
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
            // await UniTask.WaitUntil(() => !bgmSource.isPlaying, cancellationToken: token);
        }
        else
        {
            Debug.LogError($"音频源类型未找到：{BGM_TYPE}");
        }
    }

    /// <summary>
    /// 播放音效（SFX）
    /// </summary>
    /// <param name="clip">AudioClip</param>
    /// <param name="token">取消令牌</param>
    private void PlaySFX(AudioClip clip, CancellationToken token)
    {
        AudioSource sfxSource = GetPooledAudioSource(SFX_TYPE);
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
            // 异步任务处理播放完成
            HandleAudioCompletionAsync(sfxSource, clip.length, token).Forget();
        }
    }

    /// <summary>
    /// 播放引导音效（Guide）
    /// </summary>
    /// <param name="clip">AudioClip</param>
    /// <param name="token">取消令牌</param>
    private void PlayGuide(AudioClip clip, CancellationToken token)
    {
        AudioSource guideSource = GetPooledAudioSource(GUIDE_TYPE);
        if (guideSource != null)
        {
            guideSource.clip = clip;
            guideSource.loop = false;
            guideSource.Play();
            // 异步任务处理播放完成
            HandleAudioCompletionAsync(guideSource, clip.length, token).Forget();
        }
    }

    /// <summary>
    /// 处理音频播放完成后的回收
    /// </summary>
    /// <param name="source">AudioSource</param>
    /// <param name="delaySeconds">延迟秒数</param>
    /// <param name="token">取消令牌</param>
    /// <returns>异步任务</returns>
    private async UniTaskVoid HandleAudioCompletionAsync(AudioSource source, float delaySeconds, CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: token);
            source.Stop();
            source.clip = null;
        }
        catch (OperationCanceledException)
        {
            Log("音效播放等待回调被取消");
        }
    }

    #endregion

    #region 公共播放方法

    /// <summary>
    /// 从Resources文件夹加载并播放背景音乐
    /// </summary>
    /// <param name="resourcePath">资源路径（相对于 Resources/ZJNU/）</param>
    /// <param name="loop">是否循环播放</param>
    /// <param name="token">取消令牌</param>
    /// <returns>异步任务</returns>
    public void PlayBGMFromResourcesAsync(string resourcePath, bool loop = true, CancellationToken token = default)
    {
        AudioClip clip = LoadAudioClip(resourcePath);
        if (clip == null)
        {
            Log($"音频资源加载失败：{resourcePath}", LogColor.Red);
            return;
        }
        PlayBGMAsync(clip, loop, token);
    }

    /// <summary>
    /// 从Resources文件夹加载并播放音效（SFX）
    /// </summary>
    /// <param name="resourcePath">资源路径（相对于 Resources/ZJNU/）</param>
    /// <param name="token">取消令牌</param>
    public void PlaySFXFromResources(string resourcePath, CancellationToken token = default)
    {
        AudioClip clip = LoadAudioClip(resourcePath);
        if (clip == null)
        {
            Log($"音频资源加载失败：{resourcePath}", LogColor.Red);
            return;
        }

        PlaySFX(clip, token);
    }

    /// <summary>
    /// 从Resources文件夹加载并播放引导音效（Guide）
    /// </summary>
    /// <param name="resourcePath">资源路径（相对于 Resources/ZJNU/）</param>
    /// <param name="token">取消令牌</param>
    public void PlayGuideFromResources(string resourcePath, CancellationToken token = default)
    {
        AudioClip clip = LoadAudioClip(resourcePath);
        if (clip == null)
        {
            Log($"音频资源加载失败：{resourcePath}", LogColor.Red);
            return;
        }

        PlayGuide(clip, token);
    }

    /// <summary>
    /// 从Resources文件夹加载并播放延迟的音效（SFX）
    /// </summary>
    /// <param name="resourcePath">资源路径（相对于 Resources/ZJNU/）</param>
    /// <param name="delay">延迟秒数</param>
    /// <param name="token">取消令牌</param>
    /// <returns>异步任务</returns>
    public async UniTask PlayDelaySFXFromResourcesAsync(string resourcePath, float delay, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            Log("任务已被取消.", LogColor.Yellow);
            return; // 直接返回，避免重复取消
        }

        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
        PlaySFXFromResources(resourcePath, token);
    }

    /// <summary>
    /// 从Resources文件夹加载并播放延迟的引导音效（Guide）
    /// </summary>
    /// <param name="resourcePath">资源路径（相对于 Resources/ZJNU/）</param>
    /// <param name="token">取消令牌</param>
    /// <returns>异步任务</returns>
    public async UniTask PlayDelayGuideFromResourcesAsync(string resourcePath, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            Log("任务已被取消.", LogColor.Yellow);
            return; // 直接返回，避免重复取消
        }
        // 检查缓存中是否存在该音频
        if (clipCache.TryGetValue(resourcePath, out AudioClip cachedClip))
        {
            PlayGuide(cachedClip, token);
            return;
        }
        // 如果缓存中没有，加载音频
        AudioClip clip = await LoadAudioClipAsync(resourcePath);
        if (clip == null)
        {
            Log($"AudioClip 未找到: {resourcePath}", LogColor.Red);
            return;
        }

        // 将加载的音频存入缓存
        clipCache.Add(resourcePath, clip);

        // 播放音频
        PlayGuide(clip, token);
    }

    #endregion

    #region 资源加载

    /// <summary>
    /// 从Resources文件夹加载音频剪辑
    /// </summary>
    /// <param name="resourcePath">资源路径（相对于 Resources/ZJNU/）</param>
    /// <returns>加载的 AudioClip，如果未找到则返回 null</returns>
    public AudioClip LoadAudioClip(string resourcePath)
    {
        string fullPath = basePath + resourcePath;
        AudioClip clip = Resources.Load<AudioClip>(fullPath);
        if (clip == null)
        {
            Log($"音频资源加载失败：{fullPath}", LogColor.Red);
        }
        return clip;
    }

    /// <summary>
    /// 异步加载音频剪辑
    /// </summary>
    /// <param name="audioPath">音频路径（相对于 Resources 文件夹）</param>
    /// <returns>加载的 AudioClip，如果未找到则返回 null</returns>
    private async UniTask<AudioClip> LoadAudioClipAsync(string audioPath)
    {
        ResourceRequest request = Resources.LoadAsync<AudioClip>(audioPath);
        await request.ToUniTask(cancellationToken: default);
        AudioClip clip = request.asset as AudioClip;
        if (clip == null)
        {
            Log($"AudioClip 未找到: {audioPath}", LogColor.Yellow);
        }
        return clip;
    }

    #endregion

    #region 音频控制方法

    /// <summary>
    /// 暂停音效
    /// </summary>
    /// <param name="audioType">音频类型</param>
    public void PauseAudio(string audioType) => ControlAudio(audioType, source => source.Pause());

    /// <summary>
    /// 恢复音效
    /// </summary>
    /// <param name="audioType">音频类型</param>
    public void ResumeAudio(string audioType) => ControlAudio(audioType, source => source.UnPause());

    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="audioType">音频类型</param>
    public void StopAudio(string audioType) => ControlAudio(audioType, source => source.Stop());

    /// <summary>
    /// 通用音源控制方法
    /// </summary>
    /// <param name="type">音频类型</param>
    /// <param name="action">对 AudioSource 的操作</param>
    private void ControlAudio(string type, Action<AudioSource> action)
    {
        if (audioSources.TryGetValue(type, out var source))
        {
            action?.Invoke(source);
        }
        else
        {
            Log($"音频源类型未找到：{type}", LogColor.Red);
        }
    }

    #endregion
}