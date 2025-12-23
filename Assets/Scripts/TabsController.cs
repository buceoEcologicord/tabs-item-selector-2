using System;
using System.Collections.Generic;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    [SerializeField] TabsItemSelectorManager tabsItemSelectorManager;

    // Advanced settings variables
    TabsItemSelectorSettings tabsItemSelectorSettings = new TabsItemSelectorSettings();

    // tabs variables
    List<GameObject> tabsList = new List<GameObject>();
    Dictionary<string,Component> _tabsDictionary = new Dictionary<string,Component>();
    string _activeSectionName;

    bool isButton = false;
    bool isToggle = false;
    bool isImage = false;

    bool _swapImageWhenSelected;
    Image _imageUnselected;
    Image _imageSelected;
    
    bool _forceColors = false;
    Color _normalColor;
    Color _selectedColor;


    // Getters and setters
    public List<GameObject> TabsList { get { return tabsList; } set { tabsList = value; } }


    public void AddTabButtonAction(Button button, Action<string> onTabClicked, string sectionName)
    {        
        // Adds any anonymous function with no parameters as listener to this button onClick
        button.onClick.AddListener(() => { onTabClicked?.Invoke(sectionName); });
    }
    public void PopulateTabs()
    {
        _forceColors = tabsItemSelectorSettings.ForceColors;
        if (_swapImageWhenSelected)
        {
            _imageUnselected = tabsItemSelectorSettings.ImageUnselected;
            _imageSelected = tabsItemSelectorSettings.ImageSelected;
        }

        foreach (Category category in tabsItemSelectorManager.Categories)
        {
            GameObject tab = Instantiate(tabsItemSelectorManager.TabPrefab, tabsItemSelectorManager.TabsContent.transform);
            tab.name = category.CategoryName;
            tabsList.Add(tab);


            var tabText = tab.GetComponentInChildren<TextMeshProUGUI>();
            if (tabText != null) { tabText.text = category.CategoryName; }

            // Assign component to change visually when the tab is selected, it may use button/toggle colors property or the image if user wants to swap image of Game Object when tab is selected it will assign the image 
            if(_swapImageWhenSelected)
            {
                if (tab.TryGetComponent<Image>(out Image tabImage)) 
                    _tabsDictionary[tab.name] = tabImage;
            }

            // Button functionality
            if (tab.TryGetComponent<Button>(out Button tabButton))
            {
                // Adds sectionController.JumpToSection() as a button listener
                AddTabButtonAction(tabButton, tabsItemSelectorManager.SectionsController.JumpToSection, tab.name);
                isButton = true;
                if (!_swapImageWhenSelected)
                _tabsDictionary[tab.name] = tabButton;
            }
            // Toggle functionality
            else if (tab.TryGetComponent<Toggle>(out Toggle tabToggle))
            {
                // Add toggle functionality here if needed
                isToggle = true;
                _tabsDictionary[tab.name] = tabToggle;
            }

            else if (tab.TryGetComponent<Image>(out Image tabImage))
            {
                isImage = true;
                _tabsDictionary[tab.name] = tabImage;
            }

            // Recalculates the elements size sum to recalculate and reset the content size
            LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.TabsContent.transform as RectTransform);

        }
    }
    public void SetActiveSection(string activeSection)
    {
        if (_activeSectionName == activeSection) return;

        _activeSectionName = activeSection;
        Debug.Log($"active section: {activeSection} active tab: {_activeSectionName}");
        UpdateVisuals();
    }
    private void UpdateVisuals()
    {
        foreach (var tab in _tabsDictionary)
        {
            if (_swapImageWhenSelected)
            {
                var image = (Image)tab.Value;
                if (image) image = tab.Key == _activeSectionName ?  _imageSelected : _imageUnselected;
                continue;
            }

            if(isButton)
            {
                Debug.Log("isButton called for tab " + tab.Key);

                var button = (Button)tab.Value;
                var colors = button.colors;
                var targetGraphic = button.targetGraphic;

                if (!_forceColors)
                {
                    if (targetGraphic) targetGraphic.color = tab.Key == _activeSectionName ? colors.highlightedColor : colors.normalColor;
                }
                else
                {
                    if (targetGraphic) targetGraphic.color = tab.Key == _activeSectionName ? _selectedColor : _normalColor;
                }
                continue;
            }

            if(isToggle)
            {
                var toggle = (Toggle)tab.Value;
                var colors = toggle.colors;
                var targetGraphic = toggle.targetGraphic;

                if (!_forceColors)
                    if (targetGraphic) targetGraphic.color = tab.Key == _activeSectionName ? colors.highlightedColor : colors.normalColor;
                else
                    if (targetGraphic) targetGraphic.color = tab.Key == _activeSectionName ? _selectedColor : _normalColor;

                continue;
            }
            if(isImage)
            {
                var image = (Image)tab.Value;
                if (image) image.color = tab.Key == _activeSectionName ? _selectedColor : _normalColor;
                continue;
            }
        }
    }
}
