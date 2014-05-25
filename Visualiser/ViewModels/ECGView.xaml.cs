using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Visualiser.Models;

namespace Visualiser.ViewModels
{
    /// <summary>
    /// Displays ECG signal
    /// </summary>
    public partial class ECGView : UserControl
    {
        private static String _defaultViewName = "Signal name error: no ECG signal loaded to view.";
        private static String _defaultViewDescription = "Signal data error: no ECG signal loaded to view.";
        private static String _viewNameFormat = "Signal name: {0}";
        private static String _viewDescriptionFormat = "Signal data: sampled at {0} samples/second, heart rate: {1} beats/minute, signal length: {2} minutes";

        // data model to be viewed
        private ECG _ecgSignal;
        // plot model that will be displayed
        private PlotModel _plotModel;


        // line series that represent the signal
        private LineSeries _signalView;
        // collection of annoations for the signal
        private PlotElementCollection<Annotation> _annotationsView;
        // data point series that represent them R-spikes
        private ScatterSeries _spikesView;

        public ECGView()
        {
            InitializeComponent();
            AfterInitializeComponent();
        }

        private void AfterInitializeComponent()
        {
            #region plot model main setup
            {
                // instantiate plot model
                _plotModel = new PlotModel()
                {
                    TitleFontSize = 14,
                    SubtitleFontSize = 12,
                    Title = _defaultViewName,
                    Subtitle = _defaultViewDescription
                };
                // subscribe to mouse down event
                _plotModel.MouseDown += _plotModel_MouseDown;
            }
            #endregion

            #region axes initialization
            {
                // x axis instantiation (mm:ss:msec)
                TimeSpanAxis _xAxis = new TimeSpanAxis()
                {
                    Title = "Timestamp",
                    Unit = "milliseconds",
                    TitleFontWeight = OxyPlot.FontWeights.Bold,
                    IsZoomEnabled = false,
                    Position = AxisPosition.Bottom,
                    Minimum = 0,
                    Maximum = 3,
                    AbsoluteMinimum = 0,
                    MajorStep = 0.2,
                    MinorStep = 0.04,
                    StringFormat = "mm:ss:msec",
                    MajorGridlineStyle = LineStyle.Solid,
                    MajorGridlineColor = OxyColor.Parse("#4000B621"),
                    MajorGridlineThickness = 3,
                    MinorGridlineColor = OxyColor.Parse("#4000B621"),
                    MinorGridlineStyle = LineStyle.Solid
                };
                // y axis instantiation (mV)
                LinearAxis _yAxis = new LinearAxis()
                {
                    Title = "Measured voltage",
                    Unit = "millivolts",
                    TitleFontWeight = OxyPlot.FontWeights.Bold,
                    IsZoomEnabled = false,
                    Position = AxisPosition.Left,
                    Minimum = -2,
                    Maximum = 2,
                    AbsoluteMinimum = -2,
                    AbsoluteMaximum = 2,
                    MajorStep = 0.5,
                    MinorStep = 0.1,
                    MajorGridlineStyle = LineStyle.Solid,
                    MajorGridlineColor = OxyColor.Parse("#4000B621"),
                    MajorGridlineThickness = 3,
                    MinorGridlineColor = OxyColor.Parse("#4000B621"),
                    MinorGridlineStyle = LineStyle.Solid
                };
                // add axes to plotmodel
                _plotModel.Axes.Add(_xAxis);
                _plotModel.Axes.Add(_yAxis);
            }
            #endregion

            #region signal series initialization
            {
                // instantiate new line series
                _signalView = new LineSeries()
                {
                    Color = OxyColors.Black,
                    StrokeThickness = 1.2
                };
                // add the line series to the plotmodel
                _plotModel.Series.Add(_signalView);
            }
            #endregion

            #region annotations initialization
            {
                // poor library design made me do this..
                _annotationsView = _plotModel.Annotations;
                
            }
            #endregion 

            #region r-spikes series initialization
            {
                // instantiate new scatter series (individual points)
                _spikesView = new ScatterSeries()
                {
                     MarkerType = MarkerType.Circle,
                     MarkerFill = OxyColors.Red
                };
                _plotModel.Series.Add(_spikesView);
            }
            #endregion
            // some default values
            _areAnnotationsDisplayed = _areCustomSolutionAnnotationsDisplayed = true;

            // after the initialization, set the ECGPlot's model to the one we set up
            ECGPlot.Model = _plotModel;
        }

        private void _plotModel_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
           // we wont be needing this..
           // DataPoint dp = Axis.InverseTransform(e.Position, _plotModel.Axes[0], _plotModel.Axes[1]);            
        }

        /// <summary>
        /// Gets or sets the signal displayed in this view.
        /// </summary>
        public ECG ECGSignal { 
            get {
                return _ecgSignal;
            }

            set {
                // before setting the new signal, clear the current one
                _ecgSignal = null;
                // then refresh the view
                refresh();
                // then set the new value
                _ecgSignal = value;
                // and refresh the view again
                refresh();
            }
        }

        /// <summary>
        /// Clears the current view and all associated data.
        /// </summary>
        public void clear()
        {
            _ecgSignal = null;
            refresh();
        }

        private void dataCleanup()
        {
            ECGPlot.Title = _defaultViewName;
            ECGPlot.Subtitle = _defaultViewDescription;
            _signalView.Points.Clear();
            _spikesView.Points.Clear();
            _annotationsView.Clear();
        }

