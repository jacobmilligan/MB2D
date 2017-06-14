# MB2D

MB2D (Midnight Blue engine for 2D) is a free and open source (MIT License) 2D game engine built on top of the fantastic MonoGame API. A general purpose engine intended for any genre of 2D game, MB2D is code-only and uses the MonoGame content pipeline for resource management.

## Features

* Complete integration with any MonoGame project, just drop the class library in and make your first scene without any extra trouble.
	* Uses MonoGames sprite batch utility with lots of geometry extensions such as drawing a grid!
* Complete, exhaustive documentation available [here](https://jacobmilligan.github.io/MidnightBlue/)
* Entity-Component based game object system - Flexible and efficient.
* Scene management system with transitions
* Complete UI library with buttons, labels, list boxes etc. defined in ``Views`` that can be attached to any scene
* IO utilities:
	* Text input processor for keyboards
	* ``LastKeyTyped``, ``LastChar`` etc.
* Debug console with tab completion and history that allows adding test and debug functions and variables for use within the game.
* Efficient collision system that fully covers near and broad phases, just add a ``CollisionComponent`` and AABB to an entity. What exactly does efficient mean, though? The MB2D showcase game, Midnight Blue, is checking collisions against 5,000 on-screen AABB's at once at well over 60FPS.
* Physics System - setup a physics environment (with inertia etc.) and the engine takes care of everything else.
	* Currently only supports physics for top-down games but side scrollers and other types are well on the way.
* Event-based input command system - commands can be assigned to one or many keys mapped to triggers.
* Tile Map with collisions
	* Tiles have flags and can change behavior based on them
	* Tile map uses a single tile image and divides it up based on your specifications
	* Support for *Tiled* tile maps is in the works
* Audio triggers and components
* Inventory system is on the way

## Getting Started

Right now, MB2D still has a dependency on MonoGame for spritebatching, content management, and some primitives so you'll need to download that and install on your system. Then download the MB2D DLL from ____ and add to your projects packages.

Everything in MB2D happens in ``Scenes`` which makes getting started with a game super easy - All you need is ``Main`` and a ``Scene``.

First, create a new scene:

```cs
using MB2D.EntityComponent;
using MB2D.Scenes;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExampleMB2D
{
  // My first scene
  public class HelloScene : Scene
  {
    // Initializes a new instance of the HelloScene class.
    public HelloScene(EntityMap gameObjects, ContentManager content) : base(gameObjects, content)
    {

    }

    // Runs any initialization logic and transitions until
    // the transition state is changed
    public override void Initialize()
    {

      // Required otherwise the initialize transition will never end
      TransitionState = TransitionState.None;
    }

    // Handles the input for the scene.
    public override void HandleInput() 
    {
    	// all input logic should go here
    }

    // Updates the scene.
    public override void Update() 
    {
    	// all game logic should happen here
    }

    // Draws the scene to the spriteBatch and uiSpriteBatch.
    // spriteBatch - Sprite batch for world-based entities.
    // uiSpriteBatch - User interface sprite batch for screen-based items
    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
    	// draw everything to the spriteBatch for world-based coordinates and to uiSpriteBatch for camera-based coordinates
    }

    // Exits the scene and runs any transitions until
    // the transition state is changed. Once this transition
    // finishes the scene will be gone for good.
    public override void Exit()
    {
      // Required otherwise the exit transition will never end
      TransitionState = TransitionState.Null;
    }

    // Pauses the scene and runs any transitions until
    // the transition state is changed
    public override void Pause()
    {
      // Required otherwise the pause transition will never end
      TransitionState = TransitionState.None;
    }

    // Resumes the scene and runs any transitions until
    // the transition state is changed
    public override void Resume()
    {
      // Required otherwise the resume transition will never end
      TransitionState = TransitionState.None;
    }
  }
}
```

Now define a Main entry point with a new ``MBGame`` class passing in the type of scene you want the game to start with, in this case ``HelloScene``:

```cs
using MB2D;

namespace ExampleMB2D
{
  static class ExampleMB2D
  {
    private static MBGame game;
    static void Main(string[] args)
    {
      game = new MBGame(typeof(HelloScene));
      game.Run();
      game.Dispose(); // important to get rid of resources
    }
  }
}

```

And that's it! Use the MonoGame pipeline tool to import any resources needed, otherwise enjoy MB2D!

# License

The MIT License (MIT)
Copyright (c) 2016 Jacob Milligan

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
