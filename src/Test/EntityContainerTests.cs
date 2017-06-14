//
// EntityContainerTests.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MB2D.EntityComponent;
using MB2D.IO;

namespace MB2D.Testing
{
  public struct EntityContainerTests
  {
    public static void EntityContainerTest(params string[] args)
    {
      var timer = new Stopwatch();
      var map = new EntityMap();
      var eList = new List<Entity>();
      int max = 0;
      int maxSamples = 0;
      int.TryParse(args[0], out max);
      int.TryParse(args[1], out maxSamples);

      for ( int i = 0; i < max; i++ ) {
        eList.Add(new Entity(map, "tag"));
      }

      string filename = "/Users/Jacob/Uni/OOP/Midnight_Blue/scripts/test_output/entity_containers_test-" + DateTime.Now.Ticks + ".csv";
      var writer = new StreamWriter(filename);

      int test = 0;
      var eLength = eList.Count;
      for ( int sample = 0; sample < maxSamples; sample++ ) {
        timer.Start();
        for ( test = 0; test < eLength; test++ ) {
          Console.WriteLine(test);
        }
        writer.WriteLine(IOUtil.LineToCSV("0", timer.Elapsed.Milliseconds.ToString()));
        timer.Reset();
      }
      MBGame.Console.Write("Finished entity container test");
      writer.Close();
    }
  }
}

