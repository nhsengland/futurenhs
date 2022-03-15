const helpers = require('../util/helpers');
const basePage = require('./basePage');

class genericPage extends basePage{
    /**
     * Switch function containing known selectors, passed in variables find the desired selector which is then returned
     * @param {string} contentType - switch condition, based on different content types,
     * @param {string} textValue - textual value used within the selectors to find desired element from the html
     * @returns full xpath selector
     */
   _getSelector(contentType, textValue){
       switch(contentType){
            case 'header' : return `//h1[contains(normalize-space(.), "${textValue}")]|//h2[contains(normalize-space(.), "${textValue}")]|//h3[contains(normalize-space(.), "${textValue}")]|//h4[contains(normalize-space(.), "${textValue}")]|//h5[contains(normalize-space(.), "${textValue}")]`
            case 'textual value' : return `//*[contains(normalize-space(.), "${textValue}")]`
            case 'link' : return `//a[contains(normalize-space(.), "${textValue}")]`
            case 'button' : return `//button[contains(normalize-space(.), "${textValue}")]`
            case 'option' : return `//input[@value = "${textValue}"]`
            case "tab" : return `//a[@class="c-tabbed-nav_link"]/span[contains(normalize-space(.), "${textValue}")]`
            case "nav icon" : return `//li/a[@aria-label="${textValue}"]`
            case "label" : return `//label[contains(normalize-space(.), "${textValue}")]`
           default: throw new Error("need content type to be defined");
       }
    }
    
    /**
     * Function to locate content and validate it exists within the system
     * @param {string} contentType - desried content type to validate
     * @param {string} textValue - textual value used within the selector to find desired element from the html
     */
    contentValidation(contentType, textValue){
        helpers.waitForLoaded(this._getSelector(contentType,textValue));
    }
    
    /**
     * Function to validate that content does NOT exist within the system
     * @param {string} contentType - desried content type to validate
     * @param {string} textValue - textual value used within the selector to find desired element from the html
     */
    contentNotExisting(contentType, textValue){
        var content = helpers._resolveElement(this._getSelector(contentType,textValue));
        content.waitForExist({timeout: 5000, reverse: true});
    }

    /**
     * Function to locate desired interactable content and click it
     * @param {string} contentType - desired content type to interact with
     * @param {string} textValue - textual value used within the selector to find desired element from the html
     */
    contentClick(contentType, textValue){
        var element = this._getSelector(contentType, textValue);
        this.findElement(element);
        helpers.click(element);
    }

    /**
     * Click to open the details element to show the available links 
     * @param {string} detailsName - Text value used to locate the element we want to click
     */
    openAccordion(textValue){
        var accordion = $(`//summary[contains(normalize-space(.), "${textValue}")]`);
        helpers.click(accordion);
        expect(accordion.$('..').getProperty('open')).toEqual(true);
    }
    
    /**
     * Function to select an option from an accordion list
     * @param {string} linkValue - textual value of the desired link used as the selector
     */
    accordionSelect(linkValue, groupOption){
        var desiredAccordion = groupOption.replace(' ', '');
        const accordion = {
            actions : $(`//div[@id="group-actions"]/..`),
            menu : $(`//div[@id="header-accordion"]/..`),
            grouppages : $(`//div[@id="my-groups-menu"]/..`),
            groupmenu : $(`//div[@id="group-menu"]/..`)
        }
        var chosenAccordion = accordion[desiredAccordion]
        helpers.click(chosenAccordion);
        var accordionOptions = chosenAccordion.$$('./div/ul/li')
        accordionOptions.forEach(elem => {
            if(elem.getText() === linkValue){
                helpers.click(elem)
                return
            }   
        });
    }

    /**
     * Generic control to validate a dialog box is open and then using the action Type from the step to select the "confirm" or cancel option within the dialog
     * @param {string} actionType - Confirm or Cancel action, used to determine interaction type
     * @param {string} dialogName - used to determine selector of specific element from an array object
     */
    openDialogSelect(actionType, dialogName){
        var desiredDialog = dialogName.toLowerCase().replace(' ', '');
        var dialogs = {
            logout:$('//div[@id="dialog-logout"]'),
            leavegroup:$('//div[@id="dialog-leave-group"]'),
            deletefolder:$('//div[@id="dialog-delete-folder"]')
        }
        var chosenDialog = dialogs[desiredDialog]
        browser.waitUntil(() => chosenDialog.isDisplayed() === true,{timeout: 5000, timeoutMsg: `Unable to locate the "${dialogName}" open dialog`});
        var dialogButtons = {
            cancel:'./div/button[1]',
            confirm:'./div/button[2]',
        }
        helpers.click(chosenDialog.$(dialogButtons[actionType]))
    }

    /**
     * Function to check if the phase banner is existing on the page
     * @returns true OR error page and error log
     */
    checkPhaseBanner(){
        if(browser.getUrl().includes('/admin') || !browser.getUrl().includes('futurenhs')){
            return true
        } else {
            var banner = $('//div[@class="c-phase-banner"]');
            try {
                banner.waitForExist({timeout: 5000});
                expect(banner.getText()).toEqual('ALPHA This is a new service – your feedback will help us to improve it.');
                return true
            } catch (error) {
                var errorPage = browser.getUrl().toString();
                return 'URL = ' + errorPage + ' : ' + error
            }
        }
    }

    /**
     * Text compare validation of the displayed breadcrumbs in the browser
     * @param {string} expectedBreadcrumb - string variable of the expected textual value
     */
    breadcrumbValidation(expectedBreadcrumb){
        var firstBreadcrumb = expectedBreadcrumb.split(' > ')[0]
        var expectedBreadcrumb = expectedBreadcrumb.replace(/ > /g, '');
        var foundBreadcrumb = $(`//ol[@class="c-breadcrumb_list"][*[contains(normalize-space(.), "${firstBreadcrumb}")]]`);
        var breadcrumbText = foundBreadcrumb.getText();
        expect(breadcrumbText).toEqual(expectedBreadcrumb);
    }

    /**
     * 
     */
    groupImageValidation(){
        var groupImg = $(`//div[@class="c-page-header_image-wrapper"]`);
        expect(groupImg.isExisting()).toEqual(true);
    }
   
    /**
     * 
     */
    acceptCookies(){
        var cookieBanner = $(`//div[@class="u-py-6 c-cookie-banner"]`);
        if(cookieBanner.waitForDisplayed({timeout: 5000})){
            var acceptCookiesBtn = cookieBanner.$(`./button[text()="I'm OK with analytics cookies"]`);
            helpers.click(acceptCookiesBtn);
        }
    }    
}
module.exports = new genericPage();