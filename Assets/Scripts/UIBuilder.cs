using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIBuilder : MonoBehaviour
{
    [SerializeField] TabsItemSelectorManager tabsItemSelectorManager;

    // Advanced settings from manager
    TabsItemSelectorSettings tabsItemSelectorSettings;

    // Controller references
    TabsController tabsController;
    SectionsController sectionsController;

    //Content Size Fitter variables
    ContentSizeFitter tabsSizeFitter;
    ContentSizeFitter sectionsSizeFitter;


    // Sections Group layout settings
    GameObject sectionsContent;
    bool sectionsChildForceExpandWidth;
    bool sectionsChildForceExpandHeight;
    bool sectionsChildControlWidth;
    bool sectionsChildControlHeight;

    // Tabs Group layout settings
    GameObject tabsContent;
    bool tabsChildForceExpandWidth;
    bool tabsChildForceExpandHeight;
    bool tabsChildControlWidth;
    bool tabsChildControlHeight;

    private void Start()
    {
        tabsItemSelectorSettings = tabsItemSelectorManager.TabsItemSelectorSettings;
   
        // Controller references
        tabsController = tabsItemSelectorManager.TabsController;
        sectionsController = tabsItemSelectorManager.SectionsController;        

        //Content Size Fitter variables
        tabsContent = tabsItemSelectorManager.TabsContent;
        sectionsContent = tabsItemSelectorManager.SectionsContent;

        // Sections Group layout settings
        sectionsChildForceExpandWidth = tabsItemSelectorSettings.SectionsChildForceExpandWidth;
        sectionsChildForceExpandHeight = tabsItemSelectorSettings.SectionsChildForceExpandHeight;
        sectionsChildControlWidth = tabsItemSelectorSettings.SectionsChildControlWidth;
        sectionsChildControlHeight = tabsItemSelectorSettings.SectionsChildControlHeight;

        // Tabs Group layout settings
        tabsChildForceExpandWidth = tabsItemSelectorSettings.TabsChildForceExpandWidth;
        tabsChildForceExpandHeight = tabsItemSelectorSettings.TabsChildForceExpandHeight;
        tabsChildControlWidth = tabsItemSelectorSettings.TabsChildControlWidth;
        tabsChildControlHeight = tabsItemSelectorSettings.TabsChildControlHeight;
    }
    public void InitializeScrollUI(LayoutType layoutType, ScrollRect scrollView, GameObject content, ContentOrientation UIorientation)
    {
        // Set ContentFitter
        if (content.TryGetComponent<ContentSizeFitter>(out ContentSizeFitter contentSizeFitter)) { }
        else contentSizeFitter = content.AddComponent<ContentSizeFitter>();

        if (UIorientation == ContentOrientation.Vertical) contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        else contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Set tabsSizeFitter to disable it iff needed on InitializeGroupLayoutUI()
        tabsSizeFitter = content == tabsContent ?  contentSizeFitter : tabsSizeFitter ;

        // Add Layout Group and content size fitter
        // based on layout type(Vertical - Horizontal Layout Group Vs.Grid Group) and desired orientation(Vertical / Horizontal)
        if (layoutType == LayoutType.GridLayout) // If Grid Layout Group
        {
            // set Gridlayout
            var layOutGroup = content.AddComponent<GridLayoutGroup>();
            if (tabsItemSelectorSettings.SetItemCellSize) layOutGroup.cellSize = tabsItemSelectorSettings.CellSize;
            if (tabsItemSelectorSettings.SetItemSpacing) layOutGroup.spacing = tabsItemSelectorSettings.Spacing;
            layOutGroup.constraint = tabsItemSelectorSettings.GridLayoutGroupConstraint;
            if (layOutGroup.constraint != GridLayoutGroup.Constraint.Flexible) layOutGroup.constraintCount = tabsItemSelectorSettings.GridLayoutGroupConstraintCount;
        }
        else // If Sections Layout (Vertical / Horizontal Layout Group)
        {
            InitializeGroupLayoutUI(content, UIorientation);            
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.transform as RectTransform);

    }
    public void InitializeGroupLayoutUI(GameObject content, ContentOrientation UIOrientation)
    {        
        //ContentSizeFitter contentSizeFitter = content.AddComponent<ContentSizeFitter>();

        if (UIOrientation == ContentOrientation.Horizontal) // For horizontal orientation
        {
            var layoutGroup = content.AddComponent<HorizontalLayoutGroup>();          

            if (content != tabsContent) // Logic for sections layout
            {
                tabsItemSelectorManager.SectionsScrollView.vertical = false; // Block vertical scrolling

                // Sets layout Group for sections
                layoutGroup.childForceExpandWidth = sectionsChildForceExpandWidth;
                layoutGroup.childForceExpandHeight = sectionsChildForceExpandHeight;
                layoutGroup.childControlWidth = sectionsChildControlWidth;
                layoutGroup.childControlHeight = sectionsChildControlHeight;
            }
            else // Logic for tabs layout
            {
                // Add Content Size Fitters to the content of each ScrollRect
                tabsItemSelectorManager.TabsScrollView.vertical = false; // Block vertical scrolling

                if (tabsChildForceExpandWidth || tabsChildControlWidth)
                {
                    tabsSizeFitter.enabled = false; // Disable content size fitter to avoid orientation and size control conflict

                    // It has to match both expand and control size in the same direction to avoid tabs outside content or undesirable sizes
                    layoutGroup.childForceExpandWidth = true;
                    layoutGroup.childControlWidth = true;

                    // Also match both variables in UIBuilder settings in case they are called by other scripts
                    tabsChildForceExpandWidth = true;
                    tabsChildControlWidth = true;

                    // Also match both variables in manager inspector to avoid confusion
                    tabsItemSelectorSettings.TabsChildForceExpandWidth = true;
                    tabsItemSelectorSettings.TabsChildControlWidth = true;
                }

                layoutGroup.childForceExpandWidth = tabsChildForceExpandWidth;
                layoutGroup.childControlWidth = tabsChildControlWidth;
                layoutGroup.childForceExpandHeight = tabsChildForceExpandHeight;
                layoutGroup.childControlHeight = tabsChildControlHeight;
            }

            //contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // Set Content Size Fitter orientation
        }
        else // For vertical orientation
        {
            var layoutGroup = content.AddComponent<VerticalLayoutGroup>();            

            if (content != tabsContent) // Logic for sections layout
            {
                tabsItemSelectorManager.SectionsScrollView.horizontal = false; // Block horizontal scrolling

                // Sets layout Group for sections
                layoutGroup.childForceExpandWidth = sectionsChildForceExpandWidth;
                layoutGroup.childForceExpandHeight = sectionsChildForceExpandHeight;
                layoutGroup.childControlWidth = sectionsChildControlWidth;
                layoutGroup.childControlHeight = sectionsChildControlHeight;
            }
            else // Logic for tabs layout, 
            {
                tabsItemSelectorManager.TabsScrollView.horizontal = false; // Block horizontal scrolling

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

            //contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Set Content Size Fitter orientation
        }
    }

    public void PopulateUI()
    {
        tabsController.PopulateTabs();
        sectionsController.PopulateSections(tabsItemSelectorManager.Categories, tabsItemSelectorManager.SectionPrefab, tabsItemSelectorManager.SectionsScrollView);
    }

}
