using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class TabsItemSelectorManager : MonoBehaviour
{    
    enum ContentOrientation { Vertical, Horizontal }
    enum LayoutType { SectionsLayout, GridLayout }
    [Header("Layout settings")]
    [SerializeField] ScrollRect tabsScrollView;
    [SerializeField] ScrollRect sectionsScrollView;
    [SerializeField] GameObject tabsContent;
    [SerializeField] GameObject sectionsContent;
    //[SerializeField] ScrollRect itemsScrollView; // Consider removing, for now is automatically assigned inside sections initialization
    [SerializeField] ContentOrientation tabsOrientation = ContentOrientation.Vertical;
    [SerializeField] ContentOrientation itemsOrientation = ContentOrientation.Vertical;

    [Header("References")]
    [SerializeField] List<Category> categories = new List<Category>();
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] GameObject tabPrefab;
    [SerializeField] GameObject sectionPrefab;
    [SerializeField] GameObject itemCellPrefab;

    [Header("Tabs settings")]
    [SerializeField] bool enableTabs = true;
    [SerializeField] bool tabTextSameAsCategory = true;

    [Header("Sections settings")]
    [SerializeField] bool enableSections = true;
    [SerializeField] bool showCategoryLabels = true;

    [Header("Items settings")]
    [SerializeField] bool showItemText = true;


    [Header("Sections Oriented layout group settings")]
    [SerializeField] bool sectionsChildForceExpandWidth = true;
    [SerializeField] bool sectionsChildForceExpandHeight = true;
    [SerializeField] bool sectionsChildControlWidth = true;
    [SerializeField] bool sectionsChildControlHeight = true;
    
    [Header("Tabs Oriented layout group settings")]
    [SerializeField] bool tabsChildForceExpandWidth = true;
    [SerializeField] bool tabsChildForceExpandHeight = true;
    [SerializeField] bool tabsChildControlWidth = true;
    [SerializeField] bool tabsChildControlHeight = true;
    
    [Header("Grid layout group settings")]
    [SerializeField] bool setItemCellSize = false;
    [SerializeField] Vector2 cellSize;
    [SerializeField] bool setItemSpacing = false;
    [SerializeField] Vector2 spacing;
    [SerializeField] GridLayoutGroup.Constraint gridLayoutGroupConstraint;
    [SerializeField] int gridLayoutGroupConstraintCount = 0;


    // Getters and Setters
    public List<Category> Categories { get { return categories; } set { categories = value; } }
    public List<Item> ItemsNames { get { return items; } set { items = value; } }

    //---------------------------------------------------------------

    // Layout variables
    ContentSizeFitter tabsSizeFitter;
    ContentSizeFitter sectionsSizeFitter;

    // Tabs variables
    List<GameObject> tabsList = new List<GameObject>();

    // Sections variables
    List<GameObject> sectionsList = new List<GameObject>();

    private void Start()
    {
        PopulateUI();
        InitializeUI();
    }

    public void InitializeUI() // Initializes a Scroll rect for tabs, sections of items categories, and items themselves
    {
        // Add Content Size Fitters to the content of each ScrollRect
        tabsSizeFitter = tabsContent.AddComponent<ContentSizeFitter>();
        sectionsSizeFitter = sectionsContent.AddComponent<ContentSizeFitter>();
        //itemsSizeFitter = itemsScrollView.content.AddComponent<ContentSizeFitter>(); // Consider removing, for now is automatically assigned inside sections initialization

        // Initialize Content Size Fitters and Layout Groups
        InitializeScrollUI(LayoutType.SectionsLayout, tabsScrollView, tabsContent, tabsSizeFitter, tabsOrientation); //Initialize tabs
        InitializeScrollUI(LayoutType.SectionsLayout, sectionsScrollView, sectionsContent, sectionsSizeFitter, itemsOrientation); //Initialize sections
    }
    void InitializeScrollUI(LayoutType layoutType, ScrollRect scrollView, GameObject content, ContentSizeFitter contentSizeFitter, ContentOrientation UIorientation)
    {
        // Add Layout Group and content size fitter
        // based on layout type (Vertical-Horizontal Layout Group Vs. Grid Group) and desired orientation (Vertical /Horizontal)
        if (layoutType == LayoutType.GridLayout) // If Grid Layout Group
        {   
            // Set Content Fitter
            if (UIorientation == ContentOrientation.Vertical) contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            else contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            // set Gridlayout
            var layOutGroup = content.AddComponent<GridLayoutGroup>();
            if(setItemCellSize) layOutGroup.cellSize = cellSize;
            if(setItemSpacing) layOutGroup.spacing = spacing;
            layOutGroup.constraint = gridLayoutGroupConstraint;
            if(layOutGroup.constraint != GridLayoutGroup.Constraint.Flexible ) layOutGroup.constraintCount = gridLayoutGroupConstraintCount;            
        }
        else // If Sections Layout (Vertical / Horizontal Layout Group)
        {
            InitializeGroupLayoutUI(content, UIorientation);

            if (UIorientation == ContentOrientation.Vertical)
            {
                scrollView.horizontal = false; // Block horizontal scrolling
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Set Content Size Fitter orientation
            }
            else
            {                
                scrollView.vertical = false; // Block vertical scrolling
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // Set Content Size Fitter orientation
            }
        }
    }
    void InitializeGroupLayoutUI(GameObject content, ContentOrientation UIOrientation)
    {
        if (UIOrientation == ContentOrientation.Horizontal) // For horizontal orientation
        {
            var layoutGroup = content.AddComponent<HorizontalLayoutGroup>();

            if (content != tabsContent) // Logic for sections layout
            {
                // Sets layout Group for sections
                layoutGroup.childForceExpandWidth = sectionsChildForceExpandWidth;
                layoutGroup.childForceExpandHeight = sectionsChildForceExpandHeight;
                layoutGroup.childControlWidth = sectionsChildControlWidth;
                layoutGroup.childControlHeight = sectionsChildControlHeight;
            }
            else // Logic for tabs layout
            {
                if (tabsChildForceExpandWidth || tabsChildControlWidth)
                {                    
                    tabsSizeFitter.enabled = false; // Disable content size fitter to avoid orientation and size control conflict

                    // It has to match both expand and control size in the same direction to avoid tabs outside content or undesirable sizes
                    layoutGroup.childForceExpandWidth = true;
                    layoutGroup.childControlWidth = true;

                    // Also match both variables in manager inspector to avoid confusion
                    tabsChildForceExpandWidth = true;
                    tabsChildControlWidth = true;
                }

                layoutGroup.childForceExpandWidth = tabsChildForceExpandWidth;
                layoutGroup.childControlWidth = tabsChildControlWidth;
                layoutGroup.childForceExpandHeight = tabsChildForceExpandHeight;
                layoutGroup.childControlHeight = tabsChildControlHeight;
            }
        }
        else // For vertical orientation
        {
            var layoutGroup = content.AddComponent<VerticalLayoutGroup>();

            if (content != tabsContent) // Logic for sections layout
            {
                // Sets layout Group for sections
                layoutGroup.childForceExpandWidth = sectionsChildForceExpandWidth;
                layoutGroup.childForceExpandHeight = sectionsChildForceExpandHeight;
                layoutGroup.childControlWidth = sectionsChildControlWidth;
                layoutGroup.childControlHeight = sectionsChildControlHeight;
            }
            else // Logic for tabs layout, 
            {
                if (tabsChildForceExpandHeight || tabsChildControlHeight) 
                {
                    tabsSizeFitter.enabled = false; // Disable content size fitter to avoid orientation and size control conflict

                    // It has to match both expand and control size in the same direction to avoid tabs outside content or undesirable sizes
                    layoutGroup.childForceExpandHeight = true;
                    layoutGroup.childControlHeight = true;

                    // Also match both variables in manager inspector to avoid confusion
                    tabsChildForceExpandHeight = true;
                    tabsChildControlHeight = true;

                }

                layoutGroup.childForceExpandWidth = tabsChildForceExpandWidth;
                layoutGroup.childControlWidth = tabsChildControlWidth;
                layoutGroup.childForceExpandHeight = tabsChildForceExpandHeight;
                layoutGroup.childControlHeight = tabsChildControlHeight;

            }
        }
    }

    void PopulateUI()
    {
        PopulateTabs();
        PopulateSections();
        //PopulateItems(); // Consider removing, items are populated inside sections
    }

    void PopulateTabs() 
    {
        foreach (Category category in categories)
        {
            GameObject tab = Instantiate(tabPrefab, tabsContent.transform);
            tab.name = category.CategoryName;
            tabsList.Add(tab);

            var tabText = tab.GetComponentInChildren<TextMeshProUGUI>();
            if (tabText != null) { tabText.text = category.CategoryName; }
            
            if(tab.TryGetComponent<Button>(out Button tabButton))
            {
                // Add button functionality here if needed
            }

            if(tab.TryGetComponent<Toggle>(out Toggle tabToggle))
            {
                // Add toggle functionality here if needed
            }

            // Recalculates the elements size sum to recalculate and reset the content size
            LayoutRebuilder.ForceRebuildLayoutImmediate(tabsContent.transform as RectTransform);

        }
    }
    void PopulateSections() 
    {
        foreach (Category category in categories)
        {
            GameObject section = Instantiate(sectionPrefab, sectionsScrollView.content);
            InitializeGroupLayoutUI(section, itemsOrientation);
            section.name = category.CategoryName;
            sectionsList.Add(section);

            var sectionText = section.GetComponentInChildren<TextMeshProUGUI>();
            if (sectionText != null) { sectionText.text = category.CategoryName; }

            // Look for ItemsGroupLayout GO inside sectionPrefab to set itemsScrollView content and initializes items UI
            var itemsGridLayoutGroup = section.GetComponentInChildren<GridLayoutGroup>();
            var itemsContentSizeFitter = section.GetComponentInChildren<ContentSizeFitter>();            
                        
            PopulateItems(itemsGridLayoutGroup.transform, category); // Populate items inside the section

        }
        // Recalculates the elements size sum to recalculate and reset the content size
        LayoutRebuilder.ForceRebuildLayoutImmediate(sectionsContent.transform as RectTransform);
    }
    // Instantiate items inside the corresponding section/Category Layout Group
    void PopulateItems(Transform transform, Category category) 
    {
        foreach (Item item in items)
        {

            var itemCategory = item.Category;

            if (itemCategory != category) { continue; } // Skip items that do not belong to the current category
            
            GameObject itemCell = Instantiate(itemCellPrefab, transform);
            var itemText = itemCell.GetComponentInChildren<TextMeshProUGUI>();
            if (itemText != null && showItemText) { itemText.text = item.ItemName; }
        }

        // Recalculates the elements size sum to recalculate and reset the content size
        LayoutRebuilder.ForceRebuildLayoutImmediate(sectionsContent.transform as RectTransform);
    }


    // ---------------------------------------------------------------
    //Consider removing this code 
    //void InitializeItemsUI( )
    //{
    //    if(itemsOrientation == ContentOrientation.Vertical)
    //    {
    //        itemsScrollView.content.AddComponent<VerticalLayoutGroup>();
    //        itemsSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //    }
    //    else
    //    {
    //        itemsScrollView.content.AddComponent<HorizontalLayoutGroup>();
    //        itemsSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    //    }

    //}
    //void InitializeTabsUI()
    //{
    //    if(tabsOrientation == ContentOrientation.Vertical)
    //    {
    //        tabsScrollView.content.AddComponent<VerticalLayoutGroup>();
    //        tabsSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //    }
    //    else
    //    {
    //        tabsScrollView.content.AddComponent<HorizontalLayoutGroup>();
    //        tabsSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    //    }

    //}
}
