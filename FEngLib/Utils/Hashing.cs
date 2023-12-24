using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FEngLib.Utils;

public static class Hashing
{
    public static uint BinHash(this string str)
    {
        return str.Aggregate(0xFFFFFFFFu, (h, c) => h * 33 + c);
    }

	public static bool IsHash(this string hash, int numericBase)
	{
		switch (numericBase)
		{
			case 16:
				return new Regex(@"^(((0x)|(0X)){1}[0-9a-fA-F]{1,8})$").Match(hash).Success;
			case 10:
				return new Regex(@"^(((0d)|(0D)){1}[0-9]{1,10})$").Match(hash).Success;
			case 8:
				return new Regex(@"^(((0o)|(0O)){1}[0-7]{1,11})$").Match(hash).Success;
			case 2:
				return new Regex(@"^(((0b)|(0B)){1}[0-1]{1,32})$").Match(hash).Success;
			default:
				throw new NotImplementedException();
		}
	}
}
