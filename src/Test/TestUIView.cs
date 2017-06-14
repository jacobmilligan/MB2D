//
// 	TestUIView.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 3/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MB2D.UI;

namespace MB2D.Testing
{
  public class TestUIView : UIView
  {
    public TestUIView(ContentManager content) : base(25, 25)
    {
      var normal = content.Load<Texture2D>("Images/uiback");
      var down = content.Load<Texture2D>("Images/uibackselected");
      var font = content.Load<SpriteFont>("Fonts/Bender");
      var btn = new Button(normal, down, down) {
        Font = font,
        TextColor = Color.White,
        TextContent = "Test Button"
      };
      var layout1 = new Layout(this, 20, 20) {
        BorderColor = Color.Black,
        BorderWidth = 2,
      };
      this.Add(layout1, 2, 2, 10, 10);
      layout1.Add(btn, 1, 1, 4, 4);

      var list = new ListControl(font) {
        BorderColor = Color.Tomato,
        BorderDisplayed = true,
        BorderWidth = 2,
        BackgroundColor = Color.Maroon,
        Elements = {
          "Test1",
          "Test2",
          "Test3",
          "Test4",
          "Test5",
          "Test6",
          "Test1",
          "Test2",
          "Test3",
          "Test4",
          "Test5",
          "Test6"
        },
        ItemSpan = 30
      };
      layout1.Add(list, 10, 10, 5, 5);
    }
  }
}
