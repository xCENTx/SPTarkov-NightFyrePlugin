using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace TarkovModMenu
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static GameObject Hook = new GameObject("PluginHook");

        internal static ConfigEntry<bool> bEnable;                  //  universal activation. if not enabled main unity methods will get returned
        internal static ConfigEntry<bool> dbg_bDebugText;           //  draws debug information on the canvas
        internal static ConfigEntry<bool> dbg_bFindObject;          //  test find objects in loop for fps comparisons
        internal static ConfigEntry<bool> dbg_bDumpBones;           //  dumps bone names to console

        internal static ConfigEntry<bool> bPlayerUFO;               //  player fly mode | capped at 10f for speed mod
        internal static ConfigEntry<bool> bNoVisor;                 //  disables visor visual obstruction
        internal static ConfigEntry<bool> bNoMalfunctions;          //  disables weapon malfunctions
        internal static ConfigEntry<bool> bNoDurabilityLoss;        //  disables item durability loss
        internal static ConfigEntry<bool> bNoRecoil;                //  disables weapon recoil | is permanent for the weapon held until restart
        internal static ConfigEntry<bool> bNoSway;                  //  disables hand idle sway
        internal static ConfigEntry<bool> bNoStaminaDrain;          //  disables player stamina drain


        internal static ConfigEntry<bool> bShowScavs;               //  scav esp
        internal static ConfigEntry<bool> bShowPMCs;                //  pmc esp
        internal static ConfigEntry<bool> bShowDeadBodies;          //  corpse esp
        internal static ConfigEntry<bool> bShowExtracts;            //  extract esp
        internal static ConfigEntry<bool> bShowMines;               //  mine esp
        internal static ConfigEntry<bool> bShowLooseLoot;           //  loose loot esp
        internal static ConfigEntry<bool> bShowLootBoxes;           //  loot box esp

        internal static ConfigEntry<Color> colorScav;               //   
        internal static ConfigEntry<Color> colorPMC;                //   
        internal static ConfigEntry<Color> colorLooseLoot;          //   
        internal static ConfigEntry<Color> colorLootBoxes;          //   
        internal static ConfigEntry<Color> colorDeadBodies;         //   

        internal static ConfigEntry<float> fShowPlayersDistance;    //  scav and pmc draw distance
        internal static ConfigEntry<float> fShowLootDistance;       //  loot container and loose loot draw distance
        internal static ConfigEntry<float> fShowMinesDistance;      //  mines and claymores draw distance
        internal static ConfigEntry<float> fPlayerUFOSpeed;         //  player ufo mode speed   
        
        internal static GUIStyle scavStyle = new GUIStyle();
        internal static GUIStyle pmcStyle = new GUIStyle();
        internal static GUIStyle bodyStyle = new GUIStyle();
        internal static GUIStyle lootStyle = new GUIStyle();
        internal static GUIStyle looseLootStyle = new GUIStyle();

        private void Awake()
        {
            /// DEBUG CONTROLS
            dbg_bDebugText      = Config.Bind("DEBUG", "Debug Text", false, "displays debug information on the top left of the canvas");
            //  dbg_bFindObject     = Config.Bind("DEBUG", "Find Object", false, "introduces a find object call in the OnGUI method");
            //  dbg_bDumpBones      = Config.Bind("DEBUG", "Dump Bones", false, "dumps player bones to the console.");

            /// GENERAL CONTROLS
            bEnable             = Config.Bind("GENERAL", "ENABLE", false, "enables features. Useful for when a map is not loading.");

            /// PLAYER CONTROLS
            bPlayerUFO          = Config.Bind("PLAYER", "UFO Mode", false, "modifies player forward velocity.");
            //  bNoVisor            = Config.Bind("PLAYER", "No Visor", false, "disables visor visual affect.");
            fPlayerUFOSpeed      = Config.Bind("PLAYER", "Player UFO Speed", 0f, new ConfigDescription("player UFO movement speed.", new AcceptableValueRange<float>(0f, 10f)));

            /// WEAPON CONTROLS
            bNoRecoil           = Config.Bind("WEAPON", "No Recoil", false, "disables weapon recoil.");
            bNoSway             = Config.Bind("WEAPON", "No Sway", false, "disables weapon idle sway.");
            bNoMalfunctions     = Config.Bind("WEAPON", "No Malfunctions", false, "disables weapon malfunctions");

            /// ESP CONTROLS
            bShowScavs          = Config.Bind("ESP", "SCAVS", false, "name tags, boxes & bones for Scav's.");
            bShowPMCs           = Config.Bind("ESP", "PMC", false, "name tags, boxes & bones for PMC's.");
            bShowDeadBodies     = Config.Bind("ESP", "BODIES", false, "name tags for dead bodies.");
            //  bShowLootBoxes      = Config.Bind("ESP", "LOOT", false, "name tags for loot containers.");
            bShowLooseLoot      = Config.Bind("ESP", "LOOSE LOOT", false, "name tags for loose loot items.");
            bShowExtracts       = Config.Bind("ESP", "EXTRACTS", false, "name tags for Extracts.");
            bShowMines          = Config.Bind("ESP", "MINES", false, "draws name tags for active mines and claymores.");

            /// COLORS
            colorScav           = Config.Bind("COLORS", "SCAVS", Color.yellow, "color for Scav visuals.");
            colorPMC            = Config.Bind("COLORS", "PMC", Color.blue, "color for PMC visuals.");
            colorDeadBodies     = Config.Bind("COLORS", "BODIES", Color.white, "color for PMC visuals.");
            //  colorLootBoxes      = Config.Bind("COLORS", "LOOT", Color.magenta, "color for Loot Box visuals.");
            colorLooseLoot      = Config.Bind("COLORS", "LOOSE LOOT", Color.cyan, "color for Loose Loot visuals.");
            scavStyle.normal.textColor      = colorScav.Value;
            pmcStyle.normal.textColor       = colorPMC.Value;
            bodyStyle.normal.textColor      = colorDeadBodies.Value;
            //  lootStyle.normal.textColor      = colorLootBoxes.Value;
            looseLootStyle.normal.textColor = colorLooseLoot.Value;

            /// DISTANCE
            fShowPlayersDistance = Config.Bind("DISTANCE","Players & Scavs", 0f, new ConfigDescription("max distance for rendering PMCs and Scav's. use 0 for no max distance.", new AcceptableValueRange<float>(0f, 100f)));
            fShowLootDistance   = Config.Bind("DISTANCE",  "Loot & Corpses", 0f, new ConfigDescription("max distance for rendering loot and corpses. use 0 for no max distance.", new AcceptableValueRange<float>(0f, 100f)));
            fShowMinesDistance  = Config.Bind("DISTANCE", "Mines & Claymores", 0f, new ConfigDescription("max distance for rendering mines and claymores. use 0 for no max distance.", new AcceptableValueRange<float>(0f, 100f)));

            //  Hook Component
            Hook.AddComponent<TarkovModMan>();
            DontDestroyOnLoad(Hook);

            //  Log information
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
