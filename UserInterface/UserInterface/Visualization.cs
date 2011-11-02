/* 
 * author: MHO
 * 
 * created: 21.11.2008
 * 
 * modification history
 * --------------------
 * MHO:
 * New Vis-Method steps added
 * 
 * 
 * 
 */

using System;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;


namespace UserInterface
{
    /// <summary>
    /// Draws a spectrum graph to a WPF element
    /// </summary>
    class Visualization
    {
        private readonly Rectangle[] _lines;
        private Rectangle[] _steps;
        private bool _isInitialized;
        private LinearGradientBrush _myVerticalGradient;
        private SolidColorBrush _mySolidColorBrush;
        private float _dynamicOffset;
        private bool _dynamicOffsetUsed;
        private readonly float _offsetStepsUp;
        private readonly float _offsetStepsDown;

        /// <summary>
        /// Initialize the private variables
        /// </summary>
        /// <param name="bufferSize">Size of the FFT Buffer</param>
        /// <param name="timerSpeed"></param>
        public Visualization(int bufferSize, int timerSpeed)
        {
            _lines = new Rectangle[bufferSize / 2];
            _isInitialized = false;
            _dynamicOffset = 1;
            _dynamicOffsetUsed = false;
            _offsetStepsUp = timerSpeed/12500F;
            _offsetStepsDown = timerSpeed / 5000F;      
        }

        public class SpectrumLineParams
        {
            private readonly Canvas _panel;
            private readonly float[] _fftBuffer;
            private readonly int _width;
            private readonly int _height;
            private readonly System.Drawing.Color _color1;
            private readonly System.Drawing.Color _color2;
            private readonly System.Drawing.Color _background;
            private readonly int _linewidth;
            private readonly int _distance;
            private readonly bool _centerView;
            private readonly bool _useSteps;
            private readonly int _stepHeight;
            private readonly int _stepDistance;
            private readonly bool _dynamic;

            /// <summary>Initialize Params</summary>
            /// <param name="panel">WPF canvas Panel</param>
            /// <param name="fftBuffer">Buffer with the FFT Data</param>
            /// <param name="width">Width of the WPF canvasa</param>
            /// <param name="height">Height of the WPF canvas</param>
            /// <param name="color1">First color of the line</param>
            /// <param name="color2">Second color of the line</param>
            /// <param name="background">Background Color of the panel</param>
            /// <param name="linewidth">Width of the line</param>
            /// <param name="distance">Margin between two lines</param>
            /// <param name="stepDistance"></param>
            /// <param name="dynamic">Reduce all line if one is heigher than the panel</param>
            /// <param name="centerView"></param>
            /// <param name="useSteps"></param>
            /// <param name="stepHeight"></param>
            public SpectrumLineParams(Canvas panel, float[] fftBuffer, int width, int height, System.Drawing.Color color1, System.Drawing.Color color2, System.Drawing.Color background, int linewidth, int distance, bool centerView, bool useSteps, int stepHeight, int stepDistance, bool dynamic)
            {
                _panel = panel;
                _fftBuffer = fftBuffer;
                _width = width;
                _height = height;
                _color1 = color1;
                _color2 = color2;
                _background = background;
                _linewidth = linewidth;
                _distance = distance;
                _centerView = centerView;
                _useSteps = useSteps;
                _stepHeight = stepHeight;
                _stepDistance = stepDistance;
                _dynamic = dynamic;
            }

            public Canvas Panel
            {
                get { return _panel; }
            }

            public float[] FftBuffer
            {
                get { return _fftBuffer; }
            }

            public int Width
            {
                get { return _width; }
            }

            public int Height
            {
                get { return _height; }
            }

            public System.Drawing.Color Color1
            {
                get { return _color1; }
            }

            public System.Drawing.Color Color2
            {
                get { return _color2; }
            }

            public System.Drawing.Color Background
            {
                get { return _background; }
            }

            public int Linewidth
            {
                get { return _linewidth; }
            }

            public int Distance
            {
                get { return _distance; }
            }

            public bool CenterView
            {
                get { return _centerView; }
            }

            public bool UseSteps
            {
                get { return _useSteps; }
            }

            public int StepHeight
            {
                get { return _stepHeight; }
            }

            public int StepDistance
            {
                get { return _stepDistance; }
            }

            public bool Dynamic
            {
                get { return _dynamic; }
            }
        }

