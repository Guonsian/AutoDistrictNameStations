using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;

namespace AutoDistrictNameStations
{
    [FileLocation("AutoDistrictNameStations")]
    [SettingsUIShowGroupName(section)]
    public class ModOptions(IMod mod) : ModSetting(mod)
    {
        
        public const string section = "mainSection";
        
        [SettingsUIDropdown(typeof(OptionsProvider), nameof(OptionsProvider.GetOptions))]
        [SettingsUISection(section)]
        public string stationFormat { get; set; }
        
        public override void SetDefaults()
        {
            stationFormat = "{district} {station}";
        }
    }
    public static class OptionsProvider
    {
        public static DropdownItem<string>[] GetOptions()
        {
            return new[]
            {
                new DropdownItem<string> { value = "{district} {station}", displayName = "{district} {station}" },
                new DropdownItem<string> { value = "{district} | {station}", displayName = "{district} | {station}" },
                new DropdownItem<string> { value = "{district}'s {station}", displayName = "{district}'s {station}" }
            };
        }
    }
}