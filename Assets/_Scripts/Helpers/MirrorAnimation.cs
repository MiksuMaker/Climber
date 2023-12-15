using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MirrorAnimation : MonoBehaviour
{
    //public Animator animator; // Reference to the Animator component

    //public Animation animation;
    public List<AnimationClip> animations = new List<AnimationClip>();

    public string fileAddress = "Assets/Animations/xTest/Hand/";

    [ContextMenu("Clear Animations")]
    public void ClearAnimations()
    {
        animations.Clear();
    }

    [ContextMenu("Mirror the animation clip")]
    void MirrorAnimationClip()
    {
        //AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0); // Get the current animation clip

        //AnimatorClipInfo[] clips = animation.

        //AnimationClip clip = animation.clip;

        if (animations.Count == 0) { return; }

        //foreach (var clipInfo in clips)
        foreach (var animation in animations)
        {
            AnimationClip originalClip = animation;

            // Duplicate the original animation clip
            AnimationClip mirroredClip = new AnimationClip();
            mirroredClip.name = originalClip.name + "_Mirrored";
            mirroredClip.frameRate = originalClip.frameRate;

            //mirroredClip.isLooping = originalClip.isLooping;
            //mirroredClip.wrapMode = originalClip.wrapMode;
            mirroredClip.wrapMode = WrapMode.Loop;

            // Mirror the animation events
            foreach (var eventInfo in AnimationUtility.GetAnimationEvents(originalClip))
            {
                eventInfo.time = originalClip.length - eventInfo.time;
                mirroredClip.AddEvent(eventInfo);
            }

            //==============================
            //
            //  Ok so how animation keys WORK:
            //
            //  1.  If you have 7 keyframes, and you
            //      change 3 values (position for example),
            //      you will have 21 keys (7 x 3 = 21)
            //
            //  2.  So the first 7 values will be X value,
            //      the second Y, etc.
            //
            //  3.  If rotation is added in, that would
            //      probably add 3-4 more keys
            //
            //  4.  Point is that all the values of animation
            //      will be arranged by value (x position
            //      for example), not keyframe (the order of
            //      values in other hand is arranged by keyframes)
            //
            //==============================

            int round = 0;
            //int[] bannedRounds = new int[] { };
            int[] allowedRounds = new int[] { 
                                                0,  // X pos
                                                //1,  // Y pos
                                                //2,  // Z pos
                                                //3,  //      X rot
                                                4,  //      Y rot
                                                5,  //      Z rot
            };

            // Mirror the animation curves
            foreach (var binding in AnimationUtility.GetCurveBindings(originalClip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(originalClip, binding);
                Keyframe[] keys = curve.keys;


                //if (round != 0) { continue; }
                //if (bannedRounds.Contains(round)) { continue; }

                for (int i = 0; i < keys.Length; i++)
                {

                    if (!allowedRounds.Contains(round))
                    {
                        //continue;
                        keys[i].value = keys[i].value;
                    }
                    else
                    {
                        keys[i].value = -keys[i].value;
                    }
                }

                AnimationUtility.SetEditorCurve(mirroredClip, binding, new AnimationCurve(keys));

                round++;
            }

            // Save the mirrored clip
            AssetDatabase.CreateAsset(mirroredClip, fileAddress + mirroredClip.name + ".anim");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}