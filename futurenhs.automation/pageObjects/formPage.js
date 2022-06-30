const { elementTextContains } = require('selenium-webdriver/lib/until');
const helpers = require('../util/helpers');
const basePage = require('./basePage');

class formPage extends basePage{
    /**
     * Switch function to determine which form control is required
     * @param {string} valueToSet - string value to set in the form control
     * @param {string} label - expected label of the form element 
     * @param {string} interactionType - type of form control
     */
    formActionSelect(valueToSet, label, interactionType, instance){
        switch(interactionType){
            case "field" : this.inputFieldSet(valueToSet, label, instance);
                break;
            case "text area" : this.textAreaSet(valueToSet, label, instance);
                break;
            case "text editor" : this.textEditorSet(valueToSet, label, instance);
                break;
            default : throw new Error(`Type of control not found ` + interactionType);
        }
    }


    /**
     * 
     * @param {*} label
     * @param {*} foundElements
     * @returns
     */
     findInstance(label, foundElements) {
        var instance = foundElements.filter((item) => item != undefined && item.startsWith(label)).length;
        foundElements.push(label);
        return instance;
    }
    
    /**
     * Generic command to locate and element by it's label text value
     * @param {string} labelText - the textual value of the desired label used as the selector
     */
    findLabel(labelText, instance){
        if(instance === undefined){ instance = 0 }
        var label = $$(`//label[starts-with(normalize-space(.), "${labelText}")]`)[instance];
        helpers.waitForLoaded(label);
        return label
    }

    /**
     * Method for any input field to input text into the field.
     * @param {string} valueToSet - textual value to set within the input field
     * @param {string} label - the textual value of the desired label used as part of the selector
     */
    inputFieldSet(valueToSet, label, instance){
        var fieldLabel = this.findLabel(label, instance);
        var fieldInput = fieldLabel.parentElement().$('input');
        helpers.clearElement(fieldInput);
        fieldInput.addValue(valueToSet);
        browser.keys('Tab');    
    }

    /**
     * Method for any text area to input text into the field.
     * @param {string} valueToSet - textual value to set within the text area
     * @param {string} label - the textual value of the desired label used as part of the selector
     */
    textAreaSet(valueToSet, label, instance){
        var fieldLabel = this.findLabel(label, instance);
        var areaInput = fieldLabel.parentElement().$('textarea');
        helpers.clearElement(areaInput);
        areaInput.addValue(valueToSet);
        browser.keys('Tab');
    }

    /**
     * Method for setting textual value within a tinymce rich text editor
     * @param {string} valueToSet - textual value to set within the rich text editor
     */
    textEditorSet(valueToSet, label){
        if(valueToSet == null){ return }
        var txtEditorLabel = helpers.getEnabledInstance(`//label[starts-with(normalize-space(.), "${label}")]`);
        //Find the unique ID for the desired text editor iFrame
        txtEditorLabel.scrollIntoView();
        var editorId = txtEditorLabel.getAttribute('for').toString();
        //Switch to new frame and navigate to the editor to input text
        var newFrame = $(`iframe#${editorId}_ifr`)
        newFrame.waitForExist();
        browser.switchToFrame(0);
        var txtEditor = $(`//html/body[@id="tinymce"]`);
        helpers.waitForLoaded(txtEditor);
        //Custom exception for adding GUID to end of specific post for other test purposes
        if(valueToSet === 'Comment posted by the automation'){
            valueToSet = valueToSet + ' - ' + helpers.randomIDGenerator();
            global.postedComment = valueToSet
        }
        //Set value and return to main frame of the page
        helpers.clearElement(txtEditor);
        txtEditor.addValue(valueToSet);
        browser.switchToParentFrame();
    }

    /**
     * function to locate and select open a dropdown, then select the desired dropdown option
     * @param {string} dropdownOption - textual value used as the selector to find the desired option 
     * @param {string} label - the textual value of the desired label used as part of the selector
     * @param {integer} instance - numerical value of the desired instance of the dropdown, this is used to combat hidden/duplicated fields
     */
    dropdownSelect(dropdownOption, label){
        var dropdown = this.findLabel(label)
        helpers.click(dropdown);
        helpers.click(dropdown.$(`../select/option[contains(text(), "${dropdownOption}")]`));
    }

