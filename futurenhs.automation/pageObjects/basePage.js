const helpers = require('../util/helpers')

class basePage {

    /**
     * Function to find the desired element by a given xpath string, specifcally checking for the element that is displayed and not hidden
     * @param {string} path - the string xpath value used to locate the element
     */
    findElement(path){
        var element
        browser.waitUntil(()=> {
            element = $$(path).filter(item => {return item.isDisplayed()})[0]
            return element != undefined
        },
        {
            timeout: 5000,
            timeoutMsg: `Cannot find the element with '${path}'`
        });
        element.waitForEnabled({timeout:5000})
        return element
    }

    /**
     * Generic function to click the FNHS icon to return to the homepage of the platform
     */
    homepageReturn(){
        var homePageLink = $(`//a[@class="c-site-header_logo u-focus-item"]`)
        helpers.click(homePageLink)
    }

    /**
     * Function to validate if text provided is a valid pretty date format
     * @param {string} actualText - text provided to validate against the known Pretty Date formats
     * @returns - true/undefined
     */
    dateValidator(actualText){
        var prettyDateValues = [
            /less than a minute/g,
            /1 minute ago/g,
            /(\d{1,2}) minutes ago/g,
            /about 1 hour ago/g,
            /about (\d{1,2}) hours ago/g,
            /1 day ago/g,
            /(\d{1,2}) days ago/g,
            /about 1 month ago/g,
            /about 2 month ago/g,
            /(\d{1,2}) months ago/g,
            /about 1 year ago/g,
            /over 1 year ago/g,
            /almost 2 years ago/g,
            /about (\d{1,2}) years ago/g,
            /over (\d{1,2}) years ago/g,
            /almost (\d{1,2}) years ago/g,
            /(\d{1,2}) (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) (\d{4})$/g
        ]
        var isMatch = prettyDateValues.find(regex => regex.test(actualText));
        return isMatch != undefined
    }

    /**
     * Function to wait for browser alert and then attempt to accept it
     */
    acceptBrowserAlert(){  
        browser.waitUntil(() => {
            try {
                browser.acceptAlert()
                return true;
            } catch (error) {
                if(error == "no such alert"){ return false }
                throw error
            }},
        { timeout: 5000, timeoutMsg: 'No alert present after 5s' });
    }
}
module.exports = basePage;