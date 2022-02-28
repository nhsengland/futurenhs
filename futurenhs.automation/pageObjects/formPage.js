const helpers = require('../util/helpers');
const basePage = require('./basePage');

class formPage extends basePage{
    /**
     * Switch function to determine which form control is required
     * @param {string} valueToSet - string value to set in the form control
     * @param {string} label - expected label of the form element 
     * @param {string} interactionType - type of form control
     */
    formActionSelect(valueToSet, label, interactionType){
        switch(interactionType){
            case "field" : this.inputFieldSet(valueToSet, label)
                break;
            case "text area" : this.textAreaSet(valueToSet, label)
                break;
            case "text editor" : this.textEditorSet(valueToSet, label)                
                break;
            default : throw new Error(`Type of control not found ` + interactionType);
        }
    }

    /**
     * Generic command to locate and element by it's label text value
     * @param {string} labelText - the textual value of the desired label used as the selector
     */
    findLabel(labelText){
        var label = $(`//label[starts-with(normalize-space(.), "${labelText}")]`)
        helpers.waitForLoaded(label);
        return label
    }

    /**
     * Method for any input field to input text into the field.
     * @param {string} valueToSet - textual value to set within the input field
     * @param {string} label - the textual value of the desired label used as part of the selector
     */
    inputFieldSet(valueToSet, label){
        var fieldLabel = this.findLabel(label);
        var fieldInput = fieldLabel.parentElement().$('input');
        helpers.waitForLoaded(fieldInput)
        fieldInput.setValue(valueToSet);
        browser.keys('Tab');    
    }

    /**
     * Method for any text area to input text into the field.
     * @param {string} valueToSet - textual value to set within the text area
     * @param {string} label - the textual value of the desired label used as part of the selector
     */
    textAreaSet(valueToSet, label){
        var fieldLabel = this.findLabel(label);
        var areaInput = fieldLabel.parentElement().$('textarea');
        helpers.waitForLoaded(areaInput)
        areaInput.setValue(valueToSet);
        browser.keys('Tab');
    }

    /**
     * Method for setting textual value within a tinymce rich text editor
     * @param {string} valueToSet - textual value to set within the rich text editor
     */
    textEditorSet(valueToSet, label){
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
        txtEditor.setValue(valueToSet);
        browser.switchToParentFrame();
        //Find sumbit button of the editor
        var submitbtn = txtEditorLabel.parentElement().parentElement().parentElement().$('./div[2]/button');
        //Click submit button of the editor
        helpers.click(submitbtn);
    }

    /**
     * function to locate and select open a dropdown, then select the desired dropdown option
     * @param {string} dropdownOption - textual value used as the selector to find the desired option 
     * @param {string} label - the textual value of the desired label used as part of the selector
     * @param {integer} instance - numerical value of the desired instance of the dropdown, this is used to combat hidden/duplicated fields
     */
    dropdownSelect(dropdownOption, label, instance){ 
        instance = instance ? instance - 1 : 0
        var dropdown = this.findLabel(label)
        helpers.click(dropdown);
        helpers.click(dropdown.$(`../select/option[contains(text(), "${dropdownOption}")]`));
    }

    /**
     * 
     * @param {*} label 
     */
    radioButtonSelect(){}

    /**
     * 
     * @param {*} label 
     */
    checkboxSelect(label){
        try{
            var checkbox = this.findLabel(label)
            checkbox.$(`../input[@type="checkbox"]`).click();
        } catch (error){
            throw new Error(`Unable to locate the '${label}' checkbox : '${error}'`);
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
    formErrorValidation(messageTxt, errorType) {
        var foundErrors        
        var errors = ''
        if(errorType === 'summary'){
            var summaryBody = $('//div[contains(@class, "c-error-summary")][*[normalize-space(.) = "There is a problem"]]/ul');
            helpers.waitForLoaded(summaryBody);        
            foundErrors = summaryBody.$$('./li');
        } else {
            foundErrors = $$('//span[@class="nhsuk-error-message"]');
        }
        foundErrors.forEach(error => {
            errors = errors.concat(error.getText(), ', ');            
        });
        expect(errors.includes(messageTxt)).toEqual(true);
    }

}
module.exports = new formPage();