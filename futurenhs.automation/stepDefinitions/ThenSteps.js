var {Then} = require('@cucumber/cucumber');
const filesPage = require('../pageObjects/filesPage');
const formPage = require('../pageObjects/formPage');
const forumPage = require('../pageObjects/forumPage');
const genericPage = require('../pageObjects/genericPage');
const tablePOM = require('../pageObjects/table');
const axe = require('../util/axe');
const lighthouse = require('../util/lighthouse');

Then(/^I return to the homepage$/, function() {
  genericPage.homepageReturn();
});

Then(/^the current URL is '([^"]*)'$/, function (expectedUrl) {
  expect(browser.getUrl()).toEqual(expectedUrl);
});

Then(/^I (confirm|cancel) this on the open '([^"]*)' dialog$/, function (actionType, dialogName){
  genericPage.openDialogSelect(actionType, dialogName);
});

Then(/^the breadcrumb navigation displays '([^"]*)'$/, function(breadcrumb){
  genericPage.breadcrumbValidation(breadcrumb);
});

Then(/^the group image is displayed$/, function(){
  genericPage.groupImageValidation();
})

//// Generic Content Steps

Then(/^the '([^"]*)' (header|textual value|link|button|option|label) is displayed$/, function (textValue, contentType) {
  genericPage.contentValidation(contentType, textValue);
});

Then(/^the '([^"]*)' (header|textual value|link|button|option) is not displayed$/, function (textValue, contentType) {
  genericPage.contentNotExisting(contentType, textValue);
});

//// ForumPage Steps

Then(/^the '([^"]*)' (reply|discussion|comment|group|post) card is displayed$/, function (cardTitle, cardType, table) {
  var cardContent = table.raw().flat();
  forumPage.cardValidation(cardTitle, cardType, cardContent);
});

Then(/^(there are|there are more) (discussion|comment|reply) cards displayed$/, function(compareArg, cardType){
  forumPage.cardCountComparison(compareArg, cardType);
});

Then(/^the card count is displayed as '([^"]*)'$/, function(textVal){
  forumPage.paginationValidation(textVal);
})

Then(/^the '([^"]*)' discussion card is pinned$/, function(cardTitle) {
  forumPage.pinnedDiscussionValidation(cardTitle);
})

//// FormPage Steps

Then(/^the text editor is empty$/, function(){
  formPage.textEditorCleared();
})

Then(/^the '([^"]*)' error (message|summary) is displayed$/, function (messageTxt, errorType) {
  formPage.formErrorValidation(messageTxt, errorType);
});

Then(/^the '([^"]*)' (field|text area) contains '([^"]*)'$/, function (fieldLabel, fieldType, fieldValue) {
  formPage.fieldTextValidation(fieldLabel, fieldValue, fieldType);
});

Then(/^the profile values are displayed$/, function (table){
  var profileData = table.raw()
  formPage.profileDataValidation(profileData);
});

//// Files Page

Then(/^I download the '([^"]*)' file and compare against the uploaded version$/, function(fileName){
  filesPage.hashCompare(fileName);
})

Then(/^I download the (doc|pdf) and check the file for '([^"]*)'$/, function(fileType, fileContent) {
  filesPage.pdfDownloadCheck(fileContent);
})

Then(/^the collabora file( mobile)? preview is displayed$/, function(mobile){
  filesPage.filePreviewExists(mobile);
})

//// Table Steps

Then(/^the '([^"]*)' (mobile )?table is displayed$/, function(tableName, mobile, table) {
  tablePOM.tableValidation(tableName, table.raw(), mobile);  
});

Then(/^the '([^"]*)' row is displayed on the '([^"]*)' table$/, function (rowValue, tableName ) {
  tablePOM.tableRowExists(rowValue, tableName);
});

Then(/^the '([^"]*)' table has '([^"]*)' row.?$/, function(tableName, rowCount) {
  tablePOM.rowCounter(tableName, rowCount);
});

Then(/^the '([^"]*)' table exists$/, function(tableName) {
  tablePOM.tableIsExisting(tableName);
});

Then(/^the '([^"]*)' table is not displayed$/, function(tableName) {
  tablePOM.tableNotExisting(tableName);
});

Then(/^the '([^"]*)' row is displayed on the '([^"]*)' mobile table$/, function (rowKey, tableName, table) {
  tablePOM.mobileTableValidation(rowKey, tableName, table.raw());
});

// Accessibility/Lighthouse test steps

Then(/^I ensure the page is accessible( by the '([^"]*)' standard)?$/, function(option) {
  axe.axeTest(option);
});

Then(/^the( desktop)? page is performant and follows best practices$/, async function(desktop) {
  await lighthouse.runLighthouse(desktop);
});