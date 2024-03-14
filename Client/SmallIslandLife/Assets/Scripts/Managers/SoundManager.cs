using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [System.Serializable]
    public struct Sound
    {
        public string name;
        public AudioClip clip;
    }

    // 소리 정보
    [SerializeField]
    private Sound[] sfx;
    [SerializeField]
    private Sound[] bgm;

    // 소리 재생 컴포넌트
    [SerializeField]
    private AudioSource bgmPlayer;
    [SerializeField]
    private AudioSource[] sfxPlayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region BGM

    public void PlayBGM(string p_bgmName)
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (p_bgmName.Equals(bgm[i].name))
            {
                bgmPlayer.clip = bgm[i].clip;

                bgmPlayer.Play();

                return;
            }
        }
    }

    public void PauseBGM()
    {
        bgmPlayer.Pause();
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    #endregion

    #region SFX

    public void PlaySFX(string p_sfxName)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (p_sfxName.Equals(sfx[i].name))
            {
                for (int j = 0; j < sfxPlayer.Length; j++)
                {
                    if (!sfxPlayer[j].isPlaying)
                    {
                        sfxPlayer[j].clip = sfx[i].clip;

                        sfxPlayer[j].Play();

                        return;
                    }
                }

                return;
            }
        }
    }

    public void StopSFX(string p_sfxName)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (p_sfxName.Equals(sfx[i].name))
            {
                for (int j = 0; j < sfxPlayer.Length; j++)
                {
                    if (!sfxPlayer[j].isPlaying)
                    {
                        sfxPlayer[j].Stop();

                        return;
                    }
                }

                return;
            }
        }
    }

    public void StopAllSFX()
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            sfxPlayer[i].Stop();
        }
    }

    #endregion
}
