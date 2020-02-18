﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AgentMovement : Walker
{
    [System.Serializable]
    public class Settings
    {
        public float speed = 5f;
        public float navTreshold = 1f;
		public float animatorRunSpeed = 1f;
		public float minimumCarrySpeed = 1f;  
    }

    public AIPath path;
	[Range(0f, 1f)] public float slowMultiplier = 1f;
	public AgentMovement.Settings settings;

	[HideInInspector] public bool going = false;
    [HideInInspector] public bool reached = false;
    [HideInInspector] public Animator animator;
	public System.Action onDestinationReached;
	public System.Action onPathCompleted;
	public System.Action onTargetOffPath;
	public bool followingPath;
    public Transform chaseTarget;
    int currentIndexOnPath;
	NavMeshAgent agent;

	Vector3 orientation;
	Vector3 rotationLast;
	Vector3 rotationDelta;

	public override void ApplyAdherence(float adherence)
	{
		SetSpeed(settings.speed * (1f - adherence * slowMultiplier));
	}

	public void ClearEvents()
    {
        onDestinationReached = null;
        onPathCompleted = null;
        onTargetOffPath = null;
    }

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        going = false;
        SetSpeed(settings.speed);
    }

    public void FollowPath(AIPath newPath)
    {
		if (newPath == null) return;

		path = newPath;
        followingPath = true;
        ClearEvents();
		StopChase();

		onPathCompleted += () => { if (!path.loop) followingPath = false;};

		currentIndexOnPath = Random.Range(0, path.points.Count);
		GoAtPoint(currentIndexOnPath);
    }
    public void StopFollowingPath()
    {
        followingPath = false;
	}

    public void ApplyWeightToSpeed(float weight, float strength)
    {
        float s = (settings.speed - settings.minimumCarrySpeed) * Mathf.Max(strength/weight) + settings.minimumCarrySpeed;
        SetSpeed(s);
    }

	public void ResetSpeed()
    {
        SetSpeed(settings.speed);
    }

    public void SetSpeed(float value)
    {
        if(value < 0) value = 0f;
        agent.speed = value;
    }
    
    public void Chase(Transform target)
    {
		ClearEvents();
		StopFollowingPath();
		chaseTarget = target;
    }

    public void Clear()
    {
        reached = false;
        going = false;
        chaseTarget = null;
    }

    public bool GoThere(Vector3 pos, bool clearEvents = false)
    {
        if(clearEvents) ClearEvents();

		ActivateAgent();
		NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(pos, path);
        if(path.status == NavMeshPathStatus.PathPartial
        || path.status == NavMeshPathStatus.PathInvalid) return false;
        else 
        {
			//StopFollowingPath();
			agent.SetDestination(pos);
            going = true;
            reached = false;
            return true;
        }
    }

	public void Orient(Vector3 _orientation)
	{
		orientation = new Vector3(_orientation.x, 0f, _orientation.z);
	}

	public void OrientToward(Vector3 _position)
	{
		Vector3 dir = -(transform.position - _position).normalized;
		Orient(dir);
	}

	public void ActivateAgent()
	{
		agent.enabled = true;
		orientation = transform.forward;
	}
	public void DesactivateAgent()
	{
		agent.enabled = false;
	}
	public void Tick()
    {
		rotationDelta = transform.rotation.eulerAngles - rotationLast;
		rotationLast = transform.rotation.eulerAngles;

		if (!agent.enabled)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(orientation), Time.deltaTime * 5f);
		}

		if (reached) reached = false;
		if (DestinationReached())
        {
            reached = true;
            going = false;
            if(onDestinationReached != null)
            {
                onDestinationReached.Invoke();
                onDestinationReached = null;
            }

            if(currentIndexOnPath == path.points.Count - 1)
            {
                if(onPathCompleted != null)
                {
                    onPathCompleted.Invoke();
                    onPathCompleted = null;
                }
            }

            if(followingPath) GoToNextPoint();
        }
        else
        {
            if(chaseTarget != null)
            {
                if(!GoThere(chaseTarget.transform.position))
                {
                    if(onTargetOffPath != null)
                    {
                        onTargetOffPath.Invoke();
                        onTargetOffPath = null;
                    }
                }
            }
        }

        if(animator) 
        {
            animator.SetFloat("Speed", agent.velocity.magnitude/settings.animatorRunSpeed);
			animator.SetFloat("RotationSpeed", rotationDelta.magnitude/10f);
        }
    }

    public bool GoAtPoint(int index)
    {
		if (path.points.Count == 0
        || index < 0
        || index >= path.points.Count) return false;

		if (GoThere(path.GetPosition(index)))
		{
			currentIndexOnPath = index;
			return true;
		}
		else
		{
			return false;
		}
    }

    public void StopChase()
    {
        chaseTarget = null;
    }

    public void Stop()
    {
        going = false;
        reached = false;
        chaseTarget = null;
        StopFollowingPath();
		DesactivateAgent();
    }

    public bool GoToNextPoint()
    {
        return GoAtPoint(GetNextPointIndex());
    }

    public int GetNextPointIndex()
    {
        int waypoint = currentIndexOnPath + 1;
        if(waypoint >= path.points.Count) waypoint = 0;
        return waypoint;
    }

    float DistanceToDestination()
    {
        return Vector3.Distance(transform.position, agent.destination);
    }

    bool DestinationReached()
    {
        return going && DistanceToDestination() < settings.navTreshold;
    }

    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color32(255, 215, 0, 255);
        if(EditorApplication.isPlaying && agent != null && agent.destination != null)
        {
            UnityEditor.Handles.DrawLine(transform.position, agent.destination);
        }
    }

#endif
}
