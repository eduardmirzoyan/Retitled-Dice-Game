using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DieUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static float rollTime = 0.5f;

    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image dieImage;
    [SerializeField] private Outline outline;
    [SerializeField] private Outline shadow;
    

    [Header("Data")]
    [SerializeField] private Die die;
    [SerializeField] private Action action;
    [SerializeField] private List<Sprite> diceSprites;
    [SerializeField] private float travelRate = 0.1f;
    [SerializeField] private float spinRate = 3f;
    [SerializeField] private bool isInteractable;

    [SerializeField] private List<Sprite> diceDisplaySprites;

    private Transform parent;
    private bool isBeingDragged;
    private int rotationDirection = 1;
    private Coroutine rollRoutine;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public void Initialize(Action action, bool isInteractable = true, bool displayOnly = false)
    {
        this.die = action.die;
        this.action = action;
        this.isInteractable = isInteractable;

        // Save parent
        parent = transform.parent;

        // Update visual
        if (displayOnly) {
            // Display image based on max value
            dieImage.sprite = diceDisplaySprites[die.maxValue - 1];
        }
        else {
            // Display image based on current value
            dieImage.sprite = diceSprites[die.value - 1];
        }

        

        // Sub to die events
        GameEvents.instance.onDieRoll += Roll;
        GameEvents.instance.onDieExhaust += Exhaust;
        GameEvents.instance.onDieReplenish += Relpenish;
    }

    public void Uninitialize() {
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

            // Draw the die value
            dieImage.sprite = diceSprites[random - 1];

            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // At the end, draw the true number that was rolled
        dieImage.sprite = diceSprites[die.value - 1];
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;

        // If die is not exhausted
        if (!die.isExhausted)
        {
            // Update visually
            canvasGroup.alpha = 0.4f;
            canvasGroup.blocksRaycasts = false;

            // Enable shadow
            shadow.enabled = true;

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
        if (travelRate == 0) {
            transform.position = point;
        }
        else {
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
            if (!die.isExhausted) {
                // Update visually
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }

            // Disable shadow
            shadow.enabled = false;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;

        // Enable outline
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable) return;

        // Disable outline
        outline.enabled = false;
    }
}
