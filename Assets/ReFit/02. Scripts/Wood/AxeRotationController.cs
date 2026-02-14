using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AxeRotationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform axeTransform;
    [SerializeField] private Button slowSwingButton;
    [SerializeField] private Button normalSwingButton;
    [SerializeField] private Button fastSwingButton;
    [SerializeField] private Button customSwingButton;

    [Header("Rotation Settings")]
    private const float START_ROTATION_Z = 12f;
    private const float END_ROTATION_Z = -69f;
    private const float SLOW_SPEED = 50f;
    private const float NORMAL_SPEED = 150f;
    private const float FAST_SPEED = 300f;
    private const float CUSTOM_SPEED = 100f;

    private bool isRotating = false;
    private bool isSwingingForward = false;
    private bool isCustomSwinging = false;
    private Coroutine customSwingCoroutine = null;

    public bool IsSwingingForward => isSwingingForward;

    public static AxeRotationController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (slowSwingButton != null)
            slowSwingButton.onClick.AddListener(() => StartSwing(SLOW_SPEED));
        
        if (normalSwingButton != null)
            normalSwingButton.onClick.AddListener(() => StartSwing(NORMAL_SPEED));
        
        if (fastSwingButton != null)
            fastSwingButton.onClick.AddListener(() => StartSwing(FAST_SPEED));

        if (customSwingButton != null)
        {
            EventTrigger trigger = customSwingButton.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = customSwingButton.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => { OnCustomSwingDown(); });
            trigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => { OnCustomSwingUp(); });
            trigger.triggers.Add(pointerUpEntry);
        }
    }

    private void OnCustomSwingDown()
    {
        if (!isRotating)
        {
            isCustomSwinging = true;
            customSwingCoroutine = StartCoroutine(CustomSwing());
        }
    }

    private void OnCustomSwingUp()
    {
        isCustomSwinging = false;
    }

    private IEnumerator CustomSwing()
    {
        isRotating = true;
        isSwingingForward = true;
        SetButtonsInteractable(false);

        float currentZ = START_ROTATION_Z;
        float targetZ = END_ROTATION_Z;
        bool reachedEnd = false;

        while (isCustomSwinging || reachedEnd)
        {
            if (isCustomSwinging && !reachedEnd)
            {
                currentZ -= CUSTOM_SPEED * Time.deltaTime;
                
                if (currentZ <= targetZ)
                {
                    currentZ = targetZ;
                    reachedEnd = true;
                    isCustomSwinging = false;
                }
            }
            else if (reachedEnd)
            {
                isSwingingForward = false;
                currentZ += NORMAL_SPEED * Time.deltaTime;
                
                if (currentZ >= START_ROTATION_Z)
                {
                    currentZ = START_ROTATION_Z;
                    break;
                }
            }

            Vector3 newRotation = axeTransform.localEulerAngles;
            newRotation.z = currentZ;
            axeTransform.localEulerAngles = newRotation;
            
            yield return null;
        }

        Vector3 finalRotation = axeTransform.localEulerAngles;
        finalRotation.z = START_ROTATION_Z;
        axeTransform.localEulerAngles = finalRotation;

        isRotating = false;
        isSwingingForward = false;
        SetButtonsInteractable(true);
    }

    private void StartSwing(float speed)
    {
        if (!isRotating)
        {
            StartCoroutine(SwingAxe(speed));
        }
    }

    private IEnumerator SwingAxe(float swingSpeed)
    {
        isRotating = true;
        isSwingingForward = true;
        SetButtonsInteractable(false);

        Vector3 currentRotation = axeTransform.localEulerAngles;
        float startZ = START_ROTATION_Z;
        float targetZ = END_ROTATION_Z;
        
        yield return StartCoroutine(RotateToTarget(startZ, targetZ, swingSpeed));
        
        isSwingingForward = false;
        
        yield return StartCoroutine(RotateToTarget(targetZ, startZ, NORMAL_SPEED));

        isRotating = false;
        isSwingingForward = false;
        SetButtonsInteractable(true);
    }

    private IEnumerator RotateToTarget(float fromZ, float toZ, float speed)
    {
        float currentZ = fromZ;
        float direction = Mathf.Sign(toZ - fromZ);
        
        while (Mathf.Abs(currentZ - toZ) > 0.1f)
        {
            currentZ += direction * speed * Time.deltaTime;
            
            if (direction > 0 && currentZ > toZ)
                currentZ = toZ;
            else if (direction < 0 && currentZ < toZ)
                currentZ = toZ;

            Vector3 newRotation = axeTransform.localEulerAngles;
            newRotation.z = currentZ;
            axeTransform.localEulerAngles = newRotation;
            
            yield return null;
        }

        Vector3 finalRotation = axeTransform.localEulerAngles;
        finalRotation.z = toZ;
        axeTransform.localEulerAngles = finalRotation;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (slowSwingButton != null)
            slowSwingButton.interactable = interactable;
        
        if (normalSwingButton != null)
            normalSwingButton.interactable = interactable;
        
        if (fastSwingButton != null)
            fastSwingButton.interactable = interactable;

        if (customSwingButton != null)
            customSwingButton.interactable = interactable;
    }
}
