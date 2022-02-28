var {Given} = require('@cucumber/cucumber');
const loginPage = require('../pageObjects/loginPage');
const genericPage = require('../pageObjects/genericPage');
const adminPage = require('../pageObjects/adminPage');

Given(/^I have navigated to '([^"]*)'( and accept the cookies)?$/, function (url, cookies) {
  browser.maximizeWindow();  
  browser.url(url);
  if(cookies){
    genericPage.acceptCookies();
  }
});

Given(/^I have logged in as a.? '([^"]*)'$/, function (userType) {
  loginPage.loginUser(userType);
});

Given(/^I have logged off as the current user$/, function () {
  loginPage.logoutUser();
});

Given(/^I log off and log in as a.? '([^"]*)'$/, function (userType) {
  loginPage.switchUser(userType);
});

Given(/^I ensure the browser is in mobile emulation$/, function () {
  if(browser.capabilities.browserName != 'msedge'){
    browser.emulateDevice('iPhone X');
  }
});

Given(/^I create '([^"]*)' system page$/, function (pageName) {
  adminPage.createSystemPages(pageName);
});

Given(/^I pause the page for '([^"]*)' ms$/, function (seconds) {
  browser.pause(seconds);
});