using TMPro;
using UnityEngine;

public class Profile : MonoBehaviour
{
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI handText;
    public TextMeshProUGUI diagnosticText;

    public void SetProfileUI()
    {
        ProfileManager pm = GameManager.instance.MyProfileManager;

        heightText.text = pm._currentProfile.heightCm.ToString("#.#");
        weightText.text = pm._currentProfile.weightKg.ToString("#.#");

        switch (pm._currentProfile.dominantHand)
        {
            case "r":
            case "R":
                handText.text = "RIGHT";
                break;
            case "l":
            case "L":
                handText.text = "LEFT";
                break;
            case "b":
            case "B":
                handText.text = "BOTH";
                break;
        }

        diagnosticText.text = "";
        foreach (string tags in pm._currentProfile.diagnosisTags)
        {
            diagnosticText.text += tags;
            diagnosticText.text += "\n";
        }
    }
}
