using System.Linq;
using OpenQA.Selenium;

namespace MVCForumAutomation
{
    public class RolePermissionsPage
    {
        private readonly IWebDriver _webDriver;

        public RolePermissionsPage(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public void AddToGroup(Group Group, PermissionTypes permissionType)
        {
            var permissionsTable = _webDriver.FindElement(By.ClassName("permissiontable"));

            var GroupRows = permissionsTable.FindElements(By.CssSelector(".permissiontable tbody tr"));
            var GroupRow = GroupRows.Single(row => row.FindElement(By.XPath("./td")).Text == Group.Name);

            var permissionCheckboxes = GroupRow.FindElements(By.CssSelector(".permissioncheckbox input"));
            var permissionCheckbox = permissionCheckboxes[(int) permissionType];
            if (!permissionCheckbox.Selected)
                permissionCheckbox.Click();
        }
    }
}