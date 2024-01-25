using System.Numerics;
using FEngLib.Messaging;
using FEngLib.Messaging.Commands;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Scripts;
using FEngLib.Structures;
using FEngRender.Data;

namespace FEngViewer.WPF.ViewModels;

public static class DummyData
{
    public static Package TestPackage { get; }
    public static RenderTree TestRenderTree { get; }

    static DummyData()
    {
        var group = new Group(new CommonObjectData())
        {
            Guid = 0x12344,
            Name = "TestGroup"
        };

        var testScript1 = new CommonScript
        {
            Name = "TestObject1_Script1",
            Events =
            {
                new Event
                {
                    EventId = 0x13377331,
                    Target = 0x12346,
                    Time = 100
                }
            },
            Length = 1000
        };

        testScript1.SetTrack(BaseScriptTrackIds.Position, new Vector3Track
        {
            Length = 101,
            BaseKey = Vector3.UnitX,
            InterpAction = 0x1,
            InterpType = TrackInterpolationMethod.Linear
        });

        var testImgScript = new ImageScript
        {
            Name = "CoolTestImage_Script1",
            Length = 500
        };

        testImgScript.SetTrack(ImageScriptTrackIds.LowerRight, new Vector2Track
        {
            BaseKey = Vector2.UnitX,
            DeltaKeys = new LinkedList<TrackNode<Vector2>>(new []
            {
                new TrackNode<Vector2> { Time = 10, Val = -Vector2.UnitX },
                new TrackNode<Vector2> { Time = 420, Val = new(-1, 1) }
            })
        });
        testImgScript.SetTrack(ImageScriptTrackIds.UpperLeft, new Vector2Track());

        var testMultiImgScript = new MultiImageScript
        {
            Name = "Test MultiImage Script #1",
            Length = 120
        };
        testMultiImgScript.SetTrack(MultiImageScriptTrackIds.BottomRight1, new Vector2Track());
        testMultiImgScript.SetTrack(MultiImageScriptTrackIds.PivotRotation, new Vector3Track
        {
            BaseKey = Vector3.One,
            DeltaKeys = new LinkedList<TrackNode<Vector3>>(new []
            {
                new TrackNode<Vector3> { Time = 50, }
            })
        });

        var testColoredImgScript = new ColoredImageScript
        {
            Name = "Test ColoredImage Script #1",
            Length = 1001
        };
        testColoredImgScript.SetTrack(ColoredImageScriptTrackIds.BottomLeft, new ColorTrack
        {
            BaseKey = new Color4(255, 0, 255, 255),
            DeltaKeys = new LinkedList<TrackNode<Color4>>(new []
            {
                new TrackNode<Color4> { Time = 10, Val = new Color4(-20, 0, -50, -10) },
                new TrackNode<Color4> { Time = 500, Val = new Color4(-10, 0, -40, 0) },
            })
        });

        TestPackage = new Package
        {
            Filename = @"full\path\to\test.fng",
            Name = "test.fng",
            ResourceRequests =
            {
                new ResourceRequest
                {
                    ID = 0x12345678,
                    Name = "TestImage.tga",
                    Type = ResourceType.Image
                },
                new ResourceRequest
                {
                    ID = 0x12345678,
                    Name = "TestFont.ffn",
                    Type = ResourceType.Font
                },
                new ResourceRequest
                {
                    ID = 0x12345678,
                    Name = "TestMovie.avi",
                    Type = ResourceType.Movie
                },
                new ResourceRequest
                {
                    ID = 0x12345678,
                    Name = "Test MultiImage",
                    Type = ResourceType.MultiImage
                },
                new ResourceRequest
                {
                    ID = 0x12345678,
                    Name = "Test Effect (Unused)",
                    Type = ResourceType.Effect
                }
            },
            MessageDefinitions =
            {
                new Package.MessageDefinition
                {
                    Category = "TestCategory",
                    Name = "TestMessage"
                },
                new Package.MessageDefinition
                {
                    Category = "TestCategory",
                    Name = "TestMessage2"
                },
                new Package.MessageDefinition
                {
                    Category = "TestCategory2",
                    Name = "TestMessage"
                },
                new Package.MessageDefinition
                {
                    Category = "TestCategory2",
                    Name = "TestMessage2"
                },
            },
            MessageResponses =
            {
                new MessageResponse(0x12345678, new List<ResponseCommand>
                {
                    new SetScript(0x12345),
                    new PostMessageToFEng(0x1234, 0x5678),
                    new PostMessageToGame(0x13377331),
                    new PostMessageToSound(0x55555555, 0x1212),
                    new SwitchToPackage("test2.fng"),
                    new PushPackageGlobal("test3.fng"),
                    new SetInputProcessing(true)
                })
            },
            Objects =
            {
                group,
                new SimpleImage(new CommonObjectData())
                {
                    Guid = 0x12345,
                    Name = "TestObject1",
                    Scripts =
                    {
                        testScript1,
                        new CommonScript
                        {
                            Id = 0x12345,
                            Events =
                            {
                                new Event
                                {
                                    EventId = 0x13377331,
                                    Target = 0x12346,
                                    Time = 100
                                }
                            },
                            Length = 1000,
                        },
                    },
                    Parent = group
                },
                new Text(new CommonObjectData
                {
                    Color = new Color4(200, 0, 120, 160)
                })
                {
                    Guid = 0x12346,
                    NameHash = 0x41424344,
                },
                new Image(new ImageData())
                {
                    Guid = 0x12347,
                    Parent = group,
                    Name = "CoolTestImage",
                    Scripts = { testImgScript }
                },
                new MultiImage(new MultiImageData())
                {
                    Guid = 0x12348,
                    Name = "Test MultiImage",
                    Scripts = { testMultiImgScript }
                },
                new ColoredImage(new ColoredImageData())
                {
                    Guid = 0x12348,
                    Name = "Test ColoredImage",
                    Scripts = { testColoredImgScript },
                    Flags = ObjectFlags.ConsoleOnly | ObjectFlags.AffectAllScripts | ObjectFlags.IsButton
                }
            }
        };

        TestRenderTree = RenderTree.Create(TestPackage);

        //Movie movie = new Movie(new ImageData());
    }
}