const helpers = require("../util/helpers");
const basePage = require('./basePage');

class search extends basePage{
    /**
     * base function to peform search on the FNHS app
     * @param {*} inputVal - input text string for the search field
     */
    search(inputVal){
        this.searchBarValidation
        var searchInput = $('//input[@class="c-site-header-nav_search-input"]');
        searchInput.setValue(inputVal);
        helpers.click(searchInput.parentElement().$(`button`));
    }
    /**
     * basic validation that the search bar and it's content exists on a page
     */
    searchBarValidation(){
        var searchItem = $(`//div[@class="c-site-header-nav_search-item"]`);
        helpers.waitForLoaded(searchItem);
        helpers.waitForLoaded(searchItem.$('./input[@aria-label="Search"]'));
        helpers.waitForLoaded(searchItem.$('./button[@aria-label="Search button"]'));
    }

    /**
     * function containing two validation steps of results header and results cards, based on an expected amount, expects true
     * @param {*} expectedAmount - expected amount of results on the page
     */
    searchResultsValidation(expectedAmount){
        expect(this.searchResultsHeaderValidation(expectedAmount)).toEqual(true);
        if(expectedAmount != 0){
            expect(this.searchResultsCardCount(expectedAmount)).toEqual(true);
        }
    }

    /**
     * function to validate the search results header is displayed showing the expected amount of results
     * @param {*} expectedAmount - the expected number to exist within the header
     * @returns true or false
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
     * function to validate the number of results found on the page against an expected amount
     * @param {*} expectedAmount - the expected number of results cards on the page
     * @returns true or false
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