        /// <summary>
        /// Draws a spectrum line graph to a WPF element
        /// </summary>
        public void CreateSpectrumLine(SpectrumLineParams spectrumLineParams)
        {
            //Calc useable bufferSize
            //The values of the second part of the buffer are normally too low 
            int bufferSize = spectrumLineParams.FftBuffer.Length / 2;
            
            //Determine number of lines
            int lineCount = spectrumLineParams.Width / (spectrumLineParams.Linewidth + spectrumLineParams.Distance);

            //Determine fft intervall
            int intervall = bufferSize / lineCount;

            //Reset dynamic offset used sign
            _dynamicOffsetUsed = false;

            //Initialize all Lines and add them to the panel
            if (!_isInitialized)
            {
                spectrumLineParams.Panel.Children.Clear();
                InitVerticalGradient(spectrumLineParams.Color1, spectrumLineParams.Color2);

                _mySolidColorBrush = new SolidColorBrush(UserInterfaceHelper.ConvertColor(spectrumLineParams.Color1));
                
                for (int i = 0; i < lineCount; ++i)
                {
                    _lines[i] = new Rectangle();
                    //If both, color1 and color2 equals, use a SolidColorBrush
                    if (spectrumLineParams.Color1.Equals(spectrumLineParams.Color2))
                        _lines[i].Fill = _mySolidColorBrush;   
                    else
                        _lines[i].Fill = _myVerticalGradient;
                    
                    _lines[i].Width = spectrumLineParams.Linewidth;
                    spectrumLineParams.Panel.Children.Add(_lines[i]);
                    spectrumLineParams.Panel.Background = new SolidColorBrush(UserInterfaceHelper.ConvertColor(spectrumLineParams.Background));
                }
                if(spectrumLineParams.UseSteps)
                    DrawSteps(spectrumLineParams.Panel, spectrumLineParams.Height, spectrumLineParams.Width, spectrumLineParams.StepHeight, spectrumLineParams.StepDistance, UserInterfaceHelper.ConvertColor(spectrumLineParams.Background));
            }

            //Specify the height and margin
            for (int i = 0; i < lineCount; ++i)
            {
                _lines[i].Height = FitHeight(spectrumLineParams.Height, UserInterfaceHelper.GetAverageValue(spectrumLineParams.FftBuffer, i * intervall, (i + 1) * intervall), spectrumLineParams.Dynamic);
                if (spectrumLineParams.UseSteps) _lines[i].Height = UserInterfaceHelper.FitHeightToSteps((int)_lines[i].Height, spectrumLineParams.Height, spectrumLineParams.StepHeight, spectrumLineParams.StepDistance);
                double lineVerticalPos = spectrumLineParams.CenterView ? spectrumLineParams.Height / 2.0 - _lines[i].Height / 2 : spectrumLineParams.Height - _lines[i].Height;
                _lines[i].Margin = new System.Windows.Thickness(i * spectrumLineParams.Linewidth + (i + 1) * spectrumLineParams.Distance, lineVerticalPos, 0, 0);
                _lines[i].Margin = new System.Windows.Thickness(i * spectrumLineParams.Linewidth + (i + 1) * spectrumLineParams.Distance, lineVerticalPos, 0, 0);
            }

            //If offset was not used all line hights were ok, so we can reduce the offset 
            if (!_dynamicOffsetUsed)
            {
                if (_dynamicOffset < 1)
                    _dynamicOffset += _offsetStepsUp;
                
            }

            _isInitialized = true;

        }
        
        /// <summary>
        /// Create an Vertical Gradient
        /// </summary>
        /// <param name="color1">First color for the gradient</param>
        /// <param name="color2">Second color for the gradient</param>
        private void InitVerticalGradient(System.Drawing.Color color1, System.Drawing.Color color2)
        {
            _myVerticalGradient = new LinearGradientBrush
                                      {
                                          StartPoint = new System.Windows.Point(0, 0.0),
                                          EndPoint = new System.Windows.Point(0, 1)
                                      };
            _myVerticalGradient.GradientStops.Add(new GradientStop(UserInterfaceHelper.ConvertColor(color1), 0.0));
            _myVerticalGradient.GradientStops.Add(new GradientStop(UserInterfaceHelper.ConvertColor(color2), 1.5));
            _myVerticalGradient.SpreadMethod = GradientSpreadMethod.Reflect;
        }


        

        /// <summary>
        /// Fit the value to the specified high
        /// </summary>
        /// <param name="height">Height of the panel</param>
        /// <param name="value">Value to fit</param>
        /// <param name="dynamic"></param>
        /// <returns>Returns the fitted high</returns>
        private int FitHeight(int height, float value, bool dynamic)
        {
            int lineHeight = (int)(Math.Abs(value) * height/0.3);

            if(dynamic)
            {
                lineHeight = (int)(lineHeight * _dynamicOffset);
                if (lineHeight < 0) lineHeight = 0;
            }

            if (lineHeight < height)
                return lineHeight;
            
            if (dynamic && !_dynamicOffsetUsed)
            {
                _dynamicOffset -= _offsetStepsDown;
                _dynamicOffsetUsed = true;
            }
            return height - 1;
        }

        /// <summary>
        /// Draw white horizontal lines to simulate small steps in the visualization
        /// </summary>
        /// <param name="panel">Panel to draw</param>
        /// <param name="height">Hight of the Panel</param>
        /// <param name="width">Width of the Panel</param>
        /// <param name="stepHeight">Height of the step</param>
        /// <param name="stepDistance">Space between two steps</param>
        /// <param name="color">Color of the lines (normally the backgroundcolor)</param>
        private void DrawSteps(Panel panel, int height, int width, int stepHeight, int stepDistance, Color color)
        {
            _steps = new Rectangle[height / (stepHeight + stepDistance)];
            for (int i = 0; i < _steps.Length; i++)
            {
                _steps[i] = new Rectangle
                                {
                                    Height = stepDistance,
                                    Width = width,
                                    Margin =
                                        new System.Windows.Thickness(0, height - (i + 1)*(stepHeight + stepDistance), 0,
                                                                     0),
                                    Fill = new SolidColorBrush(color)
                                };

                panel.Children.Add(_steps[i]);
            }
        }

        /// <summary>
        /// Call this function if you have draw an visualization and want to draw another visualization with other parameter
        /// </summary>
        public void Reset()
        {
            _isInitialized = false;
        }

    }
}


