using System.Web.Mvc;

namespace TestMvcApplication.Context
{
    public interface IUrlHelper
    {
        string Action(ActionResult result);

        bool IsSafe(string url);
    }
}