    /**
     * function to select desired a radio button by finding the legend label, then the button label within
     * @param {*} legendLabel - label of the main control containing all radio buttons
     * @param {*} buttonLabel - label of the desired radio button
     */
    radioButtonSelect(legendLabel, buttonLabel){
        var radioLegend = $(`//legend[text()="${legendLabel}"]`).parentElement();
        helpers.waitForLoaded(radioLegend);
        if(buttonLabel === 'Theme'){
            helpers.click(radioLegend.$(`./div/label[@for="themeId[1]"]`));
        } else {
            helpers.click(radioLegend.$(`./div/label[text()="${buttonLabel}"]`));
        }
    }
    
    /**
     * function find and select a desired checkbox on a page by it's label
     * @param {*} label - textual value of the checkbox to select
     */
    checkboxSelect(label){
        if(label.includes('public?')){
            var checkbox = $(`//label[@for="isPublic[0]"]`);
            helpers.waitForLoaded(checkbox);
        } else {
            checkbox = this.findLabel(label);
        }
        checkbox.$(`../input[@type="checkbox"]`).click();
    }

    /**
     * input first 3 characters of known string, and choose item from auto suggest dropdown list 
     * @param {*} option - desired value, used for first 3 digits in the field, and to find the desired option from the suggestion list
     * @param {string} label - the textual value of the desired label used as part of the selector
     */
    autoSuggestSelect(option, label){
        // Input first 3 digits into the input field
        if(option != null){
            var input = option.slice(0, 3)
            var fieldLabel = this.findLabel(label);
            var fieldInput = fieldLabel.parentElement().$('input');
            helpers.waitForLoaded(fieldInput)
            fieldInput.setValue(input);
            // Choose item from the suggestion list
            var suggestDropdown = fieldLabel.parentElement().$(`./div/ul`);
            helpers.waitForLoaded(suggestDropdown);
            var suggestList = suggestDropdown.$$('li')
            suggestList.forEach(suggestion => {
                if(suggestion.getText() === option){
                    helpers.click(suggestion);
                    return
                }   
            });
        }
    }

    /**
     * Function to locate a field and validate the input value against what is expected
     * @param {string} labelText - the textual value of the label used for the selector
     * @param {string} expectedValue - the expected value of the field, used to validate against
     */
    fieldTextValidation(labelText, expectedValue, fieldType){
        var label = $(`//label[starts-with(normalize-space(.), "${labelText}")]/..`)
        switch (fieldType) {
            case 'field' : var field = label.$(`input`)              
                break;        
            case 'text area' :  var field = label.$(`textarea`)              
                break;
        }
        expect(field.getValue()).toEqual(expectedValue);
    }

    /**
     * Validate the read only profile data on member profile page
     * @param {*} profileData - table data of profile values
     */
    profileDataValidation(profileData){
        profileData.forEach(valueSet => {
            helpers.waitForLoaded(`//dt[@class="c-profile_data-label u-text-bold" and starts-with(normalize-space(.), "${valueSet[0]}")]/../dd[text()="${valueSet[1]}"]`)
        });
    }
      
    /**
     * Function to locate and validate an error message displayed within a form page
     * @param {string} messageTxt - textual value of the error message to validate against
     */
    formErrorValidation(messageTxt) {
        var errorSelector = `//div[contains(@class, "c-error-summary")]/ul/li|//span[contains(@class, "error-message")]`;
        var errors = ''
        helpers.waitForLoaded(errorSelector);
        var foundErrors = $$(errorSelector);
        foundErrors.forEach(error => {
            errors = errors.concat(error.getText(), ', ');            
        });
        expect(errors.includes(messageTxt)).toEqual(true);
    }
}
module.exports = new formPage();