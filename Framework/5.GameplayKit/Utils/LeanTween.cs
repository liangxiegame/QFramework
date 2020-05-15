// Copyright (c) 2014 Russell Savage - Dented Pixel
// 
// LeanTween version 2.18 - http://dentedpixel.com/developer-diary/
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

/*
TERMS OF USE - EASING EQUATIONS#
Open source under the BSD License.
Copyright (c)2001 Robert Penner
All rights reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

/**
* Pass this to the "ease" parameter in the optional hashtable, to get a different easing behavior<br><br>
* <strong>Example: </strong><br>LeanTween.rotateX(gameObject, 270.0f, 1.5f).setEase(LeanTweenType.easeInBack);
*
* @class LeanTweenType
*/

/**
* @property {integer} linear
*/
/**
* @property {integer} easeOutQuad
*/
/**
* @property {integer} easeInQuad
*/
/**
* @property {integer} easeInOutQuad
*/
/**
* @property {integer} easeInCubic
*/
/**
* @property {integer} easeOutCubic
*/
/**
* @property {integer} easeInOutCubic
*/
/**
* @property {integer} easeInQuart
*/
/**
* @property {integer} easeOutQuart
*/
/**
* @property {integer} easeInOutQuart
*/
/**
* @property {integer} easeInQuint
*/
/**
* @property {integer} easeOutQuint
*/
/**
* @property {integer} easeInOutQuint
*/
/**
* @property {integer} easeInSine
*/
/**
* @property {integer} easeOutSine
*/
/**
* @property {integer} easeInOutSine
*/
/**
* @property {integer} easeInExpo
*/
/**
* @property {integer} easeOutExpo
*/
/**
* @property {integer} easeInOutExpo
*/
/**
* @property {integer} easeInCirc
*/
/**
* @property {integer} easeOutCirc
*/
/**
* @property {integer} easeInOutCirc
*/
/**
* @property {integer} easeInBounce
*/
/**
* @property {integer} easeOutBounce
*/
/**
* @property {integer} easeInOutBounce
*/
/**
* @property {integer} easeInBack
*/
/**
* @property {integer} easeOutBack
*/
/**
* @property {integer} easeInOutBack
*/
/**
* @property {integer} easeInElastic
*/
/**
* @property {integer} easeOutElastic
*/
/**
* @property {integer} easeInOutElastic
*/
/**
* @property {integer} punch
*/
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class LeanTest : object {
	public static int expected = 0;
	private static int tests = 0;
	private static int passes = 0;

	public static void debug( string name, bool didPass, string failExplaination = null){
		float len = printOutLength(name);
		string logName = formatB(name) +" " + "".PadRight(40-(int)(len*1.05f),"_"[0]) + " [ "+ (didPass ? formatC("pass","green") : formatC("fail","red")) +" ]";
		if(didPass==false && failExplaination!=null)
			logName += " - " + failExplaination;
		Debug.Log(logName);
		if(didPass)
			passes++;
		tests++;

		if(tests==expected){
			Debug.Log(formatB("Final Report:")+" _____________________ PASSED: "+formatBC(""+passes,"green")+" FAILED: "+formatBC(""+(tests-passes),"red")+" ");
		}else if(tests>expected){
			Debug.Log(formatB("Too many tests for a final report!") + " set LeanTest.expected = "+tests);
		}
	}

	public static float printOutLength( string str ){
		float len = 0.0f;
		for(int i = 0; i < str.Length; i++){
			if(str[i]=="I"[0]){
				len += 0.5f;
			}else if(str[i]=="J"[0]){
				len += 0.85f;
			}else{
				len += 1.0f;
			}
		}
		return len;
	}

	public static string formatBC( string str, string color ){
		return formatC(formatB(str),color);
	}

	public static string formatB( string str ){
		#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		return str;
		#else
		return "<b>"+ str + "</b>";
		#endif
	}

	public static string formatC( string str, string color ){
		#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		return str;
		#else
		return "<color="+color+">"+ str + "</color>";
		#endif
	}

	public static void overview(){

	}
}

/**
* Animate GUI Elements by creating this object and passing the *.rect variable to the GUI method<br><br>
* <strong>Example Javascript: </strong><br>var bRect:LTRect = new LTRect( 0, 0, 100, 50 );<br>
* LeanTween.scale( bRect, Vector2(bRect.rect.width, bRect.rect.height) * 1.3, 0.25 );<br>
* function OnGUI(){<br>
* &nbsp; if(GUI.Button(bRect.rect, "Scale")){ }<br>
* }<br>
* <br>
* <strong>Example C#: </strong> <br>
* LTRect bRect = new LTRect( 0f, 0f, 100f, 50f );<br>
* LeanTween.scale( bRect, new Vector2(150f,75f), 0.25f );<br>
* void OnGUI(){<br>
* &nbsp; if(GUI.Button(bRect.rect, "Scale")){ }<br>
* }<br>
*
* @class LTRect
* @constructor
* @param {float} x:float X location
* @param {float} y:float Y location
* @param {float} width:float Width
* @param {float} height:float Height
* @param {float} alpha:float (Optional) initial alpha amount (0-1)
* @param {float} rotation:float (Optional) initial rotation in degrees (0-360) 
*/

[System.Serializable]
public class LTRect : System.Object{
	/**
	* Pass this value to the GUI Methods
	* 
	* @property rect
	* @type {Rect} rect:Rect Rect object that controls the positioning and size
	*/
	public Rect _rect;
	public float alpha = 1f;
	public float rotation;
	public Vector2 pivot;
	public Vector2 margin;
	public Rect relativeRect = new Rect(0f,0f,float.PositiveInfinity,float.PositiveInfinity);

	public bool rotateEnabled;
	[HideInInspector]
	public bool rotateFinished;
	public bool alphaEnabled;
	public string labelStr;
	public LTGUI.Element_Type type;
	public GUIStyle style;
	public bool useColor = false;
	public Color color = Color.white;
	public bool fontScaleToFit;
	public bool useSimpleScale;
	public bool sizeByHeight;

	public Texture texture;

	private int _id = -1;
	[HideInInspector]
	public int counter;

	public static bool colorTouched;

	public LTRect(){
		reset();
		this.rotateEnabled = this.alphaEnabled = true;
		_rect = new Rect(0f,0f,1f,1f);
	}

	public LTRect(Rect rect){
		_rect = rect;
		reset();
	}

	public LTRect(float x, float y, float width, float height){
		_rect = new Rect(x,y,width,height);
		this.alpha = 1.0f;
		this.rotation = 0.0f;
		this.rotateEnabled = this.alphaEnabled = false;
	}

	public LTRect(float x, float y, float width, float height, float alpha){
		_rect = new Rect(x,y,width,height);
		this.alpha = alpha;
		this.rotation = 0.0f;
		this.rotateEnabled = this.alphaEnabled = false;
	}

	public LTRect(float x, float y, float width, float height, float alpha, float rotation){
		_rect = new Rect(x,y,width,height);
		this.alpha = alpha;
		this.rotation = rotation;
		this.rotateEnabled = this.alphaEnabled = false;
		if(rotation!=0.0f){
			this.rotateEnabled = true;
			resetForRotation();
		}
	}

	public bool hasInitiliazed{
		get{ 
			return _id!=-1;
		}
	}

	public int id{
		get{ 
			int toId = _id | counter << 16;

			/*uint backId = toId & 0xFFFF;
			uint backCounter = toId >> 16;
			if(_id!=backId || backCounter!=counter){
				Debug.LogError("BAD CONVERSION toId:"+_id);
			}*/

			return toId;
		}
	} 

	public void setId( int id, int counter){
		this._id = id;
		this.counter = counter;
	}

	public void reset(){
		this.alpha = 1.0f;
		this.rotation = 0.0f;
		this.rotateEnabled = this.alphaEnabled = false;
		this.margin = Vector2.zero;
		this.sizeByHeight = false;
		this.useColor = false;
	}

	public void resetForRotation(){
		Vector3 scale = new Vector3(GUI.matrix[0,0], GUI.matrix[1,1], GUI.matrix[2,2]);
        if(pivot==Vector2.zero){
            pivot = new Vector2((_rect.x+((_rect.width)*0.5f )) * scale.x + GUI.matrix[0,3], (_rect.y+((_rect.height)*0.5f )) * scale.y + GUI.matrix[1,3]);
        }
	}

	public float x{
		get{ return _rect.x; }
		set{ _rect.x = value; }
	}

	public float y{
		get{ return _rect.y; }
		set{ _rect.y = value; }
	}

	public float width{
		get{ return _rect.width; }
		set{ _rect.width = value; }
	}

	public float height{
		get{ return _rect.height; }
		set{ _rect.height = value; }
	}

	public Rect rect{

		get{
			if(colorTouched){
				colorTouched = false;
				GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);
			}
			if(rotateEnabled){
				 if(rotateFinished){
                    rotateFinished = false;
                    rotateEnabled = false;
                    //this.rotation = 0.0f;
                    pivot = Vector2.zero;
                }else{
                    GUIUtility.RotateAroundPivot(rotation, pivot);
                }
			}
			if(alphaEnabled){
				GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,alpha);
				colorTouched = true;
			}
			if(fontScaleToFit){
				if(this.useSimpleScale){
					style.fontSize = (int)(_rect.height*this.relativeRect.height);
				}else{
					style.fontSize = (int)_rect.height;
				}
			}
			return _rect;
		}

		set{
			_rect = value;
		}	
	}

	public LTRect setStyle( GUIStyle style ){
		this.style = style;
		return this;
	}

	public LTRect setFontScaleToFit( bool fontScaleToFit ){
		this.fontScaleToFit = fontScaleToFit;
		return this;
	}

	public LTRect setColor( Color color ){
		this.color = color;
		this.useColor = true;
		return this;
	}

	public LTRect setAlpha( float alpha ){
		this.alpha = alpha;
		return this;
	}

	public LTRect setLabel( String str ){
		this.labelStr = str;
		return this;
	}

	public LTRect setUseSimpleScale( bool useSimpleScale, Rect relativeRect){
		this.useSimpleScale = useSimpleScale;
		this.relativeRect = relativeRect;
		return this;
	}

	public LTRect setUseSimpleScale( bool useSimpleScale){
		this.useSimpleScale = useSimpleScale;
		this.relativeRect = new Rect(0f,0f,Screen.width,Screen.height);
		return this;
	}

	public LTRect setSizeByHeight( bool sizeByHeight){
		this.sizeByHeight = sizeByHeight;
		return this;
	}

	public override string ToString(){
		return "x:"+_rect.x+" y:"+_rect.y+" width:"+_rect.width+" height:"+_rect.height;
	}
}

public class LTBezier{
	public float length;

	private Vector3 a;
	private Vector3 aa;
	private Vector3 bb;
	private Vector3 cc;
	private float len;
	private float[] arcLengths;

	public LTBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float precision){
		this.a = a;
	    aa = (-a + 3*(b-c) + d);
	    bb = 3*(a+c) - 6*b;
	    cc = 3*(b-a);

	    this.len = 1.0f / precision;
	    arcLengths = new float[(int)this.len + (int)1];
	    arcLengths[0] = 0;

	    Vector3 ov = a;
	    Vector3 v;
	    float clen = 0.0f;
	    for(int i = 1; i <= this.len; i++) {
	        v = bezierPoint(i * precision);
	        clen += (ov - v).magnitude;
	        this.arcLengths[i] = clen;
	        ov = v;
	    }
	    this.length = clen;
	}

    private float map(float u) {
        float targetLength = u * this.arcLengths[(int)this.len];
        int low = 0;
        int high = (int)this.len;
        int index = 0;
        while (low < high) {
            index = low + ((int)((high - low) / 2.0f) | 0);
            if (this.arcLengths[index] < targetLength) {
                low = index + 1;
            } else {
                high = index;
            }
        }
        if(this.arcLengths[index] > targetLength)
            index--;
        if(index<0)
        	index = 0;

        return (index + (targetLength - arcLengths[index]) / (arcLengths[index + 1] - arcLengths[index])) / this.len;
    }

   	private Vector3 bezierPoint(float t){
	    return ((aa* t + (bb))* t + cc)* t + a;
	}

    public Vector3 point(float t){ 
    	return bezierPoint( map(t) ); 
    }
}

/**
* Manually animate along a bezier path with this class
* @class LTBezierPath
* @constructor
* @param {Vector3 Array} pts A set of points that define one or many bezier paths (the paths should be passed in multiples of 4, which correspond to each individual bezier curve)
* @example 
* LTBezierPath ltPath = new LTBezierPath( new Vector3[] { new Vector3(0f,0f,0f),new Vector3(1f,0f,0f), new Vector3(1f,0f,0f), new Vector3(1f,1f,0f)} );<br><br>
* LeanTween.move(lt, ltPath.vec3, 4.0f).setOrientToPath(true).setDelay(1f).setEase(LeanTweenType.easeInOutQuad); // animate <br>
* Vector3 pt = ltPath.point( 0.6f ); // retrieve a point along the path
*/
public class LTBezierPath{
	public Vector3[] pts;
	public float length;
	public bool orientToPath;
	public bool orientToPath2d;

	private LTBezier[] beziers;
	private float[] lengthRatio;

	public LTBezierPath(){ }
	public LTBezierPath( Vector3[] pts_ ){
		setPoints( pts_ );
	}

	public void setPoints( Vector3[] pts_ ){
		if(pts_.Length<4)
			LeanTween.logError( "LeanTween - When passing values for a vector path, you must pass four or more values!" );
		if(pts_.Length%4!=0)
			LeanTween.logError( "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2..." );

		pts = pts_;
		
		int k = 0;
		beziers = new LTBezier[ pts.Length / 4 ];
		lengthRatio = new float[ beziers.Length ];
		int i;
		length = 0;
		for(i = 0; i < pts.Length; i+=4){
			beziers[k] = new LTBezier(pts[i+0],pts[i+2],pts[i+1],pts[i+3],0.05f);
			length += beziers[k].length;
			k++;
		}
		// Debug.Log("beziers.Length:"+beziers.Length + " beziers:"+beziers);
		for(i = 0; i < beziers.Length; i++){
			lengthRatio[i] = beziers[i].length / length;
		}
	}

	/**
	* Retrieve a point along a path
	* 
	* @method point
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @return {Vector3} Vector3 position of the point along the path
	* @example
	* transform.position = ltPath.point( 0.6f );
	*/
	public Vector3 point( float ratio ){
		float added = 0.0f;
		for(int i = 0; i < lengthRatio.Length; i++){
			added += lengthRatio[i];
			if(added >= ratio)
				return beziers[i].point( (ratio-(added-lengthRatio[i])) / lengthRatio[i] );
		}
		return beziers[lengthRatio.Length-1].point( 1.0f );
	}

	public void place2d( Transform transform, float ratio ){
		transform.position = point( ratio );
		ratio += 0.001f;
		if(ratio<=1.0f){
			float angle = Vector3.Angle(transform.position, point( ratio ) - transform.position );
			transform.rotation = Quaternion.identity;
			transform.RotateAround(transform.position, Vector3.up, 90.0f);
			transform.RotateAround(transform.position, Vector3.right, angle);
		}
	}

	public void placeLocal2d( Transform transform, float ratio ){
		transform.localPosition = point( ratio );
		ratio += 0.001f;
		if(ratio<=1.0f){
			float angle = Vector3.Angle(transform.localPosition, transform.parent.TransformPoint( point( ratio ) ) - transform.localPosition );
			transform.rotation = Quaternion.identity;
			transform.RotateAround(transform.position, Vector3.up, 90.0f);
			transform.RotateAround(transform.position, Vector3.right, angle);
		}
	}

	/**
	* Place an object along a certain point on the path (facing the direction perpendicular to the path)
	* 
	* @method place
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @example
	* ltPath.place( transform, 0.6f );
	*/
	public void place( Transform transform, float ratio ){
		place( transform, ratio, Vector3.up );
	}

	/**
	* Place an object along a certain point on the path, with it facing a certain direction perpendicular to the path
	* 
	* @method place
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @param {Vector3} rotation:Vector3 the direction in which to place the transform ex: Vector3.up
	* @example
	* ltPath.place( transform, 0.6f, Vector3.left );
	*/
	public void place( Transform transform, float ratio, Vector3 worldUp ){
		transform.position = point( ratio );
		ratio += 0.001f;
		if(ratio<=1.0f)
			transform.LookAt( point( ratio ), worldUp );
	}

	/**
	* Place an object along a certain point on the path (facing the direction perpendicular to the path) - Local Space, not world-space
	* 
	* @method placeLocal
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @example
	* ltPath.placeLocal( transform, 0.6f );
	*/
	public void placeLocal( Transform transform, float ratio ){
		placeLocal( transform, ratio, Vector3.up );
	}

	/**
	* Place an object along a certain point on the path, with it facing a certain direction perpendicular to the path - Local Space, not world-space
	* 
	* @method placeLocal
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @param {Vector3} rotation:Vector3 the direction in which to place the transform ex: Vector3.up
	* @example
	* ltPath.placeLocal( transform, 0.6f, Vector3.left );
	*/
	public void placeLocal( Transform transform, float ratio, Vector3 worldUp ){
		transform.localPosition = point( ratio );
		ratio += 0.001f;
		if(ratio<=1.0f)
			transform.LookAt( transform.parent.TransformPoint( point( ratio ) ), worldUp );
	}
}

/**
* Animate along a set of points that need to be in the format: controlPoint, point1, point2.... pointLast, endControlPoint
* @class LTSpline
* @constructor
* @param {Vector3 Array} pts A set of points that define the points the path will pass through (starting with starting control point, and ending with a control point)
* @example 
* LTSpline ltSpline = new LTSpline( new Vector3[] { new Vector3(0f,0f,0f),new Vector3(0f,0f,0f), new Vector3(0f,0.5f,0f), new Vector3(1f,1f,0f), new Vector3(1f,1f,0f)} );<br><br>
* LeanTween.moveSpline(lt, ltSpline.vec3, 4.0f).setOrientToPath(true).setDelay(1f).setEase(LeanTweenType.easeInOutQuad); // animate <br>
* Vector3 pt = ltSpline.point( 0.6f ); // retrieve a point along the path
*/
[System.Serializable]
public class LTSpline {
	public Vector3[] pts;
	public bool orientToPath;
	public bool orientToPath2d;
	private float[] lengthRatio;
	private float[] lengths;
	private int numSections;
	private int currPt;
	private float totalLength;
	
