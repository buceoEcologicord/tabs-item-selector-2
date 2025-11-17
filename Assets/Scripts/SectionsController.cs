using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SectionsController : MonoBehaviour
{
    [SerializeField] TabsItemSelectorManager tabsItemSelectorManager;

    // Advanced settings
    TabsItemSelectorSettings tabsItemSelectorSettings = new TabsItemSelectorSettings();

    List<GameObject> sectionsList = new List<GameObject>();
    private void Awake()
    {
        tabsItemSelectorSettings = tabsItemSelectorManager.TabsItemSelectorSettings;
    }
    public void JumpToSection()
    {

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
            var itemsContentSizeFitter = section.GetComponentInChildren<ContentSizeFitter>();

            PopulateItems(itemsGridLayoutGroup.transform, category); // Populate items inside the section

        }
        // Recalculates the elements size sum to recalculate and reset the content size
        LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.SectionsContent.transform as RectTransform);
    }
    // Instantiate items inside the corresponding section/Category Layout Group
    void PopulateItems(Transform transform, Category category)
    {
        foreach (Item item in tabsItemSelectorManager.Items)
        {

            var itemCategory = item.Category;

            if (itemCategory != category) { continue; } // Skip items that do not belong to the current category

            GameObject itemCell = Instantiate(tabsItemSelectorManager.ItemCellPrefab, transform);
            var itemText = itemCell.GetComponentInChildren<TextMeshProUGUI>();
            if (itemText != null && tabsItemSelectorSettings.ShowItemText) { itemText.text = item.ItemName; }
        }

        // Recalculates the elements size sum to recalculate and reset the content size
        LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.SectionsContent.transform as RectTransform);
    }
}
