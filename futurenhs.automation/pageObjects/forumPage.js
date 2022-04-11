const helpers = require('../util/helpers');
const basePage = require('./basePage');
const contentCard = require('./contentCard');

var foundAmount = 0

class formPage extends basePage{    
    /**
     * Function to get the pagination counter text displayed on the page, and compare against expected value
     * @param {*} text 
     */
    paginationValidation(text) {
        var paginationCounter = $(`//p[@class="c-pagination-status"]`)
        expect(paginationCounter.getText()).toEqual(text)
    }

    /**
     * Function to validate a pinned card displays the Pinned icon, and the html text is displayed
     * @param {string} cardText - The main body of the card, and used to locate the exact card expected 
     */
    pinnedDiscussionValidation(cardText) {
        var cardFound = contentCard.cardTypeSelector('discussion', cardText);
        var cardValues = cardFound.getText().split('\n')
        var stickyExists = cardValues.filter(word => word.includes('Sticky:'));
        expect(stickyExists.toString()).toEqual('Sticky:');
    }

    /**
     * Function to click the reply link on a card
     */
    replyToPostClick(cardText, cardType){
        var replyCard = contentCard.cardTypeSelector(cardType, cardText);
        var replyLink = replyCard.$(`./footer/div[starts-with(@class, "c-reply")]/details/summary`);
        helpers.click(replyLink);
    }
    /**
     * Two stage function used to like/unlike a card and validate that the like/unlike action has been completed
     * @param {string} clickAction - used for IF condition to define interaction with the card
     * @param {string} cardText - The main body of the card, and used to locate the exact card expected
     * @param {string} cardType - Defines which xPath is required to dependant on Discussion/Content/Reply card
     */
    commentLikeClick(clickAction, cardText, cardType) {
        var cardFound = contentCard.cardTypeSelector(cardType, cardText);
        cardFound.waitForExist();
        var likeButton = cardFound.$(`./footer/button`);
        if(clickAction === 'like') {
                expect(likeButton.getAttribute('aria-label')).toBe('like'); 
                helpers.click(likeButton);
                browser.pause(1500);
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
            browser.pause(1500);
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
    * Function to compare the cards that exist on the page, between state changes. 
    * With the expected scenario being that upon state change the amount of cards found is greater than originally found.
    * @param {string} compareArg - step argument used for second stage of the function to perform comparison 
    */
    discussionCountComparator(compareArg, cardType) {
        if(compareArg != "there are more") {
            helpers.waitForLoaded('//div[starts-with(@id, "discussion-")]');
            var cardsFound = $$('//div[starts-with(@id, "discussion-")]').filter(item => item.isDisplayed());
            foundAmount = cardsFound.length
            expect(foundAmount).toBeGreaterThan(0);
        } else {
            browser.waitUntil(() => {
                var cardsFound = $$('//div[starts-with(@id, "discussion-")]').filter(item => item.isDisplayed());
                return cardsFound.length > foundAmount
            },
            {
                timeout: 5000,
                timeoutMsg: `The amount of cards found is not greater than the amount originally found`
            });
        }
    }
}
module.exports = new formPage();