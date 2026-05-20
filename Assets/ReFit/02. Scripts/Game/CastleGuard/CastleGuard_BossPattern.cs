using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;

public class CastleGuard_BossPattern : MonoBehaviour
{
    public Animator anim;
    public GameObject pattern_1;
    public GameObject pattern_2;
    public GameObject pattern_3;
    public Transform pattern_Parent;

    private const string PatternParentName = "BossPattern";

    Coroutine patternCoroutine;

    Vector3 spawnPos = new Vector3(1358, 2, 25);

    private void Start()
    {
        if (pattern_Parent == null)
        {
            GameObject found = GameObject.Find(PatternParentName);
            if (found != null)
                pattern_Parent = found.transform;
            else
                Debug.LogWarning($"[CastleGuard_BossPattern] '{PatternParentName}' 오브젝트를 씬에서 찾을 수 없습니다.");
        }

        patternCoroutine = StartCoroutine(RandomAttack());
    }

    IEnumerator RandomAttack()
    {
        while (true)
        {
            yield return null;

            int random = Random.Range(0, 3);

            switch (random)
            {
                case 0:
                    Instantiate(pattern_1, spawnPos, Quaternion.identity, pattern_Parent);
                    break;
                case 1:
                    Instantiate(pattern_2, spawnPos, Quaternion.identity, pattern_Parent);
                    break;
                case 2:
                    Instantiate(pattern_3, spawnPos, Quaternion.identity, pattern_Parent);
                    break;
            }

            anim.SetTrigger("attack");
            yield return new WaitForSeconds(7.0f);

        }
    }


}