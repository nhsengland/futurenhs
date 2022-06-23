const _ = require('lodash');
const helpers = require('../util/helpers');
const basePage = require('./basePage');

class editablePages extends basePage {
    
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

    previewModeValidation(foundBlock) {
        foundBlock.$(`./div//form/div[@class="c-form_body"]`).waitForDisplayed({reverse: true});
    }

    editModeValidation(foundBlock) {
        helpers.waitForLoaded(foundBlock.$(`./div//form/div[@class="c-form_body"]`));
    }

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

    blockActionClick(blockType, buttonToClick, instance) {
        var desiredBlock = this.blockSelect(blockType, instance);
        var blockButton = desiredBlock.$(`./header//button[starts-with(normalize-space(.), "${buttonToClick}")]`);
        helpers.click(blockButton);
    }
}
module.exports = new editablePages();
