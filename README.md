# little-godot-rzeka

An example project using [rzeka event-bus architecture](https://github.com/eternalgarden/rzeka), made in `Godot 4.6`.

![](./banner.png)

How to dig around:
- Clone this repo.
- Open the `little-river` project in godot `4.6+`.
- To test rzeka debugger (🐖 very important!) also clone [rzeka](https://github.com/eternalgarden/rzeka).
  - Then in `rzeka` got to `rzeka/ui` and run `npm install` and then `npm run dev` (you will need `node.js` for that).
  - Go in your browser to the localhost url that your console printed.
  - When you start the `little-river` game a websocket connection will be established between the game and that browser tab.
  - This lets you debug both in Godot editor and in a build time.

Character controller code and the level assets taken from [Character Controller C# by expressobits](https://godotengine.org/asset-library/asset/2121), other attributions in `NOTICES.md`.
