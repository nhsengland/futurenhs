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
     */
    searchResultsCardCount(expectedAmount){
        var searchResults = $(`//ul[li[@class="c-search-result u-border-b-theme-8 u-mb-6 u-flex u-flex-col u-break-words"]]`);
        var resultsFoundCount = searchResults.$$(`li`);
        expect(resultsFoundCount.length).toEqual(parseInt(expectedAmount));
    }
}
module.exports = new search();