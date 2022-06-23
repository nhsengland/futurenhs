const helpers = require('../util/helpers');
const basePage = require('./basePage');

class contentCard extends basePage {
    /**
     * Function used to find a desired card through passing the card type through known card type selectors,
     * Once desired selector is set as variable, it is then validated to ensure it exists within the system before returning the full path
     * @param {string} cardType - used for the switch condition to return desired selector
     * @param {string} cardText - textual value of the card used for the selector
     * @returns full xpath selector of the desired card
     */
    cardTypeSelector(cardType, cardText) {
        var desiredCard = cardType.toLowerCase().replace(/ /g, '');
        const card = {
            reply: $(`//div[starts-with(@id, "comment-")]/div[div/p[starts-with(normalize-space(.), "${cardText}")]]`),
            comment: $(`//div[starts-with(@id, "comment-")]/div[div/p[starts-with(normalize-space(.), "${cardText}")]]`),
            discussion: $(`//div[starts-with(@id, "discussion-")]/div[h3/a[contains(text(), "${cardText}")]]`),
            group: $(`//div[starts-with(@id, "group-")]/div[h3/a[starts-with(normalize-space(.), "${cardText}")]]`),
            searchresult: $(`//li[h2/a[starts-with(normalize-space(.), "${cardText}")]]`),
        };
        helpers.waitForLoaded(card[desiredCard]);
        return card[desiredCard];
    }

    /**
     * Function to click the main content link on a card
     * @param {*} cardType - type of card used for the type selector
     * @param {*} cardLink - textual value of the desired link
     */
    cardLinkClick(cardLink, cardType) {
        var cardFound = this.cardTypeSelector(cardType, cardLink);
        var link = cardFound.$(`//a[contains(normalize-space(.), "${cardLink}")]`);
        helpers.click(link);
    }

    /**
     * Step used to validate the content of a FNHS Forum card - Checks all values expected exist on specific card.
     * @param {string} cardText - The main body of the card, and used to locate the exact card expected
     * @param {string} cardType - Defines which xPath is required to dependant on Discussion/Content/Reply card
     * @param {Array} expectedValues - Values expected to be on the desired card, these come in the form of an array.
     */
    cardValidation(cardText, cardType, expectedValues) {
        if (cardText === 'Comment posted by the automation') {
            cardText = global.postedComment;
        }
        var cardFound = this.cardTypeSelector(cardType, cardText);
        var foundValues = cardFound.getText().replace(/\n/g, ', ');
        var valuesArray = foundValues.split(', ');
        try {
            for (var i = 0; i < expectedValues.length; i++) {
                var expectedString = expectedValues[i].toString();
                if (expectedString.includes('[PRETTYDATE]')) {
                    var dateMatch = valuesArray.find((date) =>
                        this.dateValidator(date)
                    );
                    expect(dateMatch).toBeTruthy();
                } else {
                    expect(foundValues.includes(expectedString)).toEqual(true);
                }
            }
        } catch (error) {
            throw new Error(`Could not locate the expected value of "${expectedString}" within the found values "${foundValues}"`);
        }
    }
}
module.exports = new contentCard();
