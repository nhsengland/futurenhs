var {When} = require('@cucumber/cucumber');
const genericPage = require('../pageObjects/genericPage');
const formPage = require('../pageObjects/formPage');
const forumPage = require('../pageObjects/forumPage');
const tablePOM = require('../pageObjects/table');
const filesPage = require('../pageObjects/filesPage');
const search = require('../pageObjects/search');
const contentCard = require('../pageObjects/contentCard');

  
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
  
  When(/^I search for '([^"]*)'$/, function(inputVal){
    search.search(inputVal)
  });

  // FormPage Steps

  When(/^I enter '([^"]*)' into the ('([^"]*)' )?(field|text area|text editor)$/, function (inputTxt, label, inputType) {
    formPage.formActionSelect(inputTxt, label, inputType);
  });

  When(/^I select '([^"]*)' from the '([^"]*)' dropdown$/, function (dropdownOption, dropdownLabel) {
    formPage.dropdownSelect(dropdownOption, dropdownLabel);
  });

  When(/^I select the ('([^"]*)' )?checkbox$/, function (label) {
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
  
  When(/^I click reply on the '([^"]*)' (comment|reply) card$/, function(cardText, cardType) {
    forumPage.replyToPostClick(cardText, cardType);
  });

  When(/^I select '([^"]*)' from the (group menu|menu|actions|group pages) accordion$/, function (linkText, groupOption) {
    genericPage.selectAccordionItem(linkText, groupOption);
  })