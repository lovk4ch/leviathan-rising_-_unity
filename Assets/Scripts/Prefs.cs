using UnityEngine;

public class Prefs
{
    private const string _firstPass = "firstPass";
    public static bool FirstPass
    {
        get {
            if (PlayerPrefs.HasKey(_firstPass))
                return PlayerPrefs.GetInt(_firstPass) == 1;

            return false;
        }
        set {
            PlayerPrefs.SetInt(_firstPass, value ? 1 : 0);
        }
    }
}