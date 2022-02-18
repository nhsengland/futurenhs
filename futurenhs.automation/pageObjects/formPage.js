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
                break
            case "text editor" : this.richTextSet(valueToSet, label)
                break
            default : throw new Error(`Type of control not found ` + interactionType)
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
        try{
            var fieldLabel = this.findLabel(label);
            var fieldInput = fieldLabel.parentElement().$('input');
            helpers.waitForLoaded(fieldInput)
            fieldInput.setValue(valueToSet);
            browser.keys('Tab');    
        } catch (error){
            throw new Error (`Error setting textBox "${valueToSet}" : ${error}`)
        }
    }

    /**
     * Method for any text area to input text into the field.
     * @param {string} valueToSet - textual value to set within the text area
     * @param {string} label - the textual value of the desired label used as part of the selector
     */
    textAreaSet(valueToSet, label){
        try{
            var fieldLabel = this.findLabel(label);
            var areaInput = fieldLabel.parentElement().$('textarea');
            helpers.waitForLoaded(areaInput)
            areaInput.setValue(valueToSet);
            browser.keys('Tab');
        } catch (error){
            throw new Error (`Error setting textArea "${valueToSet}" : ${error}`)
        }
    }

    /**
     * Method for setting textual value within a tinymce rich text editor
     * @param {string} valueToSet - textual value to set within the rich text editor
     */
    richTextSet(valueToSet){
        try{
            var newFrame = $(`iframe[id=tinymce-editor_ifr]`);
            newFrame.waitForExist();
            browser.switchToFrame(newFrame);
            var txtEditor = $(`//html/body[@id="tinymce"]`);
            helpers.waitForLoaded(txtEditor)
            if(valueToSet === 'Comment posted by the automation'){
                valueToSet = valueToSet + ' - ' + helpers.randomIDGenerator();
                global.postedComment = valueToSet
            }
            txtEditor.setValue(valueToSet);
            browser.switchToParentFrame();
        } catch (error){
            throw new Error (`Error setting rich text "${valueToSet}" : ${error}`)
        }
    }

    /**
     * function to locate and select open a dropdown, then select the desired dropdown option
     * @param {string} dropdownOption - textual value used as the selector to find the desired option 
     * @param {string} label - the textual value of the desired label used as part of the selector
     * @param {integer} instance - numerical value of the desired instance of the dropdown, this is used to combat hidden/duplicated fields
     */
    dropdownSelect(dropdownOption, label, instance){ 
        try{
            instance = instance ? instance - 1 : 0
            var dropdown = this.findLabel(label)
            helpers.click(dropdown);
            helpers.click(dropdown.$(`../select/option[contains(text(), "${dropdownOption}")]`));
        } catch (error){
            throw new Error(`Unable to locate the '${dropdownOption}' option, on the '${label}' dropdown : '${error}'`);
        }
    }

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
    errorSummaryValidation(messageTxt) {
        var summaryBody
        if(browser.getUrl().includes('/admin')){
            // CONTROL FOR MVC FORUM ADMIN PORTAL
            summaryBody = $('//div[@class="validation-summary-errors"]/ul')
        } else {
            summaryBody = $('//div[@aria-labelledby="error-summary-title"][*[normalize-space(.) = "There is a problem"]]/div/ul');
        }
        helpers.waitForLoaded(summaryBody);
        var foundErrors = ''
        var summaryList = summaryBody.$$('./li');
        summaryList.forEach(error => {
            foundErrors = foundErrors.concat(error.getText(), ', ');            
        });
        expect(foundErrors.includes(messageTxt)).toEqual(true);
    }

    fieldErrorValidation(messageTxt) {
        var fieldError = $('//span[contains(@class, "c-error-message")]');
        var txtEditorError = $('//p[@class="js-tinyMCE-error-notEmpty c-error-message"]');
        if(fieldError.isDisplayed() === true){
            var errorString = ''
            var foundErrors = $$('//span[contains(@class, "c-error-message")]');
            foundErrors.forEach(error => {
                errorString = errorString.concat(error.getText(), ', ');
            });
        } else if(txtEditorError.isDisplayed() === true) {
            var foundError = txtEditorError.getText();
            expect(foundError).toEqual(messageTxt);
        } else {
            throw new Error('Unable to locate error message based on known error selectors')
        }
    }

    /**
     * Function to validate the Tiny MCE rich text editor is cleared of any set textual values
     */
    textEditorCleared(){
        var newFrame = $(`iframe[id=tinymce-editor_ifr]`);
        newFrame.waitForExist();
        browser.switchToFrame(newFrame);
        var txtEditor = $(`//html/body[@data-id="tinymce-editor"]/p`);
        helpers.waitForLoaded(txtEditor)
        expect(txtEditor.getText()).toEqual('')
        browser.switchToParentFrame();
    }
}
module.exports = new formPage();