using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    [SerializeField] TabsItemSelectorManager tabsItemSelectorManager;

    // Advanced settings variables
    TabsItemSelectorSettings tabsItemSelectorSettings = new TabsItemSelectorSettings();

    // tabs variables
    List<GameObject> tabsList = new List<GameObject>();

    // Getters and setters
    public List<GameObject> TabsList { get { return tabsList; } set { tabsList = value; } }


    public void AddTabButtonAction(Button button, Action onTabClicked)
    {        
        // Adds any anonymous function with no parameters as listener to this button onClick
        button.onClick.AddListener(() => { onTabClicked?.Invoke(); });
    }
    public void PopulateTabs()
    {
        foreach (Category category in tabsItemSelectorManager.Categories)
        {
            GameObject tab = Instantiate(tabsItemSelectorManager.TabPrefab, tabsItemSelectorManager.TabsContent.transform);
            tab.name = category.CategoryName;
            tabsList.Add(tab);


            var tabText = tab.GetComponentInChildren<TextMeshProUGUI>();
            if (tabText != null) { tabText.text = category.CategoryName; }

            // Button functionality
            if (tab.TryGetComponent<Button>(out Button tabButton))
            {
                // Adds sectionController.JumpToSection() as a button listener
                AddTabButtonAction(tabButton, tabsItemSelectorManager.SectionsController.JumpToSection);
            }

            if (tab.TryGetComponent<Toggle>(out Toggle tabToggle))
            {
                // Add toggle functionality here if needed
            }

            // Recalculates the elements size sum to recalculate and reset the content size
            LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.TabsContent.transform as RectTransform);

        }
    }
}
