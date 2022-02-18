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
            case "comment" : card = $(`//div[@class="c-card c-card--comment u-border-left-theme-8"]/div[div/p[starts-with(normalize-space(.), "${cardText}")]]`) 
            break;
            case "discussion" : card = $(`//div[@class="c-card u-border-bottom-theme-10"]/div[h2/a[contains(text(), "${cardText}")]]`)
            break;
            case "post" : card = $(`//div[@class="c-card c-card--post u-border-left-theme-8"]/div[h2[starts-with(normalize-space(.), "${cardText}")]]`)
            break;
            case "reply" : card = $(`//div[@class="c-card c-card--reply u-border-left-theme-8"]/div[p[starts-with(normalize-space(.), "${cardText}")]]`)
            break;
            case "group" : card = $(`//div[@class="c-card u-border-bottom-theme-11 u-mb-4"]/div[h3/a[starts-with(normalize-space(.), "${cardText}")]]`)
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
        try {
            for(var i = 0; i < expectedValues.length; i++){
                var expectedString = expectedValues[i].toString();
                if(expectedString.includes('[PRETTYDATE]')){
                    expectedString = expectedString.split(' [')[0];
                }
                expect(foundValues.includes(expectedString)).toEqual(true)
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
    cardCountComparison(compareArg) {
        if(compareArg != "there are more") {
            helpers.waitForLoaded('//div[@class="c-card_body"]');
            var cardsFound = $$('//div[@class="c-card_body"]').filter(item => item.isDisplayed());
            foundAmount = cardsFound.length
            expect(foundAmount).toBeGreaterThan(0);
        } else {
            browser.waitUntil(() => {
                var cardsFound = $$('//div[@class="c-card_body"]').filter(item => item.isDisplayed());
                return cardsFound.length > foundAmount
            },
            {
                timeout: 5000,
                timeoutMsg: `The amount of cards found is not greater than the amount originally found`
            });
        }
    }

    /**
     * Two stage function used to like/unlike a card and validate that the like/unlike action has been completed
     * @param {string} clickAction - used for IF condition to define interaction with the card
     * @param {string} cardText - The main body of the card, and used to locate the exact card expected
     * @param {string} cardType - Defines which xPath is required to dependant on Discussion/Content/Reply card
     */
    cardLikeClick(clickAction, cardText, cardType) {
        var cardFound = this.cardTypeSelector(cardType, cardText);
        var likeButton = cardFound.$(`./div[3]/p[1]/a`);
        if(clickAction === 'like') {
                expect(likeButton.getAttribute('data-hasvoted')).toBe('false'); 
                helpers.click(likeButton);
                browser.waitUntil(
                    () => (likeButton.getAttribute('data-hasvoted')) === 'true',
                    {
                        timeout: 2000,
                        timeoutMsg: 'expected the like button to be highlighted'
                    }
                );
        } else {
            expect(likeButton.getAttribute('data-hasvoted')).toBe('true'); 
            helpers.click(likeButton);
            browser.waitUntil(
                () => (likeButton.getAttribute('data-hasvoted')) === 'false',
                {
                    timeout: 2000,
                    timeoutMsg: 'expected the like button to not be highlighted'
                }
            );
        }
    }

    /**
     * Function to validate that "Load more replies" button does not exist on the desired card
     * @param {string} cardText - The main body of the card, and used to locate the exact card expected
     */
    loadRepliesNotExisting(cardText) {
        var cardFound = this.cardTypeSelector('comment', cardText);
        var loadMoreButton = cardFound.$(`button`);
        loadMoreButton.waitForExist({timeout: 5000, reverse: true});
    }

    /**
     * Function to validate a pinned card displays the Pinned icon, and the html text is displayed
     * @param {string} cardText - The main body of the card, and used to locate the exact card expected 
     */
    pinnedDiscussionValidation(cardText) {
        var cardFound = this.cardTypeSelector('discussion', cardText);
        var pinnedIcon = cardFound.$('./div[2]/div/p');
        pinnedIcon.waitForExist({timeout: 5000});
        expect(cardFound.$('./div[2]/div/p/span').getHTML(false)).toBe('Discussion is pinned')
    }

    /**
     * Function to click the reply link on a card
     */
    replyToPostClick(cardText, cardType){
        var replyCard = this.cardTypeSelector(cardType, cardText);
        var replyLink = replyCard.$(`./div[@class="c-card_footer"]//a/span[normalize-space(.) = "Reply"]`);
        helpers.click(replyLink);
    }
}
module.exports = new formPage();