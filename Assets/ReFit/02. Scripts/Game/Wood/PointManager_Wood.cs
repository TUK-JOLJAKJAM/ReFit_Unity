using UnityEngine;

public class PointManager_Wood : MonoBehaviour
{
    // ==================== Inspector UI ====================

    [Header("*** Read Only***")]
    [SerializeField] private int _point = 0;
    [SerializeField] private bool _isSwinging = false;
    [SerializeField] private float _totalPlayTime = 0f;


    // ==================== Hidden Datas ====================
    private bool _isValidSwing = false;

    // ==================== Functions =======================
    private void Awake()
    {
        _totalPlayTime = Time.time;
    }
    public void AddPoint()
    {
        _point++;
    }

    public void resetPoint()
    {
        _point = 0;
    }

    public int GetTotalTimeSec()
    {
        return Mathf.RoundToInt(Time.time - _totalPlayTime);
    }

    public void SetValidSwing(bool isValid)
    {
        _isValidSwing = isValid;
    }

    public bool IsValidSwing()
    {
        return _isValidSwing;
    }

    public void SetSwinging(bool isSwinging)
    {
        _isSwinging = isSwinging;
    }

    public bool isSwinging()
    {
        return _isSwinging;
    }

    public int GetPoint()
    {
        return _point;
    }
}
