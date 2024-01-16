using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Scripts;
using FEngLib.Scripts.Tags;
using FEngLib.Tags;

namespace FEngLib.Chunks;

public class ScriptDataChunk : FrontendObjectChunk
{
    public ScriptDataChunk(IObject<BaseObjectData> frontendObject) : base(frontendObject)
    {
    }

    public override IObject<BaseObjectData> Read(Package package, ObjectReaderState readerState, BinaryReader reader)
    {
        var ctx = new ScriptProcessingContext(FrontendObject.CreateScript());
        var tagStream = new ScriptTagStream(reader,
            readerState.CurrentChunkBlock.Size, FrontendObject, ctx);

        while (tagStream.HasTag())
        {
            var tag = tagStream.NextTag();
            ProcessTag(ctx, tag);
        }

        Debug.Assert(ctx.Tracks.Count == 0 ||
                     (ctx.Tracks.Any(t => t.Offset is null) ^ ctx.Tracks.Any(t => t.Offset is not null)));

        if (ctx.Tracks.Any(t => t.Offset is null))
        {
            for (var index = 0; index < ctx.Tracks.Count; index++)
            {
                var tempTrack = ctx.Tracks[index];
                ProcessTrackWithoutOffset(ctx.Script, index, tempTrack.Track);
            }
        }
        else
        {
            foreach (var tempTrack in ctx.Tracks)
            {
                ProcessTrackWithOffset(ctx.Script, tempTrack.Offset!.Value, tempTrack.Track);
            }
        }

        return FrontendObject;
    }

    public override FrontendChunkType GetChunkType()
    {
        return FrontendChunkType.ScriptData;
    }

    private void ProcessTag(ScriptProcessingContext ctx, Tag tag)
    {
        switch (tag)
        {
            case ScriptHeaderTag scriptHeaderTag:
                ProcessScriptHeaderTag(ctx, scriptHeaderTag);
                break;
            case ScriptNameTag scriptNameTag:
                ctx.Script.Name = scriptNameTag.Name;
                ctx.Script.Id = scriptNameTag.NameHash;
                break;
            case ScriptChainTag scriptChainTag:
                ctx.Script.ChainedId = scriptChainTag.Id;
                break;
            case ScriptKeyTrackTag scriptKeyTrackTag:
                ProcessScriptKeyTrackTag(ctx, scriptKeyTrackTag);
                break;
            case ScriptTrackOffsetTag scriptTrackOffsetTag:
                ProcessScriptTrackOffsetTag(ctx, scriptTrackOffsetTag);
                break;
            case ScriptEventsTag scriptEventsTag:
                ctx.Script.Events.AddRange(scriptEventsTag.Events);
                break;
            case ScriptKeyNodeTag: // Side effects are OK for this one, it's just a delegate
                break;
            default:
                throw new NotImplementedException($"Unsupported tag type: {tag.GetType()}");
        }
    }

    private void ProcessScriptTrackOffsetTag(ScriptProcessingContext ctx, ScriptTrackOffsetTag scriptTrackOffsetTag)
    {
        ctx.CurrentTrack.Offset = scriptTrackOffsetTag.Offset;
    }

    private void ProcessScriptKeyTrackTag(ScriptProcessingContext ctx, ScriptKeyTrackTag scriptKeyTrackTag)
    {
        var paramType = (TrackParamType)scriptKeyTrackTag.ParamType;
        Track newTrack = paramType switch
        {
            TrackParamType.Vector2 => new Vector2Track(),
            TrackParamType.Vector3 => new Vector3Track(),
            TrackParamType.Quaternion => new QuaternionTrack(),
            TrackParamType.Color => new ColorTrack(),
            _ => throw new ArgumentOutOfRangeException($"Unsupported parameter type: {paramType}")
        };

        newTrack.Length = scriptKeyTrackTag.Length;
        newTrack.InterpType = (TrackInterpolationMethod)scriptKeyTrackTag.InterpType;
        newTrack.InterpAction = scriptKeyTrackTag.InterpAction;

        ctx.Tracks.Add(ctx.CurrentTrack = new TempTrack(newTrack));
    }

    private void ProcessScriptHeaderTag(ScriptProcessingContext ctx,
        ScriptHeaderTag scriptHeaderTag)
    {
        ctx.Script.Id = scriptHeaderTag.Id;
        ctx.Script.Flags = scriptHeaderTag.Flags;
        ctx.Script.Length = scriptHeaderTag.Length;
    }

