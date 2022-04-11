var {Then} = require('@cucumber/cucumber');
const filesPage = require('../pageObjects/filesPage');
const formPage = require('../pageObjects/formPage');
const forumPage = require('../pageObjects/forumPage');
const contentCard = require('../pageObjects/contentCard');
const genericPage = require('../pageObjects/genericPage');
const search = require('../pageObjects/search');
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
  genericPage.selectDialogButton(actionType, dialogName);
});

Then(/^the search bar is available$/, function(){
  search.searchBarValidation();
});

Then(/^there are '([^"]*)' search results displayed$/, function(amount){
  search.searchResultsValidation(amount)
});

Then(/^the '([^"]*)' new tab is open and '([^"]*)' is displayed$/, function(url, tabMatcher){
  genericPage.newTabValidation(url, tabMatcher);
});

Then(/^the '([^"]*)' (reply|discussion|comment|group|post|search result) card is displayed$/, function (cardTitle, cardType, table) {
  var cardValues = table.raw().flat();
  contentCard.cardValidation(cardTitle, cardType, cardValues);
});

//// Generic Content Steps

Then(/^the '([^"]*)' (header|textual value|link|button|option|label) is displayed$/, function (textValue, contentType) {
  genericPage.contentValidation(contentType, textValue);
});

Then(/^the '([^"]*)' (header|textual value|link|button|option) is not displayed$/, function (textValue, contentType) {
  genericPage.contentNotDisplayed(contentType, textValue);
});

//// ForumPage Steps

Then(/^(there are|there are more) (discussion|comment|reply) cards displayed$/, function(compareArg, cardType){
  forumPage.discussionCountComparator(compareArg, cardType);
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

Then(/^the breadcrumb navigation displays '([^"]*)'$/, function(breadcrumb){
  filesPage.breadcrumbValidation(breadcrumb);
});

Then(/^I download the '([^"]*)' and check the file for '([^"]*)'$/, function(fileType, fileContent) {
  filesPage.pdfDownloadCheck(file, fileContent);
})

Then(/^the collabora file( mobile)? preview is displayed$/, function(mobile){
  filesPage.filePreviewExists(mobile);
})

Then(/^the image file '([^"]*)' is uploaded and ready$/, function(filePath){
  var fileName = filePath.replace(/(\/[a-z]*\/)/g, '').replace(/(.[a-z]*$)/g, '')
  filesPage.imageUploadValidation(fileName)
});

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