	public LTSpline(params Vector3[] pts) {
		this.pts = new Vector3[pts.Length];
		System.Array.Copy(pts, this.pts, pts.Length);

		numSections = pts.Length - 3;
		int precision = 20;
		lengthRatio = new float[precision];
		lengths = new float[precision];
		
		Vector3 lastPoint = new Vector3(Mathf.Infinity,0,0);
		
		totalLength = 0f;
		for(int i = 0; i < precision; i++){
			//Debug.Log("i:"+i);
			float fract = (i*1f) / precision;
			Vector3 point = interp( fract );

			if(i>=1){
				lengths[ i ] = (point - lastPoint).magnitude;
				// Debug.Log("fract:"+fract+" mag:"+lengths[ i ] + " i:"+i);
			}
			totalLength += lengths[ i ];

			lastPoint = point;
		}

		float ratioTotal = 0f;
		for(int i = 0; i < lengths.Length; i++){
			float t = i *1f / (lengths.Length-1);
			currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);

			float ratioLength = lengths[i] / totalLength;
			ratioTotal += ratioLength;
			lengthRatio[i] = ratioTotal;
			
			//Debug.Log("lengthRatio["+i+"]:"+lengthRatio[i]+" lengths["+i+"]:"+lengths[i] + " t:"+t);
		}
	}

	public float map( float t ){
		//Debug.Log("map t:"+t);
		for(int i = 0; i < lengthRatio.Length; i++){
			if(lengthRatio[i] >= t){
				// Debug.Log("map lengthRatio["+i+"]:"+lengthRatio[i]);
				return lengthRatio[i]+(t-lengthRatio[i]);
			}
		}
		return 1f;
	}
	
	public Vector3 interp(float t) {
		// The adjustments done to numSections, I am not sure why I needed to add them
		/*int numSections = this.numSections+1;
		if(numSections>=3)
			numSections += 1;*/
		currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		// Debug.Log("currPt:"+currPt+" numSections:"+numSections+" pts.Length :"+pts.Length );
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
	}

	/**
	* Retrieve a point along a path
	* 
	* @method point
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @return {Vector3} Vector3 position of the point along the path
	* @example
	* transform.position = ltSpline.point( 0.6f );
	*/
	public Vector3 point( float ratio ){
		float t = map( ratio );
		//Debug.Log("t:"+t+" ratio:"+ratio);
		//float t = ratio;
		return interp( t );
	}

	public void place2d( Transform transform, float ratio ){
		transform.position = point( ratio );
		ratio += 0.001f;
		if(ratio<=1.0f){
			Vector3 v3Dir = point( ratio ) - transform.position;
			float angle = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0, 0, angle-90);
		}
	}

	public void placeLocal2d( Transform transform, float ratio ){
		transform.localPosition = point( ratio );
		ratio += 0.001f;
		if(ratio<=1.0f){
			Vector3 v3Dir = transform.parent.TransformPoint( point( ratio ) ) - transform.localPosition;
			float angle = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0, 0, angle-90);
		}
	}


	/**
	* Place an object along a certain point on the path (facing the direction perpendicular to the path)
	* 
	* @method place
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @example
	* ltPath.place( transform, 0.6f );
	*/
	public void place( Transform transform, float ratio ){
		place(transform, ratio, Vector3.up);
	}

	/**
	* Place an object along a certain point on the path, with it facing a certain direction perpendicular to the path
	* 
	* @method place
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @param {Vector3} rotation:Vector3 the direction in which to place the transform ex: Vector3.up
	* @example
	* ltPath.place( transform, 0.6f, Vector3.left );
	*/
	public void place( Transform transform, float ratio, Vector3 worldUp ){
		transform.position = point( ratio );
		ratio += 0.001f;
		if(ratio<=1.0f)
			transform.LookAt( point( ratio ), worldUp );
	}

	/**
	* Place an object along a certain point on the path (facing the direction perpendicular to the path) - Local Space, not world-space
	* 
	* @method placeLocal
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @example
	* ltPath.placeLocal( transform, 0.6f );
	*/
	public void placeLocal( Transform transform, float ratio ){
		placeLocal( transform, ratio, Vector3.up );
	}

	/**
	* Place an object along a certain point on the path, with it facing a certain direction perpendicular to the path - Local Space, not world-space
	* 
	* @method placeLocal
	* @param {Transform} transform:Transform the transform of the object you wish to place along the path
	* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
	* @param {Vector3} rotation:Vector3 the direction in which to place the transform ex: Vector3.up
	* @example
	* ltPath.placeLocal( transform, 0.6f, Vector3.left );
	*/
	public void placeLocal( Transform transform, float ratio, Vector3 worldUp ){
		transform.localPosition = point( ratio );
		ratio += 0.001f;
		if(ratio<=1.0f)
			transform.LookAt( transform.parent.TransformPoint( point( ratio ) ), worldUp );
	}
	
	public void gizmoDraw(float t = -1.0f) {
		if(lengthRatio!=null && lengthRatio.Length>0){
			Vector3 prevPt = point(0);
			
			for (int i = 1; i <= 120; i++) {
				float pm = (float) i / 120f;
				Vector3 currPt = point(pm);
				Gizmos.DrawLine(currPt, prevPt);
				prevPt = currPt;
			}
		}
	}

	public Vector3 Velocity(float t) {
		t = map( t );

		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];

		return 1.5f * (-a + 3f * b - 3f * c + d) * (u * u)
				+ (2f * a -5f * b + 4f * c - d) * u
				+ .5f * c - .5f * a;
	}
}

public enum TweenAction{
	MOVE_X,
	MOVE_Y,
	MOVE_Z,
	MOVE_LOCAL_X,
	MOVE_LOCAL_Y,
	MOVE_LOCAL_Z,
	MOVE_CURVED,
	MOVE_CURVED_LOCAL,
	MOVE_SPLINE,
	MOVE_SPLINE_LOCAL,
	SCALE_X,
	SCALE_Y,
	SCALE_Z,
	ROTATE_X,
	ROTATE_Y,
	ROTATE_Z,
	ROTATE_AROUND,
	ROTATE_AROUND_LOCAL,
	ALPHA,
	ALPHA_VERTEX,
	COLOR,
	CALLBACK_COLOR,
	CANVAS_ROTATEAROUND,
	CALLBACK,
	MOVE,
	MOVE_LOCAL,
	ROTATE,
	ROTATE_LOCAL,
	SCALE,
	VALUE3,
	GUI_MOVE,
	GUI_MOVE_MARGIN,
	GUI_SCALE,
	GUI_ALPHA,
	GUI_ROTATE,
	DELAYED_SOUND,
	CANVAS_MOVE,
	CANVAS_SCALE
}

public enum LeanTweenType{
	notUsed, linear, easeOutQuad, easeInQuad, easeInOutQuad, easeInCubic, easeOutCubic, easeInOutCubic, easeInQuart, easeOutQuart, easeInOutQuart, 
	easeInQuint, easeOutQuint, easeInOutQuint, easeInSine, easeOutSine, easeInOutSine, easeInExpo, easeOutExpo, easeInOutExpo, easeInCirc, easeOutCirc, easeInOutCirc, 
	easeInBounce, easeOutBounce, easeInOutBounce, easeInBack, easeOutBack, easeInOutBack, easeInElastic, easeOutElastic, easeInOutElastic, easeSpring, easeShake, punch, once, clamp, pingPong, animationCurve
}

/**
* Internal Representation of a Tween<br>
* <br>
* This class represents all of the optional parameters you can pass to a method (it also represents the internal representation of the tween).<br><br>
* <strong id='optional'>Optional Parameters</strong> are passed at the end of every method:<br> 
* <br>
* &nbsp;&nbsp;<i>Example:</i><br>
* &nbsp;&nbsp;LeanTween.moveX( gameObject, 1f, 1f).setEase( <a href="LeanTweenType.html">LeanTweenType</a>.easeInQuad ).setDelay(1f);<br>
* <br>
* You can pass the optional parameters in any order, and chain on as many as you wish.<br>
* You can also <strong>pass parameters at a later time</strong> by saving a reference to what is returned.<br>
* <br>
* &nbsp;&nbsp;<i>Example:</i><br>
* &nbsp;&nbsp;<a href="LTDescr.html">LTDescr</a> d = LeanTween.moveX(gameObject, 1f, 1f);<br>
* &nbsp;&nbsp;&nbsp; ...later set some parameters<br>
* &nbsp;&nbsp;d.setOnComplete( onCompleteFunc ).setEase( <a href="LeanTweenType.html">LeanTweenType</a>.easeInOutBack );<br>
* <br>
* Retrieve a <strong>unique id</strong> for the tween by using the "id" property. You can pass this to LeanTween.pause, LeanTween.resume, LeanTween.cancel methods<br>
* <br>
* &nbsp;&nbsp;<i>Example:</i><br>
* &nbsp;&nbsp;int id = LeanTween.moveX(gameObject, 1f, 3f).id;<br>
* &nbsp;&nbsp;LeanTween.pause( id );<br>
* @class LTDescr
* @constructor
*/
public class LTDescr{
	public bool toggle;
	public bool useEstimatedTime;
	public bool useFrames;
	public bool useManualTime;
	public bool hasInitiliazed;
	public bool hasPhysics;
	public float passed;
	public float delay;
	public float time;
	public float lastVal;
	private uint _id;
	public int loopCount;
	public uint counter;
	public float direction;
	public bool destroyOnComplete;
	public Transform trans;
	public LTRect ltRect;
	public Vector3 from;
	public Vector3 to;
	public Vector3 diff;
	public Vector3 point;
	public Vector3 axis;
	public Quaternion origRotation;
	public LTBezierPath path;
	public LTSpline spline;
	public TweenAction type;
	public LeanTweenType tweenType;
	public AnimationCurve animationCurve;
	public LeanTweenType loopType;
	public Action<float> onUpdateFloat;
	public Action<float,object> onUpdateFloatObject;
	public Action<Vector3> onUpdateVector3;
	public Action<Vector3,object> onUpdateVector3Object;
	public Action<Color> onUpdateColor;
	public Action onComplete;
	public Action<object> onCompleteObject;
	public object onCompleteParam;
	public object onUpdateParam;
	public bool onCompleteOnRepeat;
	public bool onCompleteOnStart;

	#if !UNITY_METRO
	public Hashtable optional;
	#endif
	#if UNITY_4_6 || UNITY_5_0
	public RectTransform rectTransform;
	#endif

	private static uint global_counter = 0;

    public override string ToString(){
		return (trans!=null ? "gameObject:"+trans.gameObject : "gameObject:null")+" toggle:"+toggle+" passed:"+passed+" time:"+time+" delay:"+delay+" from:"+from+" to:"+to+" type:"+type+" ease:"+tweenType+" useEstimatedTime:"+useEstimatedTime+" id:"+id+" hasInitiliazed:"+hasInitiliazed;
	}

	public LTDescr(){

	}

	/**
	* Cancel a tween
	* 
	* @method cancel
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	*/
	public LTDescr cancel(){
		LeanTween.removeTween((int)this._id);
		return this;
	}

	public int uniqueId{
		get{ 
			uint toId = _id | counter << 16;

			/*uint backId = toId & 0xFFFF;
			uint backCounter = toId >> 16;
			if(_id!=backId || backCounter!=counter){
				Debug.LogError("BAD CONVERSION toId:"+_id);
			}*/

			return (int)toId;
		}
	}

	public int id{
		get{ 
			return uniqueId;
		}
	}

	public void reset(){
		this.toggle = true;
		#if !UNITY_METRO
		this.optional = null;
		#endif
		this.passed = this.delay = 0.0f;
		this.useEstimatedTime = this.useFrames = this.hasInitiliazed = this.onCompleteOnRepeat = this.destroyOnComplete = this.onCompleteOnStart = this.useManualTime = false;
		this.animationCurve = null;
		this.tweenType = LeanTweenType.linear;
		this.loopType = LeanTweenType.once;
		this.loopCount = 0;
		this.direction = this.lastVal = 1.0f;
		this.onUpdateFloat = null;
		this.onUpdateVector3 = null;
		this.onUpdateFloatObject = null;
		this.onUpdateVector3Object = null;
		this.onUpdateColor = null;
		this.onComplete = null;
		this.onCompleteObject = null;
		this.onCompleteParam = null;
		this.point = Vector3.zero;
		global_counter++;
	}

	/**
	* Pause a tween
	* 
	* @method pause
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	*/
	public LTDescr pause(){
		if(this.direction != 0.0f){ // check if tween is already paused
        	this.lastVal =  this.direction;
            this.direction = 0.0f;
        }

        return this;
	}

	/**
	* Resume a paused tween
	* 
	* @method resume
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	*/
	public LTDescr resume(){
		this.direction = this.lastVal;
		return this;
	}

	public LTDescr setAxis( Vector3 axis ){
		this.axis = axis;
		return this;
	}
	
	/**
	* Delay the start of a tween
	* 
	* @method setDelay
	* @param {float} float time The time to complete the tween in
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setDelay( 1.5f );
	*/
	public LTDescr setDelay( float delay ){
		if(this.useEstimatedTime){
			this.delay = delay;
		}else{
			this.delay = delay*Time.timeScale;
		}
		
		return this;
	}

	/**
	* Set the type of easing used for the tween. <br>
	* <ul><li><a href="LeanTweenType.html">List of all the ease types</a>.</li>
	* <li><a href="http://www.robertpenner.com/easing/easing_demo.html">This page helps visualize the different easing equations</a></li>
	* </ul>
	* 
	* @method setEase
	* @param {LeanTweenType} easeType:LeanTweenType the easing type to use
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeInBounce );
	*/
	public LTDescr setEase( LeanTweenType easeType ){
		this.tweenType = easeType;
		return this;
	}

	/**
	* Set the type of easing used for the tween with a custom curve. <br>
	* @method setEase (AnimationCurve)
	* @param {AnimationCurve} easeDefinition:AnimationCurve an <a href="http://docs.unity3d.com/Documentation/ScriptReference/AnimationCurve.html" target="_blank">AnimationCure</a> that describes the type of easing you want, this is great for when you want a unique type of movement
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeInBounce );
	*/
	public LTDescr setEase( AnimationCurve easeCurve ){
		this.animationCurve = easeCurve;
		return this;
	}

	public LTDescr setTo( Vector3 to ){
		this.to = to;
		return this;
	}

	public LTDescr setFrom( Vector3 from ){
		this.from = from;
		this.hasInitiliazed = true; // this is set, so that the "from" value isn't overwritten later on when the tween starts
		this.diff = this.to - this.from;
		return this;
	}

	public LTDescr setDiff( Vector3 diff ){
		this.diff = diff;
		return this;
	}

	public LTDescr setHasInitialized( bool has ){
		this.hasInitiliazed = has;
		return this;
	}

	public LTDescr setId( uint id ){
		this._id = id;
		this.counter = global_counter;
		return this;
	}

	/**
	* Set the tween to repeat a number of times.
	* @method setRepeat
	* @param {int} repeatNum:int the number of times to repeat the tween. -1 to repeat infinite times
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 10 ).setLoopPingPong();
	*/
	public LTDescr setRepeat( int repeat ){
		this.loopCount = repeat;
		if((repeat>1 && this.loopType == LeanTweenType.once) || (repeat < 0 && this.loopType == LeanTweenType.once)){
			this.loopType = LeanTweenType.clamp;
		}
		if(this.type==TweenAction.CALLBACK || this.type==TweenAction.CALLBACK_COLOR){
			this.setOnCompleteOnRepeat(true);
		}
		return this;
	}

	public LTDescr setLoopType( LeanTweenType loopType ){
		this.loopType = loopType;
		return this;
	}

	/**
	* Use estimated time when tweening an object. Great for pause screens, when you want all other action to be stopped (or slowed down)
	* @method setUseEstimatedTime
	* @param {bool} useEstimatedTime:bool whether to use estimated time or not
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 2 ).setUseEstimatedTime( true );
	*/
	public LTDescr setUseEstimatedTime( bool useEstimatedTime ){
		this.useEstimatedTime = useEstimatedTime;
		return this;
	}

	/**
	* Use frames when tweening an object, when you don't want the animation to be time-frame independent...
	* @method setUseFrames
	* @param {bool} useFrames:bool whether to use estimated time or not
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 2 ).setUseFrames( true );
	*/
	public LTDescr setUseFrames( bool useFrames ){
		this.useFrames = useFrames;
		return this;
	}

	public LTDescr setUseManualTime( bool useManualTime ){
		this.useManualTime = useManualTime;
		return this;
	}

	public LTDescr setLoopCount( int loopCount ){
		this.loopCount = loopCount;
		return this;
	}

	/**
	* No looping involved, just run once (the default)
	* @method setLoopOnce
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setLoopOnce();
	*/
	public LTDescr setLoopOnce(){ this.loopType = LeanTweenType.once; return this; }

	/**
	* When the animation gets to the end it starts back at where it began
	* @method setLoopClamp
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat(2).setLoopClamp();
	*/
	public LTDescr setLoopClamp(){ 
		this.loopType = LeanTweenType.clamp; 
		if(this.loopCount==0)
			this.loopCount = -1;
		return this;
	}

	/**
	* When the animation gets to the end it then tweens back to where it started (and on, and on)
	* @method setLoopPingPong
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat(2).setLoopPingPong();
	*/
	public LTDescr setLoopPingPong(){ 
		this.loopType = LeanTweenType.pingPong;
		if(this.loopCount==0)
			this.loopCount = -1;
		return this; 
	}

	/**
	* Have a method called when the tween finishes
	* @method setOnComplete
	* @param {Action} onComplete:Action the method that should be called when the tween is finished ex: tweenFinished(){ }
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished );
	*/
	public LTDescr setOnComplete( Action onComplete ){
		this.onComplete = onComplete;
		return this;
	}

	/**
	* Have a method called when the tween finishes
	* @method setOnComplete (object)
	* @param {Action<object>} onComplete:Action<object> the method that should be called when the tween is finished ex: tweenFinished( object myObj ){ }
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished );
	*/
	public LTDescr setOnComplete( Action<object> onComplete ){
		this.onCompleteObject = onComplete;
		return this;
	}
	public LTDescr setOnComplete( Action<object> onComplete, object onCompleteParam ){
		this.onCompleteObject = onComplete;
		if(onCompleteParam!=null)
			this.onCompleteParam = onCompleteParam;
		return this;
	}

	/**
	* Pass an object to along with the onComplete Function
	* @method setOnCompleteParam
	* @param {object} onComplete:object an object that 
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished );
	*/
	public LTDescr setOnCompleteParam( object onCompleteParam ){
		this.onCompleteParam = onCompleteParam;
		return this;
	}


	/**
	* Have a method called on each frame that the tween is being animated (passes a float value)
	* @method setOnUpdate
	* @param {Action<float>} onUpdate:Action<float> a method that will be called on every frame with the float value of the tweened object
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved );<br>
	* <br>
	* void tweenMoved( float val ){ }<br>
	*/
	public LTDescr setOnUpdate( Action<float> onUpdate ){
		this.onUpdateFloat = onUpdate;
		return this;
	}
	
	public LTDescr setOnUpdateObject( Action<float,object> onUpdate ){
		this.onUpdateFloatObject = onUpdate;
		return this;
	}
	public LTDescr setOnUpdateVector3( Action<Vector3> onUpdate ){
		this.onUpdateVector3 = onUpdate;
		return this;
	}
	public LTDescr setOnUpdateColor( Action<Color> onUpdate ){
		this.onUpdateColor = onUpdate;
		return this;
	}

	#if !UNITY_FLASH

	public LTDescr setOnUpdate( Action<Color> onUpdate ){
		this.onUpdateColor = onUpdate;
		return this;
	}

	/**
	* Have a method called on each frame that the tween is being animated (passes a float value and a object)
	* @method setOnUpdate (object)
	* @param {Action<float,object>} onUpdate:Action<float,object> a method that will be called on every frame with the float value of the tweened object, and an object of the person's choosing
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved ).setOnUpdateParam( myObject );<br>
	* <br>
	* void tweenMoved( float val, object obj ){ }<br>
	*/
	public LTDescr setOnUpdate( Action<float,object> onUpdate, object onUpdateParam = null ){
		this.onUpdateFloatObject = onUpdate;
		if(onUpdateParam!=null)
			this.onUpdateParam = onUpdateParam;
		return this;
	}

	public LTDescr setOnUpdate( Action<Vector3,object> onUpdate, object onUpdateParam = null ){
		this.onUpdateVector3Object = onUpdate;
		if(onUpdateParam!=null)
			this.onUpdateParam = onUpdateParam;
		return this;
	}

	/**
	* Have a method called on each frame that the tween is being animated (passes a float value)
	* @method setOnUpdate (Vector3)
	* @param {Action<Vector3>} onUpdate:Action<Vector3> a method that will be called on every frame with the float value of the tweened object
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved );<br>
	* <br>
	* void tweenMoved( Vector3 val ){ }<br>
	*/
	public LTDescr setOnUpdate( Action<Vector3> onUpdate, object onUpdateParam = null ){
		this.onUpdateVector3 = onUpdate;
		if(onUpdateParam!=null)
			this.onUpdateParam = onUpdateParam;
		return this;
	}
	#endif
	

	/**
	* Have an object passed along with the onUpdate method
	* @method setOnUpdateParam
	* @param {object} onUpdateParam:object an object that will be passed along with the onUpdate method
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved ).setOnUpdateParam( myObject );<br>
	* <br>
	* void tweenMoved( float val, object obj ){ }<br>
	*/
	public LTDescr setOnUpdateParam( object onUpdateParam ){
		this.onUpdateParam = onUpdateParam;
		return this;
	}

	/**
	* While tweening along a curve, set this property to true, to be perpendicalur to the path it is moving upon
	* @method setOrientToPath
	* @param {bool} doesOrient:bool whether the gameobject will orient to the path it is animating along
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.move( ltLogo, path, 1.0f ).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true).setAxis(Vector3.forward);<br>
	*/
	public LTDescr setOrientToPath( bool doesOrient ){
		if(this.type==TweenAction.MOVE_CURVED || this.type==TweenAction.MOVE_CURVED_LOCAL){
			if(this.path==null)
				this.path = new LTBezierPath();
			this.path.orientToPath = doesOrient;
		}else{
			this.spline.orientToPath = doesOrient;
		}
		return this;
	}

