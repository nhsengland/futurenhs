var {Given, When, Then} = require('@cucumber/cucumber');
var helpers = require('../util/helpers');
var elementVisualReg = null;


Then(/^the '([^"]*)' page image is taken and compared to the baseline image$/, function(pageName){
    var checkResult = browser.checkFullPageScreen(pageName)
    console.log(`Comparing the ${pageName} image against the baseline image, the difference percentage found is: `, checkResult)
    expect(checkResult).toBeLessThanOrEqual(5);
});

/////////////////////// ELEMENT STEPS ////////////////////////////////////

When(/^I save '([^"]*)' element '([^"]*)' image for visual regression$/, function (Name, elementVis){
    elementVisualReg = helpers._resolveElement(elementVis);
    browser.saveElement(elementVisualReg, Name);
});

Then(/^The element image page taken to be compared to the baseline image$/, function (){
    expect(browser.checkElement(elementVisualReg, Name)).toEqual(0);
});