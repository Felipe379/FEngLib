using System.IO;
using System.Numerics;
using System.Xml.Linq;
using FEngLib.Scripts;
using FEngLib.Utils;

namespace FEngLib.Objects;

public class ImageData : ObjectData
{
	public Vector2 UpperLeft { get; set; }
	public Vector2 LowerRight { get; set; }

	public override void Read(BinaryReader br)
	{
		base.Read(br);

		UpperLeft = br.ReadVector2();
		LowerRight = br.ReadVector2();
	}

	public override void Write(BinaryWriter bw)
	{
		base.Write(bw);

		bw.Write(UpperLeft);
		bw.Write(LowerRight);
	}

	protected void InternalClone(ImageData @object)
	{
		base.InternalClone(@object);

		this.UpperLeft = @object.UpperLeft;
		this.LowerRight = @object.LowerRight;
	}

	public override object Clone()
	{
		var result = new ImageData();

		result.InternalClone(this);

		return result;
	}
}

public class ImageScriptTracks : ScriptTracks
{
	protected void InternalClone(ImageScriptTracks @object)
	{
		base.InternalClone(@object);

		this.UpperLeft = @object.UpperLeft;
		this.LowerRight = @object.LowerRight;
	}

	public override object Clone()
	{
		var result = new ImageScriptTracks();

		result.InternalClone(this);

		return result;
	}

	public Vector2Track UpperLeft { get; set; }
	public Vector2Track LowerRight { get; set; }
}

public class ImageScript<TTracks> : Script<TTracks> where TTracks : ImageScriptTracks, new()
{
	public override object Clone()
	{
		var result = new ImageScript<TTracks>();

		result.InternalClone(this);

		return result;
	}
}

public class Image : Image<ImageData, ImageScript<ImageScriptTracks>>
{
	public Image(ImageData data) : base(data)
	{
		Type = ObjectType.Image;
	}

	public override object Clone()
	{
		var result = new Image(null);

		result.InternalClone(this);

		return result;
	}
}

public interface IImage<out TData> : IObject<TData> where TData : ImageData
{
	uint ImageFlags { get; set; }
}

public class Image<TData, TScript> : BaseObject<TData, TScript>, IImage<TData>
	where TData : ImageData, new()
	where TScript : Script, new()
{
	protected Image(TData data) : base(data)
	{
		//Data = data;
#if DEBUG
		//"Fix" render crash on some UG1 .fngs from the demo/final game
		Data = data ?? new();
#else
		Data = data;
#endif
	}

	public override void InitializeData()
	{
		Data = new TData();
	}

	protected void InternalClone(Image<TData, TScript> @object)
	{
		base.InternalClone(@object);

		this.ImageFlags = @object.ImageFlags;
	}

	public override object Clone()
	{
		var result = new Image<TData, TScript>(null);

		result.InternalClone(this);

		return result;
	}

	public uint ImageFlags { get; set; }
}
