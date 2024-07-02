using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TarkovModMenu
{
    internal class Features
    {
        public static void ESP_Scavs(GameWorld pWorld, Camera pCamera, Player pLocalPlayer, GUIStyle style, float distance = 0f)
        {
            if (pWorld == null || pCamera == null || pLocalPlayer == null)
                return;

            //  Get Alive Players List
            var players = pWorld.AllAlivePlayersList;
            if (players == null || players.Count == 0)
                return;

            // Loop Player Array
            foreach (Player scav in players)
            {
                //  Skip Local Player
                if (scav.IsYourPlayer)
                    continue;

                //  Skip dead players
                if (scav.HealthController.IsAlive == false)
                    continue;

                //  Skip Non Scav Players
                if (!GameTools.IsScav(scav))
                    continue;

                //  Get player pos          
                Vector3 pos = scav.Transform.position;                

                // Get Distance to player
                var dist = CameraTools.Distance(pCamera, pos);
                if (distance > 0f && dist > distance)
                    continue;

                // Project To Screen
                Vector2 screenOrigin;
                if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pos, out screenOrigin))
                    continue;

                //  Get Name & Color
                string name = "SCAV";
                string faction = string.Empty;
                Color color = style.normal.textColor;
                EHostileType eHostileType = GameTools.GetHostileType(scav);
                bool isBoss = eHostileType == EHostileType.EType_Boss;
                bool isCultist = eHostileType == EHostileType.EType_Cultist;
                if (isBoss)
                {
                    faction = "[BOSS] ";
                    color = Color.red;
                }
                else if(isCultist)
                {
                    faction = "[CULTIST] ";
                    color = Color.red;
                }
                float health = GameTools.GetHealthPercent(scav);
                string weaponName = GameTools.GetWeaponName(scav);

                //  Render Entity
                Draw.DrawString(screenOrigin, $"{faction}{name}\n{weaponName}\n[{health}%][{dist:F0}m]".Trim(), color);
                RenderTools.RenderBones(scav, Color.white, 2f, dist);
                RenderTools.RenderBox3D(scav, color, 2f);
            }
        }

        public static void ESP_PMC(GameWorld pWorld, Camera pCamera, Player pLocalPlayer, GUIStyle style, float distance = 0f) 
        {
            if (pWorld == null || pCamera == null || pLocalPlayer == null)
                return;

            //  Get Alive Players List
            var players = pWorld.AllAlivePlayersList;
            if (players == null || players.Count == 0)
                return;

            // Loop Player Array
            foreach (Player player in players)
            {
                //  Skip Local Player
                if (player.IsYourPlayer)
                    continue;

                //  Skip dead players
                if (player.HealthController.IsAlive == false)
                    continue;

                //  Skip Non PMC Players
                if (!GameTools.IsPMC(player))
                    continue;

                //  Get player pos
                PlayerBones bones = player.PlayerBones;                 //  
                Vector3 pos = player.Transform.position;                //  
                Vector3 headBone = bones.Head.position;                 //  
                Vector3 lShoulderBone = bones.LeftShoulder.position;    //  

                // Get Distance to player
                var dist = CameraTools.Distance(pCamera, pos);
                if (distance > 0f && dist > distance)
                    continue;

                //  Get PMC Faction
                string faction = GameTools.GetFactionName(player);
                string name = GameTools.GetPlayerName(player);
                EHostileType eType = GameTools.GetHostileType(player);
                float health = GameTools.GetHealthPercent(player);
                string weaponName = GameTools.GetWeaponName(player);

                // Project To Screen
                Vector2 screenOrigin;
                if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pos, out screenOrigin)) 
                    continue;

                //  Render Entity
                Draw.DrawString(screenOrigin, $"[{faction}] {name}\n{weaponName}\n[{health}%][{dist:F0}m]".Trim(), style.normal.textColor);
                RenderTools.RenderBones(player, Color.white, 2f, dist);
                RenderTools.RenderBox3D(player, style.normal.textColor, 2f);
            }
        }

        public void ESP_LootBoxes(GameWorld pWorld, Camera pCamera, Player pLocalPlayer, GUIStyle style, float distance = 0f) 
        {
            //  check valid context
            if (pWorld == null || pCamera == null || pLocalPlayer == null)
                return;

            //  Get corpse List
            var lootList = pWorld.ItemOwners;
            if (lootList == null || lootList.Count == 0)
                return;

            foreach (var item in lootList)
            {
                var itemOwner = item.Key;
                var rootItem = itemOwner.RootItem;
                if (rootItem.Template == null) 
                    continue;

                //  @TODO
            }
        }

        public static void ESP_LooseLoot(GameWorld pWorld, Camera pCamera, Player pLocalPlayer, GUIStyle style, float distance = 0f) 
        {
            //  check valid context
            if (pWorld == null || pCamera == null || pLocalPlayer == null)
                return;

            //  Get corpse List
            var lootList = pWorld.LootItems;
            if (lootList == null || lootList.Count == 0)
                return;

            //  Loop loot list
            for (int i = 0; i < lootList.Count; i++)
            {
                //  Get loot item by index
                var lootItem = lootList.GetByIndex(i);
                if (lootItem == null || lootItem.Item.Template == null)
                    continue;

                //  Get loot position
                Vector3 pos = lootItem.transform.position;

                //  check if is corpse type
                if (lootItem is Corpse) //  skip corpse loot items ? ?
                    continue;

                // get item name
                string itemName = lootItem.Item.ShortName.Localized();

                //  Get Distance
                var dist = CameraTools.Distance(pCamera, pos);
                if (distance > 0f && dist > distance)
                    continue;

                //  Project to screen
                Vector2 screenOrigin;
                if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pos, out screenOrigin)) continue;

                //  Render Target
                Draw.DrawTextBox(screenOrigin, $"{itemName} - {dist:F0}m", style.normal.textColor);
            }
        }
        
        public static void ESP_LootCorpses(GameWorld pWorld, Camera pCamera, Player pLocalPlayer, GUIStyle style, float distance = 0f)
        {
            //  check valid context
            if (pWorld == null || pCamera == null || pLocalPlayer == null)
                return;

            //  Get corpse List
            var lootList = pWorld.LootItems;
            if (lootList == null || lootList.Count == 0)
                return;

            for (int i = 0; i < lootList.Count; i++)
            {
                var lootItem = lootList.GetByIndex(i);
                if (lootItem == null || lootItem.Item.Template == null)
                    continue;

                if (lootItem is not Corpse) 
                    continue;

                Vector3 pos = lootItem.transform.position;

                //  Get Distance
                var dist = CameraTools.Distance(pCamera, pos);
                if (distance > 0f && dist > distance)
                    continue;

                //  Project to screen
                Vector2 screenOrigin;
                if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pos, out screenOrigin)) continue;

                //  Render Target
                Draw.DrawTextBox(screenOrigin, $"corpse - {dist:F0}m", style.normal.textColor);
            }
        }

        public static void ESP_ExtractPoints(GameWorld pWorld, Camera pCamera, Player pLocalPlayer, GUIStyle style, float distance = 0f)
        {
            //  check valid context
            if (pWorld == null || pCamera == null || pLocalPlayer == null)
                return;

            ExfiltrationControllerClass pExfilController = pWorld.ExfiltrationController;
            if (pExfilController == null) return;

            var pExfilPoints = pExfilController.ExfiltrationPoints;
            if (pExfilPoints != null)
            {
                foreach (var ex in pExfilPoints)
                {
                    if (ex == null)
                        continue;

                    //  Get Position
                    Vector3 pos = ex.transform.position;
                    if (pos == Vector3.zero)
                        continue;

                    //  Get Distance
                    var dist = CameraTools.Distance(pCamera, pos);
                    if (distance > 0f && dist > distance)
                        continue;

                    //  Project to screen
                    Vector2 screenOrigin;
                    if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pos, out screenOrigin)) continue;

                    //  Render Target
                    Draw.DrawTextBox(screenOrigin, $"{ex.name} - {dist:F0}m", Color.green);
                }
            }

            var pScavExfilPoints = pExfilController.ScavExfiltrationPoints;
            if (pScavExfilPoints != null)
            {

                foreach (var ex in pScavExfilPoints)
                {
                    if (ex == null)
                        continue;

                    //  Get Position
                    Vector3 pos = ex.transform.position;
                    if (pos == Vector3.zero)
                        continue;

                    //  Get Distance
                    var dist = CameraTools.Distance(pCamera, pos);
                    if (distance > 0f && dist > distance)
                        continue;

                    //  Project to screen
                    Vector2 screenOrigin;
                    if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pos, out screenOrigin)) continue;

                    //  Render Target
                    Draw.DrawTextBox(screenOrigin, $"{ex.name} - {dist:F0}m", Color.green);
                }
            }
        }

        public static bool bESPMines_Obtained = false;
        public static List<Vector3> Mines = new List<Vector3>(); //    list is updated in Update();
        public static void ESP_Mines(GameWorld pWorld, Camera pCamera, Player pLocalPlayer, GUIStyle style, float distance = 0f)
        {
            //  check valid context
            if (pWorld == null || pCamera == null || pLocalPlayer == null)
                return;

            foreach (var pos in Mines)
            {
                //  Get Distance
                var dist = CameraTools.Distance(pCamera, pos);
                if (distance > 0f && dist > distance)
                    continue;

                //  Project to screen
                Vector2 screenOrigin;
                if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pos, out screenOrigin)) continue;

                //  Render Target
                Draw.DrawTextBox(screenOrigin, $"mine - {dist:F0}m", Color.red);
            }
        }

        public static void PLAYER_NoVisor(Camera pCam, bool setting)
        {
            if (pCam == null)
                return;

            var component = pCam.GetComponent<VisorEffect>();
            if (component == null || Mathf.Abs(component.Intensity - Convert.ToInt32(!setting)) < Mathf.Epsilon)
                return;

            component.Intensity = Convert.ToInt32(!setting);
        }

        public static void PLAYER_SpeedHack(Player pTarget, Camera pCamera, float intensity = 2f)
        {// should be used in combination with a keyhold event
            GameWorld pWorld = GameTools.GetWorld();
            if (!pTarget || !pCamera || !pWorld)
                return;

            pTarget.Transform.position += intensity * Time.deltaTime * pCamera.transform.forward;
        }
        
        public static void WEAPON_NoRecoil(Player pTarget)
        {// 
            if (pTarget == null)
                return;

            var weapon = pTarget.ProceduralWeaponAnimation;
            if (weapon == null) return;

            var shoot = weapon.Shootingg.CurrentRecoilEffect;
            if (shoot == null) return;

            shoot.CameraRotationRecoilEffect.Intensity = 0f;
            shoot.HandPositionRecoilEffect.Intensity = 0f;
            shoot.HandRotationRecoilEffect.Intensity = 0f;
        }

        public static void WEAPON_NoSway(Player pTarget)
        {// 
            if (pTarget == null)
                return;

            var weapon = pTarget.ProceduralWeaponAnimation;
            if (weapon == null) return;

            var motion = weapon.MotionReact;
            if (motion == null) return;

            motion.Intensity = 0f;
            motion.SwayFactors = Vector3.zero;
            motion.Velocity = Vector3.zero;

            weapon.Breath.Intensity = 0;
            weapon.Walk.Intensity = 0;
            weapon.Shootingg.AimingConfiguration_0.AimProceduralIntensity = 0;
            weapon.ForceReact.Intensity = 0;
            weapon.WalkEffectorEnabled = false;
        }

        public static void WEAPON_NoMalfunction()
        {
            Player pLocalPlayer = GameTools.GetLocalPlayer();
            if (!pLocalPlayer)
                return;

            //  Get player hands controller
            var pHands = pLocalPlayer.HandsController;
            if (!pHands)
                return;

            //  get item instance
            var pHandsItem = pHands.Item;
            if (pHandsItem == null || pHandsItem is not Weapon pWeapon || pHands is not Player.FirearmController pWeaponController)
                return;

            //  get weapon item template
            var temp = pWeaponController.Item.Template;
            if (temp == null)
                return;

            //  disable malfunction states
            temp.AllowFeed = false;
            temp.AllowJam = false;
            temp.AllowMisfire = false;
            temp.AllowOverheat = false;
            temp.AllowSlide = false;
        }

        //  public static void GEAR_MaxDurability()
        //  {
        //      Player pLocalPlayer = GameTools.GetLocalPlayer();
        //      if (!pLocalPlayer)
        //          return;
        //  
        //      var pInventory = pLocalPlayer.Inventory;
        //      if (pInventory == null) 
        //          return;
        //  
        //      var items = pInventory.GetPlayerItems().ToArray();
        //      foreach ( var item in items )
        //      {
        //  
        //          var repair = item.GetItemComponent<RepairableComponent>();
        //          if (repair == null) 
        //              continue;
        //  
        //          repair.MaxDurability = repair.TemplateDurability;
        //          repair.Durability = repair.MaxDurability;
        //      }    
        //  }
    }
}
