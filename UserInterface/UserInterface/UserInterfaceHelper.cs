#region fileheader
/*  
 * author:  MHI
 * created: 06.06.09
 * description: Helper class with static methods for UserInterface
 * 
 * modification history
 * --------------------
 * 
 */
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UserInterface
{
    /// <summary>
    /// Helper class with static methods for UserInterface
    /// </summary>
    public class UserInterfaceHelper
    {
        #region ScrollText
        /// <summary>
        /// Checks if the the text has changed and adjust the scroll settings.
        /// </summary>
        /// <param name="scrollD">Specific scroll data</param>
        /// <param name="currentText">Current text of the Playing song</param>
        public static void InitScrollText(ScrollData scrollD, String currentText)
        {
            if (scrollD.LastText != currentText)
            {
                scrollD.TextChanged = true;
                scrollD.LastText = currentText;
                scrollD.ScrollPreTicks = 0;
            }
            else
            {
                scrollD.ResetText = true;
            }
        }

        /// <summary>
        /// Scrolls the label text with a scrollview
        /// </summary>
        /// <param name="label">Label to scroll</param>
        /// <param name="scrollD">Data for the scroll</param>
        /// <param name="scrollViewer">Scrollviewer which contains the label</param>
        /// <param name="text">Text to scroll</param>
        /// <param name="seperator">Seperator between the scroll texts</param>
        /// <param name="scrollWaitTime"></param>
        /// <param name="scrollSteps"></param>
        public static void ScrollText(ContentControl label, ScrollData scrollD, ScrollViewer scrollViewer, String text, String seperator,
            int scrollWaitTime, int scrollSteps)
        {
            // Text we want to show in the label
            String preparedText = text + seperator + text;

            // Helps to calculate when we want  to jump back
            String preparedTextJump = text + seperator;

            // If we have a new text we jump to beginning
            if (scrollD.TextChanged)
            {
                scrollViewer.ScrollToLeftEnd();
            }
            else if (scrollD.ResetText && scrollViewer.ScrollableWidth != 0)
            {
                scrollD.ResetText = false;
                label.Content = preparedText;
            }

            // Before scrolling we want to Stop a while
            if (scrollD.ScrollPreTicks < scrollWaitTime)
            {
                scrollD.ScrollPreTicks++;
                return;
            }


            if (scrollD.TextChanged)
            {
                scrollD.TextChanged = false;

                if (scrollViewer.ScrollableWidth == 0)
                {
                    // If now scroll space is avaiable, we dont need to scroll, because
                    // the text fits in the label.
                    label.Content = text;
                }
                else
                {
                    // Calculate when we want to jump back
                    label.Content = preparedTextJump;
                    // Update Layout to get the new size
                    label.UpdateLayout();
                    scrollViewer.UpdateLayout();
                    scrollD.ScrollWidth = scrollViewer.ScrollableWidth + scrollViewer.ViewportWidth;
                    label.Content = preparedText;
                }
            }

            //ScrollAdjust(scrollViewer, label, scrollD);

            // Return if theres nothing to scroll
            if (scrollViewer.ScrollableWidth == 0)
                return;

            // Scroll it
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + scrollSteps);

            //scrollViewer.UpdateLayout();
        }

        /// <summary>
        /// Sets the Offset of the scrollview to the beginning if a specific offset is reached
        /// </summary>
        /// <param name="scrollViewer">Scrollviewer which contains the label</param>
        /// <param name="label">Label to scroll</param>
        /// <param name="scrollD">Data for the scroll</param>
        public static void ScrollAdjust(ScrollViewer scrollViewer, UIElement label, ScrollData scrollD)
        {
            if (scrollViewer.HorizontalOffset < scrollD.ScrollWidth + scrollD.ScrollWidthCorrection) return;

            double offset = scrollViewer.HorizontalOffset - scrollD.ScrollWidth;
            scrollViewer.ScrollToHorizontalOffset(offset);
            label.UpdateLayout();
            scrollViewer.UpdateLayout();
        } 
        #endregion

        #region Visualization
        /// <summary>
        /// Converts a System.Drawing.Color to System.Windows.Media.Color
        /// </summary>
        /// <param name="oldcolor">System.Drawing.Color which will be converted</param>
        /// <returns>Converted System.Drawing.Color </returns>
        public static Color ConvertColor(System.Drawing.Color oldcolor)
        {
            var newColor = new Color { A = oldcolor.A, B = oldcolor.B, G = oldcolor.G, R = oldcolor.R };
            return newColor;
        }

        /// <summary>
        /// Determines the average value of a buffer range
        /// </summary>
        /// <param name="fftBuffer">FFT buffer to use</param>
        /// <param name="from">start element of the buffer to calc the average</param>
        /// <param name="to">end element of the buffer to calc the average</param>
        /// <returns>Returns the average value</returns>
        public static float GetAverageValue(float[] fftBuffer, int from, int to)
        {
            float averageValue = 0;

            for (int i = from; i < to; i++)
            {
                averageValue += fftBuffer[i];
            }
            averageValue /= to - from;

            //Multiplicate with to/x to get a better relation between the lines
            return averageValue * to / 3;
        }

        /// <summary>
        /// If we use steps  in the graph, a line should only get the high of
        /// the step interval. This function fits it.
        /// </summary>
        /// <param name="lineHeight">Height of the panel</param>
        /// <param name="panelHeight"></param>
        /// <param name="stepHeight">Height of the step</param>
        /// <param name="stepDistance">Space between to steps</param>
        /// <returns>Returns fitted height</returns>
        public static int FitHeightToSteps(int lineHeight, int panelHeight, int stepHeight, int stepDistance)
        {
            //Get the height of a step incl. distance
            int completeHeight = stepHeight + stepDistance;

            //The rest shows us if we have to add or remove a bit of the line height
            int rest = (lineHeight % completeHeight);

            //Calculating the new high of the line
            int newHight;

            if (rest >= (completeHeight / 2))
                if (lineHeight <= completeHeight)
                    newHight = completeHeight;
                else
                    newHight = lineHeight - rest + completeHeight;
            else
                newHight = lineHeight - rest;

            //Dont set the line high higher than die panelhight
            return newHight > panelHeight ? panelHeight : newHight;
        }
        #endregion

        /// <summary>
        /// Get the Visual Child Element of a Visual Element 
        /// Important for Scrolling while Drag&Drop
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="referenceVisual"></param>
        /// <returns></returns>
        public static T GetVisualChild<T>(Visual referenceVisual) where T : Visual
        {
            Visual child = null;
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(referenceVisual); i++)
            {
                child = VisualTreeHelper.GetChild(referenceVisual, i) as Visual;

                if (child != null && (child.GetType() == typeof(T)))
                    break;

                if (child == null) continue;
                child = GetVisualChild<T>(child);

                if (child != null && (child.GetType() == typeof(T)))
                    break;
            }

            return child as T;
        }

        /// <summary>
        /// helper function to get all directories from a path
        /// </summary>
        /// <returns>String array with all directories</returns>
        public static ArrayList GetDirectories(String directoryPath)
        {
            ArrayList directories = new ArrayList();

            try
            {
                String[] myPathes = Directory.GetDirectories(directoryPath);

                foreach (string path in myPathes)
                {
                    String[] pathSplitted = path.Split('/');
                    String pathDirName = pathSplitted[pathSplitted.Length - 1];

                    //Add no dirs that starts with '.'
                    if (pathDirName.StartsWith("."))
                        continue;

                    directories.Add(pathDirName);
                }
            }
            catch (DirectoryNotFoundException)
            {
                new Error("Missing Directory!", false, null);
            }
            catch (UnauthorizedAccessException)
            {
                new Error("Unauthorized Access in GetDirectories", false, null);
            }
            catch (ArgumentNullException)
            {
                new Error("Null Argument in GetDirectories", false, null);
            }
            catch (ArgumentException)
            {
                new Error("Argument Error in GetDirectories", false, null);
            }
            catch (PathTooLongException)
            {
                new Error("Path too long in GetDirectories", false, null);
            }
            catch (IOException)
            {
                new Error("IO Error in GetDirectories", false, null);
            }
            catch (NotSupportedException)
            {
                new Error("Not Supported in GetDirectories", false, null);
            }

            return directories;
        }

        /// <summary>
        /// Converts the elapsed time of the song to a human readable time
        /// </summary>
        /// <returns>Formated elapsed time</returns>
        public static string GetFormatedCurrentTime(double time)
        {
            return ((int)(time / 60)).ToString("00;0#") + ":" + ((int)(time % 60)).ToString("00;0#");
        }
    }

    #region ScrollData
    /// <summary>
    /// Class for Title Scrolling
    /// </summary>
    public class ScrollData
    {
        /// <summary>
        /// Constructor for Title - Scrolling
        /// </summary>
        public ScrollData()
        {
            TextChanged = false;
            ResetText = false;
            LastText = "";
            ScrollWidth = 0;
            ScrollPreTicks = 0;
            ScrollWidthCorrection = 0;
        }

        /// <summary>
        /// Test has Changed
        /// </summary>
        public bool TextChanged;

        /// <summary>
        /// The Width of the Scrolled Text
        /// </summary>
        public double ScrollWidth;

        /// <summary>
        /// Correct Album Scroll Data
        /// </summary>
        public double ScrollWidthCorrection; //sometimes it doesnt fit, but i dont know why

        /// <summary>
        /// String to Store the Text Data
        /// </summary>
        public String LastText;

        /// <summary>
        /// Ticks before Scrolling Text
        /// </summary>
        public int ScrollPreTicks;

        /// <summary>
        /// Set this if you want the Text to reset to original Position
        /// </summary>
        public bool ResetText;
    } 
    #endregion


    #region CultureUtilities
    /// <summary>
    /// Utils for CultureInfos
    /// </summary>
    public class CultureUtils
    {
        /// <summary>
        /// Compare-Cass for cultures
        /// </summary>
        internal class CultureComparer : IComparer<CultureInfo>
        {
            /// <summary>
            /// Compares the name of two cultures
            /// </summary>
            public int Compare(CultureInfo x, CultureInfo y)
            {
                return x.EnglishName.CompareTo(y.EnglishName);
            }
        }

        /// <summary>
        /// Returns die avaiable neutral an specefic cultures
        /// </summary>
        /// <returns>Returns a sorted Array of the CultureInfo-Objects</returns>
        public static CultureInfo[] GetAllCultures()
        {
            // Read all available cultures
            CultureInfo[] cultures =
               CultureInfo.GetCultures(
               CultureTypes.FrameworkCultures);

            // sorting
            if (cultures != null)
                Array.Sort(cultures, new CultureComparer());

            return cultures;
        }

        /// <summary>
        /// Returns the avaiable special cultures
        /// </summary>
        /// <returns>Returns a sorted Array of the CultureInfo-Objects</returns>
        public static CultureInfo[] GetSpecificCultures()
        {
            // Read all available cultures
            CultureInfo[] cultures =
               CultureInfo.GetCultures(
               CultureTypes.SpecificCultures);

            // sorting
            if (cultures != null)
                Array.Sort(cultures, new CultureComparer());

            return cultures;
        }

        /// <summary>
        /// Returns die avaiable neutral cultures
        /// </summary>
        /// <returns>Returns a sorted Array of the CultureInfo-Objects</returns>
        public static CultureInfo[] GetNeutralCultures()
        {
            // Read all available cultures
            CultureInfo[] cultures =
               CultureInfo.GetCultures(
               CultureTypes.NeutralCultures);

            // sorting
            if (cultures != null)
                Array.Sort(cultures, new CultureComparer());

            return cultures;
        }
    } 
    #endregion
}