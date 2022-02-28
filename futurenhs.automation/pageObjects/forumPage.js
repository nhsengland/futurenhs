const helpers = require('../util/helpers');
const basePage = require('./basePage');
var foundAmount = 0

class formPage extends basePage{
    /**
     * Function used to find a desired card through passing the card type through known card type selectors,
     * Once desired selector is set as variable, it is then validated to ensure it exists within the system before returning the full path
     * @param {string} cardType - used for the switch condition to return desired selector
     * @param {string} cardText - textual value of the card used for the selector
     * @returns full xpath selector of the desired card
     */
    cardTypeSelector(cardType, cardText){
        var card = ''
        switch(cardType){
            case "comment" : card = $(`//div[@class="nhsuk-card c-comment u-border-l-theme-8"]/div[div/p[starts-with(normalize-space(.), "${cardText}")]]`) 
            break;
            case "discussion" : card = $(`//div[@class="nhsuk-card u-border-b-theme-10 u-mb-4 nhsuk-card--clickable"]/div[h3/a[contains(text(), "${cardText}")]]`)
            break;
            case "reply" : card = $(`//div[@class="nhsuk-card c-comment c-comment--reply u-border-l-theme-8"]/div[div/p[starts-with(normalize-space(.), "${cardText}")]]`)
            break;
            case "group" : card = $(`//div[@class="nhsuk-card u-border-b-theme-11 u-mb-4 nhsuk-card--clickable"]/div[h3/a[starts-with(normalize-space(.), "${cardText}")]]`)
            break;
        }
        helpers.waitForLoaded(card)
        return card
    }
    
    /**
     * Step used to validate the content of a FNHS Forum card - Checks all values expected exist on specific card.
     * @param {string} cardText - The main body of the card, and used to locate the exact card expected
     * @param {string} cardType - Defines which xPath is required to dependant on Discussion/Content/Reply card
     * @param {Array} expectedValues - Values expected to be on the desired card, these come in the form of an array.
     */
    cardValidation(cardText, cardType, expectedValues){
        if(cardText === 'Comment posted by the automation'){
            cardText = global.postedComment
        }
        var cardFound = this.cardTypeSelector(cardType, cardText);
        var foundValues = cardFound.getText().replace(/\n/g, ', ');
        var valuesArray = foundValues.split(', ');
        try {
            for(var i = 0; i < expectedValues.length; i++){
                var expectedString = expectedValues[i].toString();
                if(expectedString.includes('[PRETTYDATE]')){
                    var dateMatch = valuesArray.find(date => this.dateValidator(date))
                    expect(dateMatch).toBeTruthy();
                } else {
                    expect(foundValues.includes(expectedString)).toEqual(true)
                }
            };
        } catch (error) {
            throw new Error(`Could not locate the expected value of "${expectedString}" within the found values "${foundValues}"`)
        }
    }
    
    /**
     * Function to compare the cards that exist on the page, between state changes. 
     * With the expected scenario being that upon state change the amount of cards found is greater than originally found.
     * @param {string} compareArg - step argument used for second stage of the function to perform comparison 
     */
    cardCountComparison(compareArg, cardType) {
        if(compareArg != "there are more") {
            helpers.waitForLoaded('//div[@class="nhsuk-card u-border-b-theme-10 u-mb-4 nhsuk-card--clickable"]');
            var cardsFound = $$('//div[@class="nhsuk-card u-border-b-theme-10 u-mb-4 nhsuk-card--clickable"]').filter(item => item.isDisplayed());
            foundAmount = cardsFound.length
            expect(foundAmount).toBeGreaterThan(0);
        } else {
            browser.waitUntil(() => {
                var cardsFound = $$('//div[@class="nhsuk-card u-border-b-theme-10 u-mb-4 nhsuk-card--clickable"]').filter(item => item.isDisplayed());
                return cardsFound.length > foundAmount
            },
            {
                timeout: 5000,
                timeoutMsg: `The amount of cards found is not greater than the amount originally found`
            });
        }
    }

    /**
     * Function to get the pagination counter text displayed on the page, and compare against expected value
     * @param {*} text 
     */
    paginationValidation(text) {
        var paginationCounter = $(`//p[@class="c-pagination-status"]`)
        expect(paginationCounter.getText()).toEqual(text)
    }

    /**
     * Two stage function used to like/unlike a card and validate that the like/unlike action has been completed
     * @param {string} clickAction - used for IF condition to define interaction with the card
     * @param {string} cardText - The main body of the card, and used to locate the exact card expected
     * @param {string} cardType - Defines which xPath is required to dependant on Discussion/Content/Reply card
     */
    cardLikeClick(clickAction, cardText, cardType) {
        var cardFound = this.cardTypeSelector(cardType, cardText);
        var likeButton = cardFound.$(`./footer/button`);
        if(clickAction === 'like') {
                expect(likeButton.getAttribute('aria-label')).toBe('like'); 
                helpers.click(likeButton);
                browser.waitUntil(
                    () => (likeButton.getAttribute('aria-label')) === 'Remove like',
                    {
                        timeout: 2000,
                        timeoutMsg: 'Expected the like to exist'
                    }
                );
        } else {
            expect(likeButton.getAttribute('aria-label')).toBe('Remove like'); 
            helpers.click(likeButton);
            browser.waitUntil(
                () => (likeButton.getAttribute('aria-label')) === 'like',
                {
                    timeout: 2000,
                    timeoutMsg: 'Expected the like to have been removed'
                }
            );
        }
    }

    /**
     * Function to validate a pinned card displays the Pinned icon, and the html text is displayed
     * @param {string} cardText - The main body of the card, and used to locate the exact card expected 
     */
    pinnedDiscussionValidation(cardText) {
        var cardFound = this.cardTypeSelector('discussion', cardText);
        console.log(cardFound.$(`./div`).nextElement().getAttribute('class'))
        var pinnedIcon = cardFound.$('./div/svg/use');
        pinnedIcon.waitForDisplayed({timeout: 5000});
        //*[@id="main"]/div[3]/div/div[2]/div/div[2]/div[1]/div[1]/ul/li[1]/div/div/div/svg/use
        //html/body/div[1]/main/div[3]/div/div[2]/div/div[2]/div[1]/div[1]/ul/li[1]/div/div/div/svg/use
    }

    /**
     * Function to click the reply link on a card
     */
    replyToPostClick(cardText, cardType){
        var replyCard = this.cardTypeSelector(cardType, cardText);
        var replyLink = replyCard.$(`./footer/div[@class="c-reply u-flex-grow"]/details/summary`);
        helpers.click(replyLink);
    }
}
module.exports = new formPage();