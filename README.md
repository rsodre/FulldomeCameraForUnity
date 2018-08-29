# FulldomeCameraForUnity

Fulldome Camera for Unity 2018+

For Unity 5.6, use [FulldomeCameraForUnity5](https://github.com/rsodre/FulldomeCameraForUnity5)

![](images/example.png)


This plugin was inspired by [this article](https://blogs.unity3d.com/2018/01/26/stereo-360-image-and-video-capture/), and relies on the `Camera.RenderToCubemap` [method](https://docs.unity3d.com/ScriptReference/Camera.RenderToCubemap.html) from Unity 2018.1. It will render your game's camera as a [cubemap](https://en.wikipedia.org/wiki/Cube_mapping) and distort it to a [Domemaster](http://download.studioavante.com/TEMPLATES/DOME/DOME_template_2K.png) format.

If we consider performance and quality, this solution is far from ideal. To make a cubemap, we need to render the scene (up to) 6 times, with 6 different cameras, one for each face of the cube. Rendering a good looking game once is already a challenge, everybody knows, just imagine six. Another problem is that some effects and shaders that depend on the camera position, like reflections, will look weird where the cube faces meet, because neigboring pixels were calculated for different cameras. Front-facing sprites commonly used on particles also will suffer from the same problem.

Ideally, Unity should provide us with a custom camera, that instead of using the usual frustum to raster each frame, would use our custom method that calculate rays from the camera to world, for each pixel. Like Cinema 4D plugins [can do](https://developers.maxon.net/docs/Cinema4DCPPSDK/html/class_video_post_data.html#a597ac521409b00572117ea604536e06f). But there's no way to do it in Unity :(

There's an [issue in Unity Feedback](https://feedback.unity3d.com/suggestions/correct-camera-distortion-issue-on-the-side-by-using-spherical-clipping-planes-instead-of-flat-near-far-clipping-plane) that suggests to solve that problem. The request description don't sound like it, but the solution to the problem is the same (see my comment). Please give some votes.

And here's the Unity [forum thread](https://forum.unity.com/threads/fulldome-camera-for-unity.547939/).


## Install

Download and import the latest [package](https://github.com/rsodre/FulldomeCameraForUnity/releases) release.

Alternatvely, you can [clone](https://help.github.com/articles/cloning-a-repository/) or [download](https://github.com/rsodre/FulldomeCameraForUnity/archive/master.zip) this repository and open as a full project in Unity 2018.1 or newer.


## Usage

This plugin needs just one prefab to run.


### FulldomeCamera prefab

Drop `FulldomeCamera/FulldomeCamera.prefab` anywhere in the scene.

It contains a camera that renders to a **RenderTexture** that defines the final resolution. By defaut, it's using `FulldomeCamera2k` (2048 x 2048). If you're using a single projector with fisheye lenses, `FulldomeCamera1080p` will be perfect.

The final Fulldome distorted image will be rendered to that texture, and stay there if you don't use it anywhere else (see your options on the next sections).

Configure your fulldome camera on this GameObject...


* **Main Camera**: The camera used to render the cubemap. If null, `Camera.main` will be used.

* **Cubemap Faces**: Depending on your camera orientation and **Horizon** setting, you can turn off some cubemap faces and save several passes. Fulldome cameras placed on the ground can turn off the **NegativeY**, for example.

* **Orientation**: The point of interest, or sweet spot, on a Fulldome is close to the horizon on the bottom of the frame. On a Fisheye, it's on the center of the frame. This setting will consider this and rotate the main camera to target the correct sweet spot. 

* **Horizon**: Usually 180 degrees (half sphere).

* **Dome Tilt**: Most planetariums are tilted, giving a more comfortable experience to viewers. Enter the venue tilt here.

* **Masked**: Will ignore and paint black the area outside the fisheye circle. That's 27% less pixels, so please mask.

There's plenty of information and considerations about the Fulldome format on my [Blendy 360 Cam](http://blendy360cam.com/)'s [manual](http://download.studioavante.com/Blendy360Cam/Blendy360Cam_Manual.pdf).


### FulldomePreview.cs (optional)

To make your main camera display the Fulldome distorted image, drop the `FulldomePreview` component into your main camera (the one linked in `FulldomeCamera`). It will display it as Fisheye on the editor and during gameplay.

If you do that, create a new **Aspect** on the **Game View** called `Fulldome`, with **Aspect Ratio** of `1:1` and use it during development to preview it correctly.

In the case where the dome is just for spectators and the player plays on the computer, there's no need to add this script. You'll see the normal main camera render on the computer screen.

You can even create different spectator static camera(s) just for the dome (don't forget to link on `FulldomeCamera`), while the player uses a standard first or third person controller.



### Capture.cs (optional)

The FulldomeCamera prefab comes with a (disabled) `Capture` component. When enabled, it will capture the game to disk, frame by frame, as static images.

This capture script is very slow for real time, use it to render scripted or timelined animated sequences.

The parameters are self-explanatory.



## To The Dome

Fulldome without a Dome is no fun at all!

Here's how I do it.

Game is played on a **Mac** (Mac Pro, MBP Touchbar or even MBP Retina), using a **Syphon** plugin to stream the RenderTexture to [Blendy Dome VJ](http://www.blendydomevj.com/), who takes care of mapping the dome and sending signal to 4 projectors, using a **Datapath FX-4** card.

For a single projector with fisheye lenses, you can just fullscreen and mirror the screen to the projector. Or even better, run windowed, sending the  Fulldome image to another application that can output it to the projector (see below), and you have your monitor free while playing the game.

There are many texture sharing frameworks that can be used to stream texture over applications, using a client-server model.


### Syphon (macOS only)

[Syphon](http://syphon.v002.info/) is the first texture sharing framework to become popular among VJs, supported by all VJ apps.

To create a Syphon server, use [KlakSyphon](https://github.com/keijiro/KlakSyphon). Just add a `SyphonServer` component to your `FulldomeCamera` instance.

Works with Metal API only. Check [here](https://docs.unity3d.com/Manual/Metal.html), Enabling Metal.


### Spout (Windows only)

[Spout](http://spout.zeal.co/) is the equivalent of Syphon on Windows, supported by all VJ apps.

To create a Spout server, use [KlakSpout](https://github.com/keijiro/KlakSpout). Just add a `SpoutSender` component to your `FulldomeCamera` instance.


### NewTek NDI (Windows & macOS)

[NDI](http://ndi.newtek.com/) has the advantage of send textures over a network. It's adoption is growing very fast, and is being used in many TV studios as a clean and cheap capture solution.

To create a NDI server, use [KlakNDI](https://github.com/keijiro/KlakNDI). Just add a `SpoutSender` component to your `FulldomeCamera` instance.

KlakNDI does not work on Metal yet, but a Syphon server can be converted to NDI using [NDISyphon](https://docs.vidvox.net/freebies_ndi_syphon.html), a very nice little tool by [Vidvox](https://github.com/Vidvox).

With this, the game can be played in one computer (Windows or macOS) with a **NDI** server, and another Mac on the network receive with **NDISyphon**, projecting with **Blendy Dome VJ**.


### BlackSyphon (macOS only)

[BlackSyphon](https://docs.vidvox.net/freebies_black_syphon.html), also by [Vidvox](https://github.com/Vidvox), creates a Syphon server from a **Black Magic** capture card.

So the game can be played in fullscreen (Windows or macOS), sending a mirror image to a Black Magic capture on a Mac, reading it with **BlackSyphon**, projecting with **Blendy Dome VJ**.



## Thanks

The work of all these people allowed us to get to this point.

Many thanks to Paul Bourke for his amazing [dome research](http://paulbourke.net/dome/)

[Anton Marini](https://github.com/vade) and [Tom Butterworth](https://github.com/bangnoise) for revolutionizing visuals with [Syphon](https://github.com/Syphon).

[David Lublin](https://github.com/dlublin) of [Vidvod](https://github.com/Vidvox) for all the freebie tools and for the amazing [VDMX](https://vidvox.net/).

