using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class DieUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Outline highlightOutline;
    [SerializeField] private Image pulseImage;
    [SerializeField] private Animator burstAnimator;
    [SerializeField] private Image[] pipImages;

    [Header("Data")]
    [SerializeField] private Die die;
    [SerializeField] private Action action;
    [SerializeField] private float travelRate = 0.1f;
    [SerializeField] private float spinRate = 3f;
    [SerializeField] private KeyCode shortcut;

    private Transform parent;
    private bool isBeingDragged;
    private int rotationDirection = 1;
    private Coroutine rollRoutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public void Initialize(Action action, KeyCode shortcut = KeyCode.None)
    {
        this.action = action;
        this.die = action.die;
        this.shortcut = shortcut;

        // Save parent
        parent = transform.parent;

        // Set pip color based on action
        foreach (var image in pipImages)
        {
            image.color = action.color;
        }

        // Display image based on current value
        DisplayValue(die.value);

        // Check exhaust state
        if (die.isExhausted)
        {
            Exhaust(die);
        }
        else
        {
            Relpenish(die);
        }

        // Update name
        gameObject.name = action.name + " Die UI";

        // Sub to die events
        GameEvents.instance.onTurnStart += StartRolling;
        GameEvents.instance.onDieRoll += Roll;
        GameEvents.instance.onDieExhaust += Exhaust;
        GameEvents.instance.onDieReplenish += Relpenish;
        GameEvents.instance.onDieBump += Bump;
    }

    public void Uninitialize()
    {
        // Unsub to die events
        GameEvents.instance.onTurnStart -= StartRolling;
        GameEvents.instance.onDieRoll -= Roll;
        GameEvents.instance.onDieExhaust -= Exhaust;
        GameEvents.instance.onDieReplenish -= Relpenish;
        GameEvents.instance.onDieBump -= Bump;
    }

    public void Pulse()
    {
        if (!die.isExhausted)
        {
            pulseImage.enabled = true;
            highlightOutline.enabled = false;
        }
    }

    public void Highlight()
    {
        if (isBeingDragged) return;

        pulseImage.enabled = false;
        highlightOutline.enabled = true;
    }

    public void Idle()
    {
        pulseImage.enabled = false;
        highlightOutline.enabled = false;
    }

    private void StartRolling(Entity entity)
    {
        // If player's turn started
        if (entity is Player)
        {
            if (die.alwaysLock || die.isLocked)
                return;

            if (rollRoutine != null)
                StopCoroutine(rollRoutine);
            rollRoutine = StartCoroutine(RollVisuals(5f));

            // Play Sound
            AudioManager.instance.PlaySFX("die_roll");
        }
    }

    private void Roll(Die die)
    {
        // If this was not the rolled die, the dip
        if (this.die != die) return;

        // Stop rolling
        if (rollRoutine != null) StopCoroutine(rollRoutine);

        // At the end, draw the true number that was rolled
        DisplayValue(die.value);

        // Play VFX
        Burst();
    }

    private IEnumerator RollVisuals(float duration)
    {
        float elapsedTime = 0;

        // Smoothly move to target
        while (elapsedTime < duration)
        {
            // Lerp target item to its spot
            int random = Random.Range(die.TrueMin, die.TrueMax + 1);

            // Draw the random die value
            DisplayValue(random);

            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Exhaust(Die die)
    {
        if (this.die == die)
        {
            canvasGroup.alpha = 0.4f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void Relpenish(Die die)
    {
        if (this.die == die)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            Pulse();
        }
    }

    private void Bump(Die die)
    {
        if (this.die == die)
        {
            // Update value
            DisplayValue(die.value);

            // Show VFX
            Burst();
        }
    }

    private void Update()
    {
        // Check if shortcut is selected
        if (Input.GetKeyDown(shortcut))
        {
            // Select this action
            GameManager.instance.SelectAction(action);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // If die is not exhausted
        if (!die.isExhausted)
        {
            // If this item is left clicked
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // Select this action
                GameManager.instance.SelectAction(action);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // If die is not exhausted
        if (!die.isExhausted)
        {
            // Update visually
            canvasGroup.alpha = 0.4f;
            canvasGroup.blocksRaycasts = false;

            // Remove from parent
            rectTransform.SetParent(transform.root);

            // Randomize rotation direction
            rotationDirection = Random.Range(0, 2) == 0 ? 1 : -1;

            // Enable flag
            isBeingDragged = true;

            // Set cursor to grab
            ResourceMananger.instance.SetGrabCursor();

            // Select this action
            GameManager.instance.SelectAction(action);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        // Nothing
    }

    private void FixedUpdate()
    {
        if (isBeingDragged)
        {
            FollowAndRotate();
        }
    }

    private void FollowAndRotate()
    {
        // Make Die smoothly travel towards mouse
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        point.z = 0;

        // If rate is set to 0, then make it immediate
        if (travelRate == 0)
        {
            transform.position = point;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, point, travelRate);
        }

        // Rotate Die
        transform.Rotate(0, 0, rotationDirection * spinRate); //rotates 50 degrees per second around z axis
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isBeingDragged)
        {
            if (!die.isExhausted)
            {
                // Update visually
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }

            // Return to parent
            rectTransform.SetParent(parent);

            // Reset rotation
            transform.rotation = Quaternion.identity;

            // Reset position
            transform.localPosition = Vector3.zero;

            // Stop dragging
            isBeingDragged = false;

            // Reset cursor
            ResourceMananger.instance.SetDefaultCursor();

            // Deselect action
            GameManager.instance.SelectAction(null);
        }

    }

    // ~~~~~ HELPERS ~~~~~

    private void Burst()
    {
        // Play animation
        burstAnimator.Play("Burst");

        // Play Sound
        AudioManager.instance.PlaySFX("die_hit");
    }

    private void DisplayValue(int value)
    {
        // Disable all pips at first
        foreach (var image in pipImages)
        {
            image.enabled = false;
        }

        // Enable certain pips based on value
        switch (value)
        {
            case 1:
                pipImages[4].enabled = true;
                break;

            case 2:
                pipImages[0].enabled = true;
                pipImages[8].enabled = true;
                break;

            case 3:
                pipImages[0].enabled = true;
                pipImages[4].enabled = true;
                pipImages[8].enabled = true;
                break;

            case 4:
                pipImages[0].enabled = true;
                pipImages[2].enabled = true;
                pipImages[6].enabled = true;
                pipImages[8].enabled = true;
                break;

            case 5:
                pipImages[0].enabled = true;
                pipImages[2].enabled = true;
                pipImages[4].enabled = true;
                pipImages[6].enabled = true;
                pipImages[8].enabled = true;
                break;

            case 6:
                pipImages[0].enabled = true;
                pipImages[2].enabled = true;
                pipImages[3].enabled = true;
                pipImages[5].enabled = true;
                pipImages[6].enabled = true;
                pipImages[8].enabled = true;
                break;

            case 7:
                pipImages[0].enabled = true;
                pipImages[2].enabled = true;
                pipImages[3].enabled = true;
                pipImages[4].enabled = true;
                pipImages[5].enabled = true;
                pipImages[6].enabled = true;
                pipImages[8].enabled = true;
                break;

            case 8:
                pipImages[0].enabled = true;
                pipImages[1].enabled = true;
                pipImages[2].enabled = true;
                pipImages[3].enabled = true;
                pipImages[5].enabled = true;
                pipImages[6].enabled = true;
                pipImages[7].enabled = true;
                pipImages[8].enabled = true;
                break;

            case 9:
                // Enable all the pips
                foreach (var image in pipImages)
                {
                    image.enabled = true;
                }
                break;

            default:
                throw new System.Exception($"Unhandled die value: {value}");
        }
    }
}
