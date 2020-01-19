using UnityEngine;

public class Consts : MonoBehaviour
{
    public static readonly Vector3 _player_start_position_ = new Vector3(75, _spawn_height_, 25f);
    public static readonly Vector3 _leviathan_start_position_ = new Vector3(75, 0, 100);

    public const float _teleport_spawn_time_ = 0.3f;
    public const float _spawn_height_ = 0.01f;
    public const float _dead_height_ = -1;
    public const float _min_terrain_ = 21.5f;
    public const float _max_terrain_ = 119.5f;

    public const string _tank_health_bar_path_ = "tank_health_bar";
    public const string _health_crystal_path_ = "health_crystal";
    public const string _health_bar_path_ = "health_bar";
    public const string _tank_path_ = "tank";
    public const string _big_tank_path_ = "big_tank";
    public const string _leviathan_path_ = "leviathan";
    public const string _view_gimbal_path_ = "view_gimbal";

    public const string _raptor_enemy_path_ = "raptor";

    public const string _cinemachine_initial_ = "cinemachine_initial";
    public const string _cinemachine_attack_ = "cinemachine_attack";
    public const string _cinemachine_death_ = "cinemachine_death";
    public const string _cinemachine_shot_ = "cinemachine_shot";

    public static Vector3 Horizontal(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    public static float GetAnimationLength(Animator animator, string clipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length
                    / animator.GetCurrentAnimatorStateInfo(0).speed;
            }
        }
        return -1;
    }
}