    private static void ProcessTrackWithoutOffset(Script script, int index, Track track)
    {
        if (index <= 4)
        {
            switch (index)
            {
                case 0:
                    script.SetTrack(BaseScriptTrackIds.Color, (ColorTrack)track);
                    break;
                case 1:
                    script.SetTrack(BaseScriptTrackIds.Pivot, (Vector3Track)track);
                    break;
                case 2:
                    script.SetTrack(BaseScriptTrackIds.Position, (Vector3Track)track);
                    break;
                case 3:
                    script.SetTrack(BaseScriptTrackIds.Rotation, (QuaternionTrack)track);
                    break;
                case 4:
                    script.SetTrack(BaseScriptTrackIds.Size, (Vector3Track)track);
                    break;
            }
        }
        else if (script is ImageScript imageScript)
        {
            switch (index)
            {
                case 5:
                    imageScript.SetTrack(ImageScriptTrackIds.UpperLeft, (Vector2Track)track);
                    break;
                case 6:
                    imageScript.SetTrack(ImageScriptTrackIds.LowerRight, (Vector2Track)track);
                    break;
                default:
                    if (script is ColoredImageScript coloredImageScript)
                    {
                        switch (index)
                        {
                            case 7:
                                coloredImageScript.SetTrack(ColoredImageScriptTrackIds.TopLeft, (ColorTrack)track);
                                break;
                            case 8:
                                coloredImageScript.SetTrack(ColoredImageScriptTrackIds.TopRight, (ColorTrack)track);
                                break;
                            case 9:
                                coloredImageScript.SetTrack(ColoredImageScriptTrackIds.BottomRight, (ColorTrack)track);
                                break;
                            case 10:
                                coloredImageScript.SetTrack(ColoredImageScriptTrackIds.BottomLeft, (ColorTrack)track);
                                break;
                            default:
                                throw new Exception($"Can't handle track index {index} for script type {script.GetType()}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Can't handle track index {index} for script type {script.GetType()}");
                    }
                    break;
            }
        }
        else
        {
            throw new Exception($"Can't handle track index {index} for script type {script.GetType()}");
        }
    }

    private static void ProcessTrackWithOffset(Script script, uint offset, Track track)
    {
        if (offset <= 14)
        {
            switch (offset)
            {
                case 0:
                    script.SetTrack(BaseScriptTrackIds.Color, (ColorTrack)track);
                    break;
                case 4:
                    script.SetTrack(BaseScriptTrackIds.Pivot, (Vector3Track)track);
                    break;
                case 7:
                    script.SetTrack(BaseScriptTrackIds.Position, (Vector3Track)track);
                    break;
                case 10:
                    script.SetTrack(BaseScriptTrackIds.Rotation, (QuaternionTrack)track);
                    break;
                case 14:
                    script.SetTrack(BaseScriptTrackIds.Size, (Vector3Track)track);
                    break;
                default:
                    throw new IndexOutOfRangeException($"Unsupported general track offset: {offset}");
            }
        }
        else if (script is ImageScript imageScript)
        {
            switch (offset)
            {
                case 17:
                    imageScript.SetTrack(ImageScriptTrackIds.UpperLeft, (Vector2Track)track);
                    break;
                case 19:
                    imageScript.SetTrack(ImageScriptTrackIds.LowerRight, (Vector2Track)track);
                    break;
                default:
                    switch (script)
                    {
                        case MultiImageScript multiImageScript:
                            switch (offset)
                            {
                                case 21:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.TopLeft1, (Vector2Track)track);
                                    break;
                                case 23:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.TopLeft2, (Vector2Track)track);
                                    break;
                                case 25:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.TopLeft3, (Vector2Track)track);
                                    break;
                                case 27:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.BottomRight1, (Vector2Track)track);
                                    break;
                                case 29:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.BottomRight2, (Vector2Track)track);
                                    break;
                                case 31:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.BottomRight3, (Vector2Track)track);
                                    break;
                                case 33:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.PivotRotation, (Vector3Track)track);
                                    break;
                                default:
                                    throw new IndexOutOfRangeException(
                                        $"Unsupported MultiImage track offset: {offset}");
                            }

                            break;
                        case ColoredImageScript coloredImageScript:
                            switch (offset)
                            {
                                case 21:
                                    coloredImageScript.SetTrack(ColoredImageScriptTrackIds.TopLeft, (ColorTrack)track);
                                    break;
                                case 25:
                                    coloredImageScript.SetTrack(ColoredImageScriptTrackIds.TopRight, (ColorTrack)track);
                                    break;
                                case 29:
                                    coloredImageScript.SetTrack(ColoredImageScriptTrackIds.BottomRight, (ColorTrack)track);
                                    break;
                                case 33:
                                    coloredImageScript.SetTrack(ColoredImageScriptTrackIds.BottomLeft, (ColorTrack)track);
                                    break;
                                default:
                                    throw new IndexOutOfRangeException(
                                        $"Unsupported ColoredImage track offset: {offset}");
                            }

                            break;
                        default:
                            throw new IndexOutOfRangeException($"Unsupported Image track offset: {offset}");
                    }

                    break;
            }
        }
        else
        {
            throw new NotImplementedException(
                $"Track offset > 14 with an unexpected script type ({script.GetType()}) ...");
        }
    }
}