﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;

namespace MLearning.Store.Components
{
    public sealed partial class CircleScrollView : Grid
    {

        ScrollViewer _scrollview;
        StackPanel _contentpanel;
        public CircleScrollView()
        {
 
        }

        void initcontrol()
        {
            _scrollview = new ScrollViewer();
        }

    }
}
