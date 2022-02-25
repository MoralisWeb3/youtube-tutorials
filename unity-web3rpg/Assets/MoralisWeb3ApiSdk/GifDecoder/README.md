# Gif Decoder

Custom gif decoder written from scratch, designed for Unity engine

There is no gif decoding library for .net, since GifBitmapDecoder is already included in PresentationCore.dll,
but you cant use it in Unity (Since mono doesn't support WPF).

With this library you can decode .gif file from any Stream (file, network, memory, you name it) from any thread.

Features
- 
- Full format support (87a, 89a, transparency, interlacing, discard methods, etc)
- Can be invoked from any thread (since there is no Unity api involved in decoding)
- Uses as little memory allocations as possible
- Extensively tested on thousands of BTTV emotes
- Uses `Color32[]` for color manipulation (Less memory usage and faster texture upload speeds)

Usage
-
Example usage, load all gif frames as textures in single frame
```c#
var frames = new List<Texture>();
var frameDelays = new List<float>();

using (var gifStream = new GifStream(yourFile))
{
    while (gifStream.HasMoreData)
    {
        switch (gifStream.CurrentToken)
        {
            case GifStream.Token.Image:
                var image = gifStream.ReadImage();
                var frame = new Texture2D(
                    gifStream.Header.width, 
                    gifStream.Header.height, 
                    TextureFormat.ARGB32, false); 

                frame.SetPixels32(image.colors);
                frame.Apply();

                frames.Add(frame);
                frameDelays.Add(image.SafeDelaySeconds); // More about SafeDelay below
                break;
            
            case GifStream.Token.Comment:
                var commentText = gifStream.ReadComment();
                Debug.Log(commentText);
                break;

            default:
                gifStream.SkipToken(); // Other tokens
                break;
        }
    }
}
```
This is just an example, and if you want to make most of this library, you should decode gif in separate thread and create textures on main thread.  
Implementation is up to you, but with proper thread management, it can load gifs [like this](https://www.youtube.com/watch?v=KfJb97aV_oc) (Please note, this is slightly older version of this library and few bugs that are visible in this video are no longer present)

Safe Delay
-
Browsers and plenty of other viewers display gifs with 10ms delay using 100ms delay.  
While it is not by specification, there are plenty of gifs which use this delay and look wrong when played at 10ms per frame.
So if you want to use same playback speed as browsers, you should use "Safe" versions of delay

Performance
-
Since gif is archaic format which requires sequential reading of all data (No multithreaded optimizations possible), gif decoding is still very CPU demanding operation, but it doesn't mean that there is no optimizations possible:
- Every allocation is reused, so most of allocations will happen on first frame and only grow when next frame require more data to hold than previous
- Lzw dictionary uses own byte array heap, so insertions and deletions not use allocations
- Pixels decoded from lzw dictionary are placed straight to final location, even when deinterlacing is used, no deinterlace postprocess step

For benchmark: SourPls 3x, 140x140, one global palette, 491 frames  
![GifDecoder](https://cdn.betterttv.net/emote/566ca38765dbbdab32ec0560/3x)  
Tested on Windows Desktop with Ryzen 3900x CPU

This library, ~0.96 ms per frame, 385.1kb of memory allocated
![GifDecoder](.Images/GifDecoderBench.png)

UniGif for comparsion, ~19.27ms per frame, 0.81**gb** of memory allocated
(It doesn't support decoding outside of coroutines, so i evaluated whole coroutine manually in single frame)  
![UniGif](.Images/UniGifBench.png)

Encoding
-
This library doesn't support gif encoding, as it is more sophisticated process than decoding and currently 
i have no programs that require encoding. So no encoding and no plans for implementation.

Portability
-
This library is designed for use in Unity engine, but if for some reason you want to port this somewhere else, 
one and only "engine-dependant" class is `UnityEngine.Color32`, swap it with your implementation and port is completed. 

Changelog
-
- 1.0.0
  - Initial version
- 1.0.1
  - Delay changed to int and few utility methods added to read it as seconds or milliseconds
- 1.0.2
  - Image decoding refactoring, resulting nearly 2x performance boost
- 1.0.3
  - Minor improvements and typo fixes

TODO:
-
- Better documentation
- Utility class for common usecases?
