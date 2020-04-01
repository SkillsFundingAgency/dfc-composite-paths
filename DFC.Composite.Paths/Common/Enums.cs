using System.ComponentModel;

namespace DFC.Composite.Paths.Common
{
    public enum Layout
    {
        [Description("No layout")]
        None = 0,

        [Description("Uses a layout which is full width ")]
        FullWidth = 1,

        [Description("Uses a layout where the sidebar is located on the right")]
        SidebarRight = 2,

        [Description("Uses a layout where the sidebar is located on the left")]
        SidebarLeft = 3
    }

    public enum PageRegion
    {
        Head = 1,
        Breadcrumb = 2,
        BodyTop = 3,
        Body = 4,
        SidebarRight = 5,
        SidebarLeft = 6,
        Footer = 7
    }
}
