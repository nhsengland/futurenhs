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
            case 'link' : return `//a[normalize-space(text()) =  "${textValue}"]`
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
    contentNotDisplayed(contentType, textValue){
        var content = helpers._resolveElement(this._getSelector(contentType,textValue));
        content.waitForDisplayed({timeout: 5000, reverse: true});
    }

    /**
     * Function to locate desired interactable content and click it
     * @param {string} contentType - desired content type to interact with
     * @param {string} textValue - textual value used within the selector to find desired element from the html
     */
    contentClick(contentType, textValue){
        var element = this._getSelector(contentType, textValue);
        var link = this.findElement(element);
        helpers.click(link);
    }

    /**
     * Function used to locate a desired accordion from known values within the NHS app
     * @param {*} accordionName - Name/Tag for each known accordion
     * @returns xpath selector of the accordion that matched the accordionName param
     */
    getAccordion(accordionName){
        var desiredAccordion = accordionName.toLowerCase().replace(/ /g, '');
        const accordion = {
            actions : $(`//div[@id="group-actions"]/..`),
            mobilemenu : $(`//div[@id="header-accordion"]/../summary`),
            usermenu : $(`//div[@id="user-accordion"]/..`),
            grouppages : $(`//div[@id="my-groups-menu"]/..`),
            groupmenu : $(`//div[@id="group-menu"]/..`),
            showmorereplies : $(`//summary[span[text()="Show more replies"]]`),
            editmember : $(`//div[@id="member-update-accordion"]/..`)
        }
        return accordion[desiredAccordion]
    }   

    /**
     * Click to open the details element to show the available links 
     * @param {string} detailsName - Text value used to locate the element we want to click
     */
    openAccordion(textValue){
        var desiredAccordion = this.getAccordion(textValue)
        helpers.click(desiredAccordion);
        if(textValue === 'Show more replies' || textValue === 'Mobile Menu'){
            expect(desiredAccordion.$('..').getProperty('open')).toEqual(true);
        } else {
            expect(desiredAccordion.getProperty('open')).toEqual(true);
        }
    }
    
    /**
     * Function to select an option from an accordion list
     * @param {string} linkValue - textual value of the desired link used as the selector
     */
    selectAccordionItem(linkValue, accordionName){        
        var chosenAccordion = this.getAccordion(accordionName)
        helpers.click(chosenAccordion);
        var accordionOptions = chosenAccordion.$$('//ul/li')
        accordionOptions.forEach(elem => {
            if(elem.getText() === linkValue){
                helpers.click(elem)
                return
            }   
        });
    }

    /**
     * Generic control to validate a dialog box is open and then to select the "confirm" or cancel button within the dialog
     * @param {string} actionType - Confirm or Cancel action, used to determine interaction type
     * @param {string} dialogName - used to determine selector of specific element from an array object
     */
    selectDialogButton(actionType, dialogName){
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
        if(!browser.getUrl().includes('futurenhs')){
            return true
        } else {
            var banner = $('//div[@class="c-phase-banner"]');
            try {
                banner.waitForExist({timeout: 5000});
                expect(banner.getText()).toEqual('BETA This is a new service – your feedback will help us to improve it.');
                return true
            } catch (error) {
                var errorPage = browser.getUrl().toString();
                return 'URL = ' + errorPage + ' : ' + error
            }
        }
    }
    
    /**
     * Function to check if the support banner is existing on the page
     * @returns true OR error page and error log
     */
    checkSupportBanner(){
        if(!browser.getUrl().includes('futurenhs')){
            return true
        } else {
            var banner = $('//p[span[text()="Need help?"]]');
            try {
                banner.waitForExist({timeout: 5000});
                expect(banner.getText().replace(/\n/g, ' ')).toEqual('Need help? Visit our support site');
                return true
            } catch (error) {
                var errorPage = browser.getUrl().toString();
                return 'URL = ' + errorPage + ' : ' + error
            }
        }
    }
   
    /**
     * Function to accept all cookies on the cookie banner popup
     */
    acceptCookies(){
        var cookieBanner = $(`//div[@class="u-py-6 c-cookie-banner"]`);
        if(cookieBanner.isDisplayed()){
            var acceptCookiesBtn = cookieBanner.$(`./button[text()="I'm OK with analytics cookies"]`);
            helpers.click(acceptCookiesBtn);
        }
    }    

    /**
     * function to switch to a new known tab, and do basic validation of content on the page
     * @param {*} url - expected url of new tab
     * @param {*} matcher - string value of expected content on the new tab
     */
    newTabValidation(url, matcher){
        browser.switchWindow(url);
        this.contentValidation('textual value', matcher);
    }
}
module.exports = new genericPage();