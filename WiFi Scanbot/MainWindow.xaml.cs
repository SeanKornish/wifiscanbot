using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using Xceed.Wpf.AvalonDock.Layout;
using WinInterop = System.Windows.Interop;

namespace Pellicom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Window_Code
        public MainWindow()
        {
            InitializeComponent();
            this.SourceInitialized += MainWindow_SourceInitialized;
        }

        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(WindowProc));
        }

        /// <summary>
        /// CloseButton_Clicked
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private static System.IntPtr WindowProc(
              System.IntPtr hwnd,
              int msg,
              System.IntPtr wParam,
              System.IntPtr lParam,
              ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (System.IntPtr)0;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }


        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }


        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// 
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);         

        private void showDashboard_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Dashboard", true);
        }        

        private void newItem_Click(object sender, RoutedEventArgs e)
        {
            NewInventoryItemWizard newItem = new NewInventoryItemWizard();
            newItem.Owner = this;
            newItem.ShowDialog();
        }

        private void manageInventory_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Manage Inventory", true);
        }

        private void inventoryReports_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Inventory Reports", true);
        }

        private void printStockStickers_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Stock Stickers", true);
        }

        private void newListing_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("New Listing", false);
        }

        private void editListings_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Edit Listings", true);
        }

        private void processOrders_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Process Orders", true);
        }

        private void editOrders_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Edit Orders", true);
        }

        private void manageReturns_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Manage Returns", true);
        }

        private void salesReports_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Sales Reports", true);
        }

        private void runReport_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Report", true);
        }

        private void showMessageBoard_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Message Board", true);
        }

        private void genBarCodes_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Generate Bar Codes", true);
        }

        private void showRoyalMailCalc_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Royal Mail Calculator", true);
        }

        private void showStandardTariff_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Standard Tariff", true);
        }

        private void postalData_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Postal Data", true);
        }

        private void userAdmin_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("User Administrator", true);
        }

        private void manageSystemData_Click(object sender, RoutedEventArgs e)
        {
            AddPanel("Manage System Data", true);
        }
        
        #endregion

        private void AddPanel(string panelTitle, bool isSinglePane)
        {
            var documentPane = dockingContentWindow.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (documentPane != null)
            {
                if (isSinglePane)
                {
                    bool panelExists = false;
                    int panelIndex = -1;
                    foreach (LayoutContent item in documentPane.Children)
                    {
                        panelIndex++;
                        if (item.Title == panelTitle)
                        {
                            panelExists = true;
                            break;
                        }
                    }

                    if (panelExists)
                    {
                        documentPane.SelectedContentIndex = panelIndex;
                        return;
                    }
                }

                LayoutDocument layoutDocument = new LayoutDocument { Title = panelTitle };

                Label l = new Label();
                l.Content = "New dashboard added..." + panelTitle;

                layoutDocument.Content = l;

                documentPane.Children.Add(layoutDocument);
                documentPane.SelectedContentIndex = documentPane.Children.Count - 1;
            }
        }
    }
}
