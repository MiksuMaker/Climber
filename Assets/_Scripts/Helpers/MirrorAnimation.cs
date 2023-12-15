using System.Linq;
using UnityEditor;
using UnityEngine;

public class MirrorAnimation : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component

    public string fileAddress = "Assets/Animations/xTest/Hand/";

    //void Start()
    //{
    //    MirrorAnimationClip();
    //}

    [ContextMenu("Mirror the animation clip")]
    void MirrorAnimationClip()
    {
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0); // Get the current animation clip

        foreach (var clipInfo in clips)
        {
            AnimationClip originalClip = clipInfo.clip;

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
            int[] allowedRounds = new int[] { 0 };

            // Mirror the animation curves
            foreach (var binding in AnimationUtility.GetCurveBindings(originalClip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(originalClip, binding);
                Keyframe[] keys = curve.keys;


                //if (round != 0) { continue; }
                //if (bannedRounds.Contains(round)) { continue; }
                if (!allowedRounds.Contains(round)) { continue; }

                for (int i = 0; i < keys.Length; i++)
                {
                    #region Old Shitshow
                    //Debug.Log(keys.Length);

                    // Stop altering for other than X values
                    //if (i >= keys.Length / 3)
                    //{
                    //    Debug.Log("NUMBER (" + i + ") B_______: " + keys[i].value.ToString() +
                    //              " || A____: " + (keys[i].value).ToString());
                    //    continue;
                    //}
                    //else
                    //{
                    //    Debug.Log("NUMBER (" + i + ") BEFORE: " + keys[i].value.ToString() +
                    //              " || AFTER: " + (-keys[i].value).ToString());

                    //}

                    //keys[i].time = originalClip.length - keys[i].time;
                    //keys[i].inTangent = -keys[i].inTangent;
                    //keys[i].outTangent = -keys[i].outTangent;

                    //keys[i].value = -keys[i].value;
                    //keys[i].value

                    //curve.MoveKey((i, curve.keys[i]));

                    //keys[i].value = -keys[i].value;

                    //Debug.Log("Keyvalue: " + keys[i].value.ToString());
                    #endregion


                    keys[i].value = -keys[i].value;
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