using System.Numerics;
using FEngLib.Objects;
using FEngLib.Scripts;
using FEngLib.Structures;

namespace FEngRender.Data;

public abstract class RenderTreeImage<TImage, TScriptTracks> : RenderTreeNode<TImage, ImageScript<TScriptTracks>, TScriptTracks>
    where TImage : IImage<ImageData>, IScriptedObject<ImageScript<TScriptTracks>>
    where TScriptTracks : ImageScriptTracks, new()
{
    public Vector2 UpperLeft { get; set; }
    public Vector2 LowerRight { get; set; }

    protected RenderTreeImage(TImage frontendObject) : base(frontendObject)
    {
    }

    protected override void ApplyScript(ImageScript<TScriptTracks> script, TScriptTracks tracks)
    {
        base.ApplyScript(script, tracks);

        if (tracks.UpperLeft is { } upperLeftTrack)
            UpperLeft = InterpolateHelper(upperLeftTrack);
        else
            UpperLeft = FrontendObject.Data.UpperLeft;
        if (tracks.LowerRight is { } lowerRightTrack)
            LowerRight = InterpolateHelper(lowerRightTrack);
        else
            LowerRight = FrontendObject.Data.LowerRight;
    }
}

public class RenderTreeImage : RenderTreeImage<Image, ImageScriptTracks>
{
    public RenderTreeImage(Image frontendObject) : base(frontendObject)
    {
    }
}

public class RenderTreeColoredImage : RenderTreeImage<ColoredImage, ColoredImageScriptTracks>
{
    public Color4 TopLeft { get; set; }
    public Color4 TopRight { get; set; }
    public Color4 BottomRight { get; set; }
    public Color4 BottomLeft { get; set; }

    public RenderTreeColoredImage(ColoredImage frontendObject) : base(frontendObject)
    {
    }

    protected override void ApplyScript(ImageScript<ColoredImageScriptTracks> script, ColoredImageScriptTracks tracks)
    {
        base.ApplyScript(script, tracks);

        if (tracks.TopLeft is { } topLeftTrack)
            TopLeft = InterpolateHelper(topLeftTrack);
        else
            TopLeft = FrontendObject.Data.TopLeft;
        if (tracks.TopRight is { } topRightTrack)
            TopRight = InterpolateHelper(topRightTrack);
        else
            TopRight = FrontendObject.Data.TopRight;
        if (tracks.BottomRight is { } bottomRightTrack)
            BottomRight = InterpolateHelper(bottomRightTrack);
        else
            BottomRight = FrontendObject.Data.BottomRight;
        if (tracks.BottomLeft is { } bottomLeftTrack)
            BottomLeft = InterpolateHelper(bottomLeftTrack);
        else
            BottomLeft = FrontendObject.Data.BottomLeft;
    }
}

public class RenderTreeMultiImage : RenderTreeImage<MultiImage, MultiImageScriptTracks>
{
    public Vector2 TopLeft1 { get; set; }
    public Vector2 TopLeft2 { get; set; }
    public Vector2 TopLeft3 { get; set; }
    public Vector2 BottomRight1 { get; set; }
    public Vector2 BottomRight2 { get; set; }
    public Vector2 BottomRight3 { get; set; }
    public Vector3 PivotRotation { get; set; }

    public RenderTreeMultiImage(MultiImage frontendObject) : base(frontendObject)
    {
    }

    protected override void ApplyScript(ImageScript<MultiImageScriptTracks> script, MultiImageScriptTracks tracks)
    {
        base.ApplyScript(script, tracks);

        if (tracks.TopLeft1 is { } topLeft1Track)
            TopLeft1 = InterpolateHelper(topLeft1Track);
        else
            TopLeft1 = FrontendObject.Data.TopLeft1;
        if (tracks.TopLeft2 is { } topLeft2Track)
            TopLeft2 = InterpolateHelper(topLeft2Track);
        else
            TopLeft2 = FrontendObject.Data.TopLeft2;
        if (tracks.TopLeft3 is { } topLeft3Track)
            TopLeft3 = InterpolateHelper(topLeft3Track);
        else
            TopLeft3 = FrontendObject.Data.TopLeft3;
        if (tracks.BottomRight1 is { } bottomRight1Track)
            BottomRight1 = InterpolateHelper(bottomRight1Track);
        else
            BottomRight1 = FrontendObject.Data.BottomRight1;
        if (tracks.BottomRight2 is { } bottomRight2Track)
            BottomRight2 = InterpolateHelper(bottomRight2Track);
        else
            BottomRight2 = FrontendObject.Data.BottomRight2;
        if (tracks.BottomRight3 is { } bottomRight3Track)
            BottomRight3 = InterpolateHelper(bottomRight3Track);
        else
            BottomRight3 = FrontendObject.Data.BottomRight3;
        if (tracks.PivotRotation is { } pivotRotationTrack)
            PivotRotation = InterpolateHelper(pivotRotationTrack);
        else
            PivotRotation = FrontendObject.Data.PivotRotation;
    }
}

public class RenderTreeSimpleImage : RenderTreeNode<SimpleImage>
{
    public RenderTreeSimpleImage(SimpleImage frontendObject) : base(frontendObject)
    {
    }
}