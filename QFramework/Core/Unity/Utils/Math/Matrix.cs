/****************************************************************************
 * Copyright (c) 2017 liangxie
 * https://github.com/mathnet/
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

//My own matrix thing based on stuff I learned here:
//http://www.senocular.com/flash/tutorials/transformmatrix/

namespace QFramework
{
	using UnityEngine;

	public class Matrix
	{
		static public Matrix tempMatrix = new Matrix()
			; //useful for doing calulations without allocating a new matrix every time

		public float a = 1; //scaleXccnode
		public float b = 0; //skewY
		public float c = 0; //skewX
		public float d = 1; //scaleY
		public float tx = 0; //translationX
		public float ty = 0; //translationY

		//a  b  u
		//c  d  v
		//tx ty z

		//x = x*a + y*c + tx
		//y = x*b + y*d + ty

		public Matrix()
		{

		}

		public Matrix Clone()
		{
			Matrix result = new Matrix();
			result.a = a;
			result.b = b;
			result.c = c;
			result.d = d;
			result.tx = tx;
			result.ty = ty;

			return result;
		}

		public void CopyValues(Matrix sourceMatrix)
		{
			a = sourceMatrix.a;
			b = sourceMatrix.b;
			c = sourceMatrix.c;
			d = sourceMatrix.d;
			tx = sourceMatrix.tx;
			ty = sourceMatrix.ty;
		}

		public void SetRotateThenScale(float x, float y, float scaleX, float scaleY,
			float rotationInRadians) //rotates then scales
		{
			float sin = Mathf.Sin(rotationInRadians);
			float cos = Mathf.Cos(rotationInRadians);

			a = scaleX * cos;
			c = scaleX * -sin;
			b = scaleY * sin;
			d = scaleY * cos;

			tx = x;
			ty = y;
		}

		public void SetScaleThenRotate(float x, float y, float scaleX, float scaleY,
			float rotationInRadians) //scales then rotates
		{
			float sin = Mathf.Sin(rotationInRadians);
			float cos = Mathf.Cos(rotationInRadians);

			a = scaleX * cos;
			b = scaleX * sin;
			c = scaleY * -sin;
			d = scaleY * cos;

			tx = x;
			ty = y;
		}

		public void Translate(float deltaX, float deltaY)
		{
			tx += deltaX;
			ty += deltaY;
		}

		public void Scale(float scaleX, float scaleY)
		{
			a *= scaleX;
			c *= scaleX;
			tx *= scaleX;

			b *= scaleY;
			d *= scaleY;
			ty *= scaleY;
		}

		public void Rotate(float rotationInRadians)
		{
			float sin = Mathf.Sin(rotationInRadians);
			float cos = Mathf.Cos(rotationInRadians);

			float oldA = a;
			float oldB = b;
			float oldC = c;
			float oldD = d;
			float oldTX = tx;
			float oldTY = ty;

			a = oldA * cos - oldB * sin;
			b = oldA * sin + oldB * cos;
			c = oldC * cos - oldD * sin;
			d = oldC * sin + oldD * cos;
			tx = oldTX * cos - oldTY * sin;
			ty = oldTX * sin + oldTY * cos;
		}

		public void RotateInPlace(float rotationInRadians)
		{
			float sin = Mathf.Sin(rotationInRadians);
			float cos = Mathf.Cos(rotationInRadians);

			float oldA = a;
			float oldB = b;
			float oldC = c;
			float oldD = d;

			a = oldA * cos - oldB * sin;
			b = oldA * sin + oldB * cos;
			c = oldC * cos - oldD * sin;
			d = oldC * sin + oldD * cos;
		}

		public float GetScaleX()
		{
			return Mathf.Sqrt(a * a + b * b);
		}

		public float GetScaleY()
		{
			return Mathf.Sqrt(c * c + d * d);
		}

		public float GetRotation()
		{
			UnityEngine.Vector2 newVector = GetNewTransformedVector(new UnityEngine.Vector2(0, 1));
			return Mathf.Atan2(newVector.y - ty, newVector.x - tx) - Mathf.PI / 2;  /*RXMath.HALF_PI*/;
		}

		public void Concat(Matrix other)
		{
			float oldA = a;
			float oldB = b;
			float oldC = c;
			float oldD = d;
			float oldTX = tx;
			float oldTY = ty;

			a = oldA * other.a + oldB * other.c;
			b = oldA * other.b + oldB * other.d;
			c = oldC * other.a + oldD * other.c;
			d = oldC * other.b + oldD * other.d;
			tx = oldTX * other.a + oldTY * other.c + other.tx;
			ty = oldTX * other.b + oldTY * other.d + other.ty;
		}

		public void ConcatOther(Matrix other) //the opposite order of Concat
		{
			float oldA = a;
			float oldB = b;
			float oldC = c;
			float oldD = d;
			float oldTX = tx;
			float oldTY = ty;

			a = other.a * oldA + other.b * oldC;
			b = other.a * oldB + other.b * oldD;
			c = other.c * oldA + other.d * oldC;
			d = other.c * oldB + other.d * oldD;
			tx = other.tx * oldA + other.ty * oldC + oldTX;
			ty = other.tx * oldB + other.ty * oldD + oldTY;
		}

		public void ConcatAndCopyValues(Matrix first, Matrix second)
		{
			a = first.a * second.a + first.b * second.c;
			b = first.a * second.b + first.b * second.d;
			c = first.c * second.a + first.d * second.c;
			d = first.c * second.b + first.d * second.d;
			tx = first.tx * second.a + first.ty * second.c + second.tx;
			ty = first.tx * second.b + first.ty * second.d + second.ty;
		}

		public void Invert()
		{
			float oldA = a;
			float oldB = b;
			float oldC = c;
			float oldD = d;
			float oldTX = tx;
			float oldTY = ty;

			float bottom = 1.0f / (a * d - b * c);

			a = oldD * bottom;
			b = -oldB * bottom;
			c = -oldC * bottom;
			d = oldA * bottom;
			tx = (oldC * oldTY - oldD * oldTX) * bottom;
			ty = -(oldA * oldTY - oldB * oldTX) * bottom;
		}

		public void InvertAndCopyValues(Matrix other)
		{
			float bottom = 1.0f / (other.a * other.d - other.b * other.c);

			a = other.d * bottom;
			b = -other.b * bottom;
			c = -other.c * bottom;
			d = other.a * bottom;
			tx = (other.c * other.ty - other.d * other.tx) * bottom;
			ty = -(other.a * other.ty - other.b * other.tx) * bottom;
		}

		public UnityEngine.Vector2 GetNewTransformedVector(UnityEngine.Vector2 vector)
		{
			return new UnityEngine.Vector2
			(
				vector.x * a + vector.y * c + tx,
				vector.x * b + vector.y * d + ty
			);
		}

		public UnityEngine.Vector2 GetTransformedUnitVector()
		{
			return new UnityEngine.Vector2(a + c + tx, b + d + ty);
		}

		public Vector3 GetVector3FromLocalVector2(UnityEngine.Vector2 localVector, float z)
		{
			return new Vector3
			(
				localVector.x * a + localVector.y * c + tx,
				localVector.x * b + localVector.y * d + ty,
				z
			);
		}

		public void ApplyVector3FromLocalVector2(ref Vector3 outVector, UnityEngine.Vector2 localVector, float z)
		{
			outVector.x = localVector.x * a + localVector.y * c + tx;
			outVector.y = localVector.x * b + localVector.y * d + ty;
			outVector.z = z;
		}

		public void ResetToIdentity()
		{
			a = 1;
			b = 0;
			c = 0;
			d = 1;
			tx = 0;
			ty = 0;
		}

		override public string ToString()
		{
			return string.Format("[[Matrix A:{0} B:{1} C:{2} D:{3} TX:{4} TY:{5} ]]", a, b, c, d, tx, ty);
		}
	}
}