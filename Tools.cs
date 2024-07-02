using Comfort.Common;
using EFT;
using EFT.Animations;
using EFT.CameraControl;
using EFT.InventoryLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TarkovModMenu
{
    public enum EHostileType
    {
        EType_Scav,
        EType_ScavRaider,
        EType_ScavAssault,
        EType_Boss,
        EType_Cultist,
        EType_Bear,
        EType_Usec,
        EType_Marksman,
        EType_RogueUsec
    }

    public class Style
    {
        public static void DrawStyleUpdate()
        {//  @TODO: only run update for each item that has a change color event . . .

            Plugin.scavStyle.normal.textColor       = Plugin.colorScav.Value;
            Plugin.pmcStyle.normal.textColor        = Plugin.colorPMC.Value;
            Plugin.bodyStyle.normal.textColor       = Plugin.colorDeadBodies.Value;
            //  Plugin.lootStyle.normal.textColor       = Plugin.colorLootBoxes.Value;
            Plugin.looseLootStyle.normal.textColor  = Plugin.colorLooseLoot.Value;

            //  Plugin.scavStyle.normal.background      = Style.MakeTexture(2, 2, Plugin.colorScav.Value);
            //  Plugin.pmcStyle.normal.background       = Style.MakeTexture(2, 2, Plugin.colorPMC.Value);
            //  Plugin.bodyStyle.normal.background      = Style.MakeTexture(2, 2, Plugin.colorDeadBodies.Value);
            //  Plugin.lootStyle.normal.background      = Style.MakeTexture(2, 2, Plugin.colorLootBoxes.Value);
            //  Plugin.looseLootStyle.normal.background = Style.MakeTexture(2, 2, Plugin.colorLooseLoot.Value);
        }

        public static Texture2D MakeTexture(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }

    public class Draw : MonoBehaviour
    {
        public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

        public static Color Color
        {
            get { return GUI.color; }
            set { GUI.color = value; }
        }

        public static Vector2 CalcTextSize(string text)
        {
            return StringStyle.CalcSize(new GUIContent(text));
        }

        public static void DrawBox(Vector2 position, Vector2 size, Color color, bool centered = true)
        {
            Color = color;
            DrawBox(position, size, centered);
        }

        public static void DrawBox(Vector2 position, Vector2 size, bool centered = true)
        {
            var upperLeft = centered ? position - size / 2f : position;
            GUI.DrawTexture(new Rect(position, size), Texture2D.whiteTexture, ScaleMode.StretchToFill);
        }

        public static void DrawTextBox(Vector2 position, string label, Color color, bool centered = true)
        {
            Color = color;
            DrawTextBox(position, label, centered);
        }

        public static void DrawTextBox(Vector2 position, string label, bool centered = true)
        {
            Vector2 szText = CalcTextSize(label);                                           //  Get text size
            Vector2 szPadding = new Vector2(szText.x + (szText.x * .33f), szText.y);        //  Get width with padding so all text is shown
            Vector2 TopLeft = centered ? position - szPadding : position;                   //  Center Text Frame
            Rect pos = new Rect(TopLeft, szPadding);                                        //  Get True Rect
            GUIContent content = new GUIContent(label);                                     //  Set GUIContent
            GUI.Box(pos, content);
        }

        public static void DrawString(Vector2 position, string label, Color color, bool centered = true)
        {
            Color = color;
            DrawString(position, label, centered);
        }

        public static void DrawString(Vector2 position, string label, bool centered = true)
        {
            var content = new GUIContent(label);
            var size = StringStyle.CalcSize(content);
            var upperLeft = centered ? position - size / 2f : position;
            GUI.Label(new Rect(upperLeft, size), content);
        }

        public static Texture2D lineTex;
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            Matrix4x4 matrix = GUI.matrix;
            if (!lineTex)
                lineTex = new Texture2D(1, 1);

            Color color2 = GUI.color;
            GUI.color = color;
            float num = Vector3.Angle(pointB - pointA, Vector2.right);

            if (pointA.y > pointB.y)
                num = -num;

            GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
            GUIUtility.RotateAroundPivot(num, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), lineTex);
            GUI.matrix = matrix;
            GUI.color = color2;
        }

        public static void DrawBox(float x, float y, float w, float h, Color color, float thickness)
        {
            DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
            DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
            DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
            DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
        }

        public static void DrawBoxOutline(Vector2 Point, float width, float height, Color color, float thickness)
        {
            DrawLine(Point, new Vector2(Point.x + width, Point.y), color, thickness);
            DrawLine(Point, new Vector2(Point.x, Point.y + height), color, thickness);
            DrawLine(new Vector2(Point.x + width, Point.y + height), new Vector2(Point.x + width, Point.y), color, thickness);
            DrawLine(new Vector2(Point.x + width, Point.y + height), new Vector2(Point.x, Point.y + height), color, thickness);
        }

        public static void DrawCircle(Vector2 center, float radius, Color color, int num_segments = 0, float thickness = 1)
        {
            int totalSegments = num_segments * 4;
            float step = 1f / totalSegments;
            var lastV = center + new Vector2(radius, 0);

            for (int i = 1; i <= totalSegments; ++i)
            {
                float t = i * step;
                var currentV = center + new Vector2(
                    radius * Mathf.Cos(2 * Mathf.PI * t),
                    radius * Mathf.Sin(2 * Mathf.PI * t)
                );
                DrawLine(lastV, currentV, color, thickness);
                lastV = currentV;
            }
        }


        //  public static void DrawCircle(Vector2 pos, float radius, Color color, int segments = 0, float thickness = 1f)
        //  { 
        //      LineRenderer lineRenderer = new LineRenderer();
        //      lineRenderer.positionCount = segments + 1; // Number of segments plus one to close the circle
        //      lineRenderer.useWorldSpace = false; // Use local space so the circle is centered around the GameObject
        //      lineRenderer.startColor = color;
        //      lineRenderer.endColor = color;
        //      lineRenderer.startWidth = 0.1f;
        //      lineRenderer.endWidth = 0.1f;
        //      
        //      RectTransform rectTransform = new RectTransform();
        //      rectTransform.anchoredPosition = pos;
        //  
        //      float angle = 0f;
        //      for (int i = 0; i <= segments; i++)
        //      {
        //          float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
        //          float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
        //          lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        //          angle += 360f / segments;
        //      }
        //  }
    }

    public class CameraTools
    {
        public static float Distance(Camera cam, Vector2 WorldPosition)
        {   //  returns the distance from the camera to a point in 3d space
            return Vector2.Distance(cam.transform.position, WorldPosition);
        }
        public static float Distance(Camera cam, Vector3 WorldPosition)
        {   //  returns the distance from the camera to a point in 3d space
            return Vector3.Distance(cam.transform.position, WorldPosition);
        }

        public static bool ProjectWorldLocationToScreen(Player player, Vector3 worldPos, out Vector2 screen)
        {
            if (!GameTools.IsAiming(player))
                return WorldToScreen(worldPos, out screen);
            else 
                return ScopeToScreen(player, worldPos, out screen);
        }

        public static bool WorldToScreen(Vector3 worldPos, out Vector2 screen)
        {
            Camera cam = Camera.main;

            //  Transform world origin to screen point
            Vector3 transform = cam.WorldToScreenPoint(worldPos);

            //  Get Screen Points
            Vector2 result;
            result.x = transform.x;
            result.y = Screen.height - transform.y;     //  Unity 0, 0 = bottom left

            //  Check player viewport relative and on screen
            if (transform.z <= 0.01f || result.x >= Screen.width || result.y >= Screen.height)
            { 
                screen = Vector2.zero;
                return false;
            }

            //  return result
            screen = result;
            return true;
        }

        public static bool ScopeToScreen(Player player, Vector3 worldPosition, out Vector2 screen)
        {
            screen = Vector2.zero;

            Camera cam = Camera.main;

            //  Player pLocalPlayer = GameTools.GetLocalPlayer();
            //  if (pLocalPlayer == null)
            //      return false;

            //  if (!GameTools.IsAiming(player))
            //      return WorldToScreen(cam, worldPosition, out screen);

            //  Get Scope Params

            var pHands = player.HandsController;
            if (pHands == null)
                return false;

            ProceduralWeaponAnimation pWeaponAnim = player.ProceduralWeaponAnimation;
            if (pWeaponAnim == null)
                return false;

            SightComponent pAimingMod = pWeaponAnim.CurrentAimingMod;
            if (pAimingMod == null)
                return false;

            if (pAimingMod.ScopesCount <= 0)
                return false;

            float zoom = pAimingMod.GetCurrentOpticZoom();
            bool bAiming = pHands.IsAiming;

            if (bAiming && zoom <= 1)
                return false;

            float scopeRadius = 0f;
            Vector3 scopeCenter = Vector3.zero;
            var cOptic = pWeaponAnim.HandsContainer.Weapon.GetComponentInChildren<OpticSight>();
            if (bAiming && cOptic != null)
            {
                //  Get Scope Params
                var opticTransform = cOptic.LensRenderer.transform;
                var lensMesh = cOptic.LensRenderer.GetComponent<MeshFilter>().mesh;
                var lensUpperRight = opticTransform.TransformPoint(lensMesh.bounds.max);
                var lensUpperLeft = opticTransform.TransformPoint(new Vector3(lensMesh.bounds.min.x, 0, lensMesh.bounds.max.z));

                var lensUpperRight3D = cam.WorldToScreenPoint(lensUpperRight);
                lensUpperRight3D.y = Screen.height - lensUpperRight3D.y;

                var lensUpperLeft3D = cam.WorldToScreenPoint(lensUpperLeft);
                lensUpperLeft3D.y = Screen.height - lensUpperLeft3D.y;

                scopeRadius = Vector2.Distance(lensUpperRight3D, lensUpperLeft3D) / 2;
                scopeCenter = cam.WorldToScreenPoint(opticTransform.position);
                scopeCenter.y = Screen.height - scopeCenter.y;
            }

            //  Get Scope Camera
            var scopeCam = Camera.allCameras.FirstOrDefault(c => c.name == "BaseOpticCamera(Clone)");
            if (scopeCam == null)
                return false;

            //  Get Camera Offset
            float scale = 0f;
            Vector2 cameraOffset;
            scale = Screen.height / (float)cam.scaledPixelHeight;
            cameraOffset = new Vector2(
            cam.pixelWidth / 2 - scopeCam.pixelWidth / 2,
            cam.pixelHeight / 2 - scopeCam.pixelHeight / 2);

            //  Get Scope World To screen Point
            var scopePoint = (Vector2)scopeCam.WorldToScreenPoint(worldPosition) + cameraOffset;
            scopePoint.y = Screen.height - scopePoint.y * scale;
            scopePoint.x *= scale;

            //  if (clamp)
            //  {
            //      //  return ClampPointToScope(scopePoint);
            //      var distance = Vector2.Distance(_scopeParameters.center, scopePoint);
            //  
            //      var clampedPoint = scopePoint;
            //  
            //      if (distance > _scopeParameters.radius)
            //      {
            //          var clampedVector = (scopePoint - _scopeParameters.center).normalized * _scopeParameters.radius;
            //          clampedPoint = _scopeParameters.center + clampedVector;
            //      }
            //  }

            var distance = Vector2.Distance(scopeCenter, scopePoint);
            if (distance > scopeRadius)
                return false;

            screen = scopePoint;
            return true;
        }
    }

    public class GameTools
    {
        private static GameWorld m_pWorld;

        public static GameWorld GetWorld() 
        {
            m_pWorld = Singleton<GameWorld>.Instance;
            return m_pWorld;
        }

        public static bool InGame()
        {
            m_pWorld = Singleton<GameWorld>.Instance;
            
            if (m_pWorld == null || m_pWorld.AllAlivePlayersList == null || m_pWorld.AllAlivePlayersList.Count <= 0)
                return false;

            return true;
        }

        public static bool InHideout()
        {
            m_pWorld = Singleton<GameWorld>.Instance;
            
            if (m_pWorld == null || m_pWorld.AllAlivePlayersList == null || m_pWorld.AllAlivePlayersList.Count <= 0)
                return false;

            return m_pWorld.AllAlivePlayersList[0] is HideoutPlayer;
        }

        public static Player GetLocalPlayer()
        {
            m_pWorld = Singleton<GameWorld>.Instance;

            if (m_pWorld == null || m_pWorld.AllAlivePlayersList == null || m_pWorld.AllAlivePlayersList.Count <= 0)
                return null;

            return m_pWorld.AllAlivePlayersList[0];
        }

        public static EHostileType GetHostileType(Player player)
        {
            var info = player.Profile?.Info;
            if (info == null)
                return EHostileType.EType_Scav;

            var settings = info.Settings;
            if (settings != null)
            {
                switch (settings.Role)
                {
                    case WildSpawnType.pmcBot: return EHostileType.EType_ScavRaider;
                    case WildSpawnType.sectantWarrior: return EHostileType.EType_Cultist;
                    case WildSpawnType.assault: return EHostileType.EType_Scav;
                    case WildSpawnType.assaultGroup: return EHostileType.EType_ScavAssault;
                    case WildSpawnType.marksman: return EHostileType.EType_Marksman;
                    case WildSpawnType.exUsec: return EHostileType.EType_RogueUsec;
                }

                if (settings.IsBoss())
                    return EHostileType.EType_Boss;
            }

            return info.Side switch
            {
                EPlayerSide.Bear => EHostileType.EType_Bear,
                EPlayerSide.Usec => EHostileType.EType_Usec,
                _ => EHostileType.EType_Scav
            };
        }

        public static bool IsScav(Player pTarget)
        {
            EHostileType eHostility = GetHostileType(pTarget);
            switch (eHostility)
            {
                case EHostileType.EType_Scav: return true;
                case EHostileType.EType_ScavRaider: return true;
                case EHostileType.EType_ScavAssault: return true;
                case EHostileType.EType_Boss: return true;
                case EHostileType.EType_Cultist: return true;
                case EHostileType.EType_Marksman: return true;
                default: return false;
            }
        }

        public static bool IsPMC(Player pTarget)
        {
            EHostileType eHostility = GetHostileType(pTarget);
            switch (eHostility)
            {
                case EHostileType.EType_Bear: return true;
                case EHostileType.EType_Usec: return true;
                case EHostileType.EType_RogueUsec: return true;
                default: return false;
            }
        }

        public static string GetPlayerName(Player pTarget)
        {// obtains the target profiles nickname

            if (pTarget == null)
                return string.Empty;

            // Get Player Profile
            var pProfile = pTarget.Profile;
            if (pProfile == null) 
                return pTarget.name;

            //  Return Profile Nickname
            return pProfile.Nickname;
        }

        public static string GetFactionName(Player pTarget)
        {
            string faction = "PMC";
            if (pTarget == null)
                return faction;

            EHostileType eType = GetHostileType(pTarget);
            switch (eType)
            {
                case EHostileType.EType_Bear: faction = "BEAR"; break;
                case EHostileType.EType_Usec: faction = "USEC"; break;
                case EHostileType.EType_RogueUsec: faction = "ROGUE"; break;
                case EHostileType.EType_Marksman: faction = "MARKSMAN"; break;
                default: faction = "PMC"; break;
            }
            return faction;
        }

        public static string GetWeaponName(Player pTarget)
        {// obtains the target held weapon name

            if (pTarget == null)
                return string.Empty;

            //  Get Hands Controller
            var hands = pTarget.HandsController;
            if (hands == null)
                return string.Empty;

            //  Is Hands Item a Weapon ?
            if (hands.Item is not Weapon)
                return string.Empty;

            //  Cast Item to Weapon
            Weapon pWeapon = hands.Item as Weapon;
            if (pWeapon == null) 
                return string.Empty;

            //  Get Weapon Name
            return pWeapon.ShortName.Localized();
        }

        public static string GetItemName(Item pTarget) { return string.Empty; }

        public static float GetHealth(Player pTarget, EBodyPart eBodyPart = EBodyPart.Common)
        {// returns the target body part health

            if (pTarget == null)
                return 0f;

            //  Get Player Health controller
            var pHealth = pTarget.HealthController;
            if (pHealth == null) 
                return 0f;

            return pHealth.GetBodyPartHealth(eBodyPart).Current;
        }

        public static float GetHealthPercent(Player pTarget, EBodyPart eBodyPart = EBodyPart.Common)
        {// returns the target body part health percentage

            if (pTarget == null)
                return 0f;

            //  Get Player Health controller
            var pHealth = pTarget.HealthController;
            if (pHealth == null)
                return 0f;

            //  Get current and maximum health values
            float curHealth = pHealth.GetBodyPartHealth(eBodyPart).Current;
            float maxHealth = pHealth.GetBodyPartHealth(eBodyPart).Maximum;

            return Mathf.Round(curHealth * 100 / maxHealth);
        }

        public static bool IsAiming(Player player)
        {
            var pHands = player.HandsController;
            if (pHands == null)
                return false;

            ProceduralWeaponAnimation pWeaponAnim = player.ProceduralWeaponAnimation;
            if (pWeaponAnim == null) 
                return false;

            SightComponent pAimingMod = pWeaponAnim.CurrentAimingMod;
            if (pAimingMod == null)
                return false;

            if (pAimingMod.ScopesCount <= 0)
                return false;

            float zoom = pAimingMod.GetCurrentOpticZoom();
            bool bAiming = pHands.IsAiming;

            if (bAiming && zoom <= 1)
                return false;

            return bAiming;
        }

        public static bool GetPlayerBounds(Player pTarget, out Bounds boxExtents)
        {
            //  Declare loos variables
            Vector2 screen_bonePos = Vector2.zero;
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;
            boxExtents = new Bounds();
            Player pLocalPlayer = GetLocalPlayer();

            if (!pTarget || !pLocalPlayer)
            {
                Debug.Log($"[GetPlayerBounds] failed to get Target:{pTarget} or LocalPlayer:{pLocalPlayer}");
                return false;
            }

            // Get Player Body
            PlayerBody pBody = pTarget.PlayerBody;
            if (!pBody)
            {
                Debug.Log($"[GetPlayerBounds] failed to get PlayerBody:{pBody}");
                return false;
            }

            //  Get body skeleton
            var pSkeleton = pBody.SkeletonRootJoint;
            if (!pSkeleton)
            {
                Debug.Log($"[GetPlayerBounds] failed to get Skeleton:{pSkeleton}");
                return false;
            }

            //   get skeleton bones
            var bones = pSkeleton.Bones;
            if (bones.Count <= 0)
            {
                Debug.Log("[GetPlayerBounds] bones count was 0");
                return false;
            }

            //  loop bones array

            foreach (var connection in Bones.sockets)
            {
                Vector3 pos = bones[connection].position;
                if (pos == Vector3.zero)
                {
                    Debug.Log($"[GetPlayerBounds] failed to get location for socket {connection}");
                    continue;
                }

                //  get bone screen position
                if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pos, out screen_bonePos))
                    return false;

                //  MIN
                if (screen_bonePos.x < min.x)
                    min.x = screen_bonePos.x;
                if (screen_bonePos.y < min.y)
                    min.y = screen_bonePos.y;

                //  MAX
                if (screen_bonePos.x > max.x)
                    max.x = screen_bonePos.x;
                if (screen_bonePos.y > max.y)
                    max.y = screen_bonePos.y;

            }
            //  foreach ( var bone in bones )
            //  {
            //      if (bone.Value == null)
            //          continue;
            //  
            //      //  Get bone position
            //      Vector3 pos = bone.Value.position;
            //  
            //      //  get bone screen position
            //      if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pos, out screen_bonePos))
            //          return false;
            //  
            //      //  MIN
            //      if (screen_bonePos.x < min.x)
            //          min.x = screen_bonePos.x;
            //      if (screen_bonePos.y < min.y)
            //          min.y = screen_bonePos.y;
            //  
            //      //  MAX
            //      if (screen_bonePos.x > max.x)
            //          max.x = screen_bonePos.x;
            //      if (screen_bonePos.y > max.y)
            //          max.y = screen_bonePos.y;
            //  }

            //  Get head position
            Vector3 headBone = bones[Bones.Head].position;
            headBone.x += 0.05f;
            headBone.y += 0.05f;

            //  get bone screen position
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, headBone, out screen_bonePos))
                return false;

            //  MIN
            if (screen_bonePos.x < min.x)
                min.x = screen_bonePos.x;
            if (screen_bonePos.y < min.y)
                min.y = screen_bonePos.y;

            //  MAX
            if (screen_bonePos.x > max.x)
                max.x = screen_bonePos.x;
            if (screen_bonePos.y > max.y)
                max.y = screen_bonePos.y;

            //  get center & size
            Vector3 center = (min + max) / 2;
            Vector3 size = max - min;
            
            //  Set bounds
            boxExtents = new Bounds(center, size);
            return true;
        }

        public static bool GetPlayerBounds3D(Player  pTarget, out Bounds boxExtents)
        {
            //  Declare loos variables
            Vector3 current_bone = Vector3.zero;
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            boxExtents = new Bounds();
            Player pLocalPlayer = GetLocalPlayer();

            if (!pTarget || !pLocalPlayer)
                return false;

            // Get Player Body
            PlayerBody pBody = pTarget.PlayerBody;
            if (!pBody)
                return false;

            //  Get body skeleton
            var pSkeleton = pBody.SkeletonRootJoint;
            if (!pSkeleton)
                return false;

            //   get skeleton bones
            var bones = pSkeleton.Bones;
            if (bones.Count <= 0)
                return false;

            //  loop bones array
            foreach (var connection in Bones.sockets)
            {
                current_bone = bones[connection].position;
                if (current_bone == Vector3.zero)
                {
                    Debug.Log($"[GetPlayerBounds] failed to get location for socket {connection}");
                    continue;
                }

                //  MIN
                if (current_bone.x < min.x) min.x = current_bone.x;
                if (current_bone.y < min.y) min.y = current_bone.y;
                if (current_bone.z < min.z) min.z = current_bone.z;

                //  MAX
                if (current_bone.x > max.x) max.x = current_bone.x;
                if (current_bone.y > max.y) max.y = current_bone.y;
                if (current_bone.z > max.z) max.z = current_bone.z;

            }

            //  Get head position
            current_bone = bones[Bones.Head].position;
            current_bone.x += 0.05f;
            current_bone.y += 0.05f;

            //  MIN
            if (current_bone.x < min.x) min.x = current_bone.x;
            if (current_bone.y < min.y) min.y = current_bone.y;
            if (current_bone.z < min.z) min.z = current_bone.z;

            //  MAX
            if (current_bone.x > max.x) max.x = current_bone.x;
            if (current_bone.y > max.y) max.y = current_bone.y;
            if (current_bone.z > max.z) max.z = current_bone.z;

            //  get center & size
            Vector3 center = (min + max) / 2;
            Vector3 size = max - min;

            //  Set bounds
            boxExtents = new Bounds(center, size);

            Debug.Log($"[GetPlayerBounds3D] Bounds Center: {center}");
            Debug.Log($"[GetPlayerBounds3D] Bounds Size: {size}");
            return true;
        }
    }

    public class DebugTools
    {
        public static void DumpPlayerBoneNames()
        {
            // Get Local Player
            Player pLocalPlayer = GameTools.GetLocalPlayer();
            if (!pLocalPlayer)
                return;

            // Get Player Body
            PlayerBody pBody = pLocalPlayer.PlayerBody;
            if (!pBody)
                return;

            //  Get body skeleton
            var pSkeleton = pBody.SkeletonRootJoint;
            if (!pSkeleton)
                return;

            //  Get bones
            var bones = pSkeleton.Bones;
            if (bones.Count <= 0)
                return;

            //  print bone names
            foreach (var bone in bones)
                Console.WriteLine($"{bone.Key}");
        }
    }

    public class Bones
    {
        //  CORE
        public const string Root                = "Root_Joint";
        public const string Pelvis              = Root          + "/Base HumanPelvis";
        public const string Spine1              = Pelvis        + "/Base HumanSpine1";
        public const string Spine2              = Spine1        + "/Base HumanSpine2";
        public const string Spine3              = Spine2        + "/Base HumanSpine3";
        public const string Neck                = Spine3        + "/Base HumanNeck";
        public const string Head                = Neck          + "/Base HumanHead";
        public const string Ribcage             = Spine3        + "/Base HumanRibcage";

        //  LEFT SIDE
        public const string LThigh1             = Pelvis        + "/Base HumanLThigh1";
        public const string LThigh2             = LThigh1       + "/Base HumanLThigh2";
        public const string LCalf               = LThigh2       + "/Base HumanLCalf";
        public const string LFoot               = LCalf         + "/Base HumanLFoot";
        public const string LToe                = LFoot         + "/Base HumanLToe";
        public const string LCollarbone         = Ribcage       + "/Base HumanLCollarbone";
        public const string LUpperArm           = LCollarbone   + "/Base HumanLUpperarm";   //  SHOULDER
        public const string LForearm1           = LUpperArm     + "/Base HumanLForearm1";
        public const string LForearm2           = LForearm1     + "/Base HumanLForearm2";   //  ELBOW
        public const string LForearm3           = LForearm2     + "/Base HumanLForearm3";
        public const string LPalm               = LForearm3     + "/Base HumanLPalm";
        public const string LDigit11            = LPalm         + "/Base HumanLDigit11";
        public const string LDigit12            = LDigit11      + "/Base HumanLDigit12";
        public const string LDigit13            = LDigit12      + "/Base HumanLDigit13";
        public const string LDigit21            = LPalm         + "/Base HumanLDigit21";
        public const string LDigit22            = LDigit21      + "/Base HumanLDigit22";
        public const string LDigit23            = LDigit22      + "/Base HumanLDigit23";
        public const string LDigit31            = LPalm         + "/Base HumanLDigit31";
        public const string LDigit32            = LDigit31      + "/Base HumanLDigit32";
        public const string LDigit33            = LDigit32      + "/Base HumanLDigit33";
        public const string LDigit41            = LPalm         + "/Base HumanLDigit41";
        public const string LDigit42            = LDigit41      + "/Base HumanLDigit42";
        public const string LDigit43            = LDigit42      + "/Base HumanLDigit43";
        public const string LDigit51            = LPalm         + "/Base HumanLDigit51";
        public const string LDigit52            = LDigit51      + "/Base HumanLDigit52";
        public const string LDigit53            = LDigit52      + "/Base HumanLDigit53";

        //  RIGHT SIDE
        public const string RThigh1             = Pelvis        + "/Base HumanRThigh1";
        public const string RThigh2             = RThigh1       + "/Base HumanRThigh2";
        public const string RCalf               = RThigh2       + "/Base HumanRCalf";
        public const string RFoot               = RCalf         + "/Base HumanRFoot";
        public const string RToe                = RFoot         + "/Base HumanRToe";
        public const string RCollarbone         = Ribcage       + "/Base HumanRCollarbone";
        public const string RUpperArm           = RCollarbone   + "/Base HumanRUpperarm";   //  SHOULDER
        public const string RForearm1           = RUpperArm     + "/Base HumanRForearm1";
        public const string RForearm2           = RForearm1     + "/Base HumanRForearm2";   //  ELBOW
        public const string RForearm3           = RForearm2     + "/Base HumanRForearm3";
        public const string RPalm               = RForearm3     + "/Base HumanRPalm";
        public const string RDigit11            = RPalm         + "/Base HumanRDigit11";
        public const string RDigit12            = RDigit11      + "/Base HumanRDigit12";
        public const string RDigit13            = RDigit12      + "/Base HumanRDigit13";
        public const string RDigit21            = RPalm         + "/Base HumanRDigit21";
        public const string RDigit22            = RDigit21      + "/Base HumanRDigit22";
        public const string RDigit23            = RDigit22      + "/Base HumanRDigit23";
        public const string RDigit31            = RPalm         + "/Base HumanRDigit31";
        public const string RDigit32            = RDigit31      + "/Base HumanRDigit32";
        public const string RDigit33            = RDigit32      + "/Base HumanRDigit33";
        public const string RDigit41            = RPalm         + "/Base HumanRDigit41";
        public const string RDigit42            = RDigit41      + "/Base HumanRDigit42";
        public const string RDigit43            = RDigit42      + "/Base HumanRDigit43";
        public const string RDigit51            = RPalm         + "/Base HumanRDigit51";
        public const string RDigit52            = RDigit51      + "/Base HumanRDigit52";
        public const string RDigit53            = RDigit52      + "/Base HumanRDigit53";

        public static readonly List<string[]> Connections =
        [
            [Spine1, LThigh1], [LThigh1, LThigh2], [LThigh2, LCalf], [LCalf, LFoot], [LFoot, LToe],
            [Spine1, RThigh1], [RThigh1, RThigh2], [RThigh2, RCalf], [RCalf, RFoot], [RFoot, RToe],
            [Spine1, Spine2], [Spine2, Spine3], [Spine3, Neck], [Neck, Head],
            [Neck, LUpperArm], [LUpperArm, LForearm1], [LForearm1, LForearm2], [LForearm2, LForearm3], [LForearm3, LPalm],
            [Neck, RUpperArm], [RUpperArm, RForearm1], [RForearm1, RForearm2], [RForearm2, RForearm3], [RForearm3, RPalm]
        ];

        public static readonly List<string[]> FingerConnections =
        [
            [LPalm, LDigit11], [LDigit11, LDigit12], [LDigit12, LDigit13], 
            [LPalm, LDigit11], [LDigit21, LDigit22], [LDigit22, LDigit23], 
            [LPalm, LDigit11], [LDigit31, LDigit32], [LDigit32, LDigit33], 
            [LPalm, LDigit11], [LDigit41, LDigit42], [LDigit42, LDigit43], 
            [LPalm, LDigit11], [LDigit51, LDigit52], [LDigit52, LDigit53], 
            [RPalm, RDigit11], [RDigit11, RDigit12], [RDigit12, RDigit13], 
            [RPalm, RDigit11], [RDigit21, RDigit22], [RDigit22, RDigit23],
            [RPalm, RDigit11], [RDigit31, RDigit32], [RDigit32, RDigit33],
            [RPalm, RDigit11], [RDigit41, RDigit42], [RDigit42, RDigit43], 
            [RPalm, RDigit11], [RDigit51, RDigit52], [RDigit52, RDigit53]
        ];

        //  Used For Obtaining Bounds
        public static readonly List<string> sockets =
        [
            Pelvis, 
            LThigh1, LThigh2, LCalf, LFoot, LToe,
            RThigh1, RThigh2, RCalf, RFoot, RToe,
            LCollarbone, LUpperArm, LForearm1, LForearm2, LForearm1, LPalm,
            RCollarbone, RUpperArm, RForearm1, RForearm2, RForearm1, RPalm,
            Spine1, Spine2, Spine3, Neck, Head
        ];
    }

    public class RenderTools
    {
        private static void RenderBone(Dictionary<string, Transform> bones, string pStart, string pEnd, Color color, float thickness = 2f)
        {
            RenderBone(bones[pStart].position, bones[pEnd].position, color, thickness);
        }

        private static void RenderBone(Vector3 pStart, Vector3 pEnd, Color color, float thickness = 2f)
        {
            Player pLocalPlayer = GameTools.GetLocalPlayer();
            if (!pLocalPlayer)
            {
                Debug.Log($"[RenderBone] failed to obtain local player {pLocalPlayer}");
                return;
            }

            //  Project to screen
            Vector2[] screen = new Vector2[2];
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pStart, out screen[0]))
            {
                Debug.Log($"[RenderBone][0] failed to project point to screen {pStart}");
                return;
            }

            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pEnd, out screen[1]))
            {
                Debug.Log($"[RenderBone][1] failed to project point to screen {pEnd}");
                return;
            }

            Draw.DrawLine(screen[0], screen[1], color, thickness);
        }

        public static void RenderBones(Player pTarget, Color color, float thickness = 2, float distance = 0)
        {
            Player pLocalPlayer = GameTools.GetLocalPlayer();
            if (!pTarget || !pLocalPlayer)
            {
                Debug.Log($"[RenderBones] failed to obtain target {pTarget} or local player {pLocalPlayer}");
                return;
            }

            var body = pTarget.PlayerBody;
            if (!body)
            {
                Debug.Log($"[RenderBones] failed to obtain target body {pTarget}");
                return;
            }

            var skeleton = pTarget.PlayerBody.SkeletonRootJoint;
            if (!skeleton)
            {
                Debug.Log($"[RenderBones] failed to obtain target skeleton {body}");
                return;
            }

            var bones = skeleton.Bones;
            if (bones.Count <= 0)
            {
                Debug.Log($"[RenderBones] failed to obtain target bones {skeleton}");
                return;
            }

            //  Render Bones
            foreach (var connection in Bones.Connections)
                RenderBone(bones, connection[0], connection[1], color, thickness);

            //  Render Fingers
            if (distance > 0f && distance < 75f)
                foreach (var finger in Bones.FingerConnections)
                    RenderBone(bones, finger[0], finger[1], color, thickness);

            /// Drawing Circles is bad for FPS
            //  Vector2 head;
            //  if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, bones[Bones.Head].position, out head))
            //  {
            //      Debug.Log($"[RenderBones] project point to screen {bones[Bones.Head].position}");
            //      return;
            //  }
            //  
            //  Vector2 neck;
            //  if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, bones[Bones.Neck].position, out neck))
            //  {
            //      Debug.Log($"[RenderBones] project point to screen {bones[Bones.Neck]}");
            //      return;
            //  }
            //  
            //  var radius = Vector2.Distance(head, neck);
            //  
            //  //  Render Head
            //  Draw.DrawCircle(head, radius, color, 8, thickness);
        }

        public static void RenderBox2D(Player pTarget, Color color, float thickness = 2f)
        {
            Player pLocalPlayer = GameTools.GetLocalPlayer();
            if (!pTarget || !pLocalPlayer)
            {
                Debug.Log($"[RenderBox2D] failed to obtain target {pTarget} or local player {pLocalPlayer}");
                return;
            }

            PlayerBones bones = pTarget.PlayerBones;
            if (!bones)
            {
                Debug.Log($"[RenderBox2D] failed to obtain target bones {pTarget}");
                return;
            }

            //  Positions
            Vector3 pos = pTarget.Transform.position;                  
            Vector3 headBone = bones.Head.position;                    
            Vector3 lShoulderBone = bones.LeftShoulder.position;

            //  Project to screen
            Vector2[] screen = new Vector2[3];
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, pTarget.Transform.position, out screen[0]))
            {
                Debug.Log($"[RenderBox2D] project point to screen {pos}");
                return;
            }

            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, bones.Head.position, out screen[1]))
            {
                Debug.Log($"[RenderBox2D] project point to screen {headBone}");
                return;
            }
            
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, lShoulderBone, out screen[2]))
            {
                Debug.Log($"[RenderBox2D] project point to screen {lShoulderBone}");
                return;
            }

            //  Get box dimensions
            var heightOffset = Mathf.Abs(screen[1].y - screen[2].y);
            var boxHeight = Mathf.Abs(screen[1].y - screen[0].y) + heightOffset * 3f;
            var boxWidth = boxHeight * 0.62f;
            var boxPositionX = screen[0].x - boxWidth / 2f;
            var boxPositionY = screen[1].y - heightOffset * 2f;

            Draw.DrawBox(boxPositionX, boxPositionY, boxWidth, boxHeight, color, thickness);
        }

        public static void RenderBox3D(Player pTarget, Color color, float thickness = 2f)
        {
            Bounds extents = new Bounds();
            Player pLocalPlayer = GameTools.GetLocalPlayer();
            if (!pTarget || !pLocalPlayer || !GameTools.GetPlayerBounds3D(pTarget, out extents))
            {
                Debug.Log($"[RenderBox3D] failed to obtain target bounds {pTarget}");
                return;
            }

            // Get the min and max points of the bounds
            Vector3 min = extents.min;
            Vector3 max = extents.max;
            Vector3 center = extents.center;

            // Calculate the eight vertices of the box
            Vector3[] v =
            [
                new Vector3(min.x, min.y, min.z), // Bottom-front-left
                new Vector3(max.x, min.y, min.z), // Bottom-front-right
                new Vector3(max.x, min.y, max.z), // Bottom-back-right
                new Vector3(min.x, min.y, max.z), // Bottom-back-left
                new Vector3(min.x, max.y, min.z), // Top-front-left
                new Vector3(max.x, max.y, min.z), // Top-front-right
                new Vector3(max.x, max.y, max.z), // Top-back-right
                new Vector3(min.x, max.y, max.z), // Top-back-left
            ];

            //  Rotate points
            Quaternion rotation = pTarget.Transform.rotation;
            for (int i = 0; i < v.Length; i++)
                v[i] = rotation * (v[i] - center) + center;

            //  project points
            Vector2[] screen = new Vector2[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, v[i], out screen[i]))
                {
                    Debug.Log($"[Render3DBox][{i}] failed to project point to screen {v[i]}");
                    return;
                }
            }
            
            //  Draw Lines
            for (int i = 0; i < 4; i++)
            {
                Draw.DrawLine(screen[i], screen[(i + 1) % 4], color, thickness);
                Draw.DrawLine(screen[i + 4], screen[((i + 1) % 4) +4], color, thickness);
                Draw.DrawLine(screen[i], screen[i + 4], color, thickness);
            }

            /* // OLD METHOD

            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, v[0], out screen0))
            {
                Debug.Log($"[Render3DBox][0] failed to project point to screen {v[0]}");
                return;
            }

            Vector2 screen1;
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, v[1], out screen1))
            {
                Debug.Log($"[Render3DBox][1] failed to project point to screen {v[1]}");
                return;
            }

            Vector2 screen2;
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, v[2], out screen2))
            {
                Debug.Log($"[Render3DBox][2] failed to project point to screen {v[2]}");
                return;
            }

            Vector2 screen3;
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, v[3], out screen3))
            {
                Debug.Log($"[Render3DBox][3] failed to project point to screen {v[3]}");
                return;
            }

            Vector2 screen4;
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, v[4], out screen4))
            {
                Debug.Log($"[Render3DBox][4] failed to project point to screen {v[4]}");
                return;
            }

            Vector2 screen5;
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, v[5], out screen5))
            {
                Debug.Log($"[Render3DBox][5] failed to project point to screen {v[5]}");
                return;
            }

            Vector2 screen6;
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, v[6], out screen6))
            {
                Debug.Log($"[Render3DBox][6] failed to project point to screen {v[6]}");
                return;
            }

            Vector2 screen7;
            if (!CameraTools.ProjectWorldLocationToScreen(pLocalPlayer, v[7], out screen7))
            {
                Debug.Log($"[Render3DBox][7] failed to project point to screen {v[7]}");
                return;
            }


            Draw.DrawLine(screen0, screen1, color, thickness);
            Draw.DrawLine(screen1, screen2, color, thickness);
            Draw.DrawLine(screen2, screen3, color, thickness);
            Draw.DrawLine(screen3, screen0, color, thickness);

            Draw.DrawLine(screen4, screen5, color, thickness);
            Draw.DrawLine(screen5, screen6, color, thickness);
            Draw.DrawLine(screen6, screen7, color, thickness);
            Draw.DrawLine(screen7, screen4, color, thickness);

            Draw.DrawLine(screen0, screen4, color, thickness);
            Draw.DrawLine(screen1, screen5, color, thickness);
            Draw.DrawLine(screen2, screen6, color, thickness);
            Draw.DrawLine(screen3, screen7, color, thickness);

            */
        }
    }
}
