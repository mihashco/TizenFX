using System.IO;
using IOPath = System.IO.Path;

using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.NUI.Extension;
using System;

namespace Tizen.NUI.Samples
{
    public class RiveTest: IExample
    {
        private Window window;
        private Layer defaultLayer;

        private TextLabel errMsg;

        private int selectedAnimationIndex = -1;
        private AnimationFile[] animationFiles;

        private RiveAnimationView riveAnimationView;

        Button playButton, stopButton, loadButton;

        struct AnimationFile
        {
            public string name;
            public string path;
            public View view;
        }

        View CreateListEntry(string name, int index)
        {
            Color color;
            if (index % 2 == 0)
            {
                color = Color.White;
            }
            else
            {
                color = Color.LightGray;
            }

            var container = new View()
            {
                SizeWidth = 480,
                SizeHeight = 35,
                Margin = new Extents(10, 10, 0, 0),
                BackgroundColor = color,
            };

            var lbl = new TextLabel()
            {
                Text = name,
                SizeWidth = 480,
                SizeHeight = 35,
            };

            container.Add(lbl);

            //create touch event handler on container
            container.TouchEvent += (object source, View.TouchEventArgs args) =>
            {
                if (args.Touch.GetState(0) == PointStateType.Up)
                {
                    //if animation index is > 0 then change selection status
                    if (selectedAnimationIndex >= 0)
                    {
                        animationFiles[selectedAnimationIndex].view.BackgroundColor = color;
                    }

                    container.BackgroundColor = Color.Yellow;
                    this.selectedAnimationIndex = index;
                }

                return true;
            };

            return container;
        }

        //Function find files list and retuns list of strings in the specified directory
        static AnimationFile[] GetRiveFiles(string directory)
        {
            //Get list of files in the specified directory
            // string[] fileList = System.IO.Directory.GetFiles(directory);
            string[] filesList = Directory.GetFiles(directory, "*.riv", SearchOption.AllDirectories);
            AnimationFile[] animationFiles = new AnimationFile[filesList.Length];

            for (int i = 0; i < filesList.Length; i++)
            {
                //Get file name
                string fileName = IOPath.GetFileName(filesList[i]);
                //Get file path
                string filePath = IOPath.GetFullPath(filesList[i]);

                //Add file name and path to the list
                animationFiles[i].name = fileName;
                animationFiles[i].path = filePath;
            }

            return animationFiles;
        }

        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            defaultLayer = window.GetDefaultLayer();

            //Match parent view with linear vertical layout
            var sampleView = new View()
            {
                Layout = new LinearLayout() {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    CellPadding = new Size2D(20, 20),
                },
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };

            //Layout should be flex
            var contentContainer = new View()
            {
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal
                },
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent
            };

            var scrollview = new ScrollableBase
            {
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                BackgroundColor = Color.Gray,
                Position = new Position(10, 120),
            };
            scrollview.ContentContainer.Layout = new LinearLayout() {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                CellPadding = new Size2D(10, 10),
            };
            scrollview.ContentContainer.WidthSpecification = LayoutParamPolicies.MatchParent;

            animationFiles = GetRiveFiles(Tizen.Applications.Application.Current.DirectoryInfo.Resource);
            for (int i = 0; i < animationFiles.Length; i++)
            {
                animationFiles[i].view = CreateListEntry(animationFiles[i].name, i);
                scrollview.ContentContainer.Add(animationFiles[i].view);
            }

            contentContainer.Add(scrollview);

            //Create Control Buttons Container
            View controlButtonsContainer = new View()
            {
                //match parent width
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout() {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    CellPadding = new Size2D(10, 10),
                },
                SizeHeight = 60,
            };

            loadButton = new Button()
            {
                Size = new Size(200, 70),
                Text = "Load"
            };

            loadButton.Clicked += (object source, ClickedEventArgs args) => {
                if (riveAnimationView) { contentContainer.Remove(riveAnimationView); }
                if (errMsg) { defaultLayer.Remove(errMsg);}

                try {
                    riveAnimationView = new Tizen.NUI.Extension.RiveAnimationView(animationFiles[selectedAnimationIndex].path)
                    {
                        WidthSpecification = LayoutParamPolicies.MatchParent,
                        HeightSpecification = LayoutParamPolicies.MatchParent,
                    };
                    contentContainer.Add(riveAnimationView);
                } catch (Exception) {
                  errMsg = new TextLabel() {
                      Text = "Error loading animation",
                      SizeWidth = 500,
                      SizeHeight = 50,
                      Position = new Position(window.Size.Width / 2 - 250, window.Size.Height / 2 - 25),
                  };

                  defaultLayer.Add(errMsg);
                }
            };

            playButton = new Button()
            {
                Size = new Size(200, 70),
                Text = "Play"
            };

            playButton.Clicked += (object source, ClickedEventArgs args) =>
            {
                riveAnimationView.Play();
            };

            stopButton = new Button()
            {
                Size = new Size(200, 70),
                Text = "Stop"
            };

            stopButton.Clicked += (object source, ClickedEventArgs args) =>
            {
                riveAnimationView.Stop();
            };

            controlButtonsContainer.Add(loadButton);
            controlButtonsContainer.Add(playButton);
            controlButtonsContainer.Add(stopButton);

            sampleView.Add(controlButtonsContainer);
            sampleView.Add(contentContainer);

            defaultLayer.Add(sampleView);
        }

        public void Deactivate()
        {
            // if (rav) { defaultLayer.Remove(rav); }
            if (playButton) { defaultLayer.Remove(playButton); }
            if (stopButton) { defaultLayer.Remove(stopButton); }
        }
    }
}
