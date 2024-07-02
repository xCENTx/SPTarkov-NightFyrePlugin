using EFT;
using UnityEngine;
using Comfort.Common;
using System.Diagnostics;
using System.Collections.Generic;
using EFT.Interactive;
namespace TarkovModMenu
{
    public class TarkovModMan : MonoBehaviour
    {
        private static GameWorld pWorld;
        private static GameObject pLocalPlayerObject;
        private static Player pLocalPlayer;
        private static Camera pCamera;

        
        //  called once per frame
        //  start game logic cycle
        void Update()
        {
            if (Plugin.bEnable.Value == false)
                return;

            //  Get World Object
            pWorld = Singleton<GameWorld>.Instance;
            
            //  Get Camera Object
            pCamera = Camera.main;

            //  Get Local Player Object
            //  pLocalPlayerObject = GameObject.Find("PlayerSuperior(clone)");  //  resource heavy

            //  Get local player 
            if (pWorld != null)
            {
                foreach (var ent in pWorld.AllAlivePlayersList)
                {
                    if (!ent.IsYourPlayer)
                        continue;

                    pLocalPlayer = ent;
                }
            }
            else
            {
                //  Clear Mines List
                if (Features.bESPMines_Obtained == true)
                {
                    Features.bESPMines_Obtained = false;
                    Features.Mines.Clear();
                }
            }

            if (pLocalPlayer != null)
            {
                /// 
                //  if (Plugin.dbg_bDumpBones.Value == true)
                //  {
                //      Plugin.dbg_bDumpBones.Value = false;
                //      DebugTools.DumpPlayerBoneNames();
                //  }

                if (Features.bESPMines_Obtained == false)
                {
                    var claymores = GameObject.FindObjectsOfType<MineDirectional>();
                    foreach (var claymore in claymores)
                    {
                        //  check if claymore is enabled
                        if (claymore.enabled == false)
                            continue;

                        //  get position
                        Features.Mines.Add(claymore.transform.position);
                    }

                    var mines = GameObject.FindObjectsOfType<Minefield>();
                    foreach (var mine in mines)
                    {
                        //  Check if mine enabled
                        if (mine.enabled == false)
                            continue;

                        //  get position
                        Features.Mines.Add(mine.transform.position);
                    }

                    Features.bESPMines_Obtained = true;
                }
            }
            else
            {
                //  Clear Mines List
                Features.bESPMines_Obtained = false;
                Features.Mines.Clear();

                //  if (Plugin.bNoVisor.Value == true)
                //      Plugin.bNoVisor.Value = false;

                if (Plugin.bNoSway.Value == true)
                    Plugin.bNoSway.Value = false;

                if (Plugin.bNoRecoil.Value == true)
                    Plugin.bNoRecoil.Value = false;

                if (Plugin.bPlayerUFO.Value == true)
                    Plugin.bPlayerUFO.Value = false;

                if (Plugin.bNoMalfunctions.Value == true)
                    Plugin.bNoMalfunctions.Value = false;
            }
        }

        //  called once per frame
        //  end game logic cycle
        void LateUpdate()
        {
            if (pLocalPlayer == null || pCamera == null)
                return;

            if (Plugin.bPlayerUFO.Value == true && Input.GetKey(KeyCode.W))
                Features.PLAYER_SpeedHack(pLocalPlayer, pCamera, Plugin.fPlayerUFOSpeed.Value);

            if (Plugin.bNoRecoil.Value == true)
                Features.WEAPON_NoRecoil(pLocalPlayer);

            if (Plugin.bNoSway.Value == true)
                Features.WEAPON_NoSway(pLocalPlayer);

            if (Plugin.bNoMalfunctions.Value == true)
                Features.WEAPON_NoMalfunction();

            //  if (Plugin.bNoVisor.Value == true)
            //      Features.PLAYER_NoVisor(pCamera, true);
        }

