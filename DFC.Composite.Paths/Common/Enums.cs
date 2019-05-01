using System.ComponentModel.DataAnnotations;

namespace DFC.Composite.Paths.Common
{
    public enum Layout
    {
        [Display(Description = "Uses a layout which is full width ")]
        FullWidth = 1,

        [Display(Description = "Uses a layout where the siebar is located on the right")]
        SidebarRight = 2,

        [Display(Description = "Uses a layout where the siebar is located on the left")]
        SidebarLeft3
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
