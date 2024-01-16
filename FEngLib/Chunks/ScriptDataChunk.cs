using System;
using System.IO;
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
        var offset = scriptTrackOffsetTag.Offset;
        var currentTrack = ctx.CurrentTrack;
        var script = ctx.Script;

        if (offset <= 14)
        {
            switch (offset)
            {
                case 0:
                    script.SetTrack(BaseScriptTrackIds.Color, (ColorTrack)currentTrack);
                    break;
                case 4:
                    script.SetTrack(BaseScriptTrackIds.Pivot, (Vector3Track)currentTrack);
                    break;
                case 7:
                    script.SetTrack(BaseScriptTrackIds.Position, (Vector3Track)currentTrack);
                    break;
                case 10:
                    script.SetTrack(BaseScriptTrackIds.Rotation, (QuaternionTrack)currentTrack);
                    break;
                case 14:
                    script.SetTrack(BaseScriptTrackIds.Size, (Vector3Track)currentTrack);
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
                    imageScript.SetTrack(ImageScriptTrackIds.UpperLeft, (Vector2Track)currentTrack);
                    break;
                case 19:
                    imageScript.SetTrack(ImageScriptTrackIds.LowerRight, (Vector2Track)currentTrack);
                    break;
                default:
                    switch (script)
                    {
                        case MultiImageScript multiImageScript:
                            switch (offset)
                            {
                                case 21:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.TopLeft1, (Vector2Track)currentTrack);
                                    break;
                                case 23:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.TopLeft2, (Vector2Track)currentTrack);
                                    break;
                                case 25:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.TopLeft3, (Vector2Track)currentTrack);
                                    break;
                                case 27:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.BottomRight1, (Vector2Track)currentTrack);
                                    break;
                                case 29:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.BottomRight2, (Vector2Track)currentTrack);
                                    break;
                                case 31:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.BottomRight3, (Vector2Track)currentTrack);
                                    break;
                                case 33:
                                    multiImageScript.SetTrack(MultiImageScriptTrackIds.PivotRotation, (Vector3Track)currentTrack);
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
                                    coloredImageScript.SetTrack(ColoredImageScriptTrackIds.TopLeft, (ColorTrack)currentTrack);
                                    break;
                                case 25:
                                    coloredImageScript.SetTrack(ColoredImageScriptTrackIds.TopRight, (ColorTrack)currentTrack);
                                    break;
                                case 29:
                                    coloredImageScript.SetTrack(ColoredImageScriptTrackIds.BottomRight, (ColorTrack)currentTrack);
                                    break;
                                case 33:
                                    coloredImageScript.SetTrack(ColoredImageScriptTrackIds.BottomLeft, (ColorTrack)currentTrack);
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

        ctx.CurrentTrack = newTrack;
    }

    private void ProcessScriptHeaderTag(ScriptProcessingContext ctx,
        ScriptHeaderTag scriptHeaderTag)
    {
        ctx.Script.Id = scriptHeaderTag.Id;
        ctx.Script.Flags = scriptHeaderTag.Flags;
        ctx.Script.Length = scriptHeaderTag.Length;
    }
}