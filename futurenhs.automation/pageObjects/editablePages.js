const _ = require('lodash');
const helpers = require('../util/helpers');
const basePage = require('./basePage');

class editablePages extends basePage {
    /**
     * Switch function to find the desired block, then move to the correct type of expected validation. Control for future expansion
     * @param {*} blockType - type of desired block
     * @param {*} blockState - the state in which to validate
     * @param {*} instance - instance IF specified of the block
     */
    blockValidationSelect(blockType, blockState, instance) {
        console.log('instance', instance)
        var foundBlock = this.blockSelect(blockType, instance);
        switch (blockState) {
            case 'preview mode': this.previewModeValidation(foundBlock);
                break;
            case 'edit mode': this.editModeValidation(foundBlock);
                break;
            default: throw new Error('invalid block state provided')
        }
    }
    
    /**
     * Function to locate the desired block in the html and validate it exists 
     * @param {*} blockType - type of desired block
     * @param {*} instance - instance IF specified of the block
     * @returns - found element within the html
     */
    blockSelect(blockType, instance) {
        blockType = _.camelCase(blockType);
        var blockPath = `//div[@data-content-type-id="${blockType}"]`
        if(instance != null){
            var desiredBlock = $$(blockPath)[instance-1]
        } else {
            var desiredBlock = helpers._resolveElement(blockPath);
        }
        helpers.waitForLoaded(desiredBlock);
        return desiredBlock;
    }

    /**
     * Validates a block that is currently in Preview Mode by confirming no form exists within
     * @param {*} foundBlock - Block element found during blockSelect
     */
    previewModeValidation(foundBlock) {
        foundBlock.$(`./div//form/div[@class="c-form_body"]`).waitForDisplayed({reverse: true});
    }

    /**
     * Validates a block that is currently in Edit Mode by confirming a form exists within
     * @param {*} foundBlock - Block element found during blockSelect
     */
    editModeValidation(foundBlock) {
        helpers.waitForLoaded(foundBlock.$(`./div//form/div[@class="c-form_body"]`));
    }

    /**
     * Finds all enabled action buttons within the desired block, validates those found match what was expected
     * @param {*} blockType - type of desired block
     * @param {*} expectedButtons - Array of text values for all expected enabled buttons at time of testing
     * @param {*} instance - instance IF specified of the block
     */
    enabledActionsValidation(blockType, expectedButtons, instance) {
        var desiredBlock = this.blockSelect(blockType, instance);
        var enabledButtons, foundButtons = []
        browser.waitUntil(() => {
            enabledButtons = desiredBlock.$$(`./header//button`).filter((element => element.isClickable()));
            return enabledButtons.length === expectedButtons.length
        },{}); 
        enabledButtons.forEach(button => foundButtons.push(button.getText()))                
        foundButtons.forEach((button, index) => {
            expect(button).toEqual(expectedButtons[index]);
        });
    }

    /**
     * Locates and Clicks a desired button within the specified block
     * @param {*} blockType - type of desired block
     * @param {*} buttonToClick - textual value of the desired button to be clicked
     * @param {*} instance - instance IF specified of the block
     */
    blockActionClick(blockType, buttonToClick, instance) {
        var desiredBlock = this.blockSelect(blockType, instance);
        var blockButton = desiredBlock.$(`./header//button[starts-with(normalize-space(.), "${buttonToClick}")]`);
        helpers.click(blockButton);
    }
}
module.exports = new editablePages();
