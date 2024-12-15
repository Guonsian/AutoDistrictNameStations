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

public partial class GenericSystem<TBuilding, TAuxComponent> : GameSystemBase  where TAuxComponent : class, IComponentData, new()
{
    
        private EntityQuery _systemQuery;
        private EntityQuery _systemQuery2;
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
                All = new ComponentType[] 
                {
                    ComponentType.ReadOnly<Created>()
                },
                Any = new ComponentType[]
                {
                    ComponentType.ReadOnly<TBuilding>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<OutsideConnection>(),
                    ComponentType.ReadOnly<DistrictNamedBuilding>()
                }
            });
            
            _systemQuery2 =  GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] {},
                Any = new ComponentType[]
                {
                    ComponentType.ReadOnly<TBuilding>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
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

            RequireForUpdate(_systemQuery);
            
        }
        
        protected override void OnUpdate()
        {
            UpdateOperation();
        }
        
        public void UpdateAll()
        {
            UpdateOperation(true);
        }

        private void UpdateStreetname(Entity targetBuildingEntity, string previousName)
        {
            Building transportBuilding = EntityManager.GetComponentData<Building>(targetBuildingEntity);

            if (EntityManager.HasComponent<Extension>(targetBuildingEntity))
            {
                _log.Info("Extension in Generic system, not updating name");
                return;
            }

            Aggregated aggregated =
                EntityManager.GetComponentData<Aggregated>(transportBuilding.m_RoadEdge);
            var streetName = Mod.GameNameSystem.GetRenderedLabelName(aggregated.m_Aggregate);

            if (streetName.Length > 0)
            {
                Mod.GameNameSystem.SetCustomName(targetBuildingEntity,
                    _modOptions.stationFormat.Replace("{district}", streetName)
                        .Replace("{station}", previousName));
                EntityManager.AddComponentData(targetBuildingEntity, componentData: new DistrictNamedBuilding());

            }
        }
        
        private void UpdateOperation(bool allBuildings = false)
        {

            if (!_modOptions.GetChangeAllowed<TAuxComponent>())
            {
                return;
            }

            var applyTo = _modOptions.GetApplyTo<TAuxComponent>();
            
            _log.Info(applyTo);
            
            var targetBuildingEntities = (allBuildings ? _systemQuery2 : _systemQuery).ToEntityArray(Allocator.Temp);

            for (int i = 0; i < targetBuildingEntities.Length; i++)
            {
                _log.Info(targetBuildingEntities[i]);
                var buildingData = BuildingDetails.getBuildingDetails(targetBuildingEntities[i], EntityManager);
                
                _log.Info(buildingData[0] + " - " + buildingData[1]);
                var previousName = Mod.GameNameSystem.GetRenderedLabelName(targetBuildingEntities[i]);
                
                if (buildingData[0] != null)
                {

                    if (!_modOptions.allowUnique && EntityManager.HasComponent<UniqueObject>(targetBuildingEntities[i]))
                    {
                        return;
                    }
                    
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
                        UpdateStreetname(targetBuildingEntities[i], previousName);
                    }
                    
                }
                else
                {
                    if (Mod.ModCustomOptions.allowStreetnaming)
                    {
                        UpdateStreetname(targetBuildingEntities[i], previousName);
                    }
                }
            }
        }
        

}