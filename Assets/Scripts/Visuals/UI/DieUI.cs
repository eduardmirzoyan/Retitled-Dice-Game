using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DieUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public static float rollTime = 0.5f;

    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private Image[] pipImages;

    [Header("Data")]
    [SerializeField] private Die die;
    [SerializeField] private Action action;
    [SerializeField] private float travelRate = 0.1f;
    [SerializeField] private float spinRate = 3f;
    [SerializeField] private bool isInteractable;
    [SerializeField] private bool isSelected;

    private Transform parent;
    private bool isBeingDragged;
    private int rotationDirection = 1;
    private Coroutine rollRoutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Action action, bool isInteractable = true)
    {
        this.die = action.die;
        this.action = action;
        this.isInteractable = isInteractable;
        isSelected = false;

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
        gameObject.name = action.name + " Die Model";

        // Sub to die events
        GameEvents.instance.onDieRoll += Roll;
        GameEvents.instance.onDieExhaust += Exhaust;
        GameEvents.instance.onDieReplenish += Relpenish;
    }

    public void Uninitialize()
    {
        // Unsub to die events
        GameEvents.instance.onDieRoll -= Roll;
        GameEvents.instance.onDieExhaust -= Exhaust;
        GameEvents.instance.onDieReplenish -= Relpenish;
    }

    public void Roll(Die die)
    {
        // If this was not the rolled die, the dip
        if (this.die != die) return;

        if (rollRoutine != null) StopCoroutine(rollRoutine);

        rollRoutine = StartCoroutine(RollVisuals(rollTime));
    }

    private IEnumerator RollVisuals(float duration)
    {
        float elapsedTime = 0;

        // Smoothly move to target
        while (elapsedTime < duration)
        {
            // Lerp target item to its spot
            int random = Random.Range(1, die.maxValue + 1);

            // Draw the random die value
            DisplayValue(random);

            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // At the end, draw the true number that was rolled
        DisplayValue(die.value);
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
        }
    }

    public void Pulse()
    {
        if (!die.isExhausted)
        {
            animator.Play("Pulse");
            isSelected = false;
        }

    }

    public void Idle()
    {
        animator.Play("Idle");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInteractable) return;

        // If die is not exhausted
        if (!die.isExhausted)
        {
            // If this item is left clicked
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (isSelected)
                {
                    // Unselect this action
                    GameManager.instance.SelectAction(null);

                    // Stop animation
                    animator.Play("Pulse");

                    // Update state
                    isSelected = false;
                }
                else
                {
                    // Select this action
                    GameManager.instance.SelectAction(action);

                    // Play animation
                    animator.Play("Outline");

                    // Update state
                    isSelected = true;
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;

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
            GameManager.instance.SetGrabCursor();

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
        if (!isInteractable) return;

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
            GameManager.instance.SetDefaultCursor();

            // Deselect action
            GameManager.instance.SelectAction(null);
        }

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
                // Debug
                print("ERROR displaying die value: " + value);
                break;
        }
    }
}
