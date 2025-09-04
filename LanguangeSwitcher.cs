using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
    // Fungsi ini akan dipanggil oleh tombol bendera Inggris
    public void SetLanguageToEnglish()
    {
        SetLanguage("en");
    }

    // Fungsi ini akan dipanggil oleh tombol bendera Indonesia
    public void SetLanguageToIndonesian()
    {
        SetLanguage("id-ID");
    }

    private void SetLanguage(string languageCode)
    {
        // Temukan Locale berdasarkan kodenya ("en", "id", dll.)
        Locale targetLocale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
        
        if (targetLocale != null)
        {
            Debug.Log($"Setting language to {targetLocale.LocaleName}");
            LocalizationSettings.SelectedLocale = targetLocale;
        }
        else
        {
            Debug.LogWarning($"Locale with code '{languageCode}' could not be found!");
        }
    }
}