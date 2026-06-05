using UnityEngine;

public class ProfileManager : MonoBehaviour, IReFitManager
{
    [SerializeField] private ProfileData UserData = new ProfileData();
    private struct ProfileData
    {
        public string UserName;
        public int Age;
        public float Height;
        public float Weight;
        public float ShoulderFlexionROM;
        public float ShoulderExtensionROM;
        public float WaistRotationRightROM;
        public float WaistRotationLeftROM;
        //손목, 팔꿈치, 고관절, 무릎 등 추가 가능
        //Flexion(굴곡), Extension(신전), Abduction(내전), Adduction(외전), Rotation 등 추가 가능
    }
    //------------------------------------------
    public void ResetReFitManager()
    {
        SetTempData();
    }

    public void UpdateReFitManager()
    {
        //업데이트
    }

    void SetTempData()
    {
        UserData.UserName = "턱티노";
        UserData.Age = 25;
        UserData.Height = 173.5f;
        UserData.Weight = 70.0f;
        UserData.ShoulderFlexionROM = 190.0f;
        UserData.ShoulderExtensionROM = 45.0f;
        UserData.WaistRotationRightROM = 90.0f;
        UserData.WaistRotationLeftROM = 90.0f;
    }
}
