const helpers = require('../util/helpers');
const basePage = require('./basePage');
const formPage = require('./formPage');

class adminPage extends basePage{
    /**
     * Custom function to build a new system page for testing purposes. This is data that we intend to edit/manipulate as part of testing
     * @param {string} pageName - The key value we use to set the name of the page, and then can reference when using throughout the rest of the test steps.
     */
    createSystemPages(pageName){
        var origin = browser.getUrl()
        helpers.click('//summary[@class="c-site-header-nav_root-nav-trigger"]')
        helpers.click('//a[@href="/admin"]');
        helpers.waitForLoaded('//a[@class="navbar-brand"]');
        helpers.click('//a[@href="/admin/adminsystempages"]');
        helpers.waitForLoaded('//h5[text()="System Pages"]');
        helpers.click('//a[@href="/admin/adminsystempages/create"]');
        helpers.waitForLoaded('//h5[text()="Create new page"]');
        formPage.inputFieldSet(pageName, `Title`);
        var regex = /\s/g;
        var pageSlug = pageName.toLowerCase().replace(regex, '-');
        formPage.inputFieldSet(pageSlug, `Slug`);            
        formPage.richTextSet('Random Content');
        helpers.click('//*[@id="page-wrapper"]/div/div[2]/div[2]/div[2]/form/div[4]/input');
        browser.waitUntil(() => ($('//h5[text()="System Pages"]').isDisplayed() ===  true),{timeout: 5000, timeoutMsg: `System Pages header was not displayed within 5s`});
        browser.url(origin)
    }
}
module.exports = new adminPage();