        /// <summary>
        /// Refreshes the current view. Should be performed after each change of the dataset by shallow copy property.
        /// </summary>
        public void refresh()
        {
            // get rid of all the old data
            dataCleanup();

            // if ecg data is present, make use of it
            if (_ecgSignal != null)
            {
                // set title and subtitle
                _plotModel.Title = String.Format(_viewNameFormat, _ecgSignal.Name);
                _plotModel.Subtitle = String.Format(_viewDescriptionFormat, _ecgSignal.SamplingRate, _ecgSignal.HeartRate, _ecgSignal.Points.Count / (60 * _ecgSignal.SamplingRate));

                // transform all ECGpoints to datapoints and add them to signalview
                _ecgSignal.Points.ForEach(
                    ecgpoint => {
                        _signalView.Points.Add(new DataPoint(ecgpoint.TimeIndex, ecgpoint.Value));
                    });

                // do the same for R-spikes (scatter points)
                loadRSpikesFromSignalView();

                // and do the same for annotations
                loadAnnotationsFromSignalView();
            }

            // in the end, force the view to refresh
            ECGPlot.InvalidatePlot(true);
        }

        private void loadRSpikesFromSignalView()
        {
            _ecgSignal.Spikes.ForEach(
                spike =>
                {
                    _spikesView.Points.Add(new ScatterPoint(spike.TimeIndex, spike.Value));
                });
        }

        private void loadAnnotationsFromSignalView(Boolean filterOutSolutions = false)
        {
            if (_ecgSignal == null || !_areAnnotationsDisplayed)
                return;

            _annotationsView.Clear();

            _ecgSignal.Annotations.ForEach(
                ecgannotation =>
                {
                    if (!_areCustomSolutionAnnotationsDisplayed && ecgannotation.Type == ANNOTATION_TYPE.SOLUTION)
                        return;

                    double lowerTimeIndex = _plotModel.Axes[0].ActualMinimum;
                    double upperTimeIndex = _plotModel.Axes[0].ActualMaximum;

                    // ignore invisible annotations
                    if (ecgannotation.TimeIndex < lowerTimeIndex || ecgannotation.TimeIndex > upperTimeIndex)
                        return;

                    DataPoint positionForAnnotation;
                    switch (ecgannotation.Type){
                        case  ANNOTATION_TYPE.PHYSIONET_STANDARD:
                            positionForAnnotation = new DataPoint(ecgannotation.TimeIndex, -1.8);
                            break;
                        case  ANNOTATION_TYPE.SOLUTION:
                            positionForAnnotation = new DataPoint(ecgannotation.TimeIndex, -1.3);
                            break;
                        default:
                            positionForAnnotation = new DataPoint(ecgannotation.TimeIndex, 1.5);
                            break;
                    }

                    TextAnnotation textAnnotation = new TextAnnotation()
                    {
                        Text = ecgannotation.Text,
                        TextPosition = positionForAnnotation,
                        Background = OxyColors.White
                    };

                    _annotationsView.Add(textAnnotation);
                });
        }


        /// <summary>
        /// Gets or sets status of display of R-spikes.
        /// </summary>
        public Boolean IsQRSDisplayed
        {
            get
            {
                return _spikesView.IsVisible;
            }
            set
            {
                // set the new value
                _spikesView.IsVisible = value;
                // then apply it
                ECGPlot.InvalidatePlot(true);
            }
        }

        private Boolean _areAnnotationsDisplayed;

        /// <summary>
        /// Gets or sets status of display of all annotations. Supersedes all other annotation toggles.
        /// Automatically refreshes the view.
        /// </summary>
        public Boolean AreAnnotationsDisplayed
        {
            get
            {
                return _areAnnotationsDisplayed; 
            }
            set
            {
                _areAnnotationsDisplayed = value;
                // clear old annotations 
                _annotationsView.Clear();

                // if annotations should be displayed, then reload them
                if (_areAnnotationsDisplayed)
                {
                    // turn off other annotations
                    _areCustomSolutionAnnotationsDisplayed = true;
                    // reload 
                    loadAnnotationsFromSignalView(false);
                }

                // do refresh
                ECGPlot.InvalidatePlot(true);
            }
        }

        private Boolean _areCustomSolutionAnnotationsDisplayed;
        
        /// <summary>
        /// Gets or sets status of display of custom solution annotations. Cannot be set to true if AreAnnotationsDisplay=false
        /// Automatically refreshes the view.
        /// </summary>
        public Boolean AreCustomSolutionAnnotationsDisplayed
        {
            get
            {
                return _areCustomSolutionAnnotationsDisplayed;
            }
            set
            {
                // if annotations are turned off, dont do anything (also if the new value is the same as the old one)
                if (!_areAnnotationsDisplayed || _areCustomSolutionAnnotationsDisplayed == value)
                    return;
                else
                {
                    _areCustomSolutionAnnotationsDisplayed = value;

                    // clear all old annotations 
                    _annotationsView.Clear();

                    loadAnnotationsFromSignalView(!_areCustomSolutionAnnotationsDisplayed);

                    // do refresh
                    ECGPlot.InvalidatePlot(true);                      
                }
            }
        }

        private void ECGPlot_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // if there is mouse movement and right key is pressed
            if (e.RightButton == MouseButtonState.Pressed){
                loadAnnotationsFromSignalView(_areCustomSolutionAnnotationsDisplayed);
            }  
        }

    }
}
