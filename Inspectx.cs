using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FrooxEngine.LogiX;

namespace Inspectx
{
    public class Inspectx : NeosMod
    {
        public override string Author => "Sharkmare";
        public override string Name => "Inspectx";
        //Built on Cyros Inspectrum
        public override string Version => "1.1.0";

        #nullable disable
        public static ModConfiguration config;
        #nullable enable

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<string> downloadurl = new ModConfigurationKey<string>("TitleURL", "Link to web hosted Text List", internalAccessOnly: true);
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.Shark.Inspectx");

            config = GetConfiguration();
            if (config.TryGetValue(downloadurl, out string dlValue))
            {
                Msg($"WebURL Loaded {dlValue}");
            }
            else
            {
                dlValue = "https://github.com/Sharkmare/SHCLD/raw/main/_inspector_title";
                Msg($"Initializing URL to {dlValue}");
            }

            config.Set(downloadurl, dlValue);

            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(VideoPlayer), "OnAttach")]
        class VideoxPatcher
        {
            static void Postfix(VideoPlayer __instance)
            {
                var Output = __instance.Slot.GetComponent<AudioOutput>();
                var LocalAudio = __instance.Slot.AttachComponent<ValueUserOverride<float>>();
                LocalAudio.CreateOverrideOnWrite.Value = true;
                LocalAudio.Target.Target = Output.Volume;
                LocalAudio.Default.Value = 0.1f;
                
                var Style = __instance.Slot.GetComponent<NeosUIStyle>();
                Style.Color.Value = color.Black; Style.UserParentedColor.Value = color.Black; Style.PrivateColor.Value = color.Black;
                var Panel = __instance.Slot.GetComponent<NeosPanel>();
                Panel.Color = color.Black;
                Panel.Color.SetA(1f);
                Panel.Thickness.Value = 0.005f; //Sleeker.
            }
        }

        public static void setupAudio(AudioClipPlayer Player,AudioOutput Output)
        {
            Output.Source.Target = Player;
            Output.RolloffMode.Value = AudioRolloffMode.Linear; //Logarythmic is heard too far, hardclamp pls
            Output.MinDistance.Value = 0f; Output.MaxDistance.Value = 5f;
            Output.Volume.Value = .5f; // half volume
            Output.DistanceSpace.Value = AudioDistanceSpace.Global; //if we are huge or tiny, always same distance
            Output.AudioTypeGroup.Value = AudioTypeGroup.SoundEffect; //Assume are a soundeffect
            Output.EnabledField.Value = true; Player.Play();
        }
        public static void FunnyBG(Slot Background)
        {
            StaticTexture2D img = Background.AttachTexture(new Uri("https://tgstation13.org/wiki/images/e/ee/NTlogo.png"), false, false, true);
            QuadMesh Quad = Background.AttachComponent<QuadMesh>();
            MeshRenderer render = Background.AttachComponent<MeshRenderer>();
            UnlitMaterial mat = Background.AttachComponent<UnlitMaterial>();
            render.Materials.Add(mat); render.Mesh.Target = Quad;Quad.Size.Value = new float2(.5f, .5f);
            Background.Position_Field.Value = new float3(0f, 0f, 0.01f);mat.Texture.Target = img; mat.BlendMode.Value = BlendMode.Cutout;
            Background.Rotation_Field.Value = floatQ.AxisAngle(new float3(0f, 180f, 0f), 180f);
        }
        [HarmonyPatch(typeof(InspectorPanel), "Setup")]
        class InspectxPatcher
        {

            static void Postfix(InspectorPanel __instance, NeosPanel __result)
            {
                AudioClipPlayer Player;
                AudioOutput Output;
                string downloadString;

                try
                {
                     downloadString = config.GetValue(downloadurl);
                    Msg("Fetched URL from config");
                }
                catch
                {
                    downloadString = "";
                    Msg("Failed to fetch config URL");
                }
                    WebClient client = new WebClient();
                try 
                {
                    string[] strarray = client.DownloadString(downloadString).Split(Environment.NewLine.ToCharArray());

                //Split string based on newlines
                Random random = new Random();
                int i = random.Next(0, strarray.Length - 1);
                //choose random option of array based on length with the last entry dropped as it will always be an empty newline due to windows filesystem adding one
                downloadString = strarray[i];
                }
                catch (Exception e)
                { downloadString = e.ToString().Split(Environment.NewLine.ToCharArray())[0]; }
                __instance.Slot.AttachComponent<DestroyOnUserLeave>().TargetUser.User.Value = __result.Slot.LocalUser.ReferenceID;
                // __instance.Slot.AttachComponent<DuplicateBlock>();
                //Blocks duplication by existing on object root.

                FrooxEngine.UIX.Image image1 = __instance.Slot.GetComponentInChildren<FrooxEngine.UIX.Image>();
                FrooxEngine.UIX.Image image2 = image1.Slot[0].GetComponentInChildren<FrooxEngine.UIX.Image>();
                FrooxEngine.UIX.Image image3 = image1.Slot[1].GetComponentInChildren<FrooxEngine.UIX.Image>();
                //The above will break if the inspector hierarchy is ever modified as it expects 2 children beneath the first images slot.
                //Slot[0] is equal to GetChild 0 in Logix
                image1.Tint.Value = color.Black;
                __result.RunInUpdates(6, () => { FunnyBG(__instance.Slot.AddSlot("BG Image")); });

                __result.RunInUpdates(2, () => { 
                switch (downloadString)
                {
                    case string a when a.Contains("Geenz"):
                            {
                                image2.Tint.Value = new color(.5f, .2f, 1f, .25f);//color = geen
                                image3.Tint.Value = color.Gray.SetA(.25f);
                            }
                            break;

                    case string a when a.Contains("Nex"):
                            {
                                image2.Tint.Value = new color(.2f, 1f, .2f, .25f);//color = gren
                                image3.Tint.Value = color.Gray.SetA(.25f);
                            }
                            break;

                    case string a when a.Contains("Froox"):
                            {
                                image2.Tint.Value = new color(1f, 1f, .2f, .25f);//color = dogman
                                image3.Tint.Value = color.Gray.SetA(.25f);
                            }
                            break;
                    
                    case string a when a.Contains("Veer"):
                            {
                                Player = __instance.Slot.AttachComponent<AudioClipPlayer>();
                                Output = __instance.Slot.AttachComponent<AudioOutput>();
                                image2.Tint.Value = new color(1f,1f,1f,.5f);//color = themanager
                                image3.Tint.Value = color.Gray.SetA(.25f);
                                downloadString = "<color=yellow>Veer: Manager of the Deep</color>\n<color=red>███████████████████████████████████████████████████████████████████████</color>";
                                Player.Clip.Target = __instance.Slot.AttachAudioClip(new Uri("https://cdn.discordapp.com/attachments/311691674130710528/961905664802820117/Deacons_of_the_Deep.ogg"));
                                __instance.Slot.Scale_Field.Value = __instance.Slot.Scale_Field.Value*3 ;
                                setupAudio(Player, Output);
                                Output.AudioTypeGroup.Value = AudioTypeGroup.Multimedia; //we arent a soundeffect
                            }
                            break;

                    case string a when a.Contains("PatCat"):
                            {
                                Player = __instance.Slot.AttachComponent<AudioClipPlayer>();
                                Output = __instance.Slot.AttachComponent<AudioOutput>();
                                image2.Tint.Value = new color(.4f, 1f, .4f, .3f);
                                image3.Tint.Value = color.White.SetA(.5f);
                                downloadString = "<color=green>Pat Cat: He Screm</color>";
                                Player.Clip.Target = __instance.Slot.AttachAudioClip(new Uri("https://cdn.discordapp.com/attachments/869012875140825148/961922988079472710/AAAAAaaaaaaaa.ogg"));
                                __instance.Slot.Scale_Field.Value = __instance.Slot.Scale_Field.Value / 2;
                                setupAudio(Player, Output);
                            }
                            break;

                        default:
                            { 
                        image2.Tint.Value = color.Gray.SetA(.5f);
                        image3.Tint.Value = color.Gray.SetA(.25f);
                            }
                            break;
                }
                });
                
                //.Localuser gives us the instantiating user "ourselves"    
                __result.RunInUpdates(3, () => {
                    __result.MarkChangeDirty();
                    __result.Slot.PersistentSelf = false;
                    __result.Title = downloadString; // Title gets set after setup finishes so this needs to run after it gets set
                });
            }
        }
        [HarmonyPatch(typeof(NeosPanel), "OnAttach")]
        class PanelxPatcher
        {
            static void Postfix(NeosPanel __instance)
            {
                //Videoplayers seem to set up in a way that breaks this
                __instance.RunInUpdates(3, () => { 
                NeosUIStyle Style;

                if (__instance.Style == null) { Style = __instance.Slot.AttachComponent<NeosUIStyle>(); __instance.Style = Style; }
                else { Style = __instance.Slot.GetComponent<NeosUIStyle>(); }
                Style.Color.Value = color.Black; Style.UserParentedColor.Value = color.Black; Style.PrivateColor.Value = color.Black;
                    __instance.Color = color.Black;
                    __instance.Color.SetA(1f);
                    __instance.Thickness.Value = 0.005f; //Sleeker.
                    __instance.ShowHandle.Value = false; //SLEEEKER!
                });
            }
        }
        [HarmonyPatch(typeof(ContextMenu), "OpenMenu")]
        public static class MenuxPatch
        {
            public static void Postfix(ContextMenu __instance)
            {
                
                ContextMenu menu = __instance;
                //delay item generation so that we arent the first menu item
                __instance.RunInUpdates(3, () =>
                {
                    var menuItem = menu.AddItem("Spawner", NeosAssets.Common.Icons.Reload, new color(1f, 1f, 0f), null);
                    menuItem.Button.LocalPressed += (IButton b, ButtonEventData d) => PressButton(b, d, menu, menuItem.Slot);
                });
            }
            private static void PressButton(IButton button, ButtonEventData eventData, ContextMenu menu,Slot sub)
            {
                sub.Destroy();
                var menuItem1 = menu.AddItem("Tool", NeosAssets.Common.Icons.Star, new color(0f, 0.5f, 1f));
                var VMK = menuItem1.Slot.AttachComponent<VirtualMultiKey>();
                VMK.Keys.Add(Key.LeftControl);
                VMK.Keys.Add(Key.V);
                menuItem1.Button.LocalHoverEnter += (IButton b, ButtonEventData d) => Copyx(0);
            }
            private static void Copyx(int choice)
            {
                string link;
                switch (choice) 
                {
                    case 0:
                        {
                            link = "neosdb:///df7e5e982dfe5eb32bf8bb34fd7ba7ae7da0ca3365a981f98a2050d4cc03dcbb.7zbson";
                        }
                        break;

                    case 1:
                        {
                            link = "neodblink";
                        }
                        break;

                    default:
                        {
                            return;
                        }
                }
                System.Windows.Forms.Clipboard.SetText(link);
            }
        }
    }
}