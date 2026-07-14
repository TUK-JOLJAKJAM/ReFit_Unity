using UnityEngine;

public class FightScene_Skill : MonoBehaviour
{
    public Transform LeftSkill;
    public Transform RightSkill;
    public Transform CenterSKill;

    public void SetSkillUI(FightScene_Logic.Skill newSkill)
    {
        foreach (Transform child in LeftSkill)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in RightSkill)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in CenterSKill)
        {
            child.gameObject.SetActive(false);
        }


        switch (newSkill)
        {
            case FightScene_Logic.Skill.Red:
                LeftSkill.GetChild(2).gameObject.SetActive(true);
                RightSkill.GetChild(2).gameObject.SetActive(true);
                CenterSKill.GetChild(2).gameObject.SetActive(true);
                break;
            case FightScene_Logic.Skill.Green:
                LeftSkill.GetChild(0).gameObject.SetActive(true);
                RightSkill.GetChild(0).gameObject.SetActive(true);
                CenterSKill.GetChild(0).gameObject.SetActive(true);
                break;
            case FightScene_Logic.Skill.Blue:
                LeftSkill.GetChild(1).gameObject.SetActive(true);
                RightSkill.GetChild(1).gameObject.SetActive(true);
                CenterSKill.GetChild(1).gameObject.SetActive(true);
                break;
        }
    }
}
