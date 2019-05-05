# ShareBuddy
A MonoGame lib for sharing things on social media.

To use on Android & iOS, first add the nuget package, then:

```
//create the ShareHelper object
var sharer = new ShareHelper(game);

//Share an image on social media.
sharer.ShareImage(filename, "Share helper test");
```