	/**
	* While tweening along a curve, set this property to true, to be perpendicalur to the path it is moving upon
	* @method setOrientToPath2d
	* @param {bool} doesOrient:bool whether the gameobject will orient to the path it is animating along
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.move( ltLogo, path, 1.0f ).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true).setAxis(Vector3.forward);<br>
	*/
	public LTDescr setOrientToPath2d( bool doesOrient2d ){
		setOrientToPath(doesOrient2d);
		if(this.type==TweenAction.MOVE_CURVED || this.type==TweenAction.MOVE_CURVED_LOCAL){
			this.path.orientToPath2d = doesOrient2d;
		}else{
			this.spline.orientToPath2d = doesOrient2d;
		}
		return this;
	}

	public LTDescr setRect( LTRect rect ){
		this.ltRect = rect;
		return this;
	}

	public LTDescr setRect( Rect rect ){
		this.ltRect = new LTRect(rect);
		return this;
	}

	public LTDescr setPath( LTBezierPath path ){
		this.path = path;
		return this;
	}

	/**
	* Set the point at which the GameObject will be rotated around
	* @method setPoint
	* @param {Vector3} point:Vector3 point at which you want the object to rotate around (local space)
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.rotateAround( cube, Vector3.up, 360.0f, 1.0f ) .setPoint( new Vector3(1f,0f,0f) ) .setEase( LeanTweenType.easeInOutBounce );<br>
	*/
	public LTDescr setPoint( Vector3 point ){
		this.point = point;
		return this;
	}

	public LTDescr setDestroyOnComplete( bool doesDestroy ){
		this.destroyOnComplete = doesDestroy;
		return this;
	}

	public LTDescr setAudio( object audio ){
		this.onCompleteParam = audio;
		return this;
	}
	
	/**
	* Set the onComplete method to be called at the end of every loop cycle (also applies to the delayedCall method)
	* @method setOnCompleteOnRepeat
	* @param {bool} isOn:bool does call onComplete on every loop cycle
	* @return {LTDescr} LTDescr an object that distinguishes the tween
	* @example
	* LeanTween.delayedCall(gameObject,0.3f, delayedMethod).setRepeat(4).setOnCompleteOnRepeat(true);
	*/
	public LTDescr setOnCompleteOnRepeat( bool isOn ){
		this.onCompleteOnRepeat = isOn;
		return this;
	}

	public LTDescr setOnCompleteOnStart( bool isOn ){
		this.onCompleteOnStart = isOn;
		return this;
	}

#if UNITY_4_6 || UNITY_5_0
	public LTDescr setRect( RectTransform rect ){
		this.rectTransform = rect;
		return this;
	}
#endif
}

/**
* LeanTween is an efficient tweening engine for Unity3d<br><br>
* <a href="#index">Index of All Methods</a> | <a href="LTDescr.html">Optional Paramaters that can be passed</a><br><br>
* <strong id='optional'>Optional Parameters</strong> are passed at the end of every method<br> 
* <br>
* <i>Example:</i><br>
* LeanTween.moveX( gameObject, 1f, 1f).setEase( <a href="LeanTweenType.html">LeanTweenType</a>.easeInQuad ).setDelay(1f);<br>
* <br>
* You can pass the optional parameters in any order, and chain on as many as you wish.<br>
* You can also pass parameters at a later time by saving a reference to what is returned.<br>
* <br>
* <i>Example:</i><br>
* <a href="LTDescr.html">LTDescr</a> d = LeanTween.moveX(gameObject, 1f, 1f);<br>
*  &nbsp; ...later set some parameters<br>
* d.setOnComplete( onCompleteFunc ).setEase( <a href="LeanTweenType.html">LeanTweenType</a>.easeInOutBack );<br>
*
* @class LeanTween
*/

