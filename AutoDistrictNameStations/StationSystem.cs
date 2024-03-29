using System;

using Colossal.Logging;
using Game;
using Game.Areas;
using Game.Common;
using Game.Tools;
using Game.Routes;
using Game.UI;
using Unity.Collections;
using Unity.Entities;
using TransportStation = Game.Buildings.TransportStation;


namespace AutoDistrictNameStations
{
    public partial class StationSystem : GameSystemBase
    {

        private EntityQuery _systemQuery;
        private ILog _log;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _log = Mod.log;
            
            _log.Info("onCreate de station system");
            
            _systemQuery =  GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] 
                {
                    ComponentType.ReadOnly<Updated>()
                },
                Any = new ComponentType[]
                {
                    ComponentType.ReadWrite<TransportStation>(),
                    ComponentType.ReadWrite<Game.Routes.TransportStop>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<TramStop>()
                }
            });
            
            // we want to use _systemQuery as it will have the station and stops, but we only want the onUpdated with new stops
            EntityQuery mQueryCreated =  GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] 
                {
                    ComponentType.ReadOnly<Created>()
                },
                Any = new ComponentType[]
                {
                    ComponentType.ReadWrite<TransportStation>(),
                    ComponentType.ReadWrite<Game.Routes.TransportStop>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>()
                }
            });
            
            RequireForUpdate(_systemQuery);
            RequireForUpdate(mQueryCreated); 
        }
        
        protected override void OnUpdate()
        {
            _log.Info("onUpdate StationSystem executed");
            
            var buildingEntitiesArray = _systemQuery.ToEntityArray(Allocator.Temp);

            if (buildingEntitiesArray.Length <= 1)
            {
                return;
            }
            
            NameSystem gameNameSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<Game.UI.NameSystem>();
            
            string districtName = "";
            for (int i = 0; i < buildingEntitiesArray.Length; i++)
            {
                try
                {
                    var transportStation = buildingEntitiesArray[i];
                    CurrentDistrict currentDistrict = EntityManager.GetComponentData<CurrentDistrict>(transportStation);
                    _log.Info(currentDistrict);
                    string districtDebugName = gameNameSystem.GetDebugName(currentDistrict.m_District);
                    string districtLabelName = gameNameSystem.GetRenderedLabelName(currentDistrict.m_District);
                    _log.Info(districtDebugName + "!" + districtLabelName);
                    if (districtLabelName.Length > 0 && districtDebugName.Contains("District Area"))
                    {
                        var previousName = gameNameSystem.GetRenderedLabelName(transportStation);
                        districtName = districtLabelName;
                        if (!previousName.Contains("|") && !previousName.Contains(","))
                        {
                            gameNameSystem.SetCustomName(transportStation, districtName + " | " + previousName);
                        }

                    }
                }
                catch (Exception ex) // The transportStops will cause an error as they do not provide currentDistrict component
                {
                    _log.Error($"An error occurred: {ex.Message}");
                }
            }

            // Set CustonName for all platforms and stops
            if (districtName.Length > 0)
            {
                for (int i = 0; i < buildingEntitiesArray.Length; i++)
                {
                    try
                    {
                        var previousName = gameNameSystem.GetRenderedLabelName(buildingEntitiesArray[i]);
                        _log.Info("previous name in stop:  " + previousName);
                        if (!previousName.Contains("|") && !previousName.Contains(","))
                        {
                            gameNameSystem.SetCustomName(buildingEntitiesArray[i], districtName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"An error occurred: {ex.Message}");
                    }
                }
            }
        }
    }
}