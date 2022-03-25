var {After, Before} = require('@cucumber/cucumber');
const adminPage = require('./pageObjects/adminPage');

Before({tags: "@createSystemPages"}, function() {
    console.log('***************************************');
    expect(adminPage.createSystemPages()).toEqual(true)
});