Rain Maker, (c) 2015 Digital Ruby, LLC
http://www.digitalruby.com
Created by Jeff Johnson

Version 1.0.3

--------------------
Change Log:
--------------------
1.0.3	-	Fix prefab material references
2.0		-	Added 2D mode
2.0.1	-	Remove deprecated methods
2.0.2	-	A few small bug fixes
--------------------

Rain Maker gives you highly configurable and realistic rain for your game or app.

I've spent many, many hours tuning, debugging and configuring Rain Maker so that it can
mostly be used as is. If you want to tweak settings and change things, that is an
option of course, and I will explain how to do that now.

Rain Maker consists of a custom shader, several particle systems and two scripts. To get
going, drag the RainPrefab into your scene. The script will automatically link up
to your main camera, but you can also assign your own custom camera if you want as well.

To make it rain, set the RainIntensity on the script. The script automatically detects
changes and will alter the rain intensity appropriately, seamlessly and cleanly. For
most users, this will be all you need.

2D mode is also available. Note that 2D mode does not yet support lighting.

Advanced Users
---------------------
So you want to tweak and configure Rain Maker? I'll explain everything now so that you
can do that. If you are going to configure the prefab, you'll probably want to break
the prefab connection and configure it, rename it and save it as your own prefab, that
way you have the original prefab as a backup or reference.

The Rain Maker prefab contains the following pieces:

1] Rain fall particle system
This is the stuff that falls from the sky. The particle system is setup to be as realistic
as possible, but you can of course tweak the particle system as you need.
This particle system uses RainMaterial.

2] Rain explosion particle system
This particle system is emitted whenever a rain drop hits something. RainCollision.cs deals
with handling the collision and spawning appropriate splash particles. Feel free to customize
the number, size, velocity, etc. of the splash particles if you want to tweak the effect. The
rain splash uses RainExplosionMaterial.

3] Rain mist particle system
For higher rain intensities, mist will start to appear. The CheckForRainChange method in
RainScript.cs controls what intensity (default is 0.5) that the mist starts to appear. The mist
uses RainMistMaterial.

4] Rain wind zone
The prefab also comes with a wind zone that is setup to blow the rain around randomly to provide
a more realistic effect. If you don't like this, feel free to delete the wind zone, replace it
with your own, or configure it a different way. The wind zone can be null.

RainScript.cs is the smarts behind the rain, and controls what sounds play, how the wind changes,
etc. If you decide to swap out the sounds for the rain and wind, be sure they are seamless and
loop cleanly. You can use a program like Audacity to create a seamless audio file and test
that it loops properly.

If you decided to tweak the wind sound or parameters, these four parameters will be of use to you:

- [Tooltip("Wind sound volume modifier, use this to lower your sound if it's too loud.")]
public float WindSoundVolumeModifier = 0.5f;
- [Tooltip("Wind zone that will affect and follow the rain")]
public WindZone WindZone;
- [Tooltip("Minimum, maximum and absolute maximum wind speed. Set to 0 if you are managing the wind speed yourself or don't want wind. " +
  "The absolute maximum should always be >= to the current maximum wind speed and should generally never change. The maximum is " +
  "used to determine how much louder your wind sound gets.")]
public Vector3 WindSpeedRange = new Vector3(50.0f, 500.0f, 500.0f);
- [Tooltip("How often the wind speed and direction changes (minimum and maximum change interval in seconds)")]
public Vector2 WindChangeInterval = new Vector2(5.0f, 30.0f);

I've also written a highly optimized and custom shader that uses per vertex lighting but still
looks great. The shader uses the 4 brightest lights from your scene, including any directional light.
The shader is called RainShader.shader. All of the rain materials are set to use this shader.
If you want to tweak parameters of the shader, I'll explain that now:

- _MainTex ("Color (RGB) Alpha (A)", 2D) = "gray" {}
This texture is the gray faded rain texture, but you can replace it if you have a texture that
better suits your needs. It should be a very translucent, grayish texture for best effect,
although you can certainly get creative if you want something special.

- _TintColor ("Tint Color (RGB)", Color) = (1, 1, 1, 1)
Tint color, with this you could make acid rain, blood rain or any other type of rain you
can think of. The alpha value is also used and will make the rain more faint if lowered.

- _PointSpotLightMultiplier ("Point/Spot Light Multiplier", Range (0, 10)) = 2
How much do point and spot lights light up the rain. Keep in mind that the rain alpha will
never go above the value in _TintColor.

- _DirectionalLightMultiplier ("Directional Light Multiplier", Range (0, 10)) = 1
How much do directional lights effect the rain color? As directional lights go below the horizon,
they effect the rain less and less until they have no effect, until they come back near the horizon
or higher.

- _InvFade ("Soft Particles Factor", Range(0.01, 3.0)) = 1.0
This works exactly the same way as the particles/alpha blended shader and can help with large
particles hitting the edges of objects. This is only enabled for the RainMistMaterial, as the
rain fall and rain explosion particles don't have issues with edges.

_AmbientLightMultiplier ("Ambient light multiplier", Range(0, 1)) = 0.25
Each vertex will start of with the ambient light multiplied by this number.

Troubleshooting:
If you see any problems with the rain, ensure that the particle systems have the correct material assigned.

I'm always happy to answer questions, so please email me at jjxtra@gmail.com and I'll do my best
to help you out.

