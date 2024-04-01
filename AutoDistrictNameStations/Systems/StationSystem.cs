using System;
using System.Linq;
using AutoDistrictNameStations.AuxComponents;
using Colossal.Logging;
using Game;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Routes;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using TransportStation = Game.Buildings.TransportStation;


namespace AutoDistrictNameStations.Systems
{
    public partial class StationSystem : GameSystemBase
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
                All = new ComponentType[] 
                {
                    ComponentType.ReadOnly<Updated>()
                },
                Any = new ComponentType[]
                {
                    ComponentType.ReadOnly<TransportStation>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>()
                }
            });
            
            // we want to use _systemQuery as it will have the station and stops, but we only want the onUpdated with new stops
            _systemStopsQuery =  GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] 
                {
                    ComponentType.ReadOnly<Created>()
                },
                Any = new ComponentType[]
                {
                    ComponentType.ReadOnly<Game.Routes.TransportStop>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<TramStop>()
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
            

            
            RequireForUpdate(_systemQuery);
            RequireForUpdate(_systemStopsQuery); 
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
            _log.Info("onUpdate StationSystem executed");
            
            if (!_modOptions.GetChangeAllowed<FirstDistrictStation>())
            {
                return;
            }
            
            var applyTo = _modOptions.GetApplyTo<FirstDistrictStation>();
            
            var transportStations = _systemQuery.ToEntityArray(Allocator.Temp);
            
            string districtName = "";
            string streetName = "";
            bool isStreetStation = false;
            for (int i = 0; i < transportStations.Length; i++)
            {
                try
                {
                    var transportStation = transportStations[i];
                    var stationDetails = getStationDetail(transportStation); //[0] district [1] stationType
                    districtName = stationDetails[0];
                    _log.Info("Station details:" + stationDetails[0] + " - " + stationDetails[1]);
                    
                    if (stationDetails[0] != null)
                    {
                        
                        var previousName = Mod.GameNameSystem.GetRenderedLabelName(transportStation);
                        _log.Info("Previous name:" + previousName);
                        if (!previousName.Contains(districtName))
                        {
                            var relevantStations = _allStations.ToEntityArray(Allocator.Temp);
                            _log.Info("Relevant number stations: " + relevantStations.Length);
                            bool isFirstStationInDistrict = true;
                            if (applyTo == 0)
                            {
                                for (var j = 0; j < relevantStations.Length; j++)
                                {
                                    var otherStationDetails = getStationDetail(relevantStations[j]);
                                    _log.Info("Other station details:" + stationDetails[0] + "--" + stationDetails[1]);
                                    if (stationDetails.SequenceEqual(otherStationDetails))
                                    {
                                        isFirstStationInDistrict = false;
                                        break;
                                    }
                                }
                            }
                            
                            if (isFirstStationInDistrict && applyTo== 0 || applyTo == 1) //add new station
                            {
                                Mod.GameNameSystem.SetCustomName(transportStation,
                                    _modOptions.stationFormat.Replace("{district}", districtName)
                                        .Replace("{station}", previousName));
                                EntityManager.AddComponentData(transportStation, new FirstDistrictStation());
                            }
                            else
                            {
                                Building transportBuilding = EntityManager.GetComponentData<Building>(transportStation);

                                Aggregated aggregated =
                                    EntityManager.GetComponentData<Aggregated>(transportBuilding.m_RoadEdge);
                                streetName = Mod.GameNameSystem.GetRenderedLabelName(aggregated.m_Aggregate);

                                isStreetStation = true;
                                
                                if (!previousName.Contains(streetName))
                                {
                                    Mod.GameNameSystem.SetCustomName(transportStation,
                                        _modOptions.stationFormat.Replace("{district}", streetName)
                                            .Replace("{station}", previousName));
                                }
                                
                            }
                            
                        }
                    } 
                }
                catch (Exception ex) // The transportStops will cause an error as they do not provide currentDistrict component
                {
                    _log.Error($"An error occurred: {ex.Message}");
                }
            }
            
            var transportStops = _systemStopsQuery.ToEntityArray(Allocator.Temp);
            if (districtName.Length > 0)
            {
                for (int i = 0; i < transportStops.Length; i++)
                {
                    try
                    {
                        var previousName = Mod.GameNameSystem.GetRenderedLabelName(transportStops[i]);
                        if (!isStreetStation)
                        {
                            if (!previousName.Contains(districtName) && !isStreetStation)
                            {
                                Mod.GameNameSystem.SetCustomName(transportStops[i], districtName);
                            }
                        }
                        else
                        {
                            if (!previousName.Contains(streetName))
                            {
                                Mod.GameNameSystem.SetCustomName(transportStops[i], streetName);
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
}