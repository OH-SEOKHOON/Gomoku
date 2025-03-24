using UnityEngine;

public static class UserInformations
{
    private const string CREDITS = "Credits";
    private const string RANKPOINTS = "Rankpoints";
    private const string PROFILEINDEX = "ProfileIndex";
    private const string BGMVOL = "BGMVol";
    private const string SFXVOL = "SFXVol";

    // 크레딧
    public static int Credits
    {
        get
        { return PlayerPrefs.GetInt(CREDITS, 100); }
        set
        { PlayerPrefs.SetInt(CREDITS, value); }
    }
    
    // 랭크포인트
    public static int Rankpoints
    {
        get { return PlayerPrefs.GetInt(RANKPOINTS, 3); }
        set { PlayerPrefs.SetInt(RANKPOINTS, value); }
    }
    
    public static int ProfileIndex
    {
        get { return PlayerPrefs.GetInt(PROFILEINDEX, 0); }
        set { PlayerPrefs.SetInt(PROFILEINDEX, value); }
    }
    
    // bgm볼륨
    public static float BGMVol
    {
        get { return PlayerPrefs.GetFloat(BGMVOL, 1); }
        set { PlayerPrefs.SetFloat(BGMVOL, value); }
    }
    
    
    // sfx볼륨
    public static float SFXVol
    {
        get { return PlayerPrefs.GetFloat(SFXVOL, 1); }
        set { PlayerPrefs.SetFloat(SFXVOL, value); }
    }
}