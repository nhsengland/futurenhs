const helpers = require("../util/helpers");
const basePage = require('./basePage');

class search extends basePage{
    /**
     * 
     */
    search(inputVal){
        this.searchBarValidation
        var searchInput = $('//input[@class="c-site-header-nav_search-input"]');
        searchInput.setValue(inputVal);
        helpers.click(searchInput.parentElement().$(`button`));
    }
    /**
         * 
         */
    searchBarValidation(){
        var searchItem = $(`//div[@class="c-site-header-nav_search-item"]`);
        helpers.waitForLoaded(searchItem);
        helpers.waitForLoaded(searchItem.$('./input[@aria-label="Search"]'));
        helpers.waitForLoaded(searchItem.$('./button[@aria-label="Search button"]'));
    }

    /**
     * 
     * @param {*} expectedAmount 
     */
    searchResultsValidation(expectedAmount){
        expect(this.searchResultsHeaderValidation(expectedAmount)).toEqual(true);
        if(expectedAmount != 0){
            expect(this.searchResultsCardCount(expectedAmount)).toEqual(true);
        }
    }

    /**
     * 
     * @param {*} expectedAmount 
     * @returns 
     */
    searchResultsHeaderValidation(expectedAmount){
        var searchResultsHeader = $(`//h1[contains(text(), "- ${expectedAmount} results found")]`);
        var result = false
        if(searchResultsHeader.isDisplayed()){
            result = true
        };
        return result
    }

    /**
     * 
     * @param {*} expectedAmount 
     * @returns 
     */
    searchResultsCardCount(expectedAmount){
        var searchResults = $(`//ul[li[contains(@class, "c-search-result")]]`);
        var resultsFoundCount = searchResults.$$(`li`);
        var result = false
        if(resultsFoundCount.length == expectedAmount){
            result = true
        };
        return result
    }
}
module.exports = new search();