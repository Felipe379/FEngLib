using System.Numerics;
using FEngLib.Messaging;
using FEngLib.Messaging.Commands;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Scripts;
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

        testImgScript.SetTrack(ImageScriptTrackIds.LowerRight, new Vector2Track());
        testImgScript.SetTrack(ImageScriptTrackIds.UpperLeft, new Vector2Track());

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
                new Text(new CommonObjectData())
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
                }
            }
        };

        TestRenderTree = RenderTree.Create(TestPackage);

        //Movie movie = new Movie(new ImageData());
    }
}