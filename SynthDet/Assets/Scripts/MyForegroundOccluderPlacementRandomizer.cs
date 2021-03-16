﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Randomizers.Utilities;
using UnityEngine.Perception.Randomization.Samplers;

/// <summary>
/// Creates a 2D layer of of evenly spaced GameObjects from a given list of prefabs
/// </summary>
[Serializable]
[AddRandomizerMenu("Perception/MyForeground Occluder Placement Randomizer")]
public class MyForegroundOccluderPlacementRandomizer : Randomizer
{
    /// <summary>
    /// The Z offset component applied to the generated layer of GameObjects
    /// </summary>
    public float depth;

    /// <summary>
    /// The minimum distance between all placed objects
    /// </summary>
    public FloatParameter occluderSeparationDistance;

    /// <summary>
    /// The size of the 2D area designated for object placement
    /// </summary>
    public Vector2 placementArea;

    /// <summary>
    /// The list of prefabs sample and randomly place
    /// </summary>
    public GameObjectParameter prefabs;

    GameObject m_Container;
    GameObjectOneWayCache m_GameObjectOneWayCache;
    
    protected override void OnAwake()
    {
        m_Container = new GameObject("Foreground Occluders");
        var transform = scenario.transform;
        m_Container.transform.parent = transform;
        m_GameObjectOneWayCache = new GameObjectOneWayCache(m_Container.transform, prefabs.categories.Select(element => element.Item1).ToArray());
    }
    
    
    /// <summary>
    /// Generates a foreground layer of objects at the start of each scenario iteration
    /// </summary>
    protected override void OnIterationStart()
    {
        var seed = SamplerState.NextRandomState();
        var placementSamples = PoissonDiskSampling.GenerateSamples(
            placementArea.x, placementArea.y, occluderSeparationDistance.Sample(), seed);
        var offset = new Vector3(placementArea.x, placementArea.y, 0f) * -0.5f;
        
        foreach (var sample in placementSamples)
        {
            var instance = m_GameObjectOneWayCache.GetOrInstantiate(prefabs.Sample());
            instance.transform.position = new Vector3(sample.x, sample.y, depth) + offset;
        }
        placementSamples.Dispose();
    }

    /// <summary>
    /// Deletes generated foreground objects after each scenario iteration is complete
    /// </summary>
    protected override void OnIterationEnd()
    {
        m_GameObjectOneWayCache.ResetAllObjects();
    }
}

