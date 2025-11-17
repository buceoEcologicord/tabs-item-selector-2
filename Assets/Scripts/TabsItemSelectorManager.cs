using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TabsItemSelectorSettings
{
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

#region Getters and setters
    public bool EnableTabs { get { return enableTabs; } set { enableTabs = value; } }
    public bool TabTextSameAsCategory { get { return tabTextSameAsCategory; } set { tabTextSameAsCategory = value; } }

    public bool EnableSections { get { return enableSections; } set { enableSections = value; } }
    public bool ShowCategoryLabels { get { return showCategoryLabels; } set { showCategoryLabels = value; } }

    public bool ShowItemText { get { return showItemText; } set { showItemText = value; } }

    public bool SectionsChildForceExpandWidth { get { return sectionsChildForceExpandWidth; } set { sectionsChildForceExpandWidth = value; } }
    public bool SectionsChildForceExpandHeight { get { return sectionsChildForceExpandHeight; } set { sectionsChildForceExpandHeight = value; } }
    public bool SectionsChildControlWidth { get { return sectionsChildControlWidth; } set { sectionsChildControlWidth = value; } }
    public bool SectionsChildControlHeight { get { return sectionsChildControlHeight; } set { sectionsChildControlHeight = value; } }

    public bool TabsChildForceExpandWidth { get { return tabsChildForceExpandWidth; } set { tabsChildForceExpandWidth = value; } }
    public bool TabsChildForceExpandHeight { get { return tabsChildForceExpandHeight; } set { tabsChildForceExpandHeight = value; } }
    public bool TabsChildControlWidth { get { return tabsChildControlWidth; } set { tabsChildControlWidth = value; } }
    public bool TabsChildControlHeight { get { return tabsChildControlHeight; } set { tabsChildControlHeight = value; } }

    public bool SetItemCellSize { get { return setItemCellSize; } set { setItemCellSize = value; } }
    public Vector2 CellSize { get { return cellSize; } set { cellSize = value; } }
    public bool SetItemSpacing { get { return setItemSpacing; } set { setItemSpacing = value; } }
    public Vector2 Spacing { get { return spacing; } set { spacing = value; } }
    public GridLayoutGroup.Constraint GridLayoutGroupConstraint { get { return gridLayoutGroupConstraint; } set { gridLayoutGroupConstraint = value; } }
    public int GridLayoutGroupConstraintCount { get { return gridLayoutGroupConstraintCount; } set { gridLayoutGroupConstraintCount = value; } }
#endregion

}
public enum ContentOrientation { Vertical, Horizontal }
   public enum LayoutType { SectionsLayout, GridLayout }


public class TabsItemSelectorManager : MonoBehaviour
{    
    [Header("General settings")]
    [SerializeField] ContentOrientation tabsOrientation = ContentOrientation.Vertical;
    [SerializeField] ContentOrientation itemsOrientation = ContentOrientation.Vertical;

    [Header("Layout references")]
    [SerializeField] ScrollRect tabsScrollView;
    [SerializeField] ScrollRect sectionsScrollView;
    [SerializeField] GameObject tabsContent;
    [SerializeField] GameObject sectionsContent;

    [Header("Categories and prefab References")]
    [SerializeField] List<Category> categories = new List<Category>();
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] GameObject tabPrefab;
    [SerializeField] TabsController tabsController;
    [SerializeField] GameObject sectionPrefab;
    [SerializeField] SectionsController sectionsController;
    [SerializeField] GameObject itemCellPrefab;

    [Header("Script references")]
    [SerializeField] UIBuilder uIBuilder;

    [Header("Advanced Settings")]
    [SerializeField] TabsItemSelectorSettings tabsItemSelectorSettings = new TabsItemSelectorSettings();

    //// Layout variables
    //ContentSizeFitter tabsSizeFitter;
    //ContentSizeFitter sectionsSizeFitter;

    // Tabs variables
    List<GameObject> tabsList = new List<GameObject>();

    // Sections variables
    List<GameObject> sectionsList = new List<GameObject>();

#region Getters and setters
    public ContentOrientation TabsOrientation { get { return tabsOrientation; } set { tabsOrientation = value; } }
    public ContentOrientation ItemsOrientation { get { return itemsOrientation; } set { itemsOrientation = value; } }

    public ScrollRect TabsScrollView { get { return tabsScrollView; } set { tabsScrollView = value; } }
    public ScrollRect SectionsScrollView { get { return sectionsScrollView; } set { sectionsScrollView = value; } }

    public GameObject TabsContent { get { return tabsContent; } set { tabsContent = value; } }
    public GameObject SectionsContent { get { return sectionsContent; } set { sectionsContent = value; } }

    public List<Category> Categories { get { return categories; } set { categories = value; } }
    public List<Item> Items { get { return items; } set { items = value; } }

    public GameObject TabPrefab { get { return tabPrefab; } set { tabPrefab = value; } }
    public TabsController TabsController { get { return tabsController; } set { tabsController = value; } }
    public GameObject SectionPrefab { get { return sectionPrefab; } set { sectionPrefab = value; } }
    public SectionsController SectionsController { get { return sectionsController; } set { sectionsController = value; } }
    public GameObject ItemCellPrefab { get { return itemCellPrefab; } set { itemCellPrefab = value; } }

    public UIBuilder UIBuilder { get { return uIBuilder; } set { uIBuilder = value; } }

    public TabsItemSelectorSettings TabsItemSelectorSettings { get { return tabsItemSelectorSettings; } set { tabsItemSelectorSettings = value; } }
    
    //public ContentSizeFitter TabsSizeFitter { get { return tabsSizeFitter; } set { tabsSizeFitter = value; } }
    //public ContentSizeFitter SectionsSizeFitter { get { return sectionsSizeFitter; } set { sectionsSizeFitter = value; } }

    public List<GameObject> TabsList { get { return tabsList; } set { tabsList = value; } }
    public List<GameObject> SectionsList { get { return sectionsList; } set { sectionsList = value; } }
#endregion
        
    private void Start()
    {
        uIBuilder.PopulateUI();
        InitializeUI();
    }
    public void InitializeUI() // Initializes a Scroll rect for tabs, sections of items categories, and items themselves
    {
        // Initialize Content Size Fitters and Layout Groups
        uIBuilder. InitializeScrollUI(LayoutType.SectionsLayout, tabsScrollView, tabsContent, tabsOrientation); //Initialize tabs
        uIBuilder. InitializeScrollUI(LayoutType.SectionsLayout, sectionsScrollView, sectionsContent, itemsOrientation); //Initialize sections
    }    
}

