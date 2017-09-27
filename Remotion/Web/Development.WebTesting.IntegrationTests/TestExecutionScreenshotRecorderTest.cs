﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.IO;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TestExecutionScreenshotRecorderTest : IntegrationTest
  {
    private string _tempSavePath = "";

    [SetUp]
    public void SetUp ()
    {
      _tempSavePath = Path.GetTempPath() + Path.GetRandomFileName();
     
      while (Directory.Exists (_tempSavePath))
        _tempSavePath = Path.GetTempPath() + Path.GetRandomFileName();

      IntegrationTestSetUp();
    }

    [TearDown]
    public void TearDown ()
    {
      var files = Directory.GetFiles (_tempSavePath, "*.png");

      foreach (var file in files)
        File.Delete (file);

      Directory.Delete (_tempSavePath);

      IntegrationTestTearDown();
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeDesktopScreenshot_SavesToCorrectPath ()
    {
      var tempFileName = "RandomFileName";
      var suffix = "Desktop";
      var extension = "png";

      var fullPath = CombineToFullPath (_tempSavePath, tempFileName, suffix, extension);

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder (_tempSavePath);

      Assert.That (File.Exists (fullPath), Is.False);
      testExecutionScreenshotRecorder.TakeDesktopScreenshot (tempFileName);
      Assert.That (File.Exists (fullPath), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeDesktopScreenshot_ReplacesInvalidFileNameChars ()
    {
      var tempFileName = "<Random\"File\"Na|me>";
      var suffix = "Desktop";
      var extension = "png";

      var fullPath = CombineToFullPath (_tempSavePath, tempFileName, suffix, extension);

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder (_tempSavePath);

      Assert.That (File.Exists (fullPath), Is.False);
      testExecutionScreenshotRecorder.TakeDesktopScreenshot (tempFileName);
      
      var tempFileNameWitCharReplaced = "_Random_File_Na_me_";
      var fullPathWitCharReplaced = CombineToFullPath (_tempSavePath, tempFileNameWitCharReplaced, suffix, extension);
      Assert.That (File.Exists (fullPathWitCharReplaced), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_SavesToCorrectPath ()
    {
      //Just open the browser so we can take a browser Screenshot
      Start();

      var tempFileName = "RandomFileName";
      var suffix = "Browser0-0";
      var extension = "png";

      var fullPath = CombineToFullPath (_tempSavePath, tempFileName, suffix, extension);

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder (_tempSavePath);

      Assert.That (File.Exists (fullPath), Is.False);
      testExecutionScreenshotRecorder.TakeBrowserScreenshot (tempFileName, new[] {Helper.MainBrowserSession }, Helper.BrowserConfiguration.Locator);
      Assert.That (File.Exists (fullPath), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_ReplacesInvalidFileNameChars ()
    {
      //Just open the browser so we can take a browser Screenshot
      Start();

      var tempFileName = "<Random\"File\"Na|me>";
      var suffix = "Browser0-0";
      var extension = "png";

      var fullPath = CombineToFullPath (_tempSavePath, tempFileName, suffix, extension);

      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder (_tempSavePath);

      Assert.That (File.Exists (fullPath), Is.False);
      testExecutionScreenshotRecorder.TakeBrowserScreenshot (tempFileName, new[] {Helper.MainBrowserSession }, Helper.BrowserConfiguration.Locator);
      
      var tempFileNameWitCharReplaced = "_Random_File_Na_me_";
      var fullPathWitCharReplaced = CombineToFullPath (_tempSavePath, tempFileNameWitCharReplaced, suffix, extension);
      Assert.That (File.Exists (fullPathWitCharReplaced), Is.True);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_DoesNotThrowWhenBrowserDisposed ()
    {
      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder (_tempSavePath);
      var fileName = "RandomFileName";

      var browserSession = Helper.CreateNewBrowserSession();

      browserSession.Dispose();


      Assert.That (
          () =>
              testExecutionScreenshotRecorder.TakeBrowserScreenshot (
                  fileName,
                  new[] { browserSession },
                  Helper.BrowserConfiguration.Locator),
          Throws.Nothing); 

      var fullPath = CombineToFullPath (_tempSavePath, fileName, "Browser0-0", "png");
      Assert.That (File.Exists (fullPath), Is.False);
    }

    [Test]
    public void TestExecutionScreenshotRecorderTest_TakeBrowserScreenshot_DoesNotThrowWhenBrowserDisposed_AndTakesNextScreenshotCorrectly ()
    {
      //Just open the browser
      Start();
      var testExecutionScreenshotRecorder = new TestExecutionScreenshotRecorder (_tempSavePath);
      var fileName = "RandomFileName";
      var secondBrowserSession = Helper.CreateNewBrowserSession();


      secondBrowserSession.Dispose();


      Assert.That (
          () =>
              testExecutionScreenshotRecorder.TakeBrowserScreenshot (
                  fileName,
                  new[] { secondBrowserSession, Helper.MainBrowserSession},
                  Helper.BrowserConfiguration.Locator),
          Throws.Nothing); 

      var fullPath = CombineToFullPath (_tempSavePath, fileName, "Browser1-0", "png");
      Assert.That (File.Exists (fullPath), Is.True);
    }

    private string CombineToFullPath (string tempPath, string fileName, string suffix, string extension)
    {
      var fullFileNameWitCharReplaced = string.Format ("{0}.{1}.{2}", fileName, suffix, extension);
      var fullPathWitCharReplaced = string.Concat (tempPath, "/", fullFileNameWitCharReplaced);
      return fullPathWitCharReplaced;
    }

    private MultiWindowTestPageObject Start ()
    {
      return Start<MultiWindowTestPageObject> ("MultiWindowTest/Main.wxe");
    }
  }
}
