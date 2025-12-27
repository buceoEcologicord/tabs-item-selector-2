using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabBuilder : MonoBehaviour
{
    [SerializeField] Image tabIconImage;
    [SerializeField] TextMeshProUGUI tabLabel;
    [SerializeField] Color tabColor;
    [SerializeField] Selectable clickableComponent;

    Category _category;

    public UnityEvent<Category> OnClick = new UnityEvent<Category>();

    // ------------------------------------------------------
    // Public APIs:
    void AssignTabIcons()
    {
        if (_category.CategoryIcon != null)
            tabIconImage.sprite = _category.CategoryIcon;
    }
    void AssignTabLabels()
    {
        if (tabLabel != null)
            tabLabel.text = _category.CategoryName;
    }
    void SetButtonActions(Button button)
    {
        // button.onClick.RemoveAllListeners(); // Uncomment to override other previous listeners        
        button.onClick.AddListener(HandleClick);
    }
    void SetToggleActions(Toggle toggle)
    {
        // toggle.onValueChanged.RemoveAllListeners(); // Uncomment to override other previous listeners  
        toggle.onValueChanged.AddListener((isOn) => { if (isOn) HandleClick(); });
    }
    void HandleClick()
    {
        OnClick.Invoke(_category);
        Debug.Log($"TabBuilder: Clicked on tab {_category.CategoryName}");
    }
    // ------------------------------------------------------
    // ------------------------------------------------------

    public void LoadTabPrefab(Category category)
    {        
        _category = category;

        AssignTabIcons();
        AssignTabLabels();
        SetClickableComponent();
    }    
    void SetClickableComponent()
    {
        if (clickableComponent == null) { Debug.LogWarning("TabBuilder: Clickable component is not assigned."); return; }

        if (clickableComponent is Button button)
            SetButtonActions(button);
        else if (clickableComponent is Toggle toggle)
            SetToggleActions(toggle);        
    }    

    

    private void OnValidate()
    {
        if (clickableComponent != null &&
            !(clickableComponent is Button) &&
            !(clickableComponent is Toggle))
        {
            Debug.LogError($"Item Builder{name}: Only Button or Toggle are allowed as clicable component in the Item prefab's ItemBuilder component");
            clickableComponent = null;
        }
    }
    private void OnDestroy()
    {
        if (clickableComponent is Button button)
        {
            button.onClick.RemoveAllListeners();
        }
        else if (clickableComponent is Toggle toggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }
}
