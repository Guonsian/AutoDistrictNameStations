using System.Linq;
using AutoDistrictNameStations.AuxComponents;
using Colossal.Logging;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using OutsideConnection = Game.Objects.OutsideConnection;

namespace AutoDistrictNameStations.Systems;

public partial class GenericSystem<TBuilding, TAuxComponent, Tupdate> : GameSystemBase  where TAuxComponent : class, IComponentData, new()
{
    
        private EntityQuery _systemQuery;
        private EntityQuery _otherRelevantBuildings;
        private ILog _log;
        private ModOptions _modOptions;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _log = Mod.log;
            _modOptions = Mod.ModCustomOptions;
            
            _log.Info("onCreate StationSystem");
            
            _systemQuery =  GetEntityQuery(new EntityQueryDesc
            {
                All = typeof(Tupdate) == typeof(Updated) ? new ComponentType[] 
                {
                    ComponentType.ReadOnly<Created>()
                } : new ComponentType[] {},
                Any = new ComponentType[]
                {
                    ComponentType.ReadOnly<TBuilding>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<UniqueObject>(), 
                    ComponentType.ReadOnly<OutsideConnection>(),
                    ComponentType.ReadOnly<DistrictNamedBuilding>()
                }
            });
            
            _otherRelevantBuildings =  GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] 
                {
                    ComponentType.ReadOnly<TAuxComponent>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                }
            });
            if (typeof(Tupdate) == typeof(Updated))
            {
                RequireForUpdate(_systemQuery);
            }
            else
            {
                UpdateOperation();
                EntityQuery dummyQuery = GetEntityQuery(new EntityQueryDesc
                {
                    All = new ComponentType[] 
                    {
                        ComponentType.ReadOnly<Created>()
                    },
                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Created>()
                    }
                });
                
                RequireForUpdate(dummyQuery); // We do not want to trigger an onUpdate, as this is the case for setting ALL labels
            }
        }
        
        protected override void OnUpdate()
        {
            UpdateOperation();
        }
        
        private void UpdateOperation()
        {

            if (!_modOptions.GetChangeAllowed<TAuxComponent>())
            {
                return;
            }

            var applyTo = _modOptions.GetApplyTo<TAuxComponent>();
            
            _log.Info(applyTo);
            
            var targetBuildingEntities = _systemQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < targetBuildingEntities.Length; i++)
            {
                _log.Info(targetBuildingEntities[i]);
                var buildingData = BuildingDetails.getBuildingDetails(targetBuildingEntities[i], EntityManager);
                
                _log.Info(buildingData[0] + " - " + buildingData[1]);
                if (buildingData[0] != null)
                {
                    var previousName = Mod.GameNameSystem.GetRenderedLabelName(targetBuildingEntities[i]);
                    
                    var relevantStations = _otherRelevantBuildings.ToEntityArray(Allocator.Temp);
                    
                    bool isFirstBuildingInDistrict = true;

                    if (applyTo == 0)
                    {
                        for (int j = 0; j < relevantStations.Length; j++)
                        {
                            var otherBuildingData = BuildingDetails.getBuildingDetails(relevantStations[j], EntityManager);
                            if (buildingData.SequenceEqual(otherBuildingData))
                            {
                                isFirstBuildingInDistrict = false;
                                break;
                            }
                        }
                    }
                    
                    if (isFirstBuildingInDistrict && applyTo== 0 || applyTo == 1) //add with district name
                    {
                        Mod.GameNameSystem.SetCustomName(targetBuildingEntities[i],
                            _modOptions.stationFormat.Replace("{district}", buildingData[0])
                                .Replace("{station}", previousName));

                        
                        EntityManager.AddComponentData(targetBuildingEntities[i], componentData: new TAuxComponent());
                        EntityManager.AddComponentData(targetBuildingEntities[i], componentData: new DistrictNamedBuilding());
                    }
                    else //applyTo == 2
                    {
                        Building transportBuilding = EntityManager.GetComponentData<Building>(targetBuildingEntities[i]);

                        Aggregated aggregated =
                            EntityManager.GetComponentData<Aggregated>(transportBuilding.m_RoadEdge);
                        var streetName = Mod.GameNameSystem.GetRenderedLabelName(aggregated.m_Aggregate);

                        if (streetName.Length > 0)
                        {
                            Mod.GameNameSystem.SetCustomName(targetBuildingEntities[i],
                                _modOptions.stationFormat.Replace("{district}", streetName)
                                    .Replace("{station}", previousName));
                            EntityManager.AddComponentData(targetBuildingEntities[i], componentData: new DistrictNamedBuilding());

                        }
                                
                    }
                    
                }
            }
        }
        

}