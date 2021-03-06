﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecipeController : Controller 
{
    public CraftPatternPlayer m_CraftPatternPlayer = null;

    public BuildingData m_CurrentBuilding = null;
    public BuildingData.CraftSet m_CurrentCraftSet = null;
    private int m_craftSetIndex = -1;

    public delegate void OnCraftSetSelected(BuildingData.CraftSet craftSet, BuildingData building);
    
    public static OnCraftSetSelected s_OnCraftSetSelected = null;

    void Awake()
    {
        Controller.RegisterController(this);
        BuildingController.s_onBuildingSelected += OnBuildingChanged;
    }
   
    void OnBuildingChanged(BuildingData newBuilding)
    {
        m_CurrentBuilding = newBuilding;        

        if(m_CurrentBuilding != null)
        {
            m_CurrentCraftSet = m_CurrentBuilding.GetCraftSet();
            m_craftSetIndex = 0;
            if(m_CurrentCraftSet != null && s_OnCraftSetSelected != null) s_OnCraftSetSelected(m_CurrentCraftSet, newBuilding);
        }
    }

    void OnDestroy()
    {
        Controller.UnregisterController(this);
    }

    void Update()
    {        
        if (m_CraftPatternPlayer && m_CraftPatternPlayer.m_State == CraftPatternPlayer.PlayerState.Stopped)
        {
            if (!HUDSectionSelection.HasSelection() && !IsLocked())
            {
                if (XInput.GetButtonUp(Buttons.RightBumper, 0))
                {
                    SelectNextCraftSet();
                }

                if (m_CurrentCraftSet != null)
                {
                    foreach (KeyValuePair<Buttons, ItemData> kvp in m_CurrentCraftSet.GetRecipes())
                    {
                        if (XInput.GetButtonDown(kvp.Key, 0))
                        {
                            if(kvp.Value.ConsumeCraftIngredients(Inventory.Instance))
                            {
                                m_CraftPatternPlayer.StartPattern(kvp.Value);
                            }
                            else
                            {
                                //NUT NUT!!
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    void SelectNextCraftSet()
    {
        m_craftSetIndex = (m_craftSetIndex + 1) % m_CurrentBuilding.m_CraftSets.Count;
        m_CurrentCraftSet = m_CurrentBuilding.GetCraftSet(m_craftSetIndex);
        if(m_CurrentCraftSet != null && s_OnCraftSetSelected != null) s_OnCraftSetSelected(m_CurrentCraftSet, m_CurrentBuilding);
    }        
}
