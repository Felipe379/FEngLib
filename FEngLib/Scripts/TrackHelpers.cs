using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FEngLib.Structures;

namespace FEngLib.Scripts;

public record TrackEntry(TrackId Id, Track Track);

/// <summary>
/// Provides useful helper functions for working with script tracks.
/// </summary>
public static class TrackHelpers
{
    /// <summary>
    /// Returns all tracks associated with a script.
    /// </summary>
    /// <param name="script">The script to retrieve tracks from.</param>
    /// <returns>The script's tracks.</returns>
    public static IEnumerable<TrackEntry> GetAllTracks(Script script)
    {
        return script.Tracks.OrderBy(e => e.Key).Select(e => new TrackEntry(e.Key, e.Value));
    }

    /// <summary>
    /// Calculates the value of a track at a given point in time.
    /// </summary>
    /// <typeparam name="T">The track data type.</typeparam>
    /// <param name="track">The track to evaluate.</param>
    /// <param name="time">The point in time to evaluate the track at.</param>
    /// <returns>The calculated track value.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T Interpolate<T>(Track<T> track, int time) where T : struct
    {
        var nextDeltaKeyNode = track.GetDeltaKeyAt(time);

        if (nextDeltaKeyNode == null || track.DeltaKeys.Count == 0)
        {
            return track.BaseKey;
        }

        var nextDeltaKey = nextDeltaKeyNode.Value;
        LinkedListNode<TrackNode<T>> prevDeltaKeyNode;
        float lerpFactor;

        if (track.InterpAction == 0)
        {
            prevDeltaKeyNode = nextDeltaKeyNode.Previous;

            if (prevDeltaKeyNode != null && time < nextDeltaKey.Time)
            {
                var prevDeltaKey = prevDeltaKeyNode.Value;
                lerpFactor = (time - prevDeltaKey.Time) / (float)(nextDeltaKey.Time - prevDeltaKey.Time);
            }
            else
            {
                lerpFactor = 1;
            }
        }
        else if (track.InterpAction == 1)
        {
            if (time == nextDeltaKey.Time)
            {
                prevDeltaKeyNode = nextDeltaKeyNode;
                lerpFactor = 0;
            }
            else if (time < nextDeltaKey.Time)
            {
                prevDeltaKeyNode = nextDeltaKeyNode.Previous;
                if (prevDeltaKeyNode != null)
                {
                    float divisor = nextDeltaKey.Time - prevDeltaKeyNode.Value.Time;
                    lerpFactor = (time - prevDeltaKeyNode.Value.Time) / divisor;
                }
                else
                {
                    prevDeltaKeyNode = track.GetDeltaKeyAt((int)track.Length);
                    int distanceToTrackEnd = (int)track.Length - prevDeltaKeyNode.Value.Time;
                    float divisor = distanceToTrackEnd + nextDeltaKey.Time;
                    lerpFactor = (time + distanceToTrackEnd) / divisor;
                }
            }
            else
            {
                prevDeltaKeyNode = nextDeltaKeyNode;
                float divisor = nextDeltaKey.Time + track.Length - prevDeltaKeyNode.Value.Time;
                lerpFactor = (time - prevDeltaKeyNode.Value.Time) / divisor;
            }
        }
        else
        {
            throw new InvalidOperationException($"Unsupported InterpAction: {track.InterpAction}");
        }

        if (lerpFactor == 0)
        {
            return (T)AddKeys(track.BaseKey, prevDeltaKeyNode!.Value.Val);
        }

        if (Math.Abs(lerpFactor - 1) < 0.000000000001f)
        {
            return (T)AddKeys(track.BaseKey, nextDeltaKey.Val);
        }

        return (T)LerpKeys(lerpFactor, prevDeltaKeyNode!.Value.Val, nextDeltaKey.Val, track.BaseKey);
    }

    /// <summary>
    /// Computes the difference between two key values.
    /// </summary>
    /// <typeparam name="T">The key value type.</typeparam>
    /// <param name="key1">The minuend, which <paramref name="key2"/> will be subtracted from.</param>
    /// <param name="key2">The subtrahend, which will be subtracted from <paramref name="key1"/></param>
    /// <returns>The difference between the two keys.</returns>
    /// <exception cref="Exception">when an unsupported key type is provided.</exception>
    public static object SubtractKeys<T>(T key1, T key2)
    {
        return (key1, key2) switch
        {
            (int i1, int i2) => i1 - i2,
            (Color4 c1, Color4 c2) => new Color4(c1.Blue - c2.Blue, c1.Green - c2.Green, c1.Red - c2.Red, c1.Alpha - c2.Alpha),
            (Vector2 v1, Vector2 v2) => v1 - v2,
            (Vector3 v1, Vector3 v2) => v1 - v2,
            (Quaternion q1, Quaternion q2) => q1 / q2,
            _ => throw new Exception($"Cannot calculate delta of values of type {typeof(T)}")
        };
    }

    /// <summary>
    /// Computes the sum of two key values.
    /// </summary>
    /// <typeparam name="T">The key value type.</typeparam>
    /// <param name="key1">The first key.</param>
    /// <param name="key2">The second key.</param>
    /// <returns>The sum of the two keys.</returns>
    /// <exception cref="Exception"></exception>
    public static object AddKeys<T>(T key1, T key2)
    {
        return (node1: key1, node2: key2) switch
        {
            (int i1, int i2) => i1 + i2,
            (Color4 c1, Color4 c2) => new Color4
            {
                Alpha = c1.Alpha + c2.Alpha,
                Blue = c1.Blue + c2.Blue,
                Green = c1.Green + c2.Green,
                Red = c1.Red + c2.Red
            },
            (Vector2 v1, Vector2 v2) => v1 + v2,
            (Vector3 v1, Vector3 v2) => v1 + v2,
            (Quaternion q1, Quaternion q2) => q1 * q2,
            _ => throw new Exception($"Cannot add values of type {typeof(T)}")
        };
    }

    private static object LerpKeys<T>(float t, T node1, T node2, T offset)
    {
        return (node1, node2, offset) switch
        {
            (int i1, int i2, int o) => LerpInteger(i1, i2, t, o),
            (Color4 c1, Color4 c2, Color4 o) => LerpColor(c1, c2, t, o),
            (Vector2 v1, Vector2 v2, Vector2 o) => Vector2.Lerp(v1, v2, t) + o,
            (Vector3 v1, Vector3 v2, Vector3 o) => Vector3.Lerp(v1, v2, t) + o,
            (Quaternion v1, Quaternion v2, Quaternion o) => Quaternion.Lerp(v1, v2, t) * o,
            _ => throw new Exception($"Cannot lerp values of type {typeof(T)}")
        };
    }

    private static Color4 LerpColor(Color4 c1, Color4 c2, float t, Color4 offset)
    {
        return new Color4
        {
            Blue = offset.Blue + c1.Blue + (int)((c2.Blue - c1.Blue) * t + 0.5f),
            Green = offset.Green + c1.Green + (int)((c2.Green - c1.Green) * t + 0.5f),
            Red = offset.Red + c1.Red + (int)((c2.Red - c1.Red) * t + 0.5f),
            Alpha = offset.Alpha + c1.Alpha + (int)((c2.Alpha - c1.Alpha) * t + 0.5f),
        };
    }

    private static int LerpInteger(int n1, int n2, float t, int offset)
    {
        return (int)(n2 + offset + ((n1 - n2) * t + 0.5f));
    }
}