        private static Stopwatch clock = new Stopwatch();           //  ONLY TO BE USED IN OnGUI
        private static int OnGui_frame = 0;                        //  ONLY TO BE USED IN OnGUI
        private static List<double> OnGUI_FrameTimes = new List<double>();
        private static double OnGui_AvgFrameTime = 0f;
        //  called at the end of the frame, after all game logic.
        //  called twice. Once for rendering and once again for gui events
        void OnGUI()
        {
            Style.DrawStyleUpdate();

            // Start Timer
            clock.Restart();

            /// 
            //  if (Plugin.dbg_bFindObject.Value == true)
            //      pLocalPlayerObject = GameObject.Find("PlayerSuperior(clone)");

            if (Plugin.bEnable.Value == true && pWorld != null && pCamera != null)
            {
                //  Scav ESP
                if (Plugin.bShowScavs.Value == true)
                    Features.ESP_Scavs(pWorld, pCamera, pLocalPlayer, Plugin.scavStyle, Plugin.fShowPlayersDistance.Value);

                //  PMC ESP
                if (Plugin.bShowPMCs.Value == true)
                    Features.ESP_PMC(pWorld, pCamera, pLocalPlayer, Plugin.pmcStyle, Plugin.fShowPlayersDistance.Value);

                //  Loose Loot ESP
                if (Plugin.bShowLooseLoot.Value == true)
                    Features.ESP_LooseLoot(pWorld, pCamera, pLocalPlayer, Plugin.looseLootStyle, Plugin.fShowLootDistance.Value);

                //  Corpse ESP
                if (Plugin.bShowDeadBodies.Value == true)
                    Features.ESP_LootCorpses(pWorld, pCamera, pLocalPlayer, Plugin.bodyStyle, Plugin.fShowLootDistance.Value);

                //  Mine ESP
                if (Plugin.bShowMines.Value == true)
                    Features.ESP_Mines(pWorld, pCamera, pLocalPlayer, null, Plugin.fShowMinesDistance.Value);

                //  Extract ESP
                if (Plugin.bShowExtracts.Value == true)
                    Features.ESP_ExtractPoints(pWorld, pCamera, pLocalPlayer, null, 0f);
            }

            //  Stop Timer
            clock.Stop();

            //  Get Timer MS
            double execution_time_ms = clock.ElapsedMilliseconds;   //  get time for this frame
            OnGui_frame++;  //  increment the frame
            if (OnGui_frame >= 100)
            {
                double avg_timing = 0f;
                foreach (var frame in OnGUI_FrameTimes)
                    avg_timing += frame;

                OnGui_AvgFrameTime = avg_timing / OnGUI_FrameTimes.Count;   //  get average frame time
                OnGUI_FrameTimes.Clear();   //  clear frame times
                OnGui_frame = 0;    //  reset frame counter
            }
            else
                OnGUI_FrameTimes.Add(execution_time_ms);

            ///  Render Watermark
            Vector2 StartPos = new Vector2(Screen.width * .01f, Screen.height * .01f);  //  
            float widthPadding = StartPos.x;                                            //
            float heightPadding = StartPos.y;                                           //

            //  Name
            string name_watermark = $"{PluginInfo.PLUGIN_NAME}";                        //  Plugin Name
            Vector2 szText = Draw.CalcTextSize(name_watermark);                         //
            GUI.Box(new Rect(StartPos.x - (szText.x * .1f), StartPos.y, szText.x + (szText.x * .2f), szText.y * 2), new GUIContent());  //  background window
            Draw.DrawString(StartPos, name_watermark, Color.white, false);      //  

            //  Version
            string version_watermark = $"v{PluginInfo.PLUGIN_VERSION}";                 //  Plugin Version
            szText = Draw.CalcTextSize(version_watermark);                              //  
            Draw.DrawString(new Vector2(StartPos.x, StartPos.y + szText.y), version_watermark, Color.white, false);

            ///  Render Debug Text
            if (Plugin.dbg_bDebugText.Value == true)
            {
                Vector2 dbgStartPos = new Vector2(Screen.width, StartPos.y + heightPadding * 3);
                string dbgText_frametime = $"OnGUI Update: {OnGui_AvgFrameTime:F1}ms";
                szText = Draw.CalcTextSize(dbgText_frametime);
                Draw.DrawString(new Vector2((dbgStartPos.x - szText.x) - widthPadding, dbgStartPos.y), dbgText_frametime, Color.yellow, false);
            
                string dbgText_gameWorld = $"World: {pWorld}";
                szText = Draw.CalcTextSize(dbgText_gameWorld);
                Draw.DrawString(new Vector2((dbgStartPos.x - szText.x) - widthPadding, dbgStartPos.y + szText.y), dbgText_gameWorld, Color.yellow, false);
            
                string dbgText_player = $"Player: {pLocalPlayer}";
                szText = Draw.CalcTextSize(dbgText_player);
                Draw.DrawString(new Vector2((dbgStartPos.x - szText.x) - widthPadding, dbgStartPos.y + szText.y * 2), dbgText_player, Color.yellow, false);
            
                if (pWorld != null)
                {
                    string dbgText_actors = $"Actors: {pWorld.AllAlivePlayersList.Count}";
                    szText = Draw.CalcTextSize(dbgText_actors);
                    Draw.DrawString(new Vector2((dbgStartPos.x - szText.x) - widthPadding, dbgStartPos.y + szText.y * 3), dbgText_actors, Color.yellow, false);
            
                    string dbgText_mines = $"Mines: {Features.Mines.Count}";
                    szText = Draw.CalcTextSize(dbgText_mines);
                    Draw.DrawString(new Vector2((dbgStartPos.x - szText.x) - widthPadding, dbgStartPos.y + szText.y * 4), dbgText_mines, Color.yellow, false);
                }
            }
        }
    
        //  called at a fixed frequency and is independent of the games frame rate / update loops
        //  *Physics Operations
        void FixedUpdate()
        {

        }

        //  called when an instance of a class is disabled by it's parent
        void OnDisable()
        {

        }

        //  called when an instance of a class is destroyed by it's parent
        void OnDestroy()
        {

        }
    }
}
