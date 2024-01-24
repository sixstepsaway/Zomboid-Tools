## Zomboid Tools
### Item List Tool
I've been modding my Project Zomboid recently and I wanted to do some custom loot and maybe some recipes, but I needed to know what items were in my modded game. Rimworld has a [lovely mod](https://steamcommunity.com/sharedfiles/filedetails/?id=2811310482) that lets you output a table of all the modded defs, but I couldn't find anything like that for Zomboid, so I made my own.
 
 ![enter image description here](https://github.com/sixstepsaway/Zomboid-Tools/blob/main/ItemListTool.png?raw=true)

It's simple to use. Boot it up, put your Workshop mods folder into input and an output folder into output and click generate. It'll produce an xml and a json.

If there's interest, I'll add a way of doing this with local mods as well (different folder trees, basically), but I don't see a need rn since I made this for me.

If you have a lot of mods, check the file size before you whip it open in Notepad++ or something and crash the thing - it might be big. I have 900+ mods and my file came out at 45mb. Notepad++ could cope, but it struggled. I recommend LTFViewer if it gets much bigger than that. 

Theoretically the XML/JSON should come without any comments that might be in the script files or other junk you don't need in something like this, but if you find anything weird please raise an [issue](https://github.com/sixstepsaway/Zomboid-Tools/issues) and tell me the mod (preferably with link) and item. If the mod is NSFW please mark it as such. I won't judge you though, I promise. 😂

## Something isn't working!

If you have a problem let me know and I'll try and fix it. Please use the [issue tracker](https://github.com/sixstepsaway/Zomboid-Tools/issues).

## Say Thanks
My work is released completely free, and you can even see the code and ask for help. That said, if my app helps you at all, and you have some spare change, please consider buying me a coffee to say thanks.

  [![](https://storage.ko-fi.com/cdn/kofi3.png)
](http://ko-fi.com/sixstepsaway)
