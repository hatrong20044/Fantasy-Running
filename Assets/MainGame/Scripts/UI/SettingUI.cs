using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] private Slider music;
    [SerializeField] private Slider sfx;

    [SerializeField] private TMPro.TMP_InputField nameInput;
    private void Start()
    {
        if (PlayerPrefs.HasKey(GameSetting.MUSIC_VOLUME) && PlayerPrefs.HasKey(GameSetting.SFX_VOLUME)){
            this.LoadVolume();
        }
        else
        {
            music.value = PlayerPrefs.GetFloat(GameSetting.MUSIC_VOLUME);
            sfx.value = PlayerPrefs.GetFloat(GameSetting.SFX_VOLUME);
        }
        this.nameInput.text = GameData.Instance.UserName;
        nameInput.onEndEdit.AddListener(GetName);
    }

    public void OpenSettingUI()
    {
        UIManager.Instance.ShowUI(UIName.Setting);
    }
    public void CloseSettingUI()
    {
        UIManager.Instance.HideUI(UIName.Setting);
    }

    public void ChangeMusic()
    {
        SoundManager.Instance.SetMusicVolume(music.value);
        PlayerPrefs.SetFloat(GameSetting.MUSIC_VOLUME, music.value);
    }

    public void ChangeSfx()
    {
        SoundManager.Instance.SetSFXVolume(sfx.value);
        PlayerPrefs.SetFloat(GameSetting.SFX_VOLUME, sfx.value);    
    }

    public void LoadVolume()
    {
        music.value = PlayerPrefs.GetFloat(GameSetting.MUSIC_VOLUME);
        sfx.value = PlayerPrefs.GetFloat (GameSetting.SFX_VOLUME);
        
        this.ChangeMusic();
        this.ChangeSfx();
    }

    public void GetName(string name)
    {
        GameData.Instance.UserName = name;
    }
}
