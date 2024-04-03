using System;
using System.Collections.Generic;
using System.Linq;
using AutoDistrictNameStations.AuxComponents;
using Colossal.Entities;
using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.UI.Localization;
using Unity.Collections;
using Unity.Entities;
using OutsideConnection = Game.Net.OutsideConnection;
using TransportStation = Game.Buildings.TransportStation;
using TransportStop = Game.Prefabs.TransportStop;


namespace AutoDistrictNameStations.Systems
{
    public partial class StationSystem<T> : GameSystemBase
    {

        private EntityQuery _systemQuery;
        private EntityQuery _systemStopsQuery;
        private EntityQuery _allStations;
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
                All = typeof(T) == typeof(Updated) ? new ComponentType[] 
                {
                    ComponentType.ReadOnly<Updated>()
                } : new ComponentType[] {},
                Any = new ComponentType[]
                {
                    ComponentType.ReadOnly<TransportStation>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<OutsideConnection>()
                }
            });
            
            // we want to use _systemQuery as it will have the station and stops, but we only want the onUpdated with new stops
            _systemStopsQuery =  GetEntityQuery(new EntityQueryDesc
            {
                All = typeof(T) == typeof(Updated) ? new ComponentType[] 
                {
                    ComponentType.ReadOnly<Created>()
                } : new ComponentType[] {},
                Any = new ComponentType[]
                {
                    ComponentType.ReadOnly<Game.Routes.TransportStop>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(), 
                    ComponentType.ReadOnly<TramStop>(),
                    ComponentType.ReadOnly<DistrictNamedBuilding>()
                }
            });
            
            _allStations =  GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] 
                {
                    ComponentType.ReadOnly<FirstDistrictStation>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<TramStop>()
                }
            });

            if (typeof(T) == typeof(Updated))
            {
                Mod.log.Info("Requiering update");
                RequireForUpdate(_systemQuery);
                RequireForUpdate(_systemStopsQuery); 
            }
            else
            {
                updateOperation();
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

        private string[] getStationDetail(Entity stationEntity)
        {
            CurrentDistrict currentDistrict = EntityManager.GetComponentData<CurrentDistrict>(stationEntity);
            string station_type = Mod.GameNameSystem.GetDebugName(stationEntity);
            station_type = station_type.Substring(0, station_type.IndexOf(' '));

            string districtLabelName;

            if (currentDistrict.m_District != Entity.Null)
            {
                try
                {
                    districtLabelName = Mod.GameNameSystem.GetRenderedLabelName(currentDistrict.m_District);
                }
                catch(Exception _)
                {
                    _log.Warn("District label not found:" + _);
                    districtLabelName = null;
                }
            }
            else
            {
                districtLabelName = null;
            }
            
            return new[]
            {
                districtLabelName, station_type
            };
        }

        protected override void OnUpdate()
        {
            updateOperation();
        }
        
        private void updateOperation()  
        {
            _log.Info("UpdateOperation StationSystem executed");
            if (!_modOptions.GetChangeAllowed<FirstDistrictStation>())
            {
                return;
            }
            
            var applyTo = _modOptions.GetApplyTo<FirstDistrictStation>();
            
            var transportStations = _systemQuery.ToEntityArray(Allocator.Temp);
            
            Dictionary<Entity, string> stations = new Dictionary<Entity, string>();

            for (int i = 0; i < transportStations.Length; i++)
            {
                try
                {
                    var transportStation = transportStations[i];
                    var stationDetails = getStationDetail(transportStation); //[0] district [1] stationType
                    string districtName  = stationDetails[0];
                    _log.Info("Station details:" + stationDetails[0] + " - " + stationDetails[1]);
                    
                    if (stationDetails[0] != null)
                    {
                        
                        var previousName = Mod.GameNameSystem.GetRenderedLabelName(transportStation);
                        //_log.Info("Previous name:" + previousName);
                        
                        //_log.Info();
                        if (!previousName.Contains(districtName))
                        {
                            var relevantStations = _allStations.ToEntityArray(Allocator.Temp);
                            //_log.Info("Relevant number stations: " + relevantStations.Length);
                            bool isFirstStationInDistrict = true;
                            if (applyTo == 0)
                            {
                                for (var j = 0; j < relevantStations.Length; j++)
                                {
                                    var otherStationDetails = getStationDetail(relevantStations[j]);
                                    //_log.Info("Other station details:" + otherStationDetails[0] + " - " + otherStationDetails[1]);
                                    if (stationDetails.SequenceEqual(otherStationDetails))
                                    {
                                        isFirstStationInDistrict = false;
                                        break;
                                    }
                                }
                            }
                            
                            if (isFirstStationInDistrict && applyTo== 0 || applyTo == 1) //add new station
                            {
                                Mod.log.Info("Added district name");
                                Mod.GameNameSystem.SetCustomName(transportStation,
                                    _modOptions.stationFormat.Replace("{district}", districtName)
                                        .Replace("{station}", previousName));
                                EntityManager.AddComponentData(transportStation, new FirstDistrictStation());
                                EntityManager.AddComponentData(transportStation, new DistrictNamedBuilding());
                                stations.Add(transportStation, districtName);
                            }
                            else
                            {
                                Building transportBuilding = EntityManager.GetComponentData<Building>(transportStation);

                                Aggregated aggregated =
                                    EntityManager.GetComponentData<Aggregated>(transportBuilding.m_RoadEdge);
                                string streetName = Mod.GameNameSystem.GetRenderedLabelName(aggregated.m_Aggregate);
                                
                                if (!previousName.Contains(streetName))
                                {
                                    Mod.log.Info("Added street name");
                                    Mod.GameNameSystem.SetCustomName(transportStation,
                                        _modOptions.stationFormat.Replace("{district}", streetName)
                                            .Replace("{station}", previousName));
                                    EntityManager.AddComponentData(transportStation, new DistrictNamedBuilding());
                                }
                                stations.Add(transportStation, streetName);
                            }
                            
                        }
                        else
                        {
                            Mod.log.Info("Station updated that had the district name");
                            stations.Add(transportStation, districtName);
                        }
                    } 
                }
                catch (Exception ex) // The transportStops will cause an error as they do not provide currentDistrict component
                {
                    _log.Error($"An error occurred: {ex.Message}");
                }
            }
            
            var transportStops = _systemStopsQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < transportStops.Length; i++)
            {
                try
                {
                    var previousName = Mod.GameNameSystem.GetRenderedLabelName(transportStops[i]);
                    Entity owner_entity = EntityManager.GetComponentData<Owner>(transportStops[i]).m_Owner;
                    if (!stations.ContainsKey(owner_entity))  // This is not necessary for airports
                    {
                        Entity attached_entity = EntityManager.GetComponentData<Attached>(transportStops[i]).m_Parent;
                        owner_entity = EntityManager.GetComponentData<Owner>(attached_entity).m_Owner;
                        if (!stations.ContainsKey(owner_entity))
                        {
                            try
                            {
                                owner_entity =
                                    EntityManager.GetComponentData<Owner>(owner_entity)
                                        .m_Owner; // In case Owner has another owner (subway for train station)
                            }
                            catch (Exception ex)
                            {
                                Mod.log.Error(ex);
                            }
                        }
                    }

                    if (stations.TryGetValue(owner_entity, out string stopName))
                    {
                        if (!previousName.Contains(stopName))
                        {
                            Mod.GameNameSystem.SetCustomName(transportStops[i], stopName);
                            EntityManager.AddComponentData(transportStops[i], new DistrictNamedBuilding());
                        }
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