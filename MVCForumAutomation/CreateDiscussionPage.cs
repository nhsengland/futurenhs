using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MVCForumAutomation
{
    public class CreateDiscussionPage
    {
        private readonly IWebDriver _webDriver;

        public CreateDiscussionPage(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public string Title
        {
            get { throw new System.NotImplementedException(); }
            set
            {
                var titleInput = _webDriver.FindElement(By.Id("Name"));
                titleInput.SendKeys(value);
            }
        }

        public string Body
        {
            get { throw new System.NotImplementedException(); }
            set
            {
                var iframe = _webDriver.FindElement(By.Id("Content_ifr"));
                _webDriver.SwitchTo().Frame(iframe);
                try
                {
                    var htmlEditor = _webDriver.FindElement(By.Id("tinymce"));
                    htmlEditor.SendKeys(value);
                }
                finally
                {
                    _webDriver.SwitchTo().ParentFrame();
                }
            }
        }

        public void CreateDiscussion()
        {
            var submitButton = _webDriver.FindElement(By.CssSelector("[type=submit]"));
            submitButton.Click();
        }

        public void SelectGroup(Group Group)
        {
            var GroupCombo = _webDriver.FindElement(By.Id("Group"));
            var select = new SelectElement(GroupCombo);
            select.SelectByText(Group.Name);
        }
    }
}