using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    [SerializeField] TabsItemSelectorManager tabsItemSelectorManager;

    // Advanced settings variables
    TabsItemSelectorSettings tabsItemSelectorSettings;

    // tabs variables
    List<GameObject> tabsList = new List<GameObject>();

    // Struct replaces Component casting
    private struct TabComponents
    {
        public Button button;
        public Toggle toggle;
        public Image image;
    }

    Dictionary<string, TabComponents> _tabsDictionary = new Dictionary<string, TabComponents>();

    string _activeSectionName;

    bool isButton = false;
    bool isToggle = false;
    bool isImage = false;

    bool _swapImageWhenSelected;
    Sprite _imageUnselected;
    Sprite _imageSelected;

    bool _forceColors = false;
    Color _normalColor;
    Color _selectedColor;

    // Getters and setters
    public List<GameObject> TabsList { get { return tabsList; } set { tabsList = value; } }

    public void AddTabButtonAction(Button button, Action<string> onTabClicked, string sectionName)
    {
        button.onClick.AddListener(() => { onTabClicked?.Invoke(sectionName); });
    }

    public void PopulateTabs()
    {
        tabsItemSelectorSettings = tabsItemSelectorManager.TabsItemSelectorSettings;
        _forceColors = tabsItemSelectorSettings.ForceColors;
        _normalColor = tabsItemSelectorSettings.NormalColor;
        _selectedColor = tabsItemSelectorSettings.SelectedColor;

        _swapImageWhenSelected = tabsItemSelectorSettings.SwapImageWhenSelected;
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
            if (tabText != null)
                tabText.text = category.CategoryName;

            // Create struct instance
            TabComponents components = new TabComponents();

            // BUTTON
            if (tab.TryGetComponent<Button>(out Button tabButton))
            {
                isButton = true;
                components.button = tabButton;

                AddTabButtonAction(tabButton, tabsItemSelectorManager.SectionsController.JumpToSection, tab.name);

                if (_swapImageWhenSelected)
                {
                    if (tab.TryGetComponent<Image>(out Image img))
                        components.image = img;
                }
                else
                {
                    // no image swap → use button directly
                }
            }
            // TOGGLE
            else if (tab.TryGetComponent<Toggle>(out Toggle tabToggle))
            {
                isToggle = true;
                components.toggle = tabToggle;

                if (_swapImageWhenSelected)
                {
                    if (tab.TryGetComponent<Image>(out Image img))
                        components.image = img;
                }
            }
            // IMAGE ONLY
            else if (tab.TryGetComponent<Image>(out Image tabImage))
            {
                isImage = true;
                components.image = tabImage;
            }

            _tabsDictionary[tab.name] = components;

            LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.TabsContent.transform as RectTransform);
        }
    }

    public void SetActiveSection(string activeSection)
    {
        if (_activeSectionName == activeSection)
            return;

        _activeSectionName = activeSection;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        foreach (var tab in _tabsDictionary)
        {
            var components = tab.Value;
            bool isActive = tab.Key == _activeSectionName;

            // IMAGE SWAP MODE
            if (_swapImageWhenSelected && components.image != null)
            {
                components.image.sprite = isActive ? _imageSelected : _imageUnselected;
                continue;
            }

            // BUTTON MODE
            if (isButton && components.button != null)
            {
                var button = components.button;
                var colors = button.colors;
                var targetGraphic = button.targetGraphic;

                if (!_forceColors)
                    targetGraphic.color = isActive ? colors.highlightedColor : colors.normalColor;
                else
                    targetGraphic.color = isActive ? _selectedColor : _normalColor;

                continue;
            }

            // TOGGLE MODE
            if (isToggle && components.toggle != null)
            {
                var toggle = components.toggle;
                var colors = toggle.colors;
                var targetGraphic = toggle.targetGraphic;

                if (!_forceColors)
                    targetGraphic.color = isActive ? colors.highlightedColor : colors.normalColor;
                else
                    targetGraphic.color = isActive ? _selectedColor : _normalColor;

                continue;
            }

            // IMAGE MODE
            if (isImage && components.image != null)
            {
                components.image.color = isActive ? _selectedColor : _normalColor;
                continue;
            }
        }
    }
}