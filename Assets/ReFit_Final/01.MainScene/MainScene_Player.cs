using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Security.Cryptography;

public class MainScene_Player : MonoBehaviour
{
    Animator animator;
    private Tween moveTween;

    public enum PathEnum
    {
        NoneToCastle,
        NoneToAxe,
        CastleToAxe,
        AxeToCastle,
        AxeToFight,
        FightToAxe
    }

    Vector3[][] targetPaths =
    {
        //NoneToCastle
        new Vector3[] { new Vector3(71.77f, 21.6f, 49.92f), new Vector3(71.59f, 21.6f, 51.21f), new Vector3(71.58f, 21.6f, 52.64f),
            new Vector3(71.54f, 21.6f, 54.06f), new Vector3(71.38f, 21.6f, 56.4f) },
        
        //NoneToAxe
        new Vector3[] { new Vector3(71.77f, 21.6f, 49.92f), new Vector3(69.4f, 21.6f, 48.14f), new Vector3(68.03f, 21.6f, 47.1f),
            new Vector3(66.58f, 21.8f, 46.07f), new Vector3(64.25f, 22.05f, 43.84f) },

        //CastleToAxe
        new Vector3[] { new Vector3(71.38f, 21.6f, 56.4f), new Vector3(71.56f, 21.6f, 52.63f), new Vector3(70.67f, 21.6f, 49.3f),
            new Vector3(66.27f, 21.84f, 46.07f), new Vector3(64.25f, 22.05f, 43.84f) },
        
        //AxeToCastle
        new Vector3[] { new Vector3(64.25f, 22.05f, 43.84f), new Vector3(66.27f, 21.84f, 46.07f), new Vector3(70.67f, 21.6f, 49.3f),
            new Vector3(71.56f, 21.6f, 52.63f), new Vector3(71.38f, 21.6f, 56.4f) },

        //AxeToFight
        new Vector3[] { new Vector3(64.25f, 22.05f, 43.84f), new Vector3(66.16f, 22.05f, 42.59f), new Vector3(67.66f, 22.05f, 41.21f),
            new Vector3(69.27f, 21.98f, 39.69f), new Vector3(70.45f, 21.98f, 38.08f) },

        //FighttoAxe
        new Vector3[] { new Vector3(70.45f, 21.98f, 38.08f), new Vector3(69.27f, 21.98f, 39.69f), new Vector3(67.66f, 22.05f, 41.21f),
            new Vector3(66.16f, 22.05f, 42.59f), new Vector3(64.25f, 22.05f, 43.84f) }
    };

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void MoveToPosition(PathEnum pathEnum)
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }

        Vector3[] targetPath = (Vector3[])targetPaths[(int)pathEnum].Clone();
        targetPath[0] = transform.position; // 현재 위치를 시작점으로 설정

        animator.SetBool("isMoving", true);

        float duration = 3.0f;

        moveTween = 
        transform.DOPath(targetPath, duration, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .SetLookAt(0.01f)
            .OnComplete(() =>
            {
                animator.SetBool("isMoving", false);
                transform.rotation = Quaternion.Euler(0, 116, 0); // 이동이 끝난 후 캐릭터가 바라보는 방향을 고정
            });
    }
}
