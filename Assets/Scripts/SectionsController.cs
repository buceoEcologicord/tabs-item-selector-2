using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public class SectionsController : MonoBehaviour
{
    [SerializeField] TabsItemSelectorManager tabsItemSelectorManager;

    // Advanced settings
    TabsItemSelectorSettings tabsItemSelectorSettings = new TabsItemSelectorSettings();

    List<GameObject> sectionsList = new List<GameObject>();
    List<(string sectionName, float topY, float bottomY)> _sectionRanges = new(); // Used to jump between sections and chan te active tab

    ScrollRect _scrollRect;
    RectTransform _content;

    private void Awake()
    {
        tabsItemSelectorSettings = tabsItemSelectorManager.TabsItemSelectorSettings;
        _scrollRect = tabsItemSelectorManager.SectionsScrollView;
        _content = tabsItemSelectorManager.SectionsContent.GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        _scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }
    private void OnDisable()
    {
        _scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
    }
    
    private void OnScrollChanged(Vector2 _) // Consider removing argument Vector2 _
    {
        float currentY = _content.anchoredPosition.y;
        var viewport = _scrollRect.viewport ? _scrollRect.viewport : (RectTransform)_content.parent;
        float viewportTop = currentY;
        float viewportBottom = currentY + viewport.rect.height;

        var topPadding = _content.GetComponent<VerticalLayoutGroup>()?.padding.top;
        var bottomPadding = _content.GetComponent<VerticalLayoutGroup>()?.padding.bottom;
        var spacing = _content.GetComponent<VerticalLayoutGroup>()?.spacing ?? 0f;

        // Set active section depending on the top y position on viewport
        string active = null;

        for (int i = 0; i < _sectionRanges.Count; i++)
        {
            var r = _sectionRanges[i];
            
            if (r.topY <= viewportTop + tabsItemSelectorSettings.TabChangeTolerance + topPadding + bottomPadding + spacing)
            {
                active = r.sectionName;
            }
            else
            {
                break;

            }
        }

        if (!string.IsNullOrEmpty(active))
            tabsItemSelectorManager.TabsController.SetActiveSection(active);
    }
    public void PopulateSections(List<Category> categories, GameObject sectionPrefab, ScrollRect scrollView)
    {
        foreach (Category category in categories)
        {
            GameObject section = Instantiate(sectionPrefab, scrollView.content);
            tabsItemSelectorManager.UIBuilder.InitializeGroupLayoutUI(section, tabsItemSelectorManager.ItemsOrientation);
            section.name = category.CategoryName;
            sectionsList.Add(section);

            var sectionText = section.GetComponentInChildren<TextMeshProUGUI>();
            if (sectionText != null) { sectionText.text = category.CategoryName; }
            if (sectionText != null) { sectionText.gameObject.SetActive(tabsItemSelectorSettings.ShowCategoryLabels);  }

            // Look for ItemsGroupLayout GO inside sectionPrefab to set itemsScrollView content and initializes items UI
            var itemsGridLayoutGroup = section.GetComponentInChildren<GridLayoutGroup>();
            var itemsToggleGroup = section.GetComponentInChildren<ToggleGroup>();
            var itemsContentSizeFitter = section.GetComponentInChildren<ContentSizeFitter>();

            // Populate items with or without toggle groups depending on TabsSelectorManagrSettings: bool useToggleGroups
            if (itemsGridLayoutGroup)
            {
                if(itemsToggleGroup != null && tabsItemSelectorSettings.UseToggleGroups)
                    PopulateItems(itemsGridLayoutGroup.transform, category, itemsToggleGroup); 
                else
                    PopulateItems(itemsGridLayoutGroup.transform, category);
            }

        }
        // Recalculates the elements size sum to recalculate and reset the content size
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);

        StartCoroutine(ComputeRangesNextFrame());

    }

    private IEnumerator ComputeRangesNextFrame()
    {
        yield return null; // wait 1 frame
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        ComputeSectionRanges();
    }

    // Instantiate items inside the corresponding section/Category Layout Group
    // Override if no Toggle Group is needed
    void PopulateItems(Transform transform, Category category)
    {
        foreach (Item item in tabsItemSelectorManager.Items)
        {

            var itemCategory = item.Category;

            if (itemCategory != category) { continue; } // Skip items that do not belong to the current category

            GameObject itemCell = Instantiate(tabsItemSelectorManager.ItemCellPrefab, transform);
            if (!itemCell.TryGetComponent<ItemBuilder>(out ItemBuilder itemBuilder))
                Debug.LogError("SectionsController: ItemCellPrefab does not have an ItemBuilder component. Add it to load items correctly");
            else
                itemBuilder.LoadItemPrefab(item);

        }

        // Recalculates the elements size sum to recalculate and reset the content size
        LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.SectionsContent.transform as RectTransform);
    }

    // Instantiate items inside the corresponding section/Category Layout Group
    // Override if no Toggle Group is needed
    void PopulateItems(Transform transform, Category category, ToggleGroup toggleGroup)
    {
        foreach (Item item in tabsItemSelectorManager.Items)
        {

            var itemCategory = item.Category;

            if (itemCategory != category) { continue; } // Skip items that do not belong to the current category

            GameObject itemCell = Instantiate(tabsItemSelectorManager.ItemCellPrefab, transform);
            if (!itemCell.TryGetComponent<ItemBuilder>(out ItemBuilder itemBuilder))
                Debug.LogError("SectionsController: ItemCellPrefab does not have an ItemBuilder component. Add it to load items correctly");
            else
                itemBuilder.LoadItemPrefab(item, toggleGroup);
        }

        // Recalculates the elements size sum to recalculate and reset the content size
        LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.SectionsContent.transform as RectTransform);
    }
    
    private void ComputeSectionRanges()
    {
        // Computes sction ranges in the content by adding spacing, padding and height of each section to the list _sectionRanges, cursorY is used to temporarily store the current height of the calculations
        _sectionRanges.Clear();

        float cursorY = 0f;
        float spacing = _content.GetComponent<VerticalLayoutGroup>()?.spacing ?? 0f;
        var verticalayoutGroup = _content.GetComponent<VerticalLayoutGroup>();
        float topPadding = verticalayoutGroup ? verticalayoutGroup.padding.top : 0f;
        float bottomPadding = verticalayoutGroup ? verticalayoutGroup.padding.bottom : 0f;

        cursorY += topPadding;

        foreach (GameObject section in sectionsList)
        {
            var rectTransform = (RectTransform)section.transform;
            float height = rectTransform.rect.height;
            float top = cursorY;
            float bottom = cursorY + height;
            _sectionRanges.Add((section.name, top, bottom));
            cursorY = bottom + spacing;
        }

        // Adds bottom padding for the bottom limit of the content (CONSIDER REMOVING)
        cursorY += bottomPadding;
    }
    public void JumpToSection(string sectionName)
    {
        // Finds index of sectionsList by categoryName, depends on section GameObjects having same names as categories and being in the same order.
        // This should be ok since both are populated from the same Categories list in TabsItemSelectorManager and section GameObjects are named after category names when instantiated in this SectionsController script.

        int idx = sectionsList.FindIndex(s => s.name == sectionName);
        if (idx < 0) return;
        var range = _sectionRanges[idx];

        //---
        // Moves the content so the top of the section relocates to the top of the viewport:

        float targetY = range.topY;

        // Clamp to avoid overscrolling
        var viewport = tabsItemSelectorManager.SectionsScrollView.viewport ? tabsItemSelectorManager.SectionsScrollView.viewport : (RectTransform)_content.parent;
        float maxY = Mathf.Max(0f, _content.rect.height - viewport.rect.height);

        targetY = Mathf.Clamp(targetY, 0f, maxY);


        var pos = _content.anchoredPosition;
        pos.y = targetY;
        _content.anchoredPosition = pos;
        //---

        tabsItemSelectorManager.TabsController.SetActiveSection(sectionName);
    }
}
