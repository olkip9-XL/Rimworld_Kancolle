using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace FPG_Kancolle
{
    public class HourlieDef : Def
    {
        [NoTranslate]
        public string hourlieClipPath = "";

        private List<SoundDef> hourlieClipsInt = new List<SoundDef>();
        public List<SoundDef> hourlieClips
        {
            get
            {
                if (hourlieClipsInt.NullOrEmpty())
                {
                    hourlieClipsInt = new List<SoundDef>();

                    for (int i = 0; i < 24; i++)
                    {
                        string path = hourlieClipPath + "/" + i.ToString("D2") + "00";

                        AudioGrain_Clip audioGrain = new AudioGrain_Clip();
                        audioGrain.clipPath = path;

                        IEnumerable<ResolvedGrain> grains = audioGrain.GetResolvedGrains();

                        foreach (var grain in grains)
                        {
                            SoundDef def = new SoundDef();
                            def.context = SoundContext.MapOnly;
                            def.maxSimultaneous = 1;
                            //def.defName = $"Pawn";

                            SubSoundDef subSoundDef = new SubSoundDef();
                            subSoundDef.parentDef = def;
                            subSoundDef.onCamera = true;

                            FieldInfo fieldInfo = typeof(SubSoundDef).GetField("resolvedGrains", BindingFlags.NonPublic | BindingFlags.Instance);
                            List<ResolvedGrain> resolvedGrains = new List<ResolvedGrain>();
                            resolvedGrains.Add(grain);
                            fieldInfo.SetValue(subSoundDef, resolvedGrains);

                            FieldInfo fieldInfo2 = typeof(SubSoundDef).GetField("distinctResolvedGrainsCount", BindingFlags.NonPublic | BindingFlags.Instance);
                            fieldInfo2.SetValue(subSoundDef, 1);

                            def.subSounds = new List<SubSoundDef>() { subSoundDef };

                            hourlieClipsInt.Add(def);
                        }
                    }
                }

                if(hourlieClipsInt.Count != 24)
                {
                    Log.Error($"HourlieDef {defName} hourlieClips count is not 24!");
                }

                return hourlieClipsInt;
            }
        }
    
        public void PlayHour(int hour)
        {
            if (hour < 0 || hour > 23) return;

            SoundDef sound = hourlieClips[hour];
            if (sound != null)
            {
                sound.PlayOneShotOnCamera();
            }
        }   

    }
}
