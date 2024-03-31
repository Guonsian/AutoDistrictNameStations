using System;
using Game.Areas;
using Unity.Entities;

namespace AutoDistrictNameStations;

public static class BuildingDetails
{

    public static string[] getBuildingDetails(Entity buildingEntity, EntityManager entityManager)
    {
        CurrentDistrict currentDistrict = entityManager.GetComponentData<CurrentDistrict>(buildingEntity);
        string building_type = Mod.GameNameSystem.GetDebugName(buildingEntity);
        building_type = building_type.Substring(0, building_type.IndexOf(' '));

        string districtLabelName;

        if (currentDistrict.m_District != Entity.Null)
        {
            try
            {
                districtLabelName = Mod.GameNameSystem.GetRenderedLabelName(currentDistrict.m_District);
            }
            catch(Exception _)
            {
                Mod.log.Warn("District label not found:" + _);
                districtLabelName = null;
            }
        }
        else
        {
            districtLabelName = null;
        }
            
        return new[]
        {
            districtLabelName, building_type
        };
    }
} 