﻿using System;
using UnityEngine;
using KSP;
using NavyFish;
using KSP.IO;
using System.Collections.Generic;


namespace NavyFish
{
    public class DPAI_RPM : InternalModule
    {
        [KSPField]
        public int buttonUp;// = 0;

        [KSPField]
        public int buttonDown;// = 1;

        [KSPField]
        public int buttonEnter;// = 2;

        [KSPField]
        public int buttonEsc;// = 3;

        [KSPField]
        public int buttonHome;// = 4;

        [KSPField]
        public int buttonRight;// = 5;

        [KSPField]
        public int buttonLeft;// = 6;

        [KSPField]
        public int buttonNext;// = 7;

        [KSPField]
        public int buttonPrev;// = 8;

        private bool showHelp = false;


        public override void OnAwake()
        {
            //Debug.Log("DPAI: DPAI_RPM OA");
            TextReader tr = KSP.IO.TextReader.CreateForType<DockingPortAlignmentIndicator>("RPM_helpscreen.txt", null);
            List<string> textStrings = new List<string>();
            while (!tr.EndOfStream)
            {
                textStrings.Add(tr.ReadLine());
            }
            tr.Close();
            tr.Dispose();

            helpString = "";
            foreach (string str in textStrings)
            {
                //Debug.Log("DPAI:  " + str);
                helpString += str;
                helpString += Environment.NewLine;
            }

            tr = KSP.IO.TextReader.CreateForType<DockingPortAlignmentIndicator>("RPM_mainscreen.txt", null);
            textStrings.Clear();
            while (!tr.EndOfStream)
            {
                textStrings.Add(tr.ReadLine());
            }
            tr.Close();
            tr.Dispose();

            helpLabel = "";
            foreach (string str in textStrings)
            {
                //Debug.Log("DPAI:  " + str);
                helpLabel += str;
                helpLabel += Environment.NewLine;
            }
        }

        public void pageActiveMethod(bool pageActive, int pageNumber)
        {
            //Debug.Log("page active = " + pageActive);
            if (pageActive)
            {
                DockingPortAlignmentIndicator.RPMPageActive = true;
                showHelp = false;
            }
            else
            {
                DockingPortAlignmentIndicator.RPMPageActive = false;
            }
        }

        int bottomGutter = 55;
        private static Rect gaugeRect = new Rect();
        public bool DrawDPAI(RenderTexture screen, float aspectRatio)
        {
            if (HighLogic.LoadedSceneIsEditor) return false;

            if (showHelp)
            {
                return false;
            }

                        
            GL.PushMatrix();

            GL.LoadPixelMatrix(0, screen.width, screen.height, 0);
            GL.Viewport(new Rect(0, 0, screen.width, screen.height));
            GL.Clear(true, true, Color.black);

            float vH = screen.height - bottomGutter;
            float vW = screen.width;
            vH = Math.Min(vH, vW);
            vW = vH;

            float hOffset = (screen.width - vW) / 2f;

            gaugeRect.Set(hOffset, 0, vW, vH);

            //Graphics.Blit(DockingPortAlignmentIndicator.guiRenderTexture, screen);

            Graphics.DrawTexture(gaugeRect, DockingPortAlignmentIndicator.guiRenderTexture);

            Drawing.DrawHorizontalLineGraphics(0, screen.height-bottomGutter, screen.width, 2, Color.white);

            DockingPortAlignmentIndicator.drawRPMText(screen.width, screen.height);

            ////drawRPMtest(screen, DockingPortAlignment.mat_background);
            //RenderTexture lastRT = RenderTexture.active;
            //RenderTexture.active = screen;
            //DockingPortAlignmentIndicator.drawIndicatorContentsRPM(screen.width, screen.height);
            //DockingPortAlignmentIndicator.rpmRenderTexture = screen;
            //RenderTexture.active = lastRT;

            GL.PopMatrix();
            return true;
        }

        private bool renameLeft = true;

        public void ButtonProcessor(int buttonID)
        {
            //Debug.Log("button pressed: " + buttonID);
            if (buttonID == buttonLeft)
            {
                showHelp = !showHelp;
            }
            else
            {
                //Debug.Log("Button Pressed: " + buttonID);
                switch (buttonID)
                {
                    case 0:
                        //Debug.Log("buttonUp");
                        ModuleDockingNodeNamed.renameWindow.closeWindow();
                        DockingPortAlignmentIndicator.cycleReferencePoint(-1);
                        break;
                    case 1:
                        //Debug.Log("buttonDown");
                        ModuleDockingNodeNamed.renameWindow.closeWindow();
                        DockingPortAlignmentIndicator.cycleReferencePoint(1);
                        break;
                    case 2:
                        //Debug.Log("buttonEnter");
                        break;
                    case 3:
                        //Debug.Log("buttonEsc");
                        break;
                    case 4:
                        //Debug.Log("buttonHome");
                        //ModuleDockingNodeNamed.renameWindow.closeWindow();
                        if (renameLeft)
                        {
                            bool result = renameTarget();
                            if (result)
                            {
                                renameLeft = !renameLeft;
                                return;
                            }
                            else
                            {
                                result = renameReference();
                                if (result) renameLeft = !renameLeft;

                                return;
                            }
                        }
                        else
                        {
                            bool result = renameReference();
                            if (result)
                            {
                                renameLeft = !renameLeft;
                                return;
                            }
                            else
                            {
                                result = renameTarget();
                                if (result) renameLeft = !renameLeft;

                                return;
                            }
                        }

                    case 5:
                        //Debug.Log("buttonRight");
                        //DockingPortAlignment.cyclePortRight();
                        break;
                    case 6:
                        ModuleDockingNodeNamed.renameWindow.closeWindow();
                        showHelp = true;
                        //Debug.Log("buttonLeft");
                        //DockingPortAlignment.cyclePortLeft();
                        break;
                    case 7:
                        //Debug.Log("buttonNext");
                        ModuleDockingNodeNamed.renameWindow.closeWindow();
                        DockingPortAlignmentIndicator.cyclePortLeft();
                        break;
                    case 8:
                        //Debug.Log("buttonPrev");
                        ModuleDockingNodeNamed.renameWindow.closeWindow();
                        DockingPortAlignmentIndicator.cyclePortRight();
                        break;
                }
            }

            //Debug.Log("Show Help " + showHelp);

        }

        private bool renameTarget()
        {
            //Rename the target
            ModuleDockingNodeNamed toRename = DockingPortAlignmentIndicator.targetNamedModule;
            if (toRename != null)
            {
                DockingPortAlignmentIndicator.setRenameHighlightBoxRPM(DockingPortAlignmentIndicator.HighlightBox.LEFT);
                ModuleDockingNodeNamed.renameWindow.DisplayForNode(toRename, "Rename Target Port");
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool renameReference()
        {
            //Rename the reference
            ModuleDockingNodeNamed toRename = DockingPortAlignmentIndicator.referencePartNamed;
            if (toRename != null)
            {
                DockingPortAlignmentIndicator.setRenameHighlightBoxRPM(DockingPortAlignmentIndicator.HighlightBox.RIGHT);
                ModuleDockingNodeNamed.renameWindow.DisplayForNode(toRename, "Rename Reference Port");
                return true;
            }
            else
            {
                return false;
            }
        }

        public string getPageText(int screenWidth, int screenHeight)
        {
            //Debug.Log("getPageText");
            if (showHelp)
            {
                return helpString;
            }
            else
            {
                return helpLabel;
            }
        }
        private string helpLabel;

        private string helpString;
    }
}