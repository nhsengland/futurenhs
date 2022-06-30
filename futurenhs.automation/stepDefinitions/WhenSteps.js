var { When, Before } = require('@cucumber/cucumber');
const genericPage = require('../pageObjects/genericPage');
const formPage = require('../pageObjects/formPage');
const forumPage = require('../pageObjects/forumPage');
const tablePOM = require('../pageObjects/table');
const filesPage = require('../pageObjects/filesPage');
const search = require('../pageObjects/search');
const contentCard = require('../pageObjects/contentCard');
const helpers = require('../util/helpers');
const editablePages = require('../pageObjects/editablePages');

Before(() => {
    this.generatedString = {};
});

When(/^I click the '([^"]*)' (button|link|option|tab|nav icon|breadcrumb)$/, function (textValue, contentType) {
    genericPage.contentClick(contentType, textValue);
});

When(/^I open the '([^"]*)' accordion$/, function (detailsName) {
    genericPage.openAccordion(detailsName);
});

When(/^I accept the browser alert$/, function () {
    genericPage.acceptBrowserAlert();
});

When(/^I click '([^"]*)' on the '([^"]*)' row of the '([^"]*)' table$/, function (linkText, rowText, tableName) {
    tablePOM.tableLinkClick(linkText, rowText, tableName);
});

When(/^I upload the '([^"]*)' file$/, function (filepath) {
    filesPage.mediaUpload(filepath);
});

When(/^I confirm and delete the '([^"]*)'$/, function (filepath) {
    genericPage.contentClick('link', 'Delete');
    genericPage.acceptBrowserAlert();
});

When(/^I search for '([^"]*)'$/, function (inputVal) {
    search.search(inputVal);
});

When(/^I select '([^"]*)' from the (group menu|menu|group actions|group pages) accordion$/, function (linkText, groupOption) {
    genericPage.selectAccordionItem(linkText, groupOption);
});

// FormPage Steps

When(/^I enter '([^"]*)' into the ('([^"]*)' )?(field|text area|text editor)$/, function (inputTxt, label, inputType) {
    if (inputTxt != null && inputTxt.includes('[STRING: ')) {
        inputTxt = helpers.randomStringGenerator(inputTxt);
        this.generatedString = inputTxt;
    }
    if (this.foundElements === undefined) { this.foundElements = [] }
    var instance = formPage.findInstance(label, this.foundElements);
    formPage.formActionSelect(inputTxt, label, inputType, instance);
});

When(/^I select '([^"]*)' from the '([^"]*)' dropdown$/, function (dropdownOption, dropdownLabel) {
    formPage.dropdownSelect(dropdownOption, dropdownLabel);
});

When(/^I choose '([^"]*)' from the '([^"]*)' auto suggest list$/, function (inputValue, autoSuggestLabel) {
    formPage.autoSuggestSelect(inputValue, autoSuggestLabel);
});

When(/^I select the '([^"]*)' checkbox$/, function (label) {
    formPage.checkboxSelect(label);
});

When(/^I select the '([^"]*)' radio button for '([^"]*)'$/, function (radioOption, legend) {
    formPage.radioButtonSelect(legend, radioOption);
});

// ForumPage Steps

When(/^I (like|unlike) the '([^"]*)' (comment|reply) card$/, function (action, cardText, cardType) {
    forumPage.commentLikeClick(action, cardText, cardType);
});

When(/^I select the '([^"]*)' (search result|group|discussion|comment|reply) card$/, function (cardText, cardType) {
    contentCard.cardLinkClick(cardText, cardType);
});

When(/^I click reply on the '([^"]*)' (comment|reply) card$/, function (cardText, cardType) {
    forumPage.replyToPostClick(cardText, cardType);
});

//// Editable Pages Steps

When(/^I click '([^"]*)' on the ('(\d+).{2}' )?(text|key links) block$/, function (button, instance, blockType) {
    editablePages.blockActionClick(blockType = blockType + ' block', button, instance);
});