public class LeanTween: MonoBehaviour {

public static bool throwErrors = true;

private static LTDescr[] tweens;
private static LTDescr tween;
private static int tweenMaxSearch = 0;
private static int maxTweens = 400;
private static int frameRendered= -1;
private static GameObject _tweenEmpty;
private static float dtEstimated;
public static float dtManual;
private static float previousRealTime;
private static float dt;
private static float dtActual;
private static int i;
private static int j;
private static AnimationCurve punch = new AnimationCurve( new Keyframe(0.0f, 0.0f ), new Keyframe(0.112586f, 0.9976035f ), new Keyframe(0.3120486f, -0.1720615f ), new Keyframe(0.4316337f, 0.07030682f ), new Keyframe(0.5524869f, -0.03141804f ), new Keyframe(0.6549395f, 0.003909959f ), new Keyframe(0.770987f, -0.009817753f ), new Keyframe(0.8838775f, 0.001939224f ), new Keyframe(1.0f, 0.0f ) );
private static AnimationCurve shake = new AnimationCurve( new Keyframe(0f, 0f), new Keyframe(0.25f, 1f), new Keyframe(0.75f, -1f), new Keyframe(1f, 0f) ) ;

public static void init(){
	init(maxTweens);
}

/**
* This line is optional. Here you can specify the maximum number of tweens you will use (the default is 400).  This must be called before any use of LeanTween is made for it to be effective.
* 
* @method LeanTween.init
* @param {integer} maxSimultaneousTweens:int The maximum number of tweens you will use, make sure you don't go over this limit, otherwise the code will throw an error
* @example
*   LeanTween.init( 800 );
*/
public static void init(int maxSimultaneousTweens){
	if(tweens==null){
		maxTweens = maxSimultaneousTweens;
		tweens = new LTDescr[maxTweens];
		_tweenEmpty = new GameObject();
		_tweenEmpty.name = "~LeanTween";
		_tweenEmpty.AddComponent(typeof(LeanTween));
		_tweenEmpty.isStatic = true;
		#if !UNITY_EDITOR
		_tweenEmpty.hideFlags = HideFlags.HideAndDontSave;
		#endif
		DontDestroyOnLoad( _tweenEmpty );
		for(int i = 0; i < maxTweens; i++){
			tweens[i] = new LTDescr();
		}
	}
}

public static void reset(){
	tweens = null;
}

public void Update(){
	LeanTween.update();
}

public void OnLevelWasLoaded( int lvl ){
	// Debug.Log("reseting gui");
	LTGUI.reset();
}

private static Transform trans;
private static float timeTotal;
private static TweenAction tweenAction;
private static float ratioPassed;
private static float from;
private static float to;
private static float val;
private static bool isTweenFinished;
private static int maxTweenReached;
private static Vector3 newVect;
private static GameObject target;
private static GameObject customTarget;

public static void update() {
	if(frameRendered != Time.frameCount){ // make sure update is only called once per frame
		init();

		dtEstimated = Time.realtimeSinceStartup - previousRealTime;
		if(dtEstimated>0.2f) // a catch put in, when at the start sometimes this number can grow unrealistically large
			dtEstimated = 0.2f;
		previousRealTime = Time.realtimeSinceStartup;
		dtActual = Time.deltaTime*Time.timeScale;
		maxTweenReached = 0;
		// if(tweenMaxSearch>1500)
		// 	Debug.Log("tweenMaxSearch:"+tweenMaxSearch +" maxTweens:"+maxTweens);
		for( int i = 0; i <= tweenMaxSearch && i < maxTweens; i++){
			
			//Debug.Log("tweens["+i+"].toggle:"+tweens[i].toggle);
			if(tweens[i].toggle){
				maxTweenReached = i;
				tween = tweens[i];
				trans = tween.trans;
				timeTotal = tween.time;
				tweenAction = tween.type;
				
				dt = dtActual;
				if( tween.useEstimatedTime ){
					dt = dtEstimated;
					timeTotal = tween.time;
				}else if( tween.useFrames ){
					dt = 1;
				}else if( tween.useManualTime ){
					dt = dtManual;
				}else if(tween.direction==0f){
					dt = 0f;
				}
				
				if(trans==null){
					removeTween(i);
					continue;
				}
				// Debug.Log("i:"+i+" tween:"+tween+" dt:"+dt);
				
				// Check for tween finished
				isTweenFinished = false;
				if(tween.delay<=0){
					if((tween.passed + dt > tween.time && tween.direction > 0.0f )){
						// Debug.Log("i:"+i+" passed:"+tween.passed+" dt:"+dt+" time:"+tween.time+" dir:"+tween.direction);
						isTweenFinished = true;
						tween.passed = tween.time; // Set to the exact end time so that it can finish tween exactly on the end value
					}else if(tween.direction<0.0f && tween.passed - dt < 0.0f){
						isTweenFinished = true;
						tween.passed = Mathf.Epsilon;
					}
				}

				if(!tween.hasInitiliazed && ((tween.passed==0.0 && tween.delay==0.0) || tween.passed>0.0) ){
					tween.hasInitiliazed = true;

					// Set time based on current timeScale
					if( !tween.useEstimatedTime ){
						tween.time = tween.time*Time.timeScale;
					}

					// Initialize From Values
					switch(tweenAction){
						case TweenAction.MOVE:
							tween.from = trans.position; break;
						case TweenAction.MOVE_X:
							tween.from.x = trans.position.x; break;
						case TweenAction.MOVE_Y:
							tween.from.x = trans.position.y; break;
						case TweenAction.MOVE_Z:
							tween.from.x = trans.position.z; break;
						case TweenAction.MOVE_LOCAL_X:
							tweens[i].from.x = trans.localPosition.x; break;
						case TweenAction.MOVE_LOCAL_Y:
							tweens[i].from.x = trans.localPosition.y; break;
						case TweenAction.MOVE_LOCAL_Z:
							tweens[i].from.x = trans.localPosition.z; break;
						case TweenAction.SCALE_X:
							tween.from.x = trans.localScale.x; break;
						case TweenAction.SCALE_Y:
							tween.from.x = trans.localScale.y; break;
						case TweenAction.SCALE_Z:
							tween.from.x = trans.localScale.z; break;
						case TweenAction.ALPHA:
							#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
								tween.from.x = trans.gameObject.renderer.material.color.a; 
								break;	
							#else
								SpriteRenderer ren = trans.gameObject.GetComponent<SpriteRenderer>();
								if(ren!=null){
									tween.from.x = ren.color.a;
								}else if(trans.gameObject.GetComponent<Renderer>()!=null){
									tween.from.x = trans.gameObject.GetComponent<Renderer>().material.color.a;
								}
								break;
							#endif
						case TweenAction.MOVE_LOCAL:
							tween.from = trans.localPosition; break;
						case TweenAction.MOVE_CURVED:
						case TweenAction.MOVE_CURVED_LOCAL:
						case TweenAction.MOVE_SPLINE:
						case TweenAction.MOVE_SPLINE_LOCAL:
							tween.from.x = 0; break;
						case TweenAction.ROTATE:
							tween.from = trans.eulerAngles; 
							tween.to = new Vector3(LeanTween.closestRot( tween.from.x, tween.to.x), LeanTween.closestRot( tween.from.y, tween.to.y), LeanTween.closestRot( tween.from.z, tween.to.z));
							break;
						case TweenAction.ROTATE_X:
							tween.from.x = trans.eulerAngles.x; 
							tween.to.x = LeanTween.closestRot( tween.from.x, tween.to.x);
							break;
						case TweenAction.ROTATE_Y:
							tween.from.x = trans.eulerAngles.y; 
							tween.to.x = LeanTween.closestRot( tween.from.x, tween.to.x);
							break;
						case TweenAction.ROTATE_Z:
							tween.from.x = trans.eulerAngles.z; 
							tween.to.x = LeanTween.closestRot( tween.from.x, tween.to.x);
							break;
						case TweenAction.ROTATE_AROUND:
							tween.lastVal = 0.0f; // optional["last"]
							tween.from.x = 0.0f;
							tween.origRotation = trans.rotation; // optional["origRotation"
							break;
						case TweenAction.ROTATE_AROUND_LOCAL:
							tween.lastVal = 0.0f; // optional["last"]
							tween.from.x = 0.0f;
							tween.origRotation = trans.localRotation; // optional["origRotation"
							break;
						case TweenAction.ROTATE_LOCAL:
							tween.from = trans.localEulerAngles; 
							tween.to = new Vector3(LeanTween.closestRot( tween.from.x, tween.to.x), LeanTween.closestRot( tween.from.y, tween.to.y), LeanTween.closestRot( tween.from.z, tween.to.z));
							break;
						case TweenAction.SCALE:
							tween.from = trans.localScale; break;
						case TweenAction.GUI_MOVE:
							tween.from = new Vector3(tween.ltRect.rect.x, tween.ltRect.rect.y, 0); break;
						case TweenAction.GUI_MOVE_MARGIN:
							tween.from = new Vector2(tween.ltRect.margin.x, tween.ltRect.margin.y); break;
						case TweenAction.GUI_SCALE:
							tween.from = new Vector3(tween.ltRect.rect.width, tween.ltRect.rect.height, 0); break;
						case TweenAction.GUI_ALPHA:
							tween.from.x = tween.ltRect.alpha; break;
						case TweenAction.GUI_ROTATE:
							if(tween.ltRect.rotateEnabled==false){
								tween.ltRect.rotateEnabled = true;
								tween.ltRect.resetForRotation();
							}
							
							tween.from.x = tween.ltRect.rotation; break;
						case TweenAction.ALPHA_VERTEX:
							tween.from.x = trans.GetComponent<MeshFilter>().mesh.colors32[0].a;
							break;
						case TweenAction.CALLBACK:
							if(tween.onCompleteOnStart){
								if(tween.onComplete!=null){
									tween.onComplete();
								}else if(tween.onCompleteObject!=null){
									tween.onCompleteObject(tween.onCompleteParam);
								}
							}
							break;
						case TweenAction.CALLBACK_COLOR:
							tween.diff = new Vector3(1.0f,0.0f,0.0f);
							break;
						case TweenAction.COLOR:
							#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
								if(trans.gameObject.renderer){
									Color col = trans.gameObject.renderer.material.color;
									tween.from = new Vector3(0.0f, col.a, 0.0f);
									tween.diff = new Vector3(1.0f,0.0f,0.0f);
									tween.axis = new Vector3( col.r, col.g, col.b );
								}
							#else
								SpriteRenderer ren2 = trans.gameObject.GetComponent<SpriteRenderer>();
								if(ren2!=null){
									tween.from = new Vector3(0.0f, ren2.color.a, 0.0f);
									tween.diff = new Vector3(1.0f,0.0f,0.0f);
									tween.axis = new Vector3( ren2.color.r, ren2.color.g, ren2.color.b );
								}else if(trans.gameObject.GetComponent<Renderer>()!=null){
									if(trans.gameObject.GetComponent<Renderer>()){
										Color col = trans.gameObject.GetComponent<Renderer>().material.color;
										tween.from = new Vector3(0.0f, col.a, 0.0f);
										tween.diff = new Vector3(1.0f,0.0f,0.0f);
										tween.axis = new Vector3( col.r, col.g, col.b );
									}
								}
							#endif
							break;
						#if UNITY_4_6 || UNITY_5_0
						case TweenAction.CANVAS_MOVE:
							tween.from = tween.rectTransform.anchoredPosition; break;
						case TweenAction.CANVAS_ROTATEAROUND:
							tween.from = tween.rectTransform.localEulerAngles; 
							tween.to = new Vector3(LeanTween.closestRot( tween.from.x, tween.to.x), LeanTween.closestRot( tween.from.y, tween.to.y), LeanTween.closestRot( tween.from.z, tween.to.z));
							tween.origRotation = tween.rectTransform.rotation; 
							break;
						case TweenAction.CANVAS_SCALE:
							tween.from = tween.rectTransform.localScale; break;
						#endif
					}
					if(tweenAction!=TweenAction.CALLBACK_COLOR && tweenAction!=TweenAction.COLOR)
						tween.diff = tween.to - tween.from;
				}
				if(tween.delay<=0){
					// Move Values
					if(timeTotal<=0f){
						//Debug.LogError("time total is zero Time.timeScale:"+Time.timeScale+" useEstimatedTime:"+tween.useEstimatedTime);
						ratioPassed = 0f;
					}else{
						ratioPassed = tween.passed / timeTotal;
					}

					if(ratioPassed>1.0f){
						ratioPassed = 1.0f;
					}else if(ratioPassed<0f){
						ratioPassed = 0f;
					}
					// Debug.Log("action:"+tweenAction+" ratioPassed:"+ratioPassed + " timeTotal:" + timeTotal + " tween.passed:"+ tween.passed +" dt:"+dt);
					
					if(tweenAction>=TweenAction.MOVE_X && tweenAction<TweenAction.MOVE){
						if(tween.animationCurve!=null){
							val = tweenOnCurve(tween, ratioPassed);
						}else {
							switch( tween.tweenType ){
								case LeanTweenType.linear:
									val = tween.from.x + tween.diff.x * ratioPassed; break;
								case LeanTweenType.easeOutQuad:
									val = easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed); break;
								case LeanTweenType.easeInQuad:
									val = easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed); break;
								case LeanTweenType.easeInOutQuad:
									val = easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed); break;
								case LeanTweenType.easeInCubic:
									val = easeInCubic(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeOutCubic:
									val = easeOutCubic(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInOutCubic:
									val = easeInOutCubic(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInQuart:
									val = easeInQuart(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeOutQuart:
									val = easeOutQuart(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInOutQuart:
									val = easeInOutQuart(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInQuint:
									val = easeInQuint(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeOutQuint:
									val = easeOutQuint(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInOutQuint:
									val = easeInOutQuint(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInSine:
									val = easeInSine(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeOutSine:
									val = easeOutSine(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInOutSine:
									val = easeInOutSine(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInExpo:
									val = easeInExpo(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeOutExpo:
									val = easeOutExpo(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInOutExpo:
									val = easeInOutExpo(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInCirc:
									val = easeInCirc(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeOutCirc:
									val = easeOutCirc(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInOutCirc:
									val = easeInOutCirc(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInBounce:
									val = easeInBounce(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeOutBounce:
									val = easeOutBounce(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInOutBounce:
									val = easeInOutBounce(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInBack:
									val = easeInBack(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeOutBack:
									val = easeOutBack(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInOutBack:
									val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInElastic:
									val = easeInElastic(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeOutElastic:
									val = easeOutElastic(tween.from.x, tween.to.x, ratioPassed); break;
								case LeanTweenType.easeInOutElastic:
									val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed); break;
                                case LeanTweenType.punch:
								case LeanTweenType.easeShake:
									if(tween.tweenType==LeanTweenType.punch){
										tween.animationCurve = LeanTween.punch;
									}else if(tween.tweenType==LeanTweenType.easeShake){
										tween.animationCurve = LeanTween.shake;
									}
									tween.to.x = tween.from.x + tween.to.x;
									tween.diff.x = tween.to.x - tween.from.x;
									val = tweenOnCurve(tween, ratioPassed); break;
								case LeanTweenType.easeSpring:
									val = spring(tween.from.x, tween.to.x, ratioPassed); break;
                                default:
                                    {
                                        val = tween.from.x + tween.diff.x * ratioPassed; break;
                                    }
							}
						
						}
						
						//Debug.Log("from:"+from+" to:"+to+" val:"+val+" ratioPassed:"+ratioPassed);
						if(tweenAction==TweenAction.MOVE_X){
							trans.position=new Vector3( val,trans.position.y,trans.position.z);
						}else if(tweenAction==TweenAction.MOVE_Y){
							trans.position =new Vector3( trans.position.x,val,trans.position.z);
						}else if(tweenAction==TweenAction.MOVE_Z){
							trans.position=new Vector3( trans.position.x,trans.position.y,val);
						}if(tweenAction==TweenAction.MOVE_LOCAL_X){
							trans.localPosition=new Vector3( val,trans.localPosition.y,trans.localPosition.z);
						}else if(tweenAction==TweenAction.MOVE_LOCAL_Y){
							trans.localPosition=new Vector3( trans.localPosition.x,val,trans.localPosition.z);
						}else if(tweenAction==TweenAction.MOVE_LOCAL_Z){
							trans.localPosition=new Vector3( trans.localPosition.x,trans.localPosition.y,val);
						}else if(tweenAction==TweenAction.MOVE_CURVED){
							if(tween.path.orientToPath){
								if(tween.path.orientToPath2d){
									tween.path.place2d( trans, val );
								}else{
									tween.path.place( trans, val );
								}
							}else{
								trans.position = tween.path.point( val );
							}
							// Debug.Log("val:"+val+" trans.position:"+trans.position + " 0:"+ tween.curves[0] +" 1:"+tween.curves[1] +" 2:"+tween.curves[2] +" 3:"+tween.curves[3]);
						}else if((TweenAction)tweenAction==TweenAction.MOVE_CURVED_LOCAL){
							if(tween.path.orientToPath){
								if(tween.path.orientToPath2d){
									tween.path.placeLocal2d( trans, val );
								}else{
									tween.path.placeLocal( trans, val );
								}
							}else{
								trans.localPosition = tween.path.point( val );
							}
							// Debug.Log("val:"+val+" trans.position:"+trans.position);
						}else if((TweenAction)tweenAction==TweenAction.MOVE_SPLINE){
							if(tween.spline.orientToPath){
								if(tween.spline.orientToPath2d){
									tween.spline.place2d( trans, val );
								}else{
									tween.spline.place( trans, val );
								}
							}else{
								trans.position = tween.spline.point( val );
							}
						}else if((TweenAction)tweenAction==TweenAction.MOVE_SPLINE_LOCAL){
							if(tween.spline.orientToPath){
								if(tween.spline.orientToPath2d){
									tween.spline.placeLocal2d( trans, val );
								}else{
									tween.spline.placeLocal( trans, val );
								}
							}else{
								trans.localPosition = tween.spline.point( val );
							}
						}else if(tweenAction==TweenAction.SCALE_X){
							trans.localScale=new Vector3(val, trans.localScale.y,trans.localScale.z);
						}else if(tweenAction==TweenAction.SCALE_Y){
							trans.localScale=new Vector3( trans.localScale.x,val,trans.localScale.z);
						}else if(tweenAction==TweenAction.SCALE_Z){
							trans.localScale=new Vector3(trans.localScale.x,trans.localScale.y,val);
						}else if(tweenAction==TweenAction.ROTATE_X){
					    	trans.eulerAngles=new Vector3(val, trans.eulerAngles.y,trans.eulerAngles.z);
					    }else if(tweenAction==TweenAction.ROTATE_Y){
					    	trans.eulerAngles=new Vector3(trans.eulerAngles.x,val,trans.eulerAngles.z);
					    }else if(tweenAction==TweenAction.ROTATE_Z){
					    	trans.eulerAngles=new Vector3(trans.eulerAngles.x,trans.eulerAngles.y,val);
					    }else if(tweenAction==TweenAction.ROTATE_AROUND){
							
							float move = val - tween.lastVal;
					    	// Debug.Log("move:"+move+" val:"+val + " timeTotal:"+timeTotal + " from:"+tween.from+ " diff:"+tween.diff + " type:"+tween.tweenType);
					    	if(isTweenFinished){
					    		if(tween.direction>0){
					    			// figure out how much the rotation has shifted the object over
					    			Vector3 origPos = trans.localPosition;
					    			trans.RotateAround((Vector3)trans.TransformPoint( tween.point ), tween.axis, -tween.to.x);
					    			Vector3 diff = origPos - trans.localPosition;
					    			
					    			trans.localPosition = origPos - diff; // Subtract the amount the object has been shifted over by the rotate, to get it back to it's orginal position
					    			trans.rotation = tween.origRotation;
						    		trans.RotateAround((Vector3)trans.TransformPoint( tween.point ), tween.axis, tween.to.x);
				    			}else{
				    				trans.rotation = tween.origRotation;
				    			}
					    	}else{
					    		/*trans.rotation = tween.origRotation;
					    		trans.RotateAround((Vector3)trans.TransformPoint( tween.point ), tween.axis, val);
								tween.lastVal = val;*/

								trans.RotateAround((Vector3)trans.TransformPoint( tween.point ), tween.axis, move);
								tween.lastVal = val;

								//trans.rotation =  * Quaternion.AngleAxis(val, tween.axis);
					    	}

					    }else if(tweenAction==TweenAction.ROTATE_AROUND_LOCAL){
							float move = val -  tween.lastVal;
					    	if(isTweenFinished){
					    		if(tween.direction>0){
					    			Vector3 origPos = trans.localPosition;
					    			trans.RotateAround((Vector3)trans.TransformPoint( tween.point ), trans.TransformDirection(tween.axis), -tween.to.x);
					    			Vector3 diff = origPos - trans.localPosition;
					    			
					    			trans.localPosition = origPos - diff; // Subtract the amount the object has been shifted over by the rotate, to get it back to it's orginal position
					    			trans.localRotation = tween.origRotation;
					    			trans.RotateAround((Vector3)trans.TransformPoint( tween.point ), trans.TransformDirection(tween.axis), tween.to.x);
				    			}else{
				    				trans.localRotation = tween.origRotation;
				    			}
					    	}else{
					    		trans.RotateAround((Vector3)trans.TransformPoint( tween.point ), trans.TransformDirection(tween.axis), move);
								tween.lastVal = val;
					    	}
					    }else if(tweenAction==TweenAction.ALPHA){
					    	#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2

							foreach(Material mat in trans.gameObject.renderer.materials){
        						mat.color = new Color( mat.color.r, mat.color.g, mat.color.b, val);
    						}

							#else

							SpriteRenderer ren = trans.gameObject.GetComponent<SpriteRenderer>();
							if(ren!=null){
								ren.color = new Color( ren.color.r, ren.color.g, ren.color.b, val);
							}else{
								if(trans.gameObject.GetComponent<Renderer>()!=null){
									foreach(Material mat in trans.gameObject.GetComponent<Renderer>().materials){
		        						mat.color = new Color( mat.color.r, mat.color.g, mat.color.b, val);
		    						}
		    					}
	    						if(trans.childCount>0){
	    							foreach (Transform child in trans) {
	    								if(child.gameObject.GetComponent<Renderer>()!=null){
		    								foreach(Material mat in child.gameObject.GetComponent<Renderer>().materials){
				        						mat.color = new Color( mat.color.r, mat.color.g, mat.color.b, val);
				    						}
				    					}
									}
	    						}
							}

    						#endif
						}else if(tweenAction==TweenAction.ALPHA_VERTEX){
							Mesh mesh = trans.GetComponent<MeshFilter>().mesh;
							Vector3[] vertices = mesh.vertices;
							Color32[] colors = new Color32[vertices.Length];
							Color32 c = mesh.colors32[0];
							c = new Color( c.r, c.g, c.b, val);
							for (int k= 0; k < vertices.Length; k++) {
								colors[k] = c;
							}
							mesh.colors32 = colors;
						}else if(tweenAction==TweenAction.COLOR || tweenAction==TweenAction.CALLBACK_COLOR){
							Color toColor = tweenColor(tween, val);
							// Debug.Log("val:"+val+" tween:"+tween+" tween.diff:"+tween.diff);
							if(tweenAction==TweenAction.COLOR){
								if(trans.gameObject.GetComponent<Renderer>()!=null){
									foreach(Material mat in trans.gameObject.GetComponent<Renderer>().materials){
		        						mat.color = toColor;
		    						}
		    					}
		    				}
		    				if(tween.onUpdateColor!=null){
								tween.onUpdateColor(toColor);
							}
						}
						#if UNITY_4_6 || UNITY_5_0
						else if(tweenAction==TweenAction.CANVAS_ROTATEAROUND){
							
							float move = val - tween.lastVal;
					    	// Debug.Log("move:"+move+" val:"+val + " timeTotal:"+timeTotal + " from:"+tween.from+ " diff:"+tween.diff + " type:"+tween.tweenType);
					    	RectTransform rect = tween.rectTransform;
					    	if(isTweenFinished){
					    		if(tween.direction>0){
					    			// figure out how much the rotation has shifted the object over
					    			Vector3 origPos = rect.localPosition;
					    			rect.RotateAround((Vector3)rect.TransformPoint( tween.point ), tween.axis, -tween.to.x);
					    			Vector3 diff = origPos - rect.localPosition;
					    			
					    			rect.localPosition = origPos - diff; // Subtract the amount the object has been shifted over by the rotate, to get it back to it's orginal position
					    			rect.rotation = tween.origRotation;
						    		rect.RotateAround((Vector3)rect.TransformPoint( tween.point ), tween.axis, tween.to.x);
				    			}else{
				    				rect.rotation = tween.origRotation;
				    			}
					    	}else{
					    		rect.RotateAround((Vector3)rect.TransformPoint( tween.point ), tween.axis, move);
								tween.lastVal = val;
					    	}

					    }
						#endif
						
					}else if(tweenAction>=TweenAction.MOVE){
						//
						
						if(tween.animationCurve!=null){
							newVect = tweenOnCurveVector(tween, ratioPassed);
						}else{
							if(tween.tweenType == LeanTweenType.linear){
								newVect = new Vector3( tween.from.x + tween.diff.x * ratioPassed, tween.from.y + tween.diff.y * ratioPassed, tween.from.z + tween.diff.z * ratioPassed);
							}else if(tween.tweenType >= LeanTweenType.linear){
								switch(tween.tweenType){
									case LeanTweenType.easeOutQuad:
										newVect = new Vector3(easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed)); break;
									case LeanTweenType.easeInQuad:
										newVect = new Vector3(easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeInQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeInQuadOpt(tween.from.z, tween.diff.z, ratioPassed)); break;
									case LeanTweenType.easeInOutQuad:
										newVect = new Vector3(easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeInOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeInOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed)); break;
									case LeanTweenType.easeInCubic:
										newVect = new Vector3(easeInCubic(tween.from.x, tween.to.x, ratioPassed), easeInCubic(tween.from.y, tween.to.y, ratioPassed), easeInCubic(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeOutCubic:
										newVect = new Vector3(easeOutCubic(tween.from.x, tween.to.x, ratioPassed), easeOutCubic(tween.from.y, tween.to.y, ratioPassed), easeOutCubic(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInOutCubic:
										newVect = new Vector3(easeInOutCubic(tween.from.x, tween.to.x, ratioPassed), easeInOutCubic(tween.from.y, tween.to.y, ratioPassed), easeInOutCubic(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInQuart:
										newVect = new Vector3(easeInQuart(tween.from.x, tween.to.x, ratioPassed), easeInQuart(tween.from.y, tween.to.y, ratioPassed), easeInQuart(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeOutQuart:
										newVect = new Vector3(easeOutQuart(tween.from.x, tween.to.x, ratioPassed), easeOutQuart(tween.from.y, tween.to.y, ratioPassed), easeOutQuart(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInOutQuart:
										newVect = new Vector3(easeInOutQuart(tween.from.x, tween.to.x, ratioPassed), easeInOutQuart(tween.from.y, tween.to.y, ratioPassed), easeInOutQuart(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInQuint:
										newVect = new Vector3(easeInQuint(tween.from.x, tween.to.x, ratioPassed), easeInQuint(tween.from.y, tween.to.y, ratioPassed), easeInQuint(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeOutQuint:
										newVect = new Vector3(easeOutQuint(tween.from.x, tween.to.x, ratioPassed), easeOutQuint(tween.from.y, tween.to.y, ratioPassed), easeOutQuint(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInOutQuint:
										newVect = new Vector3(easeInOutQuint(tween.from.x, tween.to.x, ratioPassed), easeInOutQuint(tween.from.y, tween.to.y, ratioPassed), easeInOutQuint(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInSine:
										newVect = new Vector3(easeInSine(tween.from.x, tween.to.x, ratioPassed), easeInSine(tween.from.y, tween.to.y, ratioPassed), easeInSine(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeOutSine:
										newVect = new Vector3(easeOutSine(tween.from.x, tween.to.x, ratioPassed), easeOutSine(tween.from.y, tween.to.y, ratioPassed), easeOutSine(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInOutSine:
										newVect = new Vector3(easeInOutSine(tween.from.x, tween.to.x, ratioPassed), easeInOutSine(tween.from.y, tween.to.y, ratioPassed), easeInOutSine(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInExpo:
										newVect = new Vector3(easeInExpo(tween.from.x, tween.to.x, ratioPassed), easeInExpo(tween.from.y, tween.to.y, ratioPassed), easeInExpo(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeOutExpo:
										newVect = new Vector3(easeOutExpo(tween.from.x, tween.to.x, ratioPassed), easeOutExpo(tween.from.y, tween.to.y, ratioPassed), easeOutExpo(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInOutExpo:
										newVect = new Vector3(easeInOutExpo(tween.from.x, tween.to.x, ratioPassed), easeInOutExpo(tween.from.y, tween.to.y, ratioPassed), easeInOutExpo(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInCirc:
										newVect = new Vector3(easeInCirc(tween.from.x, tween.to.x, ratioPassed), easeInCirc(tween.from.y, tween.to.y, ratioPassed), easeInCirc(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeOutCirc:
										newVect = new Vector3(easeOutCirc(tween.from.x, tween.to.x, ratioPassed), easeOutCirc(tween.from.y, tween.to.y, ratioPassed), easeOutCirc(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInOutCirc:
										newVect = new Vector3(easeInOutCirc(tween.from.x, tween.to.x, ratioPassed), easeInOutCirc(tween.from.y, tween.to.y, ratioPassed), easeInOutCirc(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInBounce:
										newVect = new Vector3(easeInBounce(tween.from.x, tween.to.x, ratioPassed), easeInBounce(tween.from.y, tween.to.y, ratioPassed), easeInBounce(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeOutBounce:
										newVect = new Vector3(easeOutBounce(tween.from.x, tween.to.x, ratioPassed), easeOutBounce(tween.from.y, tween.to.y, ratioPassed), easeOutBounce(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInOutBounce:
										newVect = new Vector3(easeInOutBounce(tween.from.x, tween.to.x, ratioPassed), easeInOutBounce(tween.from.y, tween.to.y, ratioPassed), easeInOutBounce(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInBack:
										newVect = new Vector3(easeInBack(tween.from.x, tween.to.x, ratioPassed), easeInBack(tween.from.y, tween.to.y, ratioPassed), easeInBack(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeOutBack:
										newVect = new Vector3(easeOutBack(tween.from.x, tween.to.x, ratioPassed), easeOutBack(tween.from.y, tween.to.y, ratioPassed), easeOutBack(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInOutBack:
										newVect = new Vector3(easeInOutBack(tween.from.x, tween.to.x, ratioPassed), easeInOutBack(tween.from.y, tween.to.y, ratioPassed), easeInOutBack(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInElastic:
										newVect = new Vector3(easeInElastic(tween.from.x, tween.to.x, ratioPassed), easeInElastic(tween.from.y, tween.to.y, ratioPassed), easeInElastic(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeOutElastic:
										newVect = new Vector3(easeOutElastic(tween.from.x, tween.to.x, ratioPassed), easeOutElastic(tween.from.y, tween.to.y, ratioPassed), easeOutElastic(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.easeInOutElastic:
										newVect = new Vector3(easeInOutElastic(tween.from.x, tween.to.x, ratioPassed), easeInOutElastic(tween.from.y, tween.to.y, ratioPassed), easeInOutElastic(tween.from.z, tween.to.z, ratioPassed)); break;
									case LeanTweenType.punch:
									case LeanTweenType.easeShake:
										if(tween.tweenType==LeanTweenType.punch){
											tween.animationCurve = LeanTween.punch;
										}else if(tween.tweenType==LeanTweenType.easeShake){
											tween.animationCurve = LeanTween.shake;
										}
										tween.to = tween.from + tween.to;
										tween.diff = tween.to - tween.from;
										if(tweenAction==TweenAction.ROTATE || tweenAction==TweenAction.ROTATE_LOCAL){
											tween.to = new Vector3(closestRot(tween.from.x, tween.to.x), closestRot(tween.from.y, tween.to.y), closestRot(tween.from.z, tween.to.z));
										}
										newVect = tweenOnCurveVector(tween, ratioPassed); break;
									case LeanTweenType.easeSpring:
										newVect = new Vector3(spring(tween.from.x, tween.to.x, ratioPassed), spring(tween.from.y, tween.to.y, ratioPassed), spring(tween.from.z, tween.to.z, ratioPassed)); break;
									
								}
							}else{
								newVect = new Vector3( tween.from.x + tween.diff.x * ratioPassed, tween.from.y + tween.diff.y * ratioPassed, tween.from.z + tween.diff.z * ratioPassed);
							}
						}
						 
						if(tweenAction==TweenAction.MOVE){
							trans.position = newVect;
					    }else if(tweenAction==TweenAction.MOVE_LOCAL){
							trans.localPosition = newVect;
					    }else if(tweenAction==TweenAction.ROTATE){
					    	/*if(tween.hasPhysics){
					    		trans.gameObject.rigidbody.MoveRotation(Quaternion.Euler( newVect ));
				    		}else{*/
				    			trans.eulerAngles = newVect;
				    		// }
					    }else if(tweenAction==TweenAction.ROTATE_LOCAL){
					    	trans.localEulerAngles = newVect;
					    }else if(tweenAction==TweenAction.SCALE){
					    	trans.localScale = newVect;
					    }else if(tweenAction==TweenAction.GUI_MOVE){
					    	tween.ltRect.rect = new Rect( newVect.x, newVect.y, tween.ltRect.rect.width, tween.ltRect.rect.height);
					    }else if(tweenAction==TweenAction.GUI_MOVE_MARGIN){
					    	tween.ltRect.margin = new Vector2(newVect.x, newVect.y);
					    }else if(tweenAction==TweenAction.GUI_SCALE){
					    	tween.ltRect.rect = new Rect( tween.ltRect.rect.x, tween.ltRect.rect.y, newVect.x, newVect.y);
					    }else if(tweenAction==TweenAction.GUI_ALPHA){
					    	tween.ltRect.alpha = newVect.x;
					    }else if(tweenAction==TweenAction.GUI_ROTATE){
					    	tween.ltRect.rotation = newVect.x;
					    }
					    #if UNITY_4_6 || UNITY_5_0
						else if(tweenAction==TweenAction.CANVAS_MOVE){
							tween.rectTransform.anchoredPosition = newVect;
						}else if(tweenAction==TweenAction.CANVAS_SCALE){
							tween.rectTransform.localScale = newVect;
						}
						#endif
					}
					//Debug.Log("tween.delay:"+tween.delay + " tween.passed:"+tween.passed + " tweenAction:"+tweenAction + " to:"+newVect+" axis:"+tween.axis);

					if(tween.onUpdateFloat!=null){
						tween.onUpdateFloat(val);
					}else if(tween.onUpdateFloatObject!=null){
						tween.onUpdateFloatObject(val, tween.onUpdateParam);
					}else if(tween.onUpdateVector3Object!=null){
						tween.onUpdateVector3Object(newVect, tween.onUpdateParam);
					}else if(tween.onUpdateVector3!=null){
						tween.onUpdateVector3(newVect);
					}
					#if !UNITY_METRO
					else if(tween.optional!=null){ // LeanTween 1.x legacy stuff
						
						var onUpdate = tween.optional["onUpdate"];
						if(onUpdate!=null){
							Hashtable updateParam = (Hashtable)tween.optional["onUpdateParam"];
							if((TweenAction)tweenAction==TweenAction.VALUE3){
								if(onUpdate.GetType() == typeof(string)){
									string onUpdateS = onUpdate as string;
									customTarget = tween.optional["onUpdateTarget"]!=null ? tween.optional["onUpdateTarget"] as GameObject : trans.gameObject;
									customTarget.BroadcastMessage( onUpdateS, newVect );
								}else if(onUpdate.GetType() == typeof(System.Action<Vector3, Hashtable>)){
									System.Action<Vector3, Hashtable> onUpdateA = (System.Action<Vector3, Hashtable>)onUpdate;
									onUpdateA(newVect, updateParam);
								}else{
									System.Action<Vector3> onUpdateA = (System.Action<Vector3>)onUpdate;
									onUpdateA(newVect);
								}
							}else{
								if(onUpdate.GetType() == typeof(string)){
									string onUpdateS = onUpdate as string;
									if (tween.optional["onUpdateTarget"]!=null){
										customTarget = tween.optional["onUpdateTarget"] as GameObject;
										customTarget.BroadcastMessage( onUpdateS, val );
									}else{
										trans.gameObject.BroadcastMessage( onUpdateS, val );
									}
								}else if(onUpdate.GetType() == typeof(System.Action<float, Hashtable>)){
									System.Action<float, Hashtable> onUpdateA = (System.Action<float, Hashtable>)onUpdate;
									onUpdateA(val, updateParam);
								}else if(onUpdate.GetType() == typeof(System.Action<Vector3>)){
									System.Action<Vector3> onUpdateA = (System.Action<Vector3>)onUpdate;
									onUpdateA( newVect );
								}else{
									System.Action<float> onUpdateA = (System.Action<float>)onUpdate;
									onUpdateA(val);
								}
							}
						}
					}
					#endif
				}
				
				if(isTweenFinished){
					// Debug.Log("finished tween:"+i+" tween:"+tween);
					if(tweenAction==TweenAction.GUI_ROTATE)
						tween.ltRect.rotateFinished = true;
					
					if(tween.loopType==LeanTweenType.once || tween.loopCount==1){
						if(tweenAction==TweenAction.DELAYED_SOUND){
							AudioSource.PlayClipAtPoint((AudioClip)tween.onCompleteParam, tween.to, tween.from.x);
						}
						if(tween.onComplete!=null){
							removeTween(i);
							tween.onComplete();

						}else if(tween.onCompleteObject!=null){
							removeTween(i);
							tween.onCompleteObject(tween.onCompleteParam);
						}

						#if !UNITY_METRO
						else if(tween.optional!=null){
							System.Action callback=null;
							System.Action<object> callbackWithParam = null;
							string callbackS=string.Empty;
							object callbackParam=null;
							if(tween.optional!=null && tween.trans){
								if(tween.optional["onComplete"]!=null){
									callbackParam = tween.optional["onCompleteParam"];
									if(tween.optional["onComplete"].GetType()==typeof(string)){
										callbackS = tween.optional["onComplete"] as string;
									}else{
										if(callbackParam!=null){
											callbackWithParam = (System.Action<object>)tween.optional["onComplete"];
										}else{
											callback = (System.Action)tween.optional["onComplete"];	
											if(callback==null)
												Debug.LogWarning("callback was not converted");
										}
									}
								}
							}
							removeTween(i);
							if(callbackWithParam!=null){
								callbackWithParam( callbackParam );
							}else if(callback!=null){
								callback();
							}else if(callbackS!=string.Empty){
								if (tween.optional["onCompleteTarget"]!=null){
									customTarget = tween.optional["onCompleteTarget"] as GameObject;
									if(callbackParam!=null) customTarget.BroadcastMessage ( callbackS, callbackParam );
									else customTarget.BroadcastMessage( callbackS );
								}else{
									if(callbackParam!=null) trans.gameObject.BroadcastMessage ( callbackS, callbackParam );
									else trans.gameObject.BroadcastMessage( callbackS );
								}
							}
						}
						#endif
						else{
							removeTween(i);
						}
					}else{
						if((tween.loopCount<0 && tween.type==TweenAction.CALLBACK) || tween.onCompleteOnRepeat){
							if(tweenAction==TweenAction.DELAYED_SOUND){
								AudioSource.PlayClipAtPoint((AudioClip)tween.onCompleteParam, tween.to, tween.from.x);
							}
							if(tween.onComplete!=null){
								tween.onComplete();
							}else if(tween.onCompleteObject!=null){
								tween.onCompleteObject(tween.onCompleteParam);
							}
						}
						if(tween.loopCount>=1){
							tween.loopCount--;
						}
						// Debug.Log("tween.loopType:"+tween.loopType+" tween.loopCount:"+tween.loopCount+" passed:"+tween.passed);
						if(tween.loopType==LeanTweenType.pingPong){
							tween.direction = 0.0f-(tween.direction);
						}else{
							tween.passed = Mathf.Epsilon;
						}
					}
				}else if(tween.delay<=0){
					tween.passed += dt*tween.direction;
				}else{
					tween.delay -= dt;
					// Debug.Log("dt:"+dt+" tween:"+i+" tween:"+tween);
					if(tween.delay<0){
						tween.passed = 0.0f;//-tween.delay
						tween.delay = 0.0f;
					}
				}
			}
		}
		tweenMaxSearch = maxTweenReached;
		frameRendered = Time.frameCount;
	}
}

private static Color tweenColor( LTDescr tween, float val ){
	Vector3 diff3 = tween.point - tween.axis;
	float diffAlpha = tween.to.y - tween.from.y;
	return new Color(tween.axis.x + diff3.x*val, tween.axis.y + diff3.y*val, tween.axis.z + diff3.z*val, tween.from.y + diffAlpha*val);
}

// This method is only used internally! Do not call this from your scripts. To cancel a tween use LeanTween.cancel
public static void removeTween( int i ){
	if(tweens[i].toggle){
		tweens[i].toggle = false;
		// Debug.Log("removed i:"+i);
		if(tweens[i].destroyOnComplete){
			//Debug.Log("destroying tween.type:"+tween.type);
			if(tweens[i].ltRect!=null){
			//	Debug.Log("destroy i:"+i+" id:"+tweens[i].ltRect.id);
				LTGUI.destroy( tweens[i].ltRect.id );
			}else{ // check if equal to tweenEmpty
				if(tweens[i].trans.gameObject!=_tweenEmpty){
					Destroy(tweens[i].trans.gameObject);
				}
			}
		}
		//tweens[i].optional = null;
		startSearch = i;
		//Debug.Log("start search reset:"+startSearch + " i:"+i+" tweenMaxSearch:"+tweenMaxSearch);
		if(i+1>=tweenMaxSearch){
			//Debug.Log("reset to zero");
			startSearch = 0;
			//tweenMaxSearch--;
		}
	}
}

public static Vector3[] add(Vector3[] a, Vector3 b){
	Vector3[] c = new Vector3[ a.Length ];
	for(i=0; i<a.Length; i++){
		c[i] = a[i] + b;
	}

	return c;
}

public static float closestRot( float from, float to ){
	float minusWhole = 0 - (360 - to);
	float plusWhole = 360 + to;
	float toDiffAbs = Mathf.Abs( to-from );
	float minusDiff = Mathf.Abs(minusWhole-from);
	float plusDiff = Mathf.Abs(plusWhole-from);
	if( toDiffAbs < minusDiff && toDiffAbs < plusDiff ){
		return to;
	}else {
		if(minusDiff < plusDiff){
			return minusWhole;
		}else{
			return plusWhole;
		}
	}
}

/**
* Cancel all tweens that are currently targeting the gameObject
* 
* @method LeanTween.cancel
* @param {GameObject} gameObject:GameObject gameObject whose tweens you wish to cancel
* @example LeanTween.move( gameObject, new Vector3(0f,1f,2f), 1f); <br>
* LeanTween.cancel( gameObject );
*/
public static void cancel( GameObject gameObject ){
	init();
	Transform trans = gameObject.transform;
	for(int i = 0; i <= tweenMaxSearch; i++){
		if(tweens[i].toggle && tweens[i].trans==trans){
			removeTween(i);
		}
	}
}

/**
* Cancel a specific tween with the provided id
* 
* @method LeanTween.cancel
* @param {GameObject} gameObject:GameObject gameObject whose tweens you want to cancel
* @param {float} id:int unique id that represents that tween
*/
public static void cancel( GameObject gameObject, int uniqueId ){
	if(uniqueId>=0){
		init();
		int backId = uniqueId & 0xFFFF;
		int backCounter = uniqueId >> 16;
		// Debug.Log("uniqueId:"+uniqueId+ " id:"+backId +" counter:"+backCounter + " setCounter:"+ tweens[backId].counter + " tweens[id].type:"+tweens[backId].type);
		if(tweens[backId].trans==null || (tweens[backId].trans.gameObject == gameObject && tweens[backId].counter==backCounter))
			removeTween((int)backId);
	}
}

/**
* Cancel a specific tween with the provided id
* 
* @method LeanTween.cancel
* @param {LTRect} ltRect:LTRect LTRect object whose tweens you want to cancel
* @param {float} id:int unique id that represents that tween
*/
public static void cancel( LTRect ltRect, int uniqueId ){
	if(uniqueId>=0){
		init();
		int backId = uniqueId & 0xFFFF;
		int backCounter = uniqueId >> 16;
		// Debug.Log("uniqueId:"+uniqueId+ " id:"+backId +" action:"+(TweenAction)backType + " tweens[id].type:"+tweens[backId].type);
		if(tweens[backId].ltRect == ltRect && tweens[backId].counter==backCounter)
			removeTween((int)backId);
	}
}

private static void cancel( int uniqueId ){
	if(uniqueId>=0){
		init();
		int backId = uniqueId & 0xFFFF;
		int backCounter = uniqueId >> 16;
		// Debug.Log("uniqueId:"+uniqueId+ " id:"+backId +" action:"+(TweenAction)backType + " tweens[id].type:"+tweens[backId].type);
		if(tweens[backId].hasInitiliazed && tweens[backId].counter==backCounter)
			removeTween((int)backId);
	}
}

// Deprecated
public static LTDescr description( int uniqueId ){
	int backId = uniqueId & 0xFFFF;
	int backCounter = uniqueId >> 16;

	if(tweens[backId]!=null && tweens[backId].uniqueId == uniqueId && tweens[backId].counter==backCounter)
		return tweens[backId];
	for(int i = 0; i <= tweenMaxSearch; i++){
		if(tweens[i].uniqueId == uniqueId && tweens[i].counter==backCounter)
			return tweens[i];
	}
	return null;
}

[System.Obsolete("Use 'pause( id )' instead")]
public static void pause( GameObject gameObject, int uniqueId ){
	pause( uniqueId );
}

public static void pause( int uniqueId ){
	int backId = uniqueId & 0xFFFF;
	int backCounter = uniqueId >> 16;
	if(tweens[backId].counter==backCounter){
		tweens[backId].pause();
	}
}

/**
* Pause all tweens for a GameObject
* 
* @method LeanTween.pause
* @param {GameObject} gameObject:GameObject GameObject whose tweens you want to pause
*/
public static void pause( GameObject gameObject ){
	Transform trans = gameObject.transform;
	for(int i = 0; i <= tweenMaxSearch; i++){
		if(tweens[i].trans==trans){
			tweens[i].pause();
		}
	}
}

[System.Obsolete("Use 'resume( id )' instead")]
public static void resume( GameObject gameObject, int uniqueId ){
	resume( uniqueId );
}

/**
* Resume a specific tween
* 
* @method LeanTween.resume
* @param {int} id:int Id of the tween you want to resume ex: int id = LeanTween.MoveX(gameObject, 5, 1.0).id;
*/
public static void resume( int uniqueId ){
	int backId = uniqueId & 0xFFFF;
	int backCounter = uniqueId >> 16;
	if(tweens[backId].counter==backCounter){
		tweens[backId].resume();
	}
}

/**
* Resume all the tweens on a GameObject
* 
* @method LeanTween.resume
* @param {GameObject} gameObject:GameObject GameObject whose tweens you want to resume
*/
public static void resume( GameObject gameObject ){
	Transform trans = gameObject.transform;
	for(int i = 0; i <= tweenMaxSearch; i++){
		if(tweens[i].trans==trans)
			tweens[i].resume();
	}
}

/**
* Test whether or not a tween is active on a GameObject
* 
* @method LeanTween.isTweening
* @param {GameObject} gameObject:GameObject GameObject that you want to test if it is tweening
*/
public static bool isTweening( GameObject gameObject ){
	Transform trans = gameObject.transform;
	for(int i = 0; i <= tweenMaxSearch; i++){
		if(tweens[i].toggle && tweens[i].trans==trans)
			return true;
	}
	return false;
}

/**
* Test whether or not a tween is active or not
* 
* @method LeanTween.isTweening
* @param {GameObject} id:int id of the tween that you want to test if it is tweening
* &nbsp;&nbsp;<i>Example:</i><br>
* &nbsp;&nbsp;int id = LeanTween.moveX(gameObject, 1f, 3f).id;<br>
* &nbsp;&nbsp;if(LeanTween.isTweening( id ))<br>
* &nbsp;&nbsp; &nbsp;&nbsp;Debug.Log("I am tweening!");<br>
*/
public static bool isTweening( int uniqueId ){
	int backId = uniqueId & 0xFFFF;
	int backCounter = uniqueId >> 16;
	if (backId < 0 || backId >= maxTweens) return false;
	// Debug.Log("tweens[backId].counter:"+tweens[backId].counter+" backCounter:"+backCounter +" toggle:"+tweens[backId].toggle);
	if(tweens[backId].counter==backCounter && tweens[backId].toggle){
		return true;
	}
	return false;
}

/**
* Test whether or not a tween is active on a LTRect
* 
* @method LeanTween.isTweening
* @param {LTRect} ltRect:LTRect LTRect that you want to test if it is tweening
*/
public static bool isTweening( LTRect ltRect ){
	for( int i = 0; i <= tweenMaxSearch; i++){
		if(tweens[i].toggle && tweens[i].ltRect==ltRect)
			return true;
	}
	return false;
}

public static void drawBezierPath(Vector3 a, Vector3 b, Vector3 c, Vector3 d){
    Vector3 last = a;
    Vector3 p;
    Vector3 aa = (-a + 3*(b-c) + d);
	Vector3 bb = 3*(a+c) - 6*b;
	Vector3 cc = 3*(b-a);
	float t;
    for(float k = 1.0f; k <= 30.0f; k++){
    	t = k / 30.0f;
    	p = ((aa* t + (bb))* t + cc)* t + a;
	    Gizmos.DrawLine(last, p);
	    last = p;
	}
}

public static object logError( string error ){
	if(throwErrors) Debug.LogError(error); else Debug.Log(error);
	return null;
}

// LeanTween 2.0 Methods

public static LTDescr options(LTDescr seed){ Debug.LogError("error this function is no longer used"); return null; }
public static LTDescr options(){
	init();
	
	for(j=0, i = startSearch; j < maxTweens; i++){
		if(i>=maxTweens-1)
			i = 0;
		if(tweens[i].toggle==false){
			if(i+1>tweenMaxSearch)
				tweenMaxSearch = i+1;
			startSearch = i + 1;
			break;
		}
		
		j++;
		if(j>=maxTweens)
			return logError("LeanTween - You have run out of available spaces for tweening. To avoid this error increase the number of spaces to available for tweening when you initialize the LeanTween class ex: LeanTween.init( "+(maxTweens*2)+" );") as LTDescr;
	}
	
	tweens[i].reset();
	tweens[i].setId( (uint)i );

	// Debug.Log("new tween with i:"+i+" counter:"+tweens[i].counter+" tweenMaxSearch:"+tweenMaxSearch);

	return tweens[i];
}

public static GameObject tweenEmpty{
	get{
		init(maxTweens);
		return _tweenEmpty;
	}
}

public static int startSearch = 0;
public static LTDescr descr;

private static LTDescr pushNewTween( GameObject gameObject, Vector3 to, float time, TweenAction tweenAction, LTDescr tween ){
	init(maxTweens);
	if(gameObject==null)
		return null;
	tween.trans = gameObject.transform;
	tween.to = to;
	tween.time = time;
	tween.type = tweenAction;
	//tween.hasPhysics = gameObject.rigidbody!=null;
	
	return tween;
}

/**
* Fade a gameobject's material to a certain alpha value. The material's shader needs to support alpha. <a href="http://owlchemylabs.com/content/">Owl labs has some excellent efficient shaders</a>.
* 
* @method LeanTween.alpha
* @param {GameObject} gameObject:GameObject Gameobject that you wish to fade
* @param {float} to:float the final alpha value (0-1)
* @param {float} time:float The time with which to fade the object
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* LeanTween.alpha(gameObject, 1f, 1f) .setDelay(1f);
*/
public static LTDescr alpha(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.ALPHA, options() );
}

/**
* Fade a GUI Object
* 
* @method LeanTween.alpha
* @param {LTRect} ltRect:LTRect LTRect that you wish to fade
* @param {float} to:float the final alpha value (0-1)
* @param {float} time:float The time with which to fade the object
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* LeanTween.alpha(ltRect, 1f, 1f) .setEase(LeanTweenType.easeInCirc);
*/	
public static LTDescr alpha(LTRect ltRect, float to, float time){
	ltRect.alphaEnabled = true;
	return pushNewTween( tweenEmpty, new Vector3(to,0f,0f), time, TweenAction.GUI_ALPHA, options().setRect( ltRect ) );
}

/**
* This works by tweening the vertex colors directly.<br>
<br>
Vertex-based coloring is useful because you avoid making a copy of your
object's material for each instance that needs a different color.<br>
<br>
A shader that supports vertex colors is required for it to work
(for example the shaders in Mobile/Particles/)
* 
* @method LeanTween.alphaVertex
* @param {GameObject} gameObject:GameObject Gameobject that you wish to alpha
* @param {float} to:float The alpha value you wish to tween to
* @param {float} time:float The time with which to delay before calling the function
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr alphaVertex(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0f,0f), time, TweenAction.ALPHA_VERTEX, options() );
}

/**
* Change a gameobject's material to a certain color value. The material's shader needs to support color tinting. <a href="http://owlchemylabs.com/content/">Owl labs has some excellent efficient shaders</a>.
* 
* @method LeanTween.color
* @param {GameObject} gameObject:GameObject Gameobject that you wish to change the color
* @param {Color} to:Color the final color value ex: Color.Red, new Color(1.0f,1.0f,0.0f,0.8f)
* @param {float} time:float The time with which to fade the object
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* LeanTween.color(gameObject, Color.yellow, 1f) .setDelay(1f);
*/
public static LTDescr color(GameObject gameObject, Color to, float time){
	return pushNewTween( gameObject, new Vector3(1.0f, to.a, 0.0f), time, TweenAction.COLOR, options().setPoint( new Vector3(to.r, to.g, to.b) ) );
}

public static LTDescr delayedCall( float delayTime, Action callback){
	return pushNewTween( tweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback) );
}

public static LTDescr delayedCall( float delayTime, Action<object> callback){
	return pushNewTween( tweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback) );
}

public static LTDescr delayedCall( GameObject gameObject, float delayTime, Action callback){
	return pushNewTween( gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback) );
}

public static LTDescr delayedCall( GameObject gameObject, float delayTime, Action<object> callback){
	return pushNewTween( gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback) );
}

public static LTDescr destroyAfter( LTRect rect, float delayTime){
	return pushNewTween( tweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setRect( rect ).setDestroyOnComplete(true) );
}

/*public static LTDescr delayedCall(GameObject gameObject, float delayTime, string callback){
	return pushNewTween( gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete( callback ) );
}*/

/**
* Move a GameObject to a certain location
* 
* @method LeanTween.move
* @param {GameObject} GameObject gameObject Gameobject that you wish to move
* @param {Vector3} vec:Vector3 to The final positin with which to move to
* @param {float} time:float time The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example LeanTween.move(gameObject, new Vector3(0f,-3f,5f), 2.0f) .setEase( LeanTweenType.easeOutQuad );
*/
public static LTDescr move(GameObject gameObject, Vector3 to, float time){
	return pushNewTween( gameObject, to, time, TweenAction.MOVE, options() );
}
public static LTDescr move(GameObject gameObject, Vector2 to, float time){
	return pushNewTween( gameObject, new Vector3(to.x, to.y, gameObject.transform.position.z), time, TweenAction.MOVE, options() );
}


/**
* Move a GameObject along a set of bezier curves
* 
* @method LeanTween.move
* @param {GameObject} gameObject:GameObject Gameobject that you wish to move
* @param {Vector3[]} path:Vector3[] A set of points that define the curve(s) ex: Point1,Handle2,Handle1,Point2,...
* @param {float} time:float The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* <i>Javascript:</i><br>
* LeanTween.move(gameObject, [Vector3(0,0,0),Vector3(1,0,0),Vector3(1,0,0),Vector3(1,0,1)], 2.0) .setEase(LeanTweenType.easeOutQuad).setOrientToPath(true);<br><br>
* <i>C#:</i><br>
* LeanTween.move(gameObject, new Vector3[]{new Vector3(0f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,1f)}, 1.5f).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true);;<br>
*/	
public static LTDescr move(GameObject gameObject, Vector3[] to, float time){
	descr = options();
	if(descr.path==null)
		descr.path = new LTBezierPath( to );
	else 
		descr.path.setPoints( to );

	return pushNewTween( gameObject, new Vector3(1.0f,0.0f,0.0f), time, TweenAction.MOVE_CURVED, descr );
}

/**
* Move a GameObject through a set of points
* 
* @method LeanTween.moveSpline
* @param {GameObject} gameObject:GameObject Gameobject that you wish to move
* @param {Vector3[]} path:Vector3[] A set of points that define the curve(s) ex: ControlStart,Pt1,Pt2,Pt3,.. ..ControlEnd
* @param {float} time:float The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* <i>Javascript:</i><br>
* LeanTween.moveSpline(gameObject, [Vector3(0,0,0),Vector3(1,0,0),Vector3(1,0,0),Vector3(1,0,1)], 2.0) .setEase(LeanTweenType.easeOutQuad).setOrientToPath(true);<br><br>
* <i>C#:</i><br>
* LeanTween.moveSpline(gameObject, new Vector3[]{new Vector3(0f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,1f)}, 1.5f).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true);<br>
*/
public static LTDescr moveSpline(GameObject gameObject, Vector3[] to, float time){
	descr = options();
	descr.spline = new LTSpline( to );

	return pushNewTween( gameObject, new Vector3(1.0f,0.0f,0.0f), time, TweenAction.MOVE_SPLINE, descr );
}

/**
* Move a GameObject through a set of points, in local space
* 
* @method LeanTween.moveSplineLocal
* @param {GameObject} gameObject:GameObject Gameobject that you wish to move
* @param {Vector3[]} path:Vector3[] A set of points that define the curve(s) ex: ControlStart,Pt1,Pt2,Pt3,.. ..ControlEnd
* @param {float} time:float The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* <i>Javascript:</i><br>
* LeanTween.moveSpline(gameObject, [Vector3(0,0,0),Vector3(1,0,0),Vector3(1,0,0),Vector3(1,0,1)], 2.0) .setEase(LeanTweenType.easeOutQuad).setOrientToPath(true);<br><br>
* <i>C#:</i><br>
* LeanTween.moveSpline(gameObject, new Vector3[]{new Vector3(0f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,1f)}, 1.5f).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true);<br>
*/
public static LTDescr moveSplineLocal(GameObject gameObject, Vector3[] to, float time){
	descr = options();
	descr.spline = new LTSpline( to );

	return pushNewTween( gameObject, new Vector3(1.0f,0.0f,0.0f), time, TweenAction.MOVE_SPLINE_LOCAL, descr );
}

/**
* Move a GUI Element to a certain location
* 
* @method LeanTween.move (GUI)
* @param {LTRect} ltRect:LTRect ltRect LTRect object that you wish to move
* @param {Vector2} vec:Vector2 to The final position with which to move to (pixel coordinates)
* @param {float} time:float time The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr move(LTRect ltRect, Vector2 to, float time){
	return pushNewTween( tweenEmpty, to, time, TweenAction.GUI_MOVE, options().setRect( ltRect ) );
}

public static LTDescr moveMargin(LTRect ltRect, Vector2 to, float time){
	return pushNewTween( tweenEmpty, to, time, TweenAction.GUI_MOVE_MARGIN, options().setRect( ltRect ) );
}

/**
* Move a GameObject along the x-axis
* 
* @method LeanTween.moveX
* @param {GameObject} gameObject:GameObject gameObject Gameobject that you wish to move
* @param {float} to:float to The final position with which to move to
* @param {float} time:float time The time to complete the move in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr moveX(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_X, options() );
}

/**
* Move a GameObject along the y-axis
* 
* @method LeanTween.moveY
* @param {GameObject} GameObject gameObject Gameobject that you wish to move
* @param {float} float to The final position with which to move to
* @param {float} float time The time to complete the move in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr moveY(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_Y, options() );
}

/**
* Move a GameObject along the z-axis
* 
* @method LeanTween.moveZ
* @param {GameObject} GameObject gameObject Gameobject that you wish to move
* @param {float} float to The final position with which to move to
* @param {float} float time The time to complete the move in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr moveZ(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_Z, options() );
}

/**
* Move a GameObject to a certain location relative to the parent transform. 
* 
* @method LeanTween.moveLocal
* @param {GameObject} GameObject gameObject Gameobject that you wish to rotate
* @param {Vector3} Vector3 to The final positin with which to move to
* @param {float} float time The time to complete the tween in
* @param {Hashtable} Hashtable optional Hashtable where you can pass <a href="#optional">optional items</a>.
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr moveLocal(GameObject gameObject, Vector3 to, float time){
	return pushNewTween( gameObject, to, time, TweenAction.MOVE_LOCAL, options() );
}

/**
* Move a GameObject along a set of bezier curves, in local space
* 
* @method LeanTween.moveLocal
* @param {GameObject} gameObject:GameObject Gameobject that you wish to move
* @param {Vector3[]} path:Vector3[] A set of points that define the curve(s) ex: Point1,Handle1,Handle2,Point2,...
* @param {float} time:float The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* <i>Javascript:</i><br>
* LeanTween.move(gameObject, [Vector3(0,0,0),Vector3(1,0,0),Vector3(1,0,0),Vector3(1,0,1)], 2.0).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true);<br><br>
* <i>C#:</i><br>
* LeanTween.move(gameObject, new Vector3[]{Vector3(0f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,0f),Vector3(1f,0f,1f)}).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true);<br>
*/
public static LTDescr moveLocal(GameObject gameObject, Vector3[] to, float time){
	descr = options();
	if(descr.path==null)
		descr.path = new LTBezierPath( to );
	else 
		descr.path.setPoints( to );

	return pushNewTween( gameObject, new Vector3(1.0f,0.0f,0.0f), time, TweenAction.MOVE_CURVED_LOCAL, descr );
}

public static LTDescr moveLocalX(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_LOCAL_X, options() );
}

public static LTDescr moveLocalY(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_LOCAL_Y, options() );
}

public static LTDescr moveLocalZ(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_LOCAL_Z, options() );
}

/**
* Rotate a GameObject, to values are in passed in degrees
* 
* @method LeanTween.rotate
* @param {GameObject} GameObject gameObject Gameobject that you wish to rotate
* @param {Vector3} Vector3 to The final rotation with which to rotate to
* @param {float} float time The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example LeanTween.rotate(cube, new Vector3(180f,30f,0f), 1.5f);
*/

public static LTDescr rotate(GameObject gameObject, Vector3 to, float time){
	return pushNewTween( gameObject, to, time, TweenAction.ROTATE, options() );
}

/**
* Rotate a GUI element (using an LTRect object), to a value that is in degrees
* 
* @method LeanTween.rotate
* @param {LTRect} ltRect:LTRect LTRect that you wish to rotate
* @param {float} to:float The final rotation with which to rotate to
* @param {float} time:float The time to complete the tween in
* @param {Array} optional:Array Object Array where you can pass <a href="#optional">optional items</a>.
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example 
* if(GUI.Button(buttonRect.rect, "Rotate"))<br>
*	LeanTween.rotate( buttonRect4, 150.0f, 1.0f).setEase(LeanTweenType.easeOutElastic);<br>
* GUI.matrix = Matrix4x4.identity;<br>
*/
public static LTDescr rotate(LTRect ltRect, float to, float time){
	return pushNewTween( tweenEmpty, new Vector3(to,0f,0f), time, TweenAction.GUI_ROTATE, options().setRect( ltRect ) );
}

/**
* Rotate a GameObject in the objects local space (on the transforms localEulerAngles object)
* 
* @method LeanTween.rotateLocal
* @param {GameObject} gameObject:GameObject Gameobject that you wish to rotate
* @param {Vector3} to:Vector3 The final rotation with which to rotate to
* @param {float} time:float The time to complete the rotation in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr rotateLocal(GameObject gameObject, Vector3 to, float time){
	return pushNewTween( gameObject, to, time, TweenAction.ROTATE_LOCAL, options() );
}

/**
* Rotate a GameObject only on the X axis
* 
* @method LeanTween.rotateX
* @param {GameObject} GameObject Gameobject that you wish to rotate
* @param {float} to:float The final x-axis rotation with which to rotate
* @param {float} time:float The time to complete the rotation in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr rotateX(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.ROTATE_X, options() );
}

/**
* Rotate a GameObject only on the Y axis
* 
* @method LeanTween.rotateY
* @param {GameObject} GameObject Gameobject that you wish to rotate
* @param {float} to:float The final y-axis rotation with which to rotate
* @param {float} time:float The time to complete the rotation in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr rotateY(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.ROTATE_Y, options() );
}

/**
* Rotate a GameObject only on the Z axis
* 
* @method LeanTween.rotateZ
* @param {GameObject} GameObject Gameobject that you wish to rotate
* @param {float} to:float The final z-axis rotation with which to rotate
* @param {float} time:float The time to complete the rotation in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr rotateZ(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.ROTATE_Z, options() );
}

/**
* Rotate a GameObject around a certain Axis (the best method to use when you want to rotate beyond 180 degrees)
* 
* @method LeanTween.rotateAround
* @param {GameObject} gameObject:GameObject Gameobject that you wish to rotate
* @param {Vector3} vec:Vector3 axis in which to rotate around ex: Vector3.up
* @param {float} degrees:float the degrees in which to rotate
* @param {float} time:float time The time to complete the rotation in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* <i>Example:</i><br>
* LeanTween.rotateAround ( gameObject, Vector3.left, 90f,  1f );
*/
public static LTDescr rotateAround(GameObject gameObject, Vector3 axis, float add, float time){
	return pushNewTween( gameObject, new Vector3(add,0f,0f), time, TweenAction.ROTATE_AROUND, options().setAxis(axis) );
}

/**
* Rotate a GameObject around a certain Axis in Local Space (the best method to use when you want to rotate beyond 180 degrees)
* 
* @method LeanTween.rotateAroundLocal
* @param {GameObject} gameObject:GameObject Gameobject that you wish to rotate
* @param {Vector3} vec:Vector3 axis in which to rotate around ex: Vector3.up
* @param {float} degrees:float the degrees in which to rotate
* @param {float} time:float time The time to complete the rotation in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* <i>Example:</i><br>
* LeanTween.rotateAround ( gameObject, Vector3.left, 90f,  1f );
*/
public static LTDescr rotateAroundLocal(GameObject gameObject, Vector3 axis, float add, float time){
	return pushNewTween( gameObject, new Vector3(add,0f,0f), time, TweenAction.ROTATE_AROUND_LOCAL, options().setAxis(axis) );
}

/**
* Scale a GameObject to a certain size
* 
* @method LeanTween.scale
* @param {GameObject} gameObject:GameObject gameObject Gameobject that you wish to scale
* @param {Vector3} vec:Vector3 to The size with which to tween to
* @param {float} time:float time The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr scale(GameObject gameObject, Vector3 to, float time){
	return pushNewTween( gameObject, to, time, TweenAction.SCALE, options() );
}
	
/**
* Scale a GUI Element to a certain width and height
* 
* @method LeanTween.scale (GUI)
* @param {LTRect} LTRect ltRect LTRect object that you wish to move
* @param {Vector2} Vector2 to The final width and height to scale to (pixel based)
* @param {float} float time The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* <i>Example Javascript: </i><br>
* var bRect:LTRect = new LTRect( 0, 0, 100, 50 );<br>
* LeanTween.scale( bRect, Vector2(bRect.rect.width, bRect.rect.height) * 1.3, 0.25 ).setEase(LeanTweenType.easeOutBounce);<br>
* function OnGUI(){<br>
* &nbsp; if(GUI.Button(bRect.rect, "Scale")){ }<br>
* }<br>
* <br>
* <i>Example C#: </i> <br>
* LTRect bRect = new LTRect( 0f, 0f, 100f, 50f );<br>
* LeanTween.scale( bRect, new Vector2(150f,75f), 0.25f ).setEase(LeanTweenType.easeOutBounce);<br>
* void OnGUI(){<br>
* &nbsp; if(GUI.Button(bRect.rect, "Scale")){ }<br>
* }<br>
*/
public static LTDescr scale(LTRect ltRect, Vector2 to, float time){
	return pushNewTween( tweenEmpty, to, time, TweenAction.GUI_SCALE, options().setRect( ltRect ) );
}

/**
* Scale a GameObject to a certain size along the x-axis only
* 
* @method LeanTween.scaleX
* @param {GameObject} gameObject:GameObject Gameobject that you wish to scale
* @param {float} scaleTo:float the size with which to scale to
* @param {float} time:float the time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr scaleX(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.SCALE_X, options() );
}

/**
* Scale a GameObject to a certain size along the y-axis only
* 
* @method LeanTween.scaleY
* @param {GameObject} gameObject:GameObject Gameobject that you wish to scale
* @param {float} scaleTo:float the size with which to scale to
* @param {float} time:float the time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr scaleY(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.SCALE_Y, options() );
}

/**
* Scale a GameObject to a certain size along the z-axis only
* 
* @method LeanTween.scaleZ
* @param {GameObject} gameObject:GameObject Gameobject that you wish to scale
* @param {float} scaleTo:float the size with which to scale to
* @param {float} time:float the time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr scaleZ(GameObject gameObject, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.SCALE_Z, options());
}

/**
* Tween any particular value, it does not need to be tied to any particular type or GameObject
* 
* @method LeanTween.value (float)
* @param {GameObject} GameObject gameObject GameObject with which to tie the tweening with. This is only used when you need to cancel this tween, it does not actually perform any operations on this gameObject
* @param {Action<float>} callOnUpdate:Action<float> The function that is called on every Update frame, this function needs to accept a float value ex: function updateValue( float val ){ }
* @param {float} float from The original value to start the tween from
* @param {float} float to The value to end the tween on
* @param {float} float time The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* <i>Example Javascript: </i><br>
* LeanTween.value( gameObject, updateValueExampleCallback, 180f, 270f, 1f).setEase(LeanTweenType.easeOutElastic);<br>
* function updateValueExampleCallback( val:float ){<br>
* &nbsp; Debug.Log("tweened value:"+val+" set this to whatever variable you are tweening...");<br>
* }<br>
* <br>
* <i>Example C#: </i> <br>
* LeanTween.value( gameObject, updateValueExampleCallback, 180f, 270f, 1f).setEase(LeanTweenType.easeOutElastic);<br>
* void updateValueExampleCallback( float val ){<br>
* &nbsp; Debug.Log("tweened value:"+val+" set this to whatever variable you are tweening...");<br>
* }<br>
*/

public static LTDescr value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.CALLBACK, options().setTo( new Vector3(to,0,0) ).setFrom( new Vector3(from,0,0) ).setOnUpdate(callOnUpdate) );
}

/**
* Tween from one color to another
* 
* @method LeanTween.value (Color)
* @param {GameObject} GameObject gameObject GameObject with which to tie the tweening with. This is only used when you need to cancel this tween, it does not actually perform any operations on this gameObject
* @param {Action<Color>} callOnUpdate:Action<Color> The function that is called on every Update frame, this function needs to accept a color value ex: function updateValue( Color val ){ }
* @param {Color} Color from The original value to start the tween from
* @param {Color} Color to The value to end the tween on
* @param {Color} Color time The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
* @example
* <i>Example Javascript: </i><br>
* LeanTween.value( gameObject, updateValueExampleCallback, Color.red, Color.green, 1f).setEase(LeanTweenType.easeOutElastic);<br>
* function updateValueExampleCallback( val:Color ){<br>
* &nbsp; Debug.Log("tweened color:"+val+" set this to whatever variable you are tweening...");<br>
* }<br>
* <br>
* <i>Example C#: </i> <br>
* LeanTween.value( gameObject, updateValueExampleCallback, Color.red, Color.green, 1f).setEase(LeanTweenType.easeOutElastic);<br>
* void updateValueExampleCallback( Color val ){<br>
* &nbsp; Debug.Log("tweened color:"+val+" set this to whatever variable you are tweening...");<br>
* }<br>
*/

public static LTDescr value(GameObject gameObject, Action<Color> callOnUpdate, Color from, Color to, float time){
	return pushNewTween( gameObject, new Vector3(1.0f,to.a,0.0f), time, TweenAction.CALLBACK_COLOR, options().setPoint( new Vector3(to.r, to.g, to.b) )
		.setAxis( new Vector3(from.r, from.g, from.b) ).setFrom( new Vector3(0.0f, from.a, 0.0f) ).setHasInitialized(false).setOnUpdateColor(callOnUpdate) );
}

/**
* Tween any particular value (Vector3), this could be used to tween an arbitrary value like a material color
* 
* @method LeanTween.value (Vector3)
* @param {GameObject} gameObject:GameObject Gameobject that you wish to attach the tween to
* @param {Action<Vector3>} callOnUpdate:Action<Vector3> The function that is called on every Update frame, this function needs to accept a float value ex: function updateValue( Vector3 val ){ }
* @param {float} from:Vector3 The original value to start the tween from
* @param {Vector3} to:Vector3 The final Vector3 with which to tween to
* @param {float} time:float The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time){
	return pushNewTween( gameObject, to, time, TweenAction.VALUE3, options().setTo( to ).setFrom( from ).setOnUpdateVector3(callOnUpdate) );
}

/**
* Tween any particular value (float)
* 
* @method LeanTween.value (float,object)
* @param {GameObject} gameObject:GameObject Gameobject that you wish to attach the tween to
* @param {Action<float,object>} callOnUpdate:Action<float,object> The function that is called on every Update frame, this function needs to accept a float value ex: function updateValue( Vector3 val, object obj ){ }
* @param {float} from:Vector3 The original value to start the tween from
* @param {Vector3} to:Vector3 The final Vector3 with which to tween to
* @param {float} time:float The time to complete the tween in
* @return {LTDescr} LTDescr an object that distinguishes the tween
*/
public static LTDescr value(GameObject gameObject, Action<float,object> callOnUpdate, float from, float to, float time){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.CALLBACK, options().setTo( new Vector3(to,0,0) ).setFrom( new Vector3(from,0,0) ).setOnUpdateObject(callOnUpdate) );
}

public static LTDescr value(GameObject gameObject, float from, float to, float time){
	return pushNewTween( gameObject, Vector3.zero, time, TweenAction.CALLBACK, options().setTo( new Vector3(to,0,0) ).setFrom( new Vector3(from,0,0) ) );
}

public static LTDescr value(GameObject gameObject, Vector2 from, Vector2 to, float time){
	return pushNewTween( gameObject, Vector3.zero, time, TweenAction.VALUE3, options().setTo( new Vector3(to.x,to.y,0) ).setFrom( new Vector3(from.x,from.y,0) ) );
}

public static LTDescr value(GameObject gameObject, Vector3 from, Vector3 to, float time){
	return pushNewTween( gameObject, Vector3.zero, time, TweenAction.VALUE3, options().setTo( to ).setFrom( from ) );
}

public static LTDescr value(GameObject gameObject, Color from, Color to, float time){
	return pushNewTween( gameObject, new Vector3(1f, to.a, 0f), time, TweenAction.CALLBACK_COLOR, options().setPoint( new Vector3(to.r, to.g, to.b) )
		.setFrom( new Vector3(0f, from.a, 0f) ) 
		.setDiff( new Vector3(1f, 0f, 0f) )
		.setAxis( new Vector3( from.r, from.g, from.b ) )
	);
}

public static LTDescr delayedSound( AudioClip audio, Vector3 pos, float volume ){
	return pushNewTween( tweenEmpty, pos, 0f, TweenAction.DELAYED_SOUND, options().setTo( pos ).setFrom( new Vector3(volume,0,0) ).setAudio( audio ) );
}

public static LTDescr delayedSound( GameObject gameObject, AudioClip audio, Vector3 pos, float volume ){
	return pushNewTween( gameObject, pos, 0f, TweenAction.DELAYED_SOUND, options().setTo( pos ).setFrom( new Vector3(volume,0,0) ).setAudio( audio ) );
}

#if UNITY_4_6 || UNITY_5_0

public static LTDescr move(RectTransform rectTrans, Vector3 to, float time){
	return pushNewTween( rectTrans.gameObject, to, time, TweenAction.CANVAS_MOVE, options().setRect( rectTrans ) );
}

public static LTDescr rotateAround(RectTransform rectTrans, Vector3 axis, float to, float time){
	return pushNewTween( rectTrans.gameObject, new Vector3(to,0f,0f), time, TweenAction.CANVAS_ROTATEAROUND, options().setRect( rectTrans ).setAxis(axis) );
}

public static LTDescr scale(RectTransform rectTrans, Vector3 to, float time){
	return pushNewTween( rectTrans.gameObject, to, time, TweenAction.CANVAS_SCALE, options().setRect( rectTrans ) );
}

#endif

#if !UNITY_METRO
// LeanTween 1.x Methods

public static Hashtable h( object[] arr ){
	if(arr.Length%2==1){
		logError("LeanTween - You have attempted to create a Hashtable with an odd number of values.");
		return null;
	}
	Hashtable hash = new Hashtable();
	for(i = 0; i < arr.Length; i += 2){
		hash.Add(arr[i] as string, arr[i+1]);
	}
	
	return hash;
}

private static int idFromUnique( int uniqueId ){
	return uniqueId & 0xFFFF;
}

private static int pushNewTween( GameObject gameObject, Vector3 to, float time, TweenAction tweenAction, Hashtable optional ){
	init(maxTweens);
	if(gameObject==null)
		return -1;
	
	j = 0;
	for(i = startSearch; j < maxTweens; i++){
		if(i>=maxTweens-1)
			i = 0;
		if(tweens[i].toggle==false){
			if(i+1>tweenMaxSearch)
				tweenMaxSearch = i+1;
			startSearch = i + 1;
			break;
		}
		
		j++;
		if(j>=maxTweens){
			logError("LeanTween - You have run out of available spaces for tweening. To avoid this error increase the number of spaces to available for tweening when you initialize the LeanTween class ex: LeanTween.init( "+(maxTweens*2)+" );");
			return -1;
		}
	}
	LTDescr tween = tweens[i];
	tween.toggle = true;
	tween.reset();
	tween.trans = gameObject.transform;
	tween.to = to;
	tween.time = time;
	tween.type = tweenAction;
	tween.optional = optional;
	tween.setId( (uint)i );
	//tween.hasPhysics = gameObject.rigidbody!=null;

	if(optional!=null){
        var ease = optional["ease"];
        //LeanTweenType ease;
		var optionsNotUsed = 0;
		if(ease!=null) {
            tween.tweenType = LeanTweenType.linear;
			if( ease.GetType() ==typeof( LeanTweenType) ){
                tween.tweenType = (LeanTweenType)ease;// Enum.Parse(typeof(LeanTweenType), optional["ease"].ToString());
			} else if(ease.GetType() == typeof(AnimationCurve)){
				tween.animationCurve = optional["ease"] as AnimationCurve;
			} else{
				string func = optional["ease"].ToString();
				if(func.Equals("easeOutQuad")){
					tween.tweenType = LeanTweenType.easeOutQuad;
				}else if(func.Equals("easeInQuad")){
					tween.tweenType = LeanTweenType.easeInQuad;
				}else if(func.Equals("easeInOutQuad")){
					tween.tweenType = LeanTweenType.easeInOutQuad;
				}
			}
			optionsNotUsed++;
		}
		if(optional["rect"]!=null){
			tween.ltRect = (LTRect)optional["rect"];
			optionsNotUsed++;
		}
		if(optional["path"]!=null){
			tween.path = (LTBezierPath)optional["path"];
			optionsNotUsed++;
		}
		if(optional["delay"]!=null){
			tween.delay = (float)optional["delay"];
			optionsNotUsed++;
		}
		if(optional["useEstimatedTime"]!=null){
			tween.useEstimatedTime =(bool) optional["useEstimatedTime"];
			optionsNotUsed++;
		}
		if(optional["useFrames"]!=null){
			tween.useFrames =(bool) optional["useFrames"];
			optionsNotUsed++;
		}
		if(optional["loopType"]!=null){
			tween.loopType = (LeanTweenType)optional["loopType"];
			optionsNotUsed++;
		}
		if(optional["repeat"]!=null){
			tween.loopCount = (int)optional["repeat"];
			if(tween.loopType==LeanTweenType.once)
				tween.loopType = LeanTweenType.clamp;
			optionsNotUsed++;
		}
		if(optional["point"]!=null){
			tween.point = (Vector3)optional["point"];
			optionsNotUsed++;
		}
		if(optional["axis"]!=null){
			tween.axis = (Vector3)optional["axis"];
			optionsNotUsed++;
		}
		if(optional.Count <= optionsNotUsed)
			tween.optional = null;  // nothing else is used with the extra piece, so set to null
	}else{
		tween.optional = null;
	}
	//Debug.Log("pushing new tween["+i+"]:"+tweens[i]);
	
	return tweens[i].uniqueId;
}

public static int value(string callOnUpdate, float from, float to, float time, Hashtable optional){
	return value( tweenEmpty, callOnUpdate, from, to, time, optional );
}

public static int value(GameObject gameObject, string callOnUpdate, float from, float to, float time){
	return value(gameObject, callOnUpdate, from, to, time, new Hashtable()); 
}
public static int value(GameObject gameObject, string callOnUpdate, float from, float to, float time, object[] optional){
	return value(gameObject, callOnUpdate, from, to, time, h(optional)); 
}
	
public static int value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time, object[] optional){
	return value(gameObject, callOnUpdate, from, to, time, h(optional)); 
}
public static int value(GameObject gameObject, Action<float,Hashtable> callOnUpdate, float from, float to, float time, object[] optional){
	return value(gameObject, callOnUpdate, from, to, time, h(optional)); 
}

public static int value(GameObject gameObject,string callOnUpdate, float from, float to, float time, Hashtable optional){
	if(optional==null || optional.Count == 0)
		optional = new Hashtable();
		
	optional["onUpdate"] = callOnUpdate;
	int id = idFromUnique( pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.CALLBACK, optional ) );
	tweens[id].from = new Vector3(from,0,0);
	return id;
}

public static int value(GameObject gameObject,Action<float> callOnUpdate, float from, float to, float time, Hashtable optional){
	if(optional==null || optional.Count == 0)
		optional = new Hashtable();
		
	optional["onUpdate"] = callOnUpdate;
	int id = idFromUnique( pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.CALLBACK, optional ) );
	tweens[id].from = new Vector3(from,0,0);
	return id;
}

public static int value(GameObject gameObject,Action<float,Hashtable> callOnUpdate, float from, float to, float time, Hashtable optional){
	if(optional==null || optional.Count == 0)
		optional = new Hashtable();
		
	optional["onUpdate"] = callOnUpdate;
	int id = idFromUnique( pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.CALLBACK, optional ) );
	tweens[id].from = new Vector3(from,0,0);
	return id;
}

public static int value(GameObject gameObject, String callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional){
	if(optional==null || optional.Count==0)
		optional = new Hashtable();
		
	optional["onUpdate"] = callOnUpdate;
	int id = idFromUnique( pushNewTween( gameObject, to, time, TweenAction.VALUE3, optional ) );
	tweens[id].from = from;
	return id;
}
public static int value(GameObject gameObject, String callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional){
	return value(gameObject, callOnUpdate, from, to, time, h(optional)); 
}

public static int value(GameObject gameObject, System.Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional){
	if(optional==null || optional.Count==0)
		optional = new Hashtable();
		
	optional["onUpdate"] = callOnUpdate;
	int id = idFromUnique( pushNewTween( gameObject, to, time, TweenAction.VALUE3, optional ) );
	tweens[id].from = from;
	return id;
}
public static int value(GameObject gameObject, System.Action<Vector3,Hashtable> callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional){
	if(optional==null || optional.Count==0)
		optional = new Hashtable();
		
	optional["onUpdate"] = callOnUpdate;
	int id = idFromUnique( pushNewTween( gameObject, to, time, TweenAction.VALUE3, optional ) );
	tweens[id].from = from;
	return id;
}
public static int value(GameObject gameObject, System.Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional){
	return value(gameObject, callOnUpdate, from, to, time, h(optional)); 
}
public static int value(GameObject gameObject, System.Action<Vector3,Hashtable> callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional){
	return value(gameObject, callOnUpdate, from, to, time, h(optional)); 
}

public static int rotate(GameObject gameObject, Vector3 to, float time, Hashtable optional){
	return pushNewTween( gameObject, to, time, TweenAction.ROTATE, optional );
}
public static int rotate(GameObject gameObject, Vector3 to, float time, object[] optional){
	return rotate( gameObject, to, time, h( optional ) );
}


public static int rotate(LTRect ltRect, float to, float time, Hashtable optional){
	init();
	if( optional==null || optional.Count == 0 )
		optional = new Hashtable();

	optional["rect"] = ltRect;
	return pushNewTween( tweenEmpty, new Vector3(to,0f,0f), time, TweenAction.GUI_ROTATE, optional );
}

public static int rotate(LTRect ltRect, float to, float time, object[] optional){
	return rotate( ltRect, to, time, h(optional) );
}

public static int rotateX(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.ROTATE_X, optional );
}

public static int rotateX(GameObject gameObject, float to, float time, object[] optional){
	return rotateX( gameObject, to, time, h(optional) );
}

public static int rotateY(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.ROTATE_Y, optional );
}
public static int rotateY(GameObject gameObject, float to, float time, object[] optional){
	return rotateY( gameObject, to, time, h(optional) );
}
	
public static int rotateZ(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.ROTATE_Z, optional );
}

public static int rotateZ(GameObject gameObject, float to, float time, object[] optional){
	return rotateZ( gameObject, to, time, h(optional) );
}

public static int rotateLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional){
	return pushNewTween( gameObject, to, time, TweenAction.ROTATE_LOCAL, optional );
}
public static int rotateLocal(GameObject gameObject, Vector3 to, float time, object[] optional){
	return rotateLocal( gameObject, to, time, h(optional) );
}

/*public static int rotateAround(GameObject gameObject, Vector3 point, Vector3 axis, float add, float time, Hashtable optional){
	if(optional==null || optional.Count==0)
		optional = new Hashtable();
		
	optional["axis"] = axis;
	if(optional["point"]!=null)
		optional["point"] = Vector3.zero;
	return pushNewTween( gameObject, new Vector3(add,0f,0f), time, TweenAction.ROTATE_AROUND, optional );
}*/

public static int rotateAround(GameObject gameObject, Vector3 axis, float add, float time, Hashtable optional){
	if(optional==null || optional.Count==0)
		optional = new Hashtable();
		
	optional["axis"] = axis;
	if(optional["point"]==null)
		optional["point"] = Vector3.zero;

	return pushNewTween( gameObject, new Vector3(add,0f,0f), time, TweenAction.ROTATE_AROUND, optional );
}

public static int rotateAround(GameObject gameObject, Vector3 axis, float add, float time, object[] optional){
	return rotateAround(gameObject, axis, add, time, h(optional));
}

public static int moveX(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_X, optional );
}
public static int moveX(GameObject gameObject, float to, float time, object[] optional){
	return moveX( gameObject, to, time, h(optional) );
}

public static int moveY(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_Y, optional );
}
public static int moveY(GameObject gameObject, float to, float time, object[] optional){
	return moveY( gameObject, to, time, h(optional) );
}

public static int moveZ(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_Z, optional );
}
public static int moveZ(GameObject gameObject, float to, float time, object[] optional){
	return moveZ( gameObject, to, time, h(optional) );
}

public static int move(GameObject gameObject, Vector3 to, float time, Hashtable optional){
	return pushNewTween( gameObject, to, time, TweenAction.MOVE, optional );
}

public static int move(GameObject gameObject, Vector3 to, float time, object[] optional){
	return move( gameObject, to, time, LeanTween.h( optional ) );
}

public static int move(GameObject gameObject, Vector3[] to, float time, Hashtable optional){
	if(to.Length<4){
		string errorMsg = "LeanTween - When passing values for a vector path, you must pass four or more values!";
		if(throwErrors) Debug.LogError(errorMsg); else Debug.Log(errorMsg);
		return -1;
	}
	if(to.Length%4!=0){
		string errorMsg2 = "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...";
		if(throwErrors) Debug.LogError(errorMsg2); else Debug.Log(errorMsg2);
		return -1;
	}

	init();
	if( optional==null || optional.Count == 0 )
		optional = new Hashtable();

	LTBezierPath ltPath = new LTBezierPath( to );
	if(optional["orientToPath"]!=null)
		ltPath.orientToPath = true;
	optional["path"] = ltPath;

	return pushNewTween( gameObject, new Vector3(1.0f,0.0f,0.0f), time, TweenAction.MOVE_CURVED, optional );
}
public static int move(GameObject gameObject, Vector3[] to, float time, object[] optional){
	return move( gameObject, to, time, LeanTween.h( optional ) );
}

public static int move(LTRect ltRect, Vector2 to, float time, Hashtable optional){
	init();
	if( optional==null || optional.Count == 0 )
		optional = new Hashtable();

	optional["rect"] = ltRect;
	return pushNewTween( tweenEmpty, to, time, TweenAction.GUI_MOVE, optional );
}
public static int move(LTRect ltRect, Vector3 to, float time, object[] optional){
	return move( ltRect, to, time, LeanTween.h( optional ) );
}
	
public static int moveLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional){
	return pushNewTween( gameObject, to, time, TweenAction.MOVE_LOCAL, optional );
}
public static int moveLocal(GameObject gameObject, Vector3 to, float time, object[] optional){
	return moveLocal( gameObject, to, time, LeanTween.h( optional ) );
}

public static int moveLocal(GameObject gameObject, Vector3[] to, float time, Hashtable optional){
	if(to.Length<4){
		string errorMsg = "LeanTween - When passing values for a vector path, you must pass four or more values!";
		if(throwErrors) Debug.LogError(errorMsg); else Debug.Log(errorMsg);
		return -1;
	}
	if(to.Length%4!=0){
		string errorMsg2 = "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...";
		if(throwErrors) Debug.LogError(errorMsg2); else Debug.Log(errorMsg2);
		return -1;
	}

	init();
	if( optional == null )
		optional = new Hashtable();

	LTBezierPath ltPath = new LTBezierPath( to );
	if(optional["orientToPath"]!=null)
		ltPath.orientToPath = true;
	optional["path"] = ltPath;

	return pushNewTween( gameObject, new Vector3(1.0f,0.0f,0.0f), time, TweenAction.MOVE_CURVED_LOCAL, optional );
}
public static int moveLocal(GameObject gameObject, Vector3[] to, float time,object[] optional){
	return moveLocal( gameObject, to, time, LeanTween.h( optional ) );
}

public static int moveLocalX(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_LOCAL_X, optional );
}
public static int moveLocalX(GameObject gameObject, float to, float time, object[] optional){
	return moveLocalX( gameObject, to, time, h(optional) );
}

public static int moveLocalY(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_LOCAL_Y, optional );
}
public static int moveLocalY(GameObject gameObject, float to, float time, object[] optional){
	return moveLocalY( gameObject, to, time, h(optional) );
}

public static int moveLocalZ(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.MOVE_LOCAL_Z, optional );
}
public static int moveLocalZ(GameObject gameObject, float to, float time, object[] optional){
	return moveLocalZ( gameObject, to, time, h(optional) );
}
	
public static int scale(GameObject gameObject, Vector3 to, float time, Hashtable optional){
	return pushNewTween( gameObject, to, time, TweenAction.SCALE, optional );
}
public static int scale(GameObject gameObject, Vector3 to, float time, object[] optional){
	return scale( gameObject, to, time, h(optional) );
}

public static int scale(LTRect ltRect,Vector2 to, float time, Hashtable optional)
{ 
	init();
	if( optional==null || optional.Count == 0 )
		optional = new Hashtable();

	optional["rect"] = ltRect;
	return pushNewTween( tweenEmpty, to, time, TweenAction.GUI_SCALE, optional );
}
public static int scale(LTRect ltRect, Vector2 to, float time, object[] optional){
	return scale( ltRect, to, time, h(optional) );
}

public static int alpha(LTRect ltRect, float to, float time, Hashtable optional){
	init();
	if( optional==null || optional.Count == 0 )
		optional = new Hashtable();

	ltRect.alphaEnabled = true;
	optional["rect"] = ltRect;
	return pushNewTween( tweenEmpty, new Vector3(to,0f,0f), time, TweenAction.GUI_ALPHA, optional );
}
public static int alpha(LTRect ltRect, float to, float time, object[] optional){
	return alpha( ltRect, to, time, h(optional) );
}
	
public static int scaleX(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.SCALE_X, optional );
}
public static int scaleX(GameObject gameObject, float to, float time, object[] optional){
	return scaleX( gameObject, to, time, h(optional) );
}
	
public static int scaleY(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.SCALE_Y, optional );
}
public static int scaleY(GameObject gameObject, float to, float time, object[] optional){
	return scaleY( gameObject, to, time, h(optional) );
}

public static int scaleZ(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.SCALE_Z, optional );
}
public static int scaleZ(GameObject gameObject, float to, float time, object[] optional){
	return scaleZ( gameObject, to, time, h(optional) );
}

public static int delayedCall( float delayTime, string callback, Hashtable optional ){
	init();
	return delayedCall( tweenEmpty, delayTime, callback, optional );
}
public static int delayedCall( float delayTime, Action callback, object[] optional){
	init();
	return delayedCall( tweenEmpty, delayTime, callback, h(optional) );
}

public static int delayedCall( GameObject gameObject, float delayTime, string callback, object[] optional){
	return delayedCall( gameObject, delayTime, callback, h(optional) );
}

public static int delayedCall( GameObject gameObject, float delayTime, Action callback, object[] optional){
	return delayedCall( gameObject, delayTime, callback, h(optional) );
}

public static int delayedCall( GameObject gameObject, float delayTime, string callback, Hashtable optional){
	if(optional==null || optional.Count == 0)
		optional = new Hashtable();
	optional["onComplete"] = callback;

	return pushNewTween( gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional );
}

public static int delayedCall( GameObject gameObject, float delayTime, Action callback, Hashtable optional){
	if(optional==null)
		optional = new Hashtable();
	optional["onComplete"] = callback;

	return pushNewTween( gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional );
}

public static int delayedCall( GameObject gameObject, float delayTime, Action<object> callback, Hashtable optional){
	if(optional==null)
		optional = new Hashtable();
	optional["onComplete"] = callback;

	return pushNewTween( gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional );
}

public static int alpha(GameObject gameObject, float to, float time, Hashtable optional){
	return pushNewTween( gameObject, new Vector3(to,0,0), time, TweenAction.ALPHA, optional );
}
public static int alpha(GameObject gameObject, float to, float time, object[] optional){
	return alpha(gameObject, to, time, h(optional)); 
}
#endif

// Tweening Functions - Thanks to Robert Penner and GFX47

private static float tweenOnCurve( LTDescr tweenDescr, float ratioPassed ){
	// Debug.Log("single ratio:"+ratioPassed+" tweenDescr.animationCurve.Evaluate(ratioPassed):"+tweenDescr.animationCurve.Evaluate(ratioPassed));
	return tweenDescr.from.x + (tweenDescr.diff.x) * tweenDescr.animationCurve.Evaluate(ratioPassed);
}

private static Vector3 tweenOnCurveVector( LTDescr tweenDescr, float ratioPassed ){
	return	new Vector3(tweenDescr.from.x + (tweenDescr.diff.x) * tweenDescr.animationCurve.Evaluate(ratioPassed),
						tweenDescr.from.y + (tweenDescr.diff.y) * tweenDescr.animationCurve.Evaluate(ratioPassed),
						tweenDescr.from.z + (tweenDescr.diff.z) * tweenDescr.animationCurve.Evaluate(ratioPassed) );
}

private static float easeOutQuadOpt( float start, float diff, float ratioPassed ){
	return -diff * ratioPassed * (ratioPassed - 2) + start;
}

private static float easeInQuadOpt( float start, float diff, float ratioPassed ){
	return diff * ratioPassed * ratioPassed + start;
}

private static float easeInOutQuadOpt( float start, float diff, float ratioPassed ){
	ratioPassed /= .5f;
	if (ratioPassed < 1) return diff / 2 * ratioPassed * ratioPassed + start;
	ratioPassed--;
	return -diff / 2 * (ratioPassed * (ratioPassed - 2) - 1) + start;
}

private static float linear(float start, float end, float val){
	return Mathf.Lerp(start, end, val);
}

private static float clerp(float start, float end, float val){
	float min = 0.0f;
	float max = 360.0f;
	float half = Mathf.Abs((max - min) / 2.0f);
	float retval = 0.0f;
	float diff = 0.0f;
	if ((end - start) < -half){
		diff = ((max - start) + end) * val;
		retval = start + diff;
	}else if ((end - start) > half){
		diff = -((max - end) + start) * val;
		retval = start + diff;
	}else retval = start + (end - start) * val;
	return retval;
}

private static float spring(float start, float end, float val){
	val = Mathf.Clamp01(val);
	val = (Mathf.Sin(val * Mathf.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) * (1f + (1.2f * (1f - val)));
	return start + (end - start) * val;
}

private static float easeInQuad(float start, float end, float val){
	end -= start;
	return end * val * val + start;
}

private static float easeOutQuad(float start, float end, float val){
	end -= start;
	return -end * val * (val - 2) + start;
}

private static float easeInOutQuad(float start, float end, float val){
	val /= .5f;
	end -= start;
	if (val < 1) return end / 2 * val * val + start;
	val--;
	return -end / 2 * (val * (val - 2) - 1) + start;
}

private static float easeInCubic(float start, float end, float val){
	end -= start;
	return end * val * val * val + start;
}

private static float easeOutCubic(float start, float end, float val){
	val--;
	end -= start;
	return end * (val * val * val + 1) + start;
}

private static float easeInOutCubic(float start, float end, float val){
	val /= .5f;
	end -= start;
	if (val < 1) return end / 2 * val * val * val + start;
	val -= 2;
	return end / 2 * (val * val * val + 2) + start;
}

private static float easeInQuart(float start, float end, float val){
	end -= start;
	return end * val * val * val * val + start;
}

private static float easeOutQuart(float start, float end, float val){
	val--;
	end -= start;
	return -end * (val * val * val * val - 1) + start;
}

private static float easeInOutQuart(float start, float end, float val){
	val /= .5f;
	end -= start;
	if (val < 1) return end / 2 * val * val * val * val + start;
	val -= 2;
	return -end / 2 * (val * val * val * val - 2) + start;
}

private static float easeInQuint(float start, float end, float val){
	end -= start;
	return end * val * val * val * val * val + start;
}

private static float easeOutQuint(float start, float end, float val){
	val--;
	end -= start;
	return end * (val * val * val * val * val + 1) + start;
}

private static float easeInOutQuint(float start, float end, float val){
	val /= .5f;
	end -= start;
	if (val < 1) return end / 2 * val * val * val * val * val + start;
	val -= 2;
	return end / 2 * (val * val * val * val * val + 2) + start;
}

private static float easeInSine(float start, float end, float val){
	end -= start;
	return -end * Mathf.Cos(val / 1 * (Mathf.PI / 2)) + end + start;
}

private static float easeOutSine(float start, float end, float val){
	end -= start;
	return end * Mathf.Sin(val / 1 * (Mathf.PI / 2)) + start;
}

private static float easeInOutSine(float start, float end, float val){
	end -= start;
	return -end / 2 * (Mathf.Cos(Mathf.PI * val / 1) - 1) + start;
}

private static float easeInExpo(float start, float end, float val){
	end -= start;
	return end * Mathf.Pow(2, 10 * (val / 1 - 1)) + start;
}

private static float easeOutExpo(float start, float end, float val){
	end -= start;
	return end * (-Mathf.Pow(2, -10 * val / 1) + 1) + start;
}

private static float easeInOutExpo(float start, float end, float val){
	val /= .5f;
	end -= start;
	if (val < 1) return end / 2 * Mathf.Pow(2, 10 * (val - 1)) + start;
	val--;
	return end / 2 * (-Mathf.Pow(2, -10 * val) + 2) + start;
}

private static float easeInCirc(float start, float end, float val){
	end -= start;
	return -end * (Mathf.Sqrt(1 - val * val) - 1) + start;
}

private static float easeOutCirc(float start, float end, float val){
	val--;
	end -= start;
	return end * Mathf.Sqrt(1 - val * val) + start;
}

private static float easeInOutCirc(float start, float end, float val){
	val /= .5f;
	end -= start;
	if (val < 1) return -end / 2 * (Mathf.Sqrt(1 - val * val) - 1) + start;
	val -= 2;
	return end / 2 * (Mathf.Sqrt(1 - val * val) + 1) + start;
}

/* GFX47 MOD START */
private static float easeInBounce(float start, float end, float val){
	end -= start;
	float d = 1f;
	return end - easeOutBounce(0, end, d-val) + start;
}
/* GFX47 MOD END */

/* GFX47 MOD START */
//public static function bounce(float start, float end, float val){
private static float easeOutBounce(float start, float end, float val){
	val /= 1f;
	end -= start;
	if (val < (1 / 2.75f)){
		return end * (7.5625f * val * val) + start;
	}else if (val < (2 / 2.75f)){
		val -= (1.5f / 2.75f);
		return end * (7.5625f * (val) * val + .75f) + start;
	}else if (val < (2.5 / 2.75)){
		val -= (2.25f / 2.75f);
		return end * (7.5625f * (val) * val + .9375f) + start;
	}else{
		val -= (2.625f / 2.75f);
		return end * (7.5625f * (val) * val + .984375f) + start;
	}
}
/* GFX47 MOD END */

/* GFX47 MOD START */
private static float easeInOutBounce(float start, float end, float val){
	end -= start;
	float d= 1f;
	if (val < d/2) return easeInBounce(0, end, val*2) * 0.5f + start;
	else return easeOutBounce(0, end, val*2-d) * 0.5f + end*0.5f + start;
}
/* GFX47 MOD END */

private static float easeInBack(float start, float end, float val){
	end -= start;
	val /= 1;
	float s= 1.70158f;
	return end * (val) * val * ((s + 1) * val - s) + start;
}

private static float easeOutBack(float start, float end, float val){
	float s= 1.70158f;
	end -= start;
	val = (val / 1) - 1;
	return end * ((val) * val * ((s + 1) * val + s) + 1) + start;
}

private static float easeInOutBack(float start, float end, float val){
	float s= 1.70158f;
	end -= start;
	val /= .5f;
	if ((val) < 1){
		s *= (1.525f);
		return end / 2 * (val * val * (((s) + 1) * val - s)) + start;
	}
	val -= 2;
	s *= (1.525f);
	return end / 2 * ((val) * val * (((s) + 1) * val + s) + 2) + start;
}

/* GFX47 MOD START */
private static float easeInElastic(float start, float end, float val){
	end -= start;
	
	float d = 1f;
	float p = d * .3f;
	float s= 0;
	float a = 0;
	
	if (val == 0) return start;
	val = val/d;
	if (val == 1) return start + end;
	
	if (a == 0f || a < Mathf.Abs(end)){
		a = end;
		s = p / 4;
		}else{
		s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
	}
	val = val-1;
	return -(a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
}		
/* GFX47 MOD END */

/* GFX47 MOD START */
//public static function elastic(float start, float end, float val){
private static float easeOutElastic(float start, float end, float val){
/* GFX47 MOD END */
	//Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
	end -= start;
	
	float d = 1f;
	float p= d * .3f;
	float s= 0;
	float a= 0;
	
	if (val == 0) return start;
	
	val = val / d;
	if (val == 1) return start + end;
	
	if (a == 0f || a < Mathf.Abs(end)){
		a = end;
		s = p / 4;
		}else{
		s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
	}
	
	return (a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) + end + start);
}		

/* GFX47 MOD START */
private static float easeInOutElastic(float start, float end, float val)
{
	end -= start;
	
	float d = 1f;
	float p= d * .3f;
	float s= 0;
	float a = 0;
	
	if (val == 0) return start;
	
	val = val / (d/2);
	if (val == 2) return start + end;
	
	if (a == 0f || a < Mathf.Abs(end)){
		a = end;
		s = p / 4;
		}else{
		s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
	}
	
	if (val < 1){
	 val = val-1;
	 return -0.5f * (a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
	}
	val = val-1;
	return a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
}

// LeanTween Listening/Dispatch

private static System.Action<LTEvent>[] eventListeners;
private static GameObject[] goListeners;
private static int eventsMaxSearch = 0;
public static int EVENTS_MAX = 10;
public static int LISTENERS_MAX = 10;
private static int INIT_LISTENERS_MAX = LISTENERS_MAX;

public static void addListener( int eventId, System.Action<LTEvent> callback ){
	addListener(tweenEmpty, eventId, callback);
}

/**
* Add a listener method to be called when the appropriate LeanTween.dispatchEvent is called
* @method LeanTween.addListener
* @param {GameObject} caller:GameObject the gameObject the listener is attached to
* @param {int} eventId:int a unique int that describes the event (best to use an enum)
* @param {System.Action<LTEvent>} callback:System.Action<LTEvent> the method to call when the event has been dispatched
* @example
* LeanTween.addListener(gameObject, (int)MyEvents.JUMP, jumpUp);<br>
* <br>
* void jumpUp( LTEvent e ){ Debug.Log("jump!"); }<br>
*/
public static void addListener( GameObject caller, int eventId, System.Action<LTEvent> callback ){
	if(eventListeners==null){
		INIT_LISTENERS_MAX = LISTENERS_MAX;
		eventListeners = new System.Action<LTEvent>[ EVENTS_MAX * LISTENERS_MAX ];
		goListeners = new GameObject[ EVENTS_MAX * LISTENERS_MAX ];
	}
	// Debug.Log("searching for an empty space for:"+caller + " eventid:"+event);
	for(i = 0; i < INIT_LISTENERS_MAX; i++){
		int point = eventId*INIT_LISTENERS_MAX + i;
		if(goListeners[ point ]==null || eventListeners[ point ]==null){
			eventListeners[ point ] = callback;
			goListeners[ point ] = caller;
			if(i>=eventsMaxSearch)
				eventsMaxSearch = i+1;
			// Debug.Log("adding event for:"+caller.name);

			return;
		}
		if(goListeners[ point ] == caller && System.Object.Equals( eventListeners[ point ], callback)){
			// Debug.Log("This event is already being listened for.");
			return;
		}
	}
	Debug.LogError("You ran out of areas to add listeners, consider increasing INIT_LISTENERS_MAX, ex: LeanTween.INIT_LISTENERS_MAX = "+(INIT_LISTENERS_MAX*2));
}

public static bool removeListener( int eventId, System.Action<LTEvent> callback ){
	return removeListener( tweenEmpty, eventId, callback);
}

/**
* Remove an event listener you have added
* @method LeanTween.removeListener
* @param {GameObject} caller:GameObject the gameObject the listener is attached to
* @param {int} eventId:int a unique int that describes the event (best to use an enum)
* @param {System.Action<LTEvent>} callback:System.Action<LTEvent> the method that was specified to call when the event has been dispatched
* @example
* LeanTween.removeListener(gameObject, (int)MyEvents.JUMP, jumpUp);<br>
* <br>
* void jumpUp( LTEvent e ){ }<br>
*/
public static bool removeListener( GameObject caller, int eventId, System.Action<LTEvent> callback ){
	for(i = 0; i < eventsMaxSearch; i++){
		int point = eventId*INIT_LISTENERS_MAX + i;
		if(goListeners[ point ] == caller && System.Object.Equals( eventListeners[ point ], callback) ){
			eventListeners[ point ] = null;
			goListeners[ point ] = null;
			return true;
		}
	}
	return false;
}

/**
* Tell the added listeners that you are dispatching the event
* @method LeanTween.dispatchEvent
* @param {int} eventId:int a unique int that describes the event (best to use an enum)
* @example
* LeanTween.dispatchEvent( (int)MyEvents.JUMP );<br>
*/
public static void dispatchEvent( int eventId ){
	dispatchEvent( eventId, null);
}

/**
* Tell the added listeners that you are dispatching the event
* @method LeanTween.dispatchEvent
* @param {int} eventId:int a unique int that describes the event (best to use an enum)
* @param {object} data:object Pass data to the listener, access it from the listener with *.data on the LTEvent object
* @example
* LeanTween.dispatchEvent( (int)MyEvents.JUMP, transform );<br>
* <br>
* void jumpUp( LTEvent e ){<br>
* &nbsp; Transform tran = (Transform)e.data;<br>
* }<br>
*/
public static void dispatchEvent( int eventId, object data ){
	for(int k = 0; k < eventsMaxSearch; k++){
		int point = eventId*INIT_LISTENERS_MAX + k;
		if(eventListeners[ point ]!=null){
			if(goListeners[point]){
				eventListeners[ point ]( new LTEvent(eventId, data) );
			}else{
				eventListeners[ point ] = null;
			}
		}
	}
}

}

/**
* Object that describes the event to an event listener
* @class LTEvent
* @constructor
* @param {object} data:object Data that has been passed from the dispatchEvent method
*/
public class LTEvent{
	public int id;
	public object data;

	public LTEvent(int id, object data){
		this.id = id;
		this.data = data;
	}
}

public class LTGUI{
	public static int RECT_LEVELS = 5;
	public static int RECTS_PER_LEVEL = 10;
	public static int BUTTONS_MAX = 24;

	private static LTRect[] levels;
	private static int[] levelDepths;
	private static Rect[] buttons;
	private static int[] buttonLevels;
	private static int[] buttonLastFrame;
	private static LTRect r;
	private static Color color = Color.white;
	private static bool isGUIEnabled = false;
	private static int global_counter = 0;

	public enum Element_Type{
		Texture,
		Label
	}

	public static void init(){
		if(levels==null){
			levels = new LTRect[RECT_LEVELS*RECTS_PER_LEVEL];
			levelDepths = new int[RECT_LEVELS];
		}
	}

	public static void initRectCheck(){
		if(buttons==null){
			buttons = new Rect[BUTTONS_MAX];
			buttonLevels = new int[BUTTONS_MAX];
			buttonLastFrame = new int[BUTTONS_MAX];
			for(int i = 0; i < buttonLevels.Length; i++){
				buttonLevels[i] = -1;
			}
		}
	}

	public static void reset(){
		if(isGUIEnabled){
			isGUIEnabled = false;
			for(int i = 0; i < levels.Length; i++){
				levels[i] = null;
			}

			for(int i = 0; i < levelDepths.Length; i++){
				levelDepths[i] = 0;
			}
		}
	}

	public static void update( int updateLevel ){
		if(isGUIEnabled){
			init();
			if(levelDepths[updateLevel]>0){
				color = GUI.color;
				int baseI = updateLevel*RECTS_PER_LEVEL;
				int maxLoop = baseI + levelDepths[updateLevel];// RECTS_PER_LEVEL;//;
				
				for(int i = baseI; i < maxLoop; i++){
					r = levels[i];
					// Debug.Log("r:"+r+" i:"+i);
					if(r!=null /*&& checkOnScreen(r.rect)*/){
						//Debug.Log("label:"+r.labelStr+" textColor:"+r.style.normal.textColor);
						if(r.useColor)
							GUI.color = r.color;
						if(r.type == Element_Type.Label){
							if(r.style!=null)
								GUI.skin.label = r.style;
							if(r.useSimpleScale){
								GUI.Label( new Rect((r.rect.x + r.margin.x + r.relativeRect.x)*r.relativeRect.width, (r.rect.y + r.margin.y + r.relativeRect.y)*r.relativeRect.height, r.rect.width*r.relativeRect.width, r.rect.height*r.relativeRect.height), r.labelStr );
							}else{
								GUI.Label( new Rect(r.rect.x + r.margin.x, r.rect.y + r.margin.y, r.rect.width, r.rect.height), r.labelStr );
							}
						}else if(r.type == Element_Type.Texture && r.texture!=null){
							Vector2 size = r.useSimpleScale ? new Vector2(0f, r.rect.height*r.relativeRect.height) : new Vector2(r.rect.width, r.rect.height);
							if(r.sizeByHeight){
								size.x = (float)r.texture.width/(float)r.texture.height * size.y;
							}
							if(r.useSimpleScale){
								GUI.DrawTexture( new Rect((r.rect.x + r.margin.x + r.relativeRect.x)*r.relativeRect.width, (r.rect.y + r.margin.y + r.relativeRect.y)*r.relativeRect.height, size.x, size.y), r.texture );
							}else{
								GUI.DrawTexture( new Rect(r.rect.x + r.margin.x, r.rect.y + r.margin.y, size.x, size.y), r.texture );
							}
						}
					}
				}
				GUI.color = color;
			}
		}
	}

	public static bool checkOnScreen(Rect rect){
		bool offLeft = rect.x + rect.width < 0f;
		bool offRight = rect.x > Screen.width;
		bool offBottom = rect.y > Screen.height;
		bool offTop = rect.y + rect.height < 0f;

		return !(offLeft || offRight || offBottom || offTop);
	}

	public static void destroy( int id ){
		int backId = id & 0xFFFF;
		int backCounter = id >> 16;
		if(id>=0 && levels[backId]!=null && levels[backId].hasInitiliazed && levels[backId].counter==backCounter)
			levels[backId] = null;
	}

	public static void destroyAll( int depth ){ // clears all gui elements on depth
		int maxLoop = depth*RECTS_PER_LEVEL + RECTS_PER_LEVEL;
		for(int i = depth*RECTS_PER_LEVEL; i < maxLoop; i++){
			levels[i] = null;
		}
	}

	public static LTRect label( Rect rect, string label, int depth){
		return LTGUI.label(new LTRect(rect), label, depth);
	}

	public static LTRect label( LTRect rect, string label, int depth){
		rect.type = Element_Type.Label;
		rect.labelStr = label;
		return element(rect, depth);
	}

	public static LTRect texture( Rect rect, Texture texture, int depth){
		return LTGUI.texture( new LTRect(rect), texture, depth);
	}

	public static LTRect texture( LTRect rect, Texture texture, int depth){
		rect.type = Element_Type.Texture;
		rect.texture = texture;
		return element(rect, depth);
	}

	public static LTRect element( LTRect rect, int depth){
		isGUIEnabled = true;
		init();
		int maxLoop = depth*RECTS_PER_LEVEL + RECTS_PER_LEVEL;
		int k = 0;
		if(rect!=null){
			destroy(rect.id);
		}
		if(rect.type==LTGUI.Element_Type.Label && rect.style!=null){
			if(rect.style.normal.textColor.a<=0f){
				Debug.LogWarning("Your GUI normal color has an alpha of zero, and will not be rendered.");
			}
		}
		if(rect.relativeRect.width==float.PositiveInfinity){
			rect.relativeRect = new Rect(0f,0f,Screen.width,Screen.height);
		}
		for(int i = depth*RECTS_PER_LEVEL; i < maxLoop; i++){
			r = levels[i];
			if(r==null){
				r = rect;
				r.rotateEnabled = true;
				r.alphaEnabled = true;
				r.setId( i, global_counter );
				levels[i] = r;
				// Debug.Log("k:"+k+ " maxDepth:"+levelDepths[depth]);
				if(k>=levelDepths[depth]){
					levelDepths[depth] = k + 1;
				}
				global_counter++;
				return r;
			}
			k++;
		}

		Debug.LogError("You ran out of GUI Element spaces");

		return null;
	}

	public static bool hasNoOverlap( Rect rect, int depth ){
		initRectCheck();
		bool hasNoOverlap = true;
		bool wasAddedToList = false;
		for(int i = 0; i < buttonLevels.Length; i++){
			// Debug.Log("buttonLastFrame["+i+"]:"+buttonLastFrame[i]);
			//Debug.Log("buttonLevels["+i+"]:"+buttonLevels[i]);
			if(buttonLevels[i]>=0){
				//Debug.Log("buttonLastFrame["+i+"]:"+buttonLastFrame[i]+" Time.frameCount:"+Time.frameCount);
				if( buttonLastFrame[i] + 1 < Time.frameCount ){ // It has to have been visible within the current, or
					buttonLevels[i] = -1;
					// Debug.Log("resetting i:"+i);
				}else{
					//if(buttonLevels[i]>=0)
					//	 Debug.Log("buttonLevels["+i+"]:"+buttonLevels[i]);
					if(buttonLevels[i]>depth){
						/*if(firstTouch().x > 0){
							Debug.Log("buttons["+i+"]:"+buttons[i] + " firstTouch:");
							Debug.Log(firstTouch());
							Debug.Log(buttonLevels[i]);
						}*/
						if(pressedWithinRect( buttons[i] )){
							hasNoOverlap = false; // there is an overlapping button that is higher
						}
					}
				}
			}

			if(wasAddedToList==false && buttonLevels[i]<0){
				wasAddedToList = true;
				buttonLevels[i] = depth;
				buttons[i] = rect;
				buttonLastFrame[i] = Time.frameCount;
			}
		}

		return hasNoOverlap;
	}

	public static bool pressedWithinRect( Rect rect ){
		Vector2 vec2 = firstTouch();
		if(vec2.x<0f)
			return false;
		float vecY = Screen.height-vec2.y;
		return (vec2.x > rect.x && vec2.x < rect.x + rect.width && vecY > rect.y && vecY < rect.y + rect.height);
	}

	public static bool checkWithinRect(Vector2 vec2, Rect rect){
		vec2.y = Screen.height-vec2.y;
		return (vec2.x > rect.x && vec2.x < rect.x + rect.width && vec2.y > rect.y && vec2.y < rect.y + rect.height);
	}

	public static Vector2 firstTouch(){
		if(Input.touchCount>0){
			return Input.touches[0].position;
		}else if(Input.GetMouseButton(0)){
			return Input.mousePosition;
		}

		return new Vector2(Mathf.NegativeInfinity,Mathf.NegativeInfinity);
	}

}
