using UnityEngine;
using System;
using System.Collections;

namespace Avante
{
	public class Lib : MonoBehaviour
	{
		//
		// Enums
		public static string GetEnumNameByValue(Type enumType, int value)
		{
			foreach (var v in Enum.GetValues(enumType))
				if ( (int)v == value )
					return v.ToString();
			return null;
		}
		public static int GetEnumValueByName(Type enumType, string name)
		{
			foreach (var v in Enum.GetValues(enumType))
				if ( v.ToString().Equals(name) )
					return (int)v;
			return -999999;
		}

		//
		// longs
		static public long Max(long a, long b)
		{
			return a > b ? a : b;
		}
		static public long Min(long a, long b)
		{
			return a < b ? a : b;
		}

		//
		// Lmap
		static public float Map(float value, float istart, float istop, float ostart, float ostop)
		{
			return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
		}

		//
		// Angle between 2 vectors
		static public float AngleBetween(Vector3 v0, Vector3 v1)
		{
			return Mathf.Atan2(v1.y-v0.y, v1.x-v0.x) * Mathf.Rad2Deg;
		}

		//
		// String
		static public string ToStringSpaces(int value, int size)
		{
			return String.Format("{0,"+size+"}", value);
		}
		static public string ToStringZeroes(int value, int size)
		{
			return value.ToString("D"+size);
		}



		//
		// Dome Stuff
		//

		// store 3D dome coordinate as texture
		// Unit coordinates		(-1.0 .. 1.0)
		// Texel coordinates	( 0.0 .. 1.0)
		public static Vector2 texelToUnit2(Vector2 uv)
		{
			return (uv * 2f) - Vector2.one;
		}
		public static Vector3 texelToUnit3(Vector3 uv)
		{
			return (uv * 2f) - Vector3.one;
		}
		public static Vector2 unitToTexel2(Vector2 uv)
		{
			return (uv + Vector2.one) * 0.5f;
		}
		public static Vector3 unitToTexel3(Vector3 uv)
		{
			return (uv + Vector3.one) * 0.5f;
		}

		// pbourke bangalore.pdf pg 14
		// http://paulbourke.net/miscellaneous/domefisheye/fisheye/
		// From:	Dome 3D coordinates (-1.0 .. 1.0)
		// To:		Texture coordinates (0.0 .. 1.0)
		public static Vector2 domeToTexel( Vector3 pos, float horizon )
		{
			float theta = Mathf.Atan2( Mathf.Sqrt( pos.x * pos.x + pos.y * pos.y ), pos.z);	// invert y/z ???
			float phi = Mathf.Atan2( pos.y, pos.x );
			float r = theta / (horizon * 0.5f);
			Vector2 st = new Vector2( r * Mathf.Cos(phi), r * Mathf.Sin(phi) );
			return unitToTexel2( st );
		}
		public static Vector2 domeToTexel( Vector3 pos )
		{
			return domeToTexel( pos, Mathf.PI );
		}
		public static Vector3 texelToDome( Vector2 st, float horizon )
		{
			st = texelToUnit2( st );
			float r = Mathf.Sqrt( st.x * st.x + st.y * st.y );
			float theta = Mathf.Atan2( st.y, st.x );
			float phi = r * (horizon * 0.5f);
			Vector3 pos = new Vector3();
			pos.x = Mathf.Sin(phi) * Mathf.Cos(theta);
			pos.y = Mathf.Cos(phi);
			pos.z = Mathf.Sin(phi) * Mathf.Sin(theta);
			float y = pos.y; pos.y = pos.z; pos.z = y;	// invert y/z ???
			return pos;
		}
		public static Vector3 texelToDome( Vector2 st )
		{
			return texelToDome( st, Mathf.PI );
		}